using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DonkeycarManager
{
    public partial class MainForm : Form
    {
        private string dataFolderPath = "";
        private string imagesFolderPath = "";
        private string catalogFilePath = "";
        private string modelFilePath = "";

        private List<DonkeyFrame> allFrames = new List<DonkeyFrame>();
        private List<DonkeyFrame> visibleFrames = new List<DonkeyFrame>();

        private int currentIndex = -1;
        private bool isUpdatingSelection = false;

        private Process? trainProcess;

        private readonly System.Windows.Forms.Timer autoPlayTimer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer cleanerRangePlayTimer = new System.Windows.Forms.Timer();

        private const string WslDistroName = "Ubuntu-22.04";
        private const string CondaEnvName = "e2e_env";

        private const int CatalogChunkSize = 1000;

        private double? overlayActualAngle = null;
        private double? overlayPredictedAngle = null;
        private double? overlayActualThrottle = null;
        private double? overlayPredictedThrottle = null;

        private int cleanerRangeStartIndex = -1;
        private int cleanerRangeEndIndex = -1;
        private int cleanerRangePlayIndex = -1;
        private bool isDraggingCleanerRange = false;

        public MainForm()
        {
            InitializeComponent();
            ConnectEvents();

            txtMycarPath.Text = "~/mycar";
            txtPythonExe.Text = "wsl";
            txtTrainArgs.Text = "train.py --tub ./data --model ./models/mypilot.h5";
            txtModelPath.Text = "~/mycar/models/mypilot.h5";

            autoPlayTimer.Interval = 150;
            autoPlayTimer.Tick += AutoPlayTimer_Tick;

            cleanerRangePlayTimer.Interval = 120;
            cleanerRangePlayTimer.Tick += CleanerRangePlayTimer_Tick;

            picPilotTest.Paint += picPilotTest_Paint;
            picPilotTest.Resize += (s, e) => picPilotTest.Invalidate();

            AppendLog("프로그램 실행 완료");
        }

        private void ConnectEvents()
        {
            btnOpenDataFolder.Click += btnOpenDataFolder_Click;
            btnReload.Click += btnReload_Click;
            btnAutoPlay.Click += btnAutoPlay_Click;

            btnApplyFilter.Click += btnApplyFilter_Click;
            btnClearFilter.Click += btnClearFilter_Click;
            btnDeleteFrame.Click += btnDeleteFrame_Click;

            pnlCleanerTimeline.Paint += pnlCleanerTimeline_Paint;
            pnlCleanerTimeline.MouseDown += pnlCleanerTimeline_MouseDown;
            pnlCleanerTimeline.MouseMove += pnlCleanerTimeline_MouseMove;
            pnlCleanerTimeline.MouseUp += pnlCleanerTimeline_MouseUp;

            btnDeleteRange.Click += btnDeleteRange_Click;
            btnPlayRange.Click += btnPlayRange_Click;
            btnClearRange.Click += btnClearRange_Click;

            btnBrowseMycar.Click += btnBrowseMycar_Click;
            btnTrain.Click += btnTrain_Click;
            btnStopTrain.Click += btnStopTrain_Click;

            btnBrowseModel.Click += btnBrowseModel_Click;
            btnRunPilotTest.Click += btnRunPilotTest_Click;
            btnUseViewerFrame.Click += btnUseViewerFrame_Click;

            lstFrames.SelectedIndexChanged += lstFrames_SelectedIndexChanged;
            lstCleanerFrames.SelectedIndexChanged += lstCleanerFrames_SelectedIndexChanged;
            lstPilotFrames.SelectedIndexChanged += lstPilotFrames_SelectedIndexChanged;

            trbFrame.Scroll += trbFrame_Scroll;
        }

        private void btnOpenDataFolder_Click(object? sender, EventArgs e)
        {
            using FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Donkeycar data 폴더를 선택하세요.";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            dataFolderPath = dlg.SelectedPath;
            imagesFolderPath = Path.Combine(dataFolderPath, "images");

            if (!Directory.Exists(imagesFolderPath))
            {
                MessageBox.Show(
                    "선택한 폴더 안에 images 폴더가 없습니다.\n\n" +
                    "mycar 폴더가 아니라 mycar/data 폴더를 선택해야 합니다."
                );
                return;
            }

            string[] catalogFiles = GetCatalogFiles();

            if (catalogFiles.Length == 0)
            {
                MessageBox.Show(
                    "선택한 폴더 안에 catalog 파일이 없습니다.\n\n" +
                    "catalog_0.catalog, catalog_1.catalog 같은 파일이 있어야 합니다."
                );
                return;
            }

            catalogFilePath = catalogFiles[0];

            txtMycarPath.Text = "~/mycar";
            lblDataPath.Text = "Data Folder: " + dataFolderPath;

            LoadCatalog();
        }

        private void btnReload_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(dataFolderPath) || !Directory.Exists(dataFolderPath))
            {
                MessageBox.Show("먼저 Donkeycar data 폴더를 열어주세요.");
                return;
            }

            string[] catalogFiles = GetCatalogFiles();

            if (catalogFiles.Length == 0)
            {
                MessageBox.Show("data 폴더 안에 catalog_*.catalog 파일이 없습니다.");
                return;
            }

            LoadCatalog();
        }

        private void btnAutoPlay_Click(object? sender, EventArgs e)
        {
            if (visibleFrames.Count == 0)
                return;

            autoPlayTimer.Enabled = !autoPlayTimer.Enabled;
            btnAutoPlay.Text = autoPlayTimer.Enabled ? "자동 재생 중지" : "자동 재생";
        }

        private void AutoPlayTimer_Tick(object? sender, EventArgs e)
        {
            if (visibleFrames.Count == 0)
                return;

            int next = currentIndex + 1;

            if (next >= visibleFrames.Count)
                next = 0;

            ShowFrame(next);
        }

        private void btnApplyFilter_Click(object? sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void btnClearFilter_Click(object? sender, EventArgs e)
        {
            chkThrottlePositive.Checked = false;
            chkExcludeZeroAngle.Checked = false;
            chkStopDataOnly.Checked = false;

            visibleFrames = allFrames.ToList();

            BindFrameLists();
            SetupTrackBar();
            ResetCleanerRange();

            if (visibleFrames.Count > 0)
                ShowFrame(0);
            else
                ClearViewer();

            AppendLog("필터 해제: 전체 데이터 표시");
        }

        private void btnDeleteFrame_Click(object? sender, EventArgs e)
        {
            if (visibleFrames.Count == 0)
            {
                MessageBox.Show("삭제할 데이터가 없습니다.");
                return;
            }

            List<DonkeyFrame> framesToDelete = new List<DonkeyFrame>();

            foreach (int selectedIndex in lstCleanerFrames.SelectedIndices)
            {
                if (selectedIndex >= 0 && selectedIndex < visibleFrames.Count)
                    framesToDelete.Add(visibleFrames[selectedIndex]);
            }

            if (framesToDelete.Count == 0 && currentIndex >= 0 && currentIndex < visibleFrames.Count)
                framesToDelete.Add(visibleFrames[currentIndex]);

            if (framesToDelete.Count == 0)
            {
                MessageBox.Show("삭제할 프레임을 먼저 선택하세요.");
                return;
            }

            DialogResult result = MessageBox.Show(
                $"선택한 {framesToDelete.Count}개 프레임을 삭제할까요?\n\n" +
                "이미지 파일과 catalog 데이터가 함께 삭제됩니다.\n" +
                "삭제 전 data 폴더 백업을 권장합니다.",
                "다중 삭제 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            DeleteFrames(framesToDelete, "다중 삭제");
        }

        private void btnDeleteRange_Click(object? sender, EventArgs e)
        {
            if (!TryGetNormalizedCleanerRange(out int start, out int end))
            {
                MessageBox.Show("먼저 타임라인에서 삭제할 구간을 드래그해서 선택하세요.");
                return;
            }

            List<DonkeyFrame> framesToDelete = new List<DonkeyFrame>();

            for (int i = start; i <= end; i++)
                framesToDelete.Add(visibleFrames[i]);

            DialogResult result = MessageBox.Show(
                $"선택 구간 {start + 1} ~ {end + 1}의 {framesToDelete.Count}개 프레임을 삭제할까요?\n\n" +
                "이미지 파일과 catalog 데이터가 함께 삭제됩니다.\n" +
                "삭제 전 data 폴더 백업을 권장합니다.",
                "구간 삭제 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            DeleteFrames(framesToDelete, "구간 삭제");
        }

        private void DeleteFrames(List<DonkeyFrame> framesToDelete, string logTitle)
        {
            try
            {
                DisposeCurrentImages();

                HashSet<string> deleteKeys = new HashSet<string>();

                foreach (DonkeyFrame frame in framesToDelete)
                {
                    string key = MakeFrameKey(frame);
                    deleteKeys.Add(key);

                    string imagePath = Path.Combine(imagesFolderPath, frame.ImageFileName);

                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                        AppendLog($"{logTitle} 이미지 삭제: {frame.ImageFileName}");
                    }
                    else
                    {
                        AppendLog($"{logTitle} 이미지 없음: {frame.ImageFileName}");
                    }
                }

                int removedCount = allFrames.RemoveAll(f => deleteKeys.Contains(MakeFrameKey(f)));

                SaveCatalog();

                ResetCleanerRange();
                ApplyFilter();

                AppendLog($"{logTitle} 완료: {removedCount}개 프레임 삭제");
                MessageBox.Show($"{removedCount}개 프레임 삭제가 완료되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("삭제 중 오류가 발생했습니다.\n\n" + ex.Message);
            }
        }

        private string MakeFrameKey(DonkeyFrame frame)
        {
            return $"{frame.Index}|{frame.ImageFileName}";
        }

        private void btnPlayRange_Click(object? sender, EventArgs e)
        {
            if (!TryGetNormalizedCleanerRange(out int start, out int end))
            {
                MessageBox.Show("먼저 타임라인에서 재생할 구간을 드래그해서 선택하세요.");
                return;
            }

            if (cleanerRangePlayTimer.Enabled)
            {
                cleanerRangePlayTimer.Stop();
                btnPlayRange.Text = "구간 재생";
                return;
            }

            cleanerRangePlayIndex = start;
            ShowFrame(cleanerRangePlayIndex);

            cleanerRangePlayTimer.Start();
            btnPlayRange.Text = "재생 중지";

            AppendLog($"구간 재생 시작: {start + 1} ~ {end + 1}");
        }

        private void CleanerRangePlayTimer_Tick(object? sender, EventArgs e)
        {
            if (!TryGetNormalizedCleanerRange(out int start, out int end))
            {
                cleanerRangePlayTimer.Stop();
                btnPlayRange.Text = "구간 재생";
                return;
            }

            if (cleanerRangePlayIndex < start || cleanerRangePlayIndex > end)
                cleanerRangePlayIndex = start;

            ShowFrame(cleanerRangePlayIndex);
            pnlCleanerTimeline.Invalidate();

            cleanerRangePlayIndex++;

            if (cleanerRangePlayIndex > end)
            {
                cleanerRangePlayTimer.Stop();
                btnPlayRange.Text = "구간 재생";
                AppendLog("구간 재생 종료");
            }
        }

        private void btnClearRange_Click(object? sender, EventArgs e)
        {
            ResetCleanerRange();
            AppendLog("Cleaner 구간 선택 해제");
        }

        private void pnlCleanerTimeline_MouseDown(object? sender, MouseEventArgs e)
        {
            if (visibleFrames.Count == 0)
                return;

            int index = HitTestCleanerTimelineIndex(e.X);

            if (index < 0)
                return;

            cleanerRangeStartIndex = index;
            cleanerRangeEndIndex = index;
            isDraggingCleanerRange = true;

            ShowFrame(index);
            UpdateCleanerRangeUi();
            pnlCleanerTimeline.Invalidate();
        }

        private void pnlCleanerTimeline_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!isDraggingCleanerRange || visibleFrames.Count == 0)
                return;

            int index = HitTestCleanerTimelineIndex(e.X);

            if (index < 0)
                return;

            cleanerRangeEndIndex = index;

            ShowFrame(index);
            UpdateCleanerRangeUi();
            pnlCleanerTimeline.Invalidate();
        }

        private void pnlCleanerTimeline_MouseUp(object? sender, MouseEventArgs e)
        {
            if (!isDraggingCleanerRange)
                return;

            isDraggingCleanerRange = false;

            NormalizeCleanerRange();
            UpdateCleanerRangeUi();
            pnlCleanerTimeline.Invalidate();
        }

        private int HitTestCleanerTimelineIndex(int mouseX)
        {
            if (visibleFrames.Count == 0)
                return -1;

            Rectangle rect = pnlCleanerTimeline.ClientRectangle;
            rect.Inflate(-6, -8);

            if (mouseX < rect.Left)
                mouseX = rect.Left;

            if (mouseX > rect.Right)
                mouseX = rect.Right;

            double ratio = (double)(mouseX - rect.Left) / Math.Max(1, rect.Width);
            int index = (int)(ratio * visibleFrames.Count);

            if (index < 0)
                index = 0;

            if (index >= visibleFrames.Count)
                index = visibleFrames.Count - 1;

            return index;
        }

        private bool TryGetNormalizedCleanerRange(out int start, out int end)
        {
            start = -1;
            end = -1;

            if (cleanerRangeStartIndex < 0 || cleanerRangeEndIndex < 0)
                return false;

            if (visibleFrames.Count == 0)
                return false;

            start = Math.Min(cleanerRangeStartIndex, cleanerRangeEndIndex);
            end = Math.Max(cleanerRangeStartIndex, cleanerRangeEndIndex);

            start = Math.Max(0, Math.Min(start, visibleFrames.Count - 1));
            end = Math.Max(0, Math.Min(end, visibleFrames.Count - 1));

            return start <= end;
        }

        private void NormalizeCleanerRange()
        {
            if (!TryGetNormalizedCleanerRange(out int start, out int end))
                return;

            cleanerRangeStartIndex = start;
            cleanerRangeEndIndex = end;
        }

        private void ResetCleanerRange()
        {
            cleanerRangeStartIndex = -1;
            cleanerRangeEndIndex = -1;
            cleanerRangePlayIndex = -1;
            isDraggingCleanerRange = false;

            cleanerRangePlayTimer.Stop();

            if (btnPlayRange != null)
                btnPlayRange.Text = "구간 재생";

            UpdateCleanerRangeUi();

            if (pnlCleanerTimeline != null)
                pnlCleanerTimeline.Invalidate();
        }

        private void UpdateCleanerRangeUi()
        {
            if (lblCleanerRangeInfo == null)
                return;

            if (!TryGetNormalizedCleanerRange(out int start, out int end))
            {
                lblCleanerRangeInfo.Text = "선택 구간: 없음";
                return;
            }

            int count = end - start + 1;

            lblCleanerRangeInfo.Text =
                $"선택 구간: {start + 1} ~ {end + 1} / {visibleFrames.Count}    선택: {count}개";
        }

        private void pnlCleanerTimeline_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = pnlCleanerTimeline.ClientRectangle;
            rect.Inflate(-6, -8);

            using SolidBrush bgBrush = new SolidBrush(Color.FromArgb(18, 26, 42));
            g.FillRectangle(bgBrush, pnlCleanerTimeline.ClientRectangle);

            if (visibleFrames.Count == 0)
            {
                using Font font = new Font("맑은 고딕", 9, FontStyle.Bold);
                using SolidBrush brush = new SolidBrush(Color.White);
                g.DrawString("로드된 프레임이 없습니다.", font, brush, 12, 12);
                return;
            }

            int count = visibleFrames.Count;
            float itemWidth = Math.Max(1f, (float)rect.Width / count);

            for (int i = 0; i < count; i++)
            {
                DonkeyFrame frame = visibleFrames[i];

                float x = rect.Left + i * itemWidth;
                float w = Math.Max(1f, itemWidth);

                Color frameColor;

                if (Math.Abs(frame.Throttle) <= 0.000001)
                    frameColor = Color.FromArgb(80, 80, 80);
                else if (Math.Abs(frame.Angle) <= 0.000001)
                    frameColor = Color.FromArgb(70, 95, 130);
                else if (frame.Angle > 0)
                    frameColor = Color.FromArgb(85, 120, 180);
                else
                    frameColor = Color.FromArgb(55, 85, 135);

                using SolidBrush frameBrush = new SolidBrush(frameColor);
                g.FillRectangle(frameBrush, x, rect.Top, w, rect.Height);
            }

            if (TryGetNormalizedCleanerRange(out int start, out int end))
            {
                float startX = rect.Left + start * itemWidth;
                float endX = rect.Left + (end + 1) * itemWidth;

                using SolidBrush rangeBrush = new SolidBrush(Color.FromArgb(120, 245, 180, 45));
                using Pen rangePen = new Pen(Color.FromArgb(245, 180, 45), 3);

                g.FillRectangle(rangeBrush, startX, rect.Top, endX - startX, rect.Height);
                g.DrawRectangle(rangePen, startX, rect.Top, endX - startX, rect.Height);

                using Pen handlePen = new Pen(Color.FromArgb(255, 230, 80), 4);
                g.DrawLine(handlePen, startX, rect.Top - 3, startX, rect.Bottom + 3);
                g.DrawLine(handlePen, endX, rect.Top - 3, endX, rect.Bottom + 3);
            }

            if (currentIndex >= 0 && currentIndex < count)
            {
                float currentX = rect.Left + currentIndex * itemWidth;

                using Pen currentPen = new Pen(Color.White, 2);
                g.DrawLine(currentPen, currentX, rect.Top - 5, currentX, rect.Bottom + 5);
            }

            using Pen borderPen = new Pen(Color.FromArgb(120, 255, 255, 255));
            g.DrawRectangle(borderPen, rect);
        }

        private void btnBrowseMycar_Click(object? sender, EventArgs e)
        {
            using FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "mycar 폴더를 선택하세요.";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtMycarPath.Text = dlg.SelectedPath;
                UpdateModelStatus();
            }
        }

        private async void btnTrain_Click(object? sender, EventArgs e)
        {
            string mycarPath = txtMycarPath.Text.Trim();
            string pythonExe = txtPythonExe.Text.Trim();
            string trainArgs = txtTrainArgs.Text.Trim();

            if (string.IsNullOrWhiteSpace(mycarPath))
            {
                MessageBox.Show("mycar 경로를 입력하세요.\n예: ~/mycar");
                return;
            }

            if (string.IsNullOrWhiteSpace(pythonExe))
            {
                MessageBox.Show("Python 실행명을 입력하세요.\nWSL을 사용할 경우 wsl을 입력하세요.");
                return;
            }

            if (string.IsNullOrWhiteSpace(trainArgs))
            {
                MessageBox.Show("학습 명령 인자를 입력하세요.");
                return;
            }

            bool useWsl = IsWslMode(pythonExe);

            if (!useWsl && !Directory.Exists(mycarPath))
            {
                MessageBox.Show(
                    "Windows 경로의 mycar 폴더를 찾을 수 없습니다.\n" +
                    "WSL을 사용할 경우 Python 실행명에 wsl을 입력하고 mycar 경로는 ~/mycar로 입력하세요."
                );
                return;
            }

            txtLog.Clear();

            AppendLog("학습 시작");
            AppendLog("실행 방식: " + (useWsl ? "WSL + Conda" : "Windows Python"));
            AppendLog("mycar 경로 = " + mycarPath);
            AppendLog("학습 인자 = " + trainArgs);

            ProcessStartInfo psi;

            if (useWsl)
                psi = CreateWslTrainProcessStartInfo(mycarPath, trainArgs);
            else
                psi = CreateLocalTrainProcessStartInfo(pythonExe, mycarPath, trainArgs);

            try
            {
                trainProcess = new Process();
                trainProcess.StartInfo = psi;
                trainProcess.EnableRaisingEvents = true;

                trainProcess.OutputDataReceived += (s, ev) =>
                {
                    if (!string.IsNullOrWhiteSpace(ev.Data))
                    {
                        BeginInvoke(new Action(() =>
                        {
                            AppendLog(ev.Data);
                        }));
                    }
                };

                trainProcess.ErrorDataReceived += (s, ev) =>
                {
                    if (!string.IsNullOrWhiteSpace(ev.Data))
                    {
                        BeginInvoke(new Action(() =>
                        {
                            AppendLog("[ERR] " + ev.Data);
                        }));
                    }
                };

                trainProcess.Start();
                trainProcess.BeginOutputReadLine();
                trainProcess.BeginErrorReadLine();

                await Task.Run(() => trainProcess.WaitForExit());

                AppendLog("학습 종료. ExitCode = " + trainProcess.ExitCode);

                trainProcess.Dispose();
                trainProcess = null;

                UpdateModelStatus();
            }
            catch (Exception ex)
            {
                AppendLog("학습 실행 실패: " + ex.Message);

                MessageBox.Show(
                    "학습 실행에 실패했습니다.\n\n" +
                    "확인할 것:\n" +
                    "1. WSL 이름이 맞는지 확인\n" +
                    "2. Conda 환경 이름이 맞는지 확인\n" +
                    "3. ~/mycar 폴더 안에 train.py와 data 폴더가 있는지 확인\n" +
                    "4. ~/mycar/data 안에 manifest.json이 있는지 확인\n" +
                    "5. Ubuntu 터미널에서 직접 학습 명령이 되는지 확인\n\n" +
                    ex.Message
                );
            }
        }

        private void btnStopTrain_Click(object? sender, EventArgs e)
        {
            try
            {
                if (trainProcess != null && !trainProcess.HasExited)
                {
                    trainProcess.Kill(true);
                    AppendLog("학습 프로세스 중지 요청 완료");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("학습 중지 실패\n\n" + ex.Message);
            }
        }

        private void btnBrowseModel_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "모델 파일 선택";
            dlg.Filter = "Keras Model (*.h5)|*.h5|All Files (*.*)|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                modelFilePath = dlg.FileName;
                txtModelPath.Text = modelFilePath;
                AppendLog("모델 파일 선택: " + modelFilePath);
            }
        }

        private void btnUseViewerFrame_Click(object? sender, EventArgs e)
        {
            if (currentIndex < 0 || currentIndex >= visibleFrames.Count)
            {
                MessageBox.Show("먼저 Viewer 탭에서 사용할 이미지를 선택하세요.");
                return;
            }

            isUpdatingSelection = true;

            if (lstPilotFrames.SelectedIndex != currentIndex)
                lstPilotFrames.SelectedIndex = currentIndex;

            isUpdatingSelection = false;

            ShowFrame(currentIndex);

            AppendLog($"Pilot Test 이미지 선택: frame index {currentIndex}");
        }

        private async void btnRunPilotTest_Click(object? sender, EventArgs e)
        {
            if (currentIndex < 0 || currentIndex >= visibleFrames.Count)
            {
                MessageBox.Show("먼저 테스트할 프레임을 선택하세요.");
                return;
            }

            DonkeyFrame frame = visibleFrames[currentIndex];

            string modelPath = txtModelPath.Text.Trim();

            if (string.IsNullOrWhiteSpace(modelPath))
            {
                modelPath = "~/mycar/models/mypilot.h5";
                txtModelPath.Text = modelPath;
            }

            string imagePath = Path.Combine(imagesFolderPath, frame.ImageFileName);

            overlayActualAngle = null;
            overlayPredictedAngle = null;
            overlayActualThrottle = null;
            overlayPredictedThrottle = null;
            picPilotTest.Invalidate();

            lblActualAngle.Text = $"실제 Angle: {frame.Angle:F4}";
            lblActualThrottle.Text = $"실제 Throttle: {frame.Throttle:F4}";
            lblPredictedAngle.Text = "예측 Angle: 실행 중...";
            lblPredictedThrottle.Text = "예측 Throttle: 실행 중...";
            lblAngleError.Text = "Angle Error: 계산 중...";
            lblPilotWarning.Text = "판정: 예측 실행 중";
            lblPilotWarning.ForeColor = Color.DimGray;

            LoadImageToPictureBox(picPilotTest, imagePath);

            AppendLog("Pilot Test 시작");
            AppendLog("Model = " + modelPath);
            AppendLog("Image = " + imagePath);

            try
            {
                (double predictedAngle, double predictedThrottle) =
                    await RunPredictOneInWslAsync(modelPath, imagePath);

                double angleError = Math.Abs(frame.Angle - predictedAngle);

                lblPredictedAngle.Text = $"예측 Angle: {predictedAngle:F4}";
                lblPredictedThrottle.Text = $"예측 Throttle: {predictedThrottle:F4}";
                lblAngleError.Text = $"Angle Error: {angleError:F4}";

                Color warningColor = GetErrorColor(angleError);
                lblPilotWarning.ForeColor = warningColor;
                lblPilotWarning.Text = "판정: " + GetErrorMessage(angleError);

                overlayActualAngle = frame.Angle;
                overlayPredictedAngle = predictedAngle;
                overlayActualThrottle = frame.Throttle;
                overlayPredictedThrottle = predictedThrottle;

                picPilotTest.Invalidate();

                AppendLog($"실제 Angle = {frame.Angle:F4}");
                AppendLog($"예측 Angle = {predictedAngle:F4}");
                AppendLog($"Angle Error = {angleError:F4}");
                AppendLog($"실제 Throttle = {frame.Throttle:F4}");
                AppendLog($"예측 Throttle = {predictedThrottle:F4}");
                AppendLog("Pilot Test 완료");
            }
            catch (Exception ex)
            {
                lblPredictedAngle.Text = "예측 Angle: 실패";
                lblPredictedThrottle.Text = "예측 Throttle: 실패";
                lblAngleError.Text = "Angle Error: 실패";
                lblPilotWarning.Text = "판정: 예측 실패";
                lblPilotWarning.ForeColor = Color.Red;

                AppendLog("Pilot Test 실패: " + ex.Message);

                MessageBox.Show(
                    "예측 테스트 실행에 실패했습니다.\n\n" +
                    "확인할 것:\n" +
                    "1. ~/mycar/predict_one.py 파일이 있는지 확인\n" +
                    "2. ~/mycar/models/mypilot.h5 파일이 있는지 확인\n" +
                    "3. 현재 선택한 이미지 파일이 실제로 존재하는지 확인\n" +
                    "4. WSL 이름과 Conda 환경 이름이 맞는지 확인\n\n" +
                    ex.Message
                );
            }
        }

        private void lstFrames_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (isUpdatingSelection)
                return;

            if (lstFrames.SelectedIndex >= 0 && lstFrames.SelectedIndex < visibleFrames.Count)
                ShowFrame(lstFrames.SelectedIndex);
        }

        private void lstCleanerFrames_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (isUpdatingSelection)
                return;

            if (lstCleanerFrames.SelectedIndices.Count == 0)
                return;

            int lastSelectedIndex = lstCleanerFrames.SelectedIndices[lstCleanerFrames.SelectedIndices.Count - 1];

            if (lastSelectedIndex >= 0 && lastSelectedIndex < visibleFrames.Count)
                ShowFrame(lastSelectedIndex);
        }

        private void lstPilotFrames_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (isUpdatingSelection)
                return;

            if (lstPilotFrames.SelectedIndex >= 0 && lstPilotFrames.SelectedIndex < visibleFrames.Count)
                ShowFrame(lstPilotFrames.SelectedIndex);
        }

        private void trbFrame_Scroll(object? sender, EventArgs e)
        {
            ShowFrame(trbFrame.Value);
        }

        private void LoadCatalog()
        {
            allFrames.Clear();

            try
            {
                string[] catalogFiles = GetCatalogFiles();

                if (catalogFiles.Length == 0)
                {
                    MessageBox.Show("catalog_*.catalog 파일을 찾을 수 없습니다.");
                    return;
                }

                int totalLines = 0;
                int parseErrorCount = 0;

                foreach (string catalogPath in catalogFiles)
                {
                    foreach (string line in File.ReadLines(catalogPath))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        totalLines++;

                        try
                        {
                            DonkeyFrame? frame = JsonSerializer.Deserialize<DonkeyFrame>(line);

                            if (frame != null && !string.IsNullOrWhiteSpace(frame.ImageFileName))
                                allFrames.Add(frame);
                        }
                        catch
                        {
                            parseErrorCount++;
                        }
                    }
                }

                allFrames = allFrames
                    .GroupBy(f => MakeFrameKey(f))
                    .Select(g => g.First())
                    .OrderBy(f => f.Index)
                    .ToList();

                visibleFrames = allFrames.ToList();

                BindFrameLists();
                SetupTrackBar();
                ResetCleanerRange();

                if (visibleFrames.Count > 0)
                    ShowFrame(0);
                else
                    ClearViewer();

                AppendLog(
                    $"로드 완료: {visibleFrames.Count}개 프레임 / catalog 파일 {catalogFiles.Length}개 / 전체 줄 {totalLines}개"
                );

                if (parseErrorCount > 0)
                    AppendLog($"catalog 파싱 실패 줄: {parseErrorCount}개");

                UpdateModelStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("catalog 파일을 읽는 중 오류가 발생했습니다.\n\n" + ex.Message);
            }
        }

        private string[] GetCatalogFiles()
        {
            if (string.IsNullOrWhiteSpace(dataFolderPath) || !Directory.Exists(dataFolderPath))
                return Array.Empty<string>();

            return Directory
                .GetFiles(dataFolderPath, "catalog_*.catalog")
                .OrderBy(GetCatalogNumber)
                .ThenBy(path => path)
                .ToArray();
        }

        private int GetCatalogNumber(string catalogPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(catalogPath);
            string[] parts = fileName.Split('_');

            if (parts.Length >= 2 && int.TryParse(parts[parts.Length - 1], out int number))
                return number;

            return int.MaxValue;
        }

        private void BackupCatalogFiles()
        {
            try
            {
                string[] catalogFiles = GetCatalogFiles();

                if (catalogFiles.Length == 0)
                    return;

                string backupRoot = Path.Combine(dataFolderPath, "catalog_backup");
                Directory.CreateDirectory(backupRoot);

                string backupFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFolder = Path.Combine(backupRoot, backupFolderName);
                Directory.CreateDirectory(backupFolder);

                foreach (string catalogFile in catalogFiles)
                {
                    string dest = Path.Combine(backupFolder, Path.GetFileName(catalogFile));
                    File.Copy(catalogFile, dest, true);
                }

                AppendLog($"catalog 백업 완료: {backupFolder}");
            }
            catch (Exception ex)
            {
                AppendLog("catalog 백업 실패: " + ex.Message);
            }
        }

        private void BindFrameLists()
        {
            isUpdatingSelection = true;

            lstFrames.BeginUpdate();
            lstCleanerFrames.BeginUpdate();
            lstPilotFrames.BeginUpdate();

            lstFrames.Items.Clear();
            lstCleanerFrames.Items.Clear();
            lstPilotFrames.Items.Clear();

            foreach (DonkeyFrame frame in visibleFrames)
            {
                string text =
                    $"{frame.Index:D5} | angle={frame.Angle:F3} | throttle={frame.Throttle:F3} | mode={frame.Mode}";

                lstFrames.Items.Add(text);
                lstCleanerFrames.Items.Add(text);
                lstPilotFrames.Items.Add(text);
            }

            lstFrames.EndUpdate();
            lstCleanerFrames.EndUpdate();
            lstPilotFrames.EndUpdate();

            isUpdatingSelection = false;

            pnlCleanerTimeline.Invalidate();
            UpdateCleanerRangeUi();
        }

        private void SetupTrackBar()
        {
            trbFrame.Minimum = 0;
            trbFrame.Maximum = Math.Max(0, visibleFrames.Count - 1);
            trbFrame.Value = 0;
        }

        private void ShowFrame(int index)
        {
            if (index < 0 || index >= visibleFrames.Count)
                return;

            currentIndex = index;
            DonkeyFrame frame = visibleFrames[index];

            string imagePath = Path.Combine(imagesFolderPath, frame.ImageFileName);

            LoadImageToPictureBox(picFrame, imagePath);
            LoadImageToPictureBox(picCleanerPreview, imagePath);
            LoadImageToPictureBox(picPilotTest, imagePath);

            overlayActualAngle = null;
            overlayPredictedAngle = null;
            overlayActualThrottle = null;
            overlayPredictedThrottle = null;
            picPilotTest.Invalidate();

            lblFrameInfo.Text = $"Frame: {index + 1} / {visibleFrames.Count}";
            lblAngle.Text = $"Angle: {frame.Angle:F4}";
            lblThrottle.Text = $"Throttle: {frame.Throttle:F4}";
            lblMode.Text = $"Mode: {frame.Mode}";

            lblCleanerInfo.Text =
                $"선택 프레임 정보: index={frame.Index}, angle={frame.Angle:F4}, throttle={frame.Throttle:F4}, mode={frame.Mode}";

            lblActualAngle.Text = $"실제 Angle: {frame.Angle:F4}";
            lblActualThrottle.Text = $"실제 Throttle: {frame.Throttle:F4}";
            lblPredictedAngle.Text = "예측 Angle: -";
            lblPredictedThrottle.Text = "예측 Throttle: -";
            lblAngleError.Text = "Angle Error: -";
            lblPilotWarning.Text = "판정: -";
            lblPilotWarning.ForeColor = Color.DimGray;

            if (trbFrame.Value != index)
                trbFrame.Value = index;

            isUpdatingSelection = true;

            if (lstFrames.SelectedIndex != index)
                lstFrames.SelectedIndex = index;

            if (!lstCleanerFrames.Focused)
            {
                if (lstCleanerFrames.SelectedIndex != index)
                {
                    lstCleanerFrames.ClearSelected();
                    lstCleanerFrames.SelectedIndex = index;
                }
            }

            if (lstPilotFrames.SelectedIndex != index)
                lstPilotFrames.SelectedIndex = index;

            isUpdatingSelection = false;

            pnlCleanerTimeline.Invalidate();
        }

        private void LoadImageToPictureBox(PictureBox pictureBox, string imagePath)
        {
            try
            {
                if (pictureBox.Image != null)
                {
                    pictureBox.Image.Dispose();
                    pictureBox.Image = null;
                }

                if (File.Exists(imagePath))
                {
                    byte[] bytes = File.ReadAllBytes(imagePath);

                    using MemoryStream ms = new MemoryStream(bytes);
                    using Bitmap temp = new Bitmap(ms);

                    pictureBox.Image = new Bitmap(temp);
                }
            }
            catch
            {
                pictureBox.Image = null;
                AppendLog("이미지 로드 실패: " + Path.GetFileName(imagePath));
            }
        }

        private void ApplyFilter()
        {
            IEnumerable<DonkeyFrame> query = allFrames;

            if (chkThrottlePositive.Checked)
                query = query.Where(f => f.Throttle > 0);

            if (chkExcludeZeroAngle.Checked)
                query = query.Where(f => Math.Abs(f.Angle) > 0.000001);

            if (chkStopDataOnly.Checked)
                query = query.Where(f => Math.Abs(f.Throttle) <= 0.000001);

            visibleFrames = query.ToList();

            BindFrameLists();
            SetupTrackBar();
            ResetCleanerRange();

            if (visibleFrames.Count > 0)
                ShowFrame(0);
            else
                ClearViewer();

            AppendLog($"필터 적용 후 {visibleFrames.Count}개 프레임");
        }

        private void ClearViewer()
        {
            currentIndex = -1;

            DisposeCurrentImages();

            overlayActualAngle = null;
            overlayPredictedAngle = null;
            overlayActualThrottle = null;
            overlayPredictedThrottle = null;

            lblFrameInfo.Text = "Frame: -";
            lblAngle.Text = "Angle: -";
            lblThrottle.Text = "Throttle: -";
            lblMode.Text = "Mode: -";
            lblCleanerInfo.Text = "선택 프레임 정보: -";

            lblActualAngle.Text = "실제 Angle: -";
            lblPredictedAngle.Text = "예측 Angle: -";
            lblActualThrottle.Text = "실제 Throttle: -";
            lblPredictedThrottle.Text = "예측 Throttle: -";
            lblAngleError.Text = "Angle Error: -";
            lblPilotWarning.Text = "판정: -";
            lblPilotWarning.ForeColor = Color.DimGray;

            isUpdatingSelection = true;
            lstFrames.Items.Clear();
            lstCleanerFrames.Items.Clear();
            lstPilotFrames.Items.Clear();
            isUpdatingSelection = false;

            trbFrame.Minimum = 0;
            trbFrame.Maximum = 0;
            trbFrame.Value = 0;

            ResetCleanerRange();
        }

        private void SaveCatalog()
        {
            try
            {
                BackupCatalogFiles();

                string[] oldCatalogFiles = GetCatalogFiles();

                foreach (string oldCatalog in oldCatalogFiles)
                {
                    if (File.Exists(oldCatalog))
                        File.Delete(oldCatalog);
                }

                List<DonkeyFrame> orderedFrames = allFrames
                    .OrderBy(f => f.Index)
                    .ToList();

                int catalogIndex = 0;
                int writtenCount = 0;

                for (int i = 0; i < orderedFrames.Count; i += CatalogChunkSize)
                {
                    List<string> lines = new List<string>();

                    List<DonkeyFrame> chunk = orderedFrames
                        .Skip(i)
                        .Take(CatalogChunkSize)
                        .ToList();

                    foreach (DonkeyFrame frame in chunk)
                    {
                        Dictionary<string, object?> obj = new Dictionary<string, object?>
                        {
                            ["_index"] = frame.Index,
                            ["_session_id"] = frame.SessionId,
                            ["_timestamp_ms"] = frame.TimestampMs,
                            ["cam/image_array"] = frame.ImageFileName,
                            ["user/angle"] = frame.Angle,
                            ["user/mode"] = frame.Mode,
                            ["user/throttle"] = frame.Throttle
                        };

                        string json = JsonSerializer.Serialize(obj);
                        lines.Add(json);
                    }

                    string catalogPath = Path.Combine(dataFolderPath, $"catalog_{catalogIndex}.catalog");
                    File.WriteAllLines(catalogPath, lines);

                    writtenCount += lines.Count;
                    catalogIndex++;
                }

                catalogFilePath = Path.Combine(dataFolderPath, "catalog_0.catalog");

                AppendLog(
                    $"catalog 저장 완료: {writtenCount}개 프레임 / catalog 파일 {catalogIndex}개로 분할 저장"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("catalog 저장 중 오류가 발생했습니다.\n\n" + ex.Message);
            }
        }

        private bool IsWslMode(string pythonExe)
        {
            return pythonExe.Equals("wsl", StringComparison.OrdinalIgnoreCase)
                || pythonExe.Equals("wsl.exe", StringComparison.OrdinalIgnoreCase);
        }

        private ProcessStartInfo CreateLocalTrainProcessStartInfo(string pythonExe, string mycarPath, string trainArgs)
        {
            return new ProcessStartInfo
            {
                FileName = pythonExe,
                Arguments = trainArgs,
                WorkingDirectory = mycarPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        private ProcessStartInfo CreateWslTrainProcessStartInfo(string mycarPath, string trainArgs)
        {
            string wslMycarPath = ConvertPathToWslPath(mycarPath);

            string command =
                "source ~/miniconda3/etc/profile.d/conda.sh && " +
                $"conda activate {CondaEnvName} && " +
                $"cd {BashCdArgument(wslMycarPath)} && " +
                $"python {trainArgs}";

            AppendLog("WSL Train Command = " + command);

            return new ProcessStartInfo
            {
                FileName = "wsl.exe",
                Arguments = $"-d {WslDistroName} -- bash -lc {QuoteWindowsArgument(command)}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        private async Task<(double angle, double throttle)> RunPredictOneInWslAsync(string modelPath, string imagePath)
        {
            string wslModelPath = ConvertPathToWslPath(modelPath);
            string wslImagePath = ConvertPathToWslPath(imagePath);

            string command =
                "source ~/miniconda3/etc/profile.d/conda.sh && " +
                $"conda activate {CondaEnvName} && " +
                "cd ~/mycar && " +
                $"python predict_one.py --model {BashQuote(wslModelPath)} --image {BashQuote(wslImagePath)}";

            AppendLog("WSL Predict Command = " + command);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "wsl.exe",
                Arguments = $"-d {WslDistroName} -- bash -lc {QuoteWindowsArgument(command)}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            StringBuilder outputBuilder = new StringBuilder();
            StringBuilder errorBuilder = new StringBuilder();

            using Process process = new Process();
            process.StartInfo = psi;

            process.OutputDataReceived += (s, ev) =>
            {
                if (!string.IsNullOrWhiteSpace(ev.Data))
                {
                    outputBuilder.AppendLine(ev.Data);

                    BeginInvoke(new Action(() =>
                    {
                        AppendLog(ev.Data);
                    }));
                }
            };

            process.ErrorDataReceived += (s, ev) =>
            {
                if (!string.IsNullOrWhiteSpace(ev.Data))
                {
                    errorBuilder.AppendLine(ev.Data);

                    BeginInvoke(new Action(() =>
                    {
                        AppendLog("[ERR] " + ev.Data);
                    }));
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await Task.Run(() => process.WaitForExit());

            string stdout = outputBuilder.ToString();
            string stderr = errorBuilder.ToString();

            if (process.ExitCode != 0)
            {
                throw new Exception(
                    "Python 예측 스크립트가 실패했습니다.\n\n" +
                    stderr
                );
            }

            string? jsonLine = stdout
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault(line => line.TrimStart().StartsWith("{"));

            if (string.IsNullOrWhiteSpace(jsonLine))
            {
                throw new Exception(
                    "Python 예측 결과 JSON을 찾지 못했습니다.\n\n출력:\n" +
                    stdout
                );
            }

            using JsonDocument doc = JsonDocument.Parse(jsonLine);
            JsonElement root = doc.RootElement;

            bool ok = root.GetProperty("ok").GetBoolean();

            if (!ok)
            {
                string error = root.TryGetProperty("error", out JsonElement err)
                    ? err.GetString() ?? "Unknown error"
                    : "Unknown error";

                throw new Exception(error);
            }

            double angle = root.GetProperty("angle").GetDouble();
            double throttle = root.GetProperty("throttle").GetDouble();

            return (angle, throttle);
        }

        private string ConvertPathToWslPath(string path)
        {
            path = path.Trim().Trim('"');

            if (path.StartsWith("~/"))
                return path;

            if (path.StartsWith("/"))
                return path;

            string prefix1 = @"\\wsl.localhost\" + WslDistroName + @"\";
            string prefix2 = @"\\wsl$\" + WslDistroName + @"\";

            if (path.StartsWith(prefix1, StringComparison.OrdinalIgnoreCase))
            {
                string relative = path.Substring(prefix1.Length);
                return "/" + relative.Replace("\\", "/");
            }

            if (path.StartsWith(prefix2, StringComparison.OrdinalIgnoreCase))
            {
                string relative = path.Substring(prefix2.Length);
                return "/" + relative.Replace("\\", "/");
            }

            if (path.Length >= 3 && path[1] == ':' && path[2] == '\\')
            {
                char drive = char.ToLower(path[0]);
                string rest = path.Substring(3).Replace("\\", "/");
                return $"/mnt/{drive}/{rest}";
            }

            return path.Replace("\\", "/");
        }

        private string BashCdArgument(string value)
        {
            if (value.StartsWith("~/"))
                return value;

            return BashQuote(value);
        }

        private string BashQuote(string value)
        {
            return "'" + value.Replace("'", "'\"'\"'") + "'";
        }

        private string QuoteWindowsArgument(string value)
        {
            return "\"" + value.Replace("\"", "\\\"") + "\"";
        }

        private void UpdateModelStatus()
        {
            string mycarPath = txtMycarPath.Text.Trim();

            if (string.IsNullOrWhiteSpace(mycarPath))
            {
                lblModelStatus.Text = "모델 상태: mycar 경로 없음";
                return;
            }

            if (mycarPath.StartsWith("~/"))
            {
                lblModelStatus.Text = "모델 상태: WSL 경로 사용 중";
                return;
            }

            string modelPath = Path.Combine(mycarPath, "models", "mypilot.h5");

            if (File.Exists(modelPath))
                lblModelStatus.Text = "모델 상태: mypilot.h5 존재";
            else
                lblModelStatus.Text = "모델 상태: mypilot.h5 없음";
        }

        private void DisposeCurrentImages()
        {
            if (picFrame != null && picFrame.Image != null)
            {
                picFrame.Image.Dispose();
                picFrame.Image = null;
            }

            if (picCleanerPreview != null && picCleanerPreview.Image != null)
            {
                picCleanerPreview.Image.Dispose();
                picCleanerPreview.Image = null;
            }

            if (picPilotTest != null && picPilotTest.Image != null)
            {
                picPilotTest.Image.Dispose();
                picPilotTest.Image = null;
            }
        }

        private void AppendLog(string message)
        {
            if (txtLog == null)
                return;

            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }

        private void picPilotTest_Paint(object? sender, PaintEventArgs e)
        {
            if (picPilotTest.Image == null)
                return;

            Rectangle imgRect = GetZoomedImageRectangle(picPilotTest);

            if (imgRect.Width <= 0 || imgRect.Height <= 0)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            DrawCenterGuideLine(e.Graphics, imgRect);

            if (overlayActualAngle.HasValue && overlayPredictedAngle.HasValue)
                DrawSteeringGapArea(e.Graphics, imgRect, overlayActualAngle.Value, overlayPredictedAngle.Value);

            if (overlayActualAngle.HasValue)
            {
                DrawSteeringOverlay(
                    e.Graphics,
                    imgRect,
                    overlayActualAngle.Value,
                    Color.DeepSkyBlue,
                    5f,
                    "Actual"
                );
            }

            if (overlayPredictedAngle.HasValue)
            {
                DrawSteeringOverlay(
                    e.Graphics,
                    imgRect,
                    overlayPredictedAngle.Value,
                    Color.LimeGreen,
                    5f,
                    "Pred"
                );
            }

            DrawThrottleBars(e.Graphics, imgRect);
            DrawErrorPanel(e.Graphics, imgRect);
            DrawOverlayLegend(e.Graphics, imgRect);
        }

        private void DrawSteeringOverlay(Graphics g, Rectangle imgRect, double angle, Color color, float width, string label)
        {
            (PointF start, PointF end) = CalculateSteeringLine(imgRect, angle);

            using Pen shadowPen = new Pen(Color.FromArgb(130, 0, 0, 0), width + 3)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            using Pen mainPen = new Pen(color, width)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            g.DrawLine(shadowPen, start.X + 2, start.Y + 2, end.X + 2, end.Y + 2);
            g.DrawLine(mainPen, start, end);

            using Font font = new Font("맑은 고딕", 10, FontStyle.Bold);
            using SolidBrush brush = new SolidBrush(color);

            g.DrawString($"{label}: {angle:F3}", font, brush, end.X + 8, end.Y - 10);
        }

        private void DrawSteeringGapArea(Graphics g, Rectangle imgRect, double actualAngle, double predictedAngle)
        {
            (PointF startA, PointF endA) = CalculateSteeringLine(imgRect, actualAngle);
            (PointF startP, PointF endP) = CalculateSteeringLine(imgRect, predictedAngle);

            double error = Math.Abs(actualAngle - predictedAngle);
            Color errorColor = GetErrorColor(error);

            using GraphicsPath path = new GraphicsPath();
            path.AddPolygon(new PointF[]
            {
                startA,
                endA,
                endP
            });

            using SolidBrush brush = new SolidBrush(Color.FromArgb(80, errorColor));
            g.FillPath(brush, path);
        }

        private (PointF start, PointF end) CalculateSteeringLine(Rectangle imgRect, double angle)
        {
            double clamped = Math.Max(-1.0, Math.Min(1.0, angle));

            float startX = imgRect.Left + imgRect.Width / 2f;
            float startY = imgRect.Bottom - 16f;

            float lineLength = imgRect.Height * 0.45f;
            float maxHorizontalShift = imgRect.Width * 0.30f;

            float endX = startX + (float)(clamped * maxHorizontalShift);
            float endY = startY - lineLength;

            return (new PointF(startX, startY), new PointF(endX, endY));
        }

        private void DrawCenterGuideLine(Graphics g, Rectangle imgRect)
        {
            (PointF start, PointF end) = CalculateSteeringLine(imgRect, 0);

            using Pen guidePen = new Pen(Color.FromArgb(180, 255, 255, 255), 2f)
            {
                DashStyle = DashStyle.Dash,
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            g.DrawLine(guidePen, start, end);
        }

        private void DrawThrottleBars(Graphics g, Rectangle imgRect)
        {
            if (!overlayActualThrottle.HasValue && !overlayPredictedThrottle.HasValue)
                return;

            int panelX = imgRect.Left + 12;
            int panelY = imgRect.Bottom - 82;
            int panelW = 260;
            int panelH = 66;

            using SolidBrush bg = new SolidBrush(Color.FromArgb(145, 20, 20, 20));
            using Pen border = new Pen(Color.FromArgb(190, 255, 255, 255), 1);

            g.FillRectangle(bg, panelX, panelY, panelW, panelH);
            g.DrawRectangle(border, panelX, panelY, panelW, panelH);

            using Font font = new Font("맑은 고딕", 8, FontStyle.Bold);
            using SolidBrush white = new SolidBrush(Color.White);

            g.DrawString("Throttle", font, white, panelX + 10, panelY + 6);

            if (overlayActualThrottle.HasValue)
                DrawSingleThrottleBar(g, panelX + 85, panelY + 27, 150, 10, overlayActualThrottle.Value, Color.DeepSkyBlue, "A");

            if (overlayPredictedThrottle.HasValue)
                DrawSingleThrottleBar(g, panelX + 85, panelY + 47, 150, 10, overlayPredictedThrottle.Value, Color.LimeGreen, "P");
        }

        private void DrawSingleThrottleBar(Graphics g, int x, int y, int w, int h, double value, Color color, string label)
        {
            double clamped = Math.Max(0.0, Math.Min(1.0, value));
            int filled = (int)(w * clamped);

            using Font font = new Font("맑은 고딕", 8, FontStyle.Bold);
            using SolidBrush textBrush = new SolidBrush(Color.White);
            using SolidBrush fillBrush = new SolidBrush(color);
            using SolidBrush emptyBrush = new SolidBrush(Color.FromArgb(80, 255, 255, 255));
            using Pen borderPen = new Pen(Color.FromArgb(180, 255, 255, 255), 1);

            g.DrawString(label, font, textBrush, x - 22, y - 5);
            g.FillRectangle(emptyBrush, x, y, w, h);
            g.FillRectangle(fillBrush, x, y, filled, h);
            g.DrawRectangle(borderPen, x, y, w, h);
            g.DrawString(value.ToString("F3"), font, textBrush, x + w + 8, y - 5);
        }

        private void DrawErrorPanel(Graphics g, Rectangle imgRect)
        {
            if (!overlayActualAngle.HasValue || !overlayPredictedAngle.HasValue)
                return;

            double error = Math.Abs(overlayActualAngle.Value - overlayPredictedAngle.Value);
            Color errorColor = GetErrorColor(error);
            string msg = GetErrorMessage(error);

            int panelW = 230;
            int panelH = 72;
            int panelX = imgRect.Right - panelW - 12;
            int panelY = imgRect.Top + 12;

            using SolidBrush bg = new SolidBrush(Color.FromArgb(150, 20, 20, 20));
            using Pen border = new Pen(errorColor, 2);
            using Font titleFont = new Font("맑은 고딕", 9, FontStyle.Bold);
            using Font valueFont = new Font("맑은 고딕", 11, FontStyle.Bold);
            using SolidBrush white = new SolidBrush(Color.White);
            using SolidBrush colorBrush = new SolidBrush(errorColor);

            g.FillRectangle(bg, panelX, panelY, panelW, panelH);
            g.DrawRectangle(border, panelX, panelY, panelW, panelH);

            g.DrawString("Angle Error", titleFont, white, panelX + 10, panelY + 8);
            g.DrawString(error.ToString("F4"), valueFont, colorBrush, panelX + 10, panelY + 31);
            g.DrawString(msg, titleFont, colorBrush, panelX + 100, panelY + 34);
        }

        private void DrawOverlayLegend(Graphics g, Rectangle imgRect)
        {
            int boxX = imgRect.Left + 12;
            int boxY = imgRect.Top + 12;
            int boxW = 210;
            int boxH = 78;

            using SolidBrush bg = new SolidBrush(Color.FromArgb(145, 20, 20, 20));
            using Pen border = new Pen(Color.FromArgb(180, 255, 255, 255), 1);

            g.FillRectangle(bg, boxX, boxY, boxW, boxH);
            g.DrawRectangle(border, boxX, boxY, boxW, boxH);

            using Font font = new Font("맑은 고딕", 8, FontStyle.Bold);
            using SolidBrush whiteBrush = new SolidBrush(Color.White);
            using SolidBrush actualBrush = new SolidBrush(Color.DeepSkyBlue);
            using SolidBrush predBrush = new SolidBrush(Color.LimeGreen);
            using SolidBrush gapBrush = new SolidBrush(Color.Gold);

            g.DrawString("Guide: white dashed", font, whiteBrush, boxX + 10, boxY + 7);
            g.DrawString("● Actual Angle", font, actualBrush, boxX + 10, boxY + 25);
            g.DrawString("● Predicted Angle", font, predBrush, boxX + 10, boxY + 43);
            g.DrawString("■ Error Area", font, gapBrush, boxX + 10, boxY + 61);
        }

        private Rectangle GetZoomedImageRectangle(PictureBox pb)
        {
            if (pb.Image == null)
                return Rectangle.Empty;

            if (pb.ClientSize.Width <= 0 || pb.ClientSize.Height <= 0)
                return Rectangle.Empty;

            float imageRatio = (float)pb.Image.Width / pb.Image.Height;
            float boxRatio = (float)pb.ClientSize.Width / pb.ClientSize.Height;

            int drawWidth;
            int drawHeight;
            int drawX;
            int drawY;

            if (imageRatio > boxRatio)
            {
                drawWidth = pb.ClientSize.Width;
                drawHeight = (int)(pb.ClientSize.Width / imageRatio);
                drawX = 0;
                drawY = (pb.ClientSize.Height - drawHeight) / 2;
            }
            else
            {
                drawHeight = pb.ClientSize.Height;
                drawWidth = (int)(pb.ClientSize.Height * imageRatio);
                drawX = (pb.ClientSize.Width - drawWidth) / 2;
                drawY = 0;
            }

            return new Rectangle(drawX, drawY, drawWidth, drawHeight);
        }

        private Color GetErrorColor(double error)
        {
            if (error <= 0.05)
                return Color.LimeGreen;

            if (error <= 0.15)
                return Color.Orange;

            return Color.Red;
        }

        private string GetErrorMessage(double error)
        {
            if (error <= 0.05)
                return "Good";

            if (error <= 0.15)
                return "Warning";

            return "High Error";
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                autoPlayTimer.Stop();
                autoPlayTimer.Dispose();

                cleanerRangePlayTimer.Stop();
                cleanerRangePlayTimer.Dispose();

                if (trainProcess != null && !trainProcess.HasExited)
                    trainProcess.Kill(true);

                trainProcess?.Dispose();
                DisposeCurrentImages();
            }
            catch
            {
            }

            base.OnFormClosed(e);
        }
    }
}
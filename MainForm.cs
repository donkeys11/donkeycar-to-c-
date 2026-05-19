using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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

        // 폼의 전역 변수로 강조 렌즈(Indicator) 역할을 할 패널을 추가합니다.
        private Panel? indicatorPanel;

        // 폼의 전역 변수로 강조 렌즈(포커스 링) 역할을 할 Label을 추가합니다.
        private Label? indicatorRing;

        public MainForm()
        {
            InitializeComponent();
            ConnectEvents();

            txtPythonExe.Text = "python";
            txtTrainArgs.Text = "train.py --tub ./data --model ./models/mypilot.h5";

            autoPlayTimer.Interval = 150;
            autoPlayTimer.Tick += AutoPlayTimer_Tick;

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

            btnBrowseMycar.Click += btnBrowseMycar_Click;
            btnTrain.Click += btnTrain_Click;
            btnStopTrain.Click += btnStopTrain_Click;

            btnBrowseModel.Click += btnBrowseModel_Click;
            btnRunPilotTest.Click += btnRunPilotTest_Click;

            lstFrames.SelectedIndexChanged += lstFrames_SelectedIndexChanged;
            lstCleanerFrames.SelectedIndexChanged += lstCleanerFrames_SelectedIndexChanged;
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
            catalogFilePath = Path.Combine(dataFolderPath, "catalog_0.catalog");

            if (!Directory.Exists(imagesFolderPath))
            {
                MessageBox.Show("선택한 폴더 안에 images 폴더가 없습니다.\nmycar 폴더가 아니라 mycar/data 폴더를 선택해야 합니다.");
                return;
            }

            if (!File.Exists(catalogFilePath))
            {
                MessageBox.Show("선택한 폴더 안에 catalog_0.catalog 파일이 없습니다.");
                return;
            }

            DirectoryInfo? parent = Directory.GetParent(dataFolderPath);
            if (parent != null)
                txtMycarPath.Text = parent.FullName;

            lblDataPath.Text = "Data Folder: " + dataFolderPath;

            LoadCatalog();
        }

        private void btnReload_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(catalogFilePath) || !File.Exists(catalogFilePath))
            {
                MessageBox.Show("먼저 Donkeycar data 폴더를 열어주세요.");
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

            // 모든 프레임이 다 선택 상태라면 (필터링되어 볼 이미지가 없다면) 자동재생 중지
            if (lstCleanerFrames.SelectedIndices.Count == visibleFrames.Count)
            {
                autoPlayTimer.Enabled = false;
                btnAutoPlay.Text = "자동 재생";
                AppendLog("모든 프레임이 필터링에 걸려 자동 재생을 중지합니다.");
                return;
            }

            int next = currentIndex + 1;
            int maxChecks = visibleFrames.Count; // 무한루프 방지

            for (int i = 0; i < maxChecks; i++)
            {
                if (next >= visibleFrames.Count)
                    next = 0;

                // 백업/삭제 대상인 프레임(선택 목록에 포함된 프레임)이 아니면 해당 프레임 재생
                if (!lstCleanerFrames.SelectedIndices.Contains(next))
                {
                    ShowFrame(next);
                    return;
                }

                next++;
            }
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

            if (visibleFrames.Count > 0)
                ShowFrame(0);
            else
                ClearViewer();

            PopulateTimeline(); // <--- 여기 추가!

            AppendLog("필터 해제: 전체 데이터 표시");
        }

        private void btnDeleteFrame_Click(object? sender, EventArgs e)
        {
            List<DonkeyFrame> framesToDelete = new List<DonkeyFrame>();

            // Cleaner 리스트에서 다중 선택된 항목이 있으면 그 항목들을 삭제 대상에 추가
            if (lstCleanerFrames.SelectedIndices.Count > 0)
            {
                foreach (int index in lstCleanerFrames.SelectedIndices)
                {
                    framesToDelete.Add(visibleFrames[index]);
                }
            }
            // 다중 선택된게 없다면 현재 선택(currentIndex) 프레임 추가
            else if (currentIndex >= 0 && currentIndex < visibleFrames.Count)
            {
                framesToDelete.Add(visibleFrames[currentIndex]);
            }

            if (framesToDelete.Count == 0)
            {
                MessageBox.Show("삭제할 프레임을 먼저 선택하세요.");
                return;
            }

            try
            {
                DisposeCurrentImages();

                // backup 폴더 생성
                string backupFolderPath = Path.Combine(dataFolderPath, "backup");
                if (!Directory.Exists(backupFolderPath))
                {
                    Directory.CreateDirectory(backupFolderPath);
                }

                // backup/images 서브 폴더 생성 (이미지를 백업할 폴더)
                string backupImagesFolderPath = Path.Combine(backupFolderPath, "images");
                if (!Directory.Exists(backupImagesFolderPath))
                {
                    Directory.CreateDirectory(backupImagesFolderPath);
                }

                // 삭제 작업 전에 모든 카탈로그 파일들을 backup 루트 폴더로 복사
                string[] catalogFiles = Directory.GetFiles(dataFolderPath, "catalog_*.catalog");
                foreach (string catalogFile in catalogFiles)
                {
                    string fileName = Path.GetFileName(catalogFile);
                    string backupCatalogPath = Path.Combine(backupFolderPath, fileName);

                    // 원본 카탈로그를 백업 폴더로 복사 (이미 있으면 덮어쓰기)
                    File.Copy(catalogFile, backupCatalogPath, true);

                    // 향후 SaveCatalog()가 전체 데이터를 catalog_0.catalog 하나로 통합하여 저장하므로,
                    // 다음 로드 시 데이터가 중복으로 나오는 것을 방지하기 위해 원본 폴더의 나머지 분할 파일들은 삭제 (백업에 남아있음)
                    if (fileName.ToLower() != "catalog_0.catalog")
                    {
                        File.Delete(catalogFile);
                    }
                }

                int deletedCount = 0;
                foreach (var frame in framesToDelete)
                {
                    string imagePath = Path.Combine(imagesFolderPath, frame.ImageFileName);
                    string backupImagePath = Path.Combine(backupImagesFolderPath, frame.ImageFileName);

                    // 이미지가 존재하면 백업/images 폴더로 이동 (삭제된 사진 보관)
                    if (File.Exists(imagePath))
                    {
                        // 백업 이미지 폴더에 이미 동일한 이름의 파일이 있다면 덮어쓰기
                        if (File.Exists(backupImagePath))
                        {
                            File.Delete(backupImagePath);
                        }
                        File.Move(imagePath, backupImagePath);
                    }

                    allFrames.RemoveAll(f =>
                        f.Index == frame.Index &&
                        f.ImageFileName == frame.ImageFileName
                    );

                    deletedCount++;
                }

                SaveCatalog();
                // 갱신을 위해 데이터 목록 필터 다시 적용
                ApplyFilter();

                AppendLog($"삭제 완료: {deletedCount}개의 프레임이 삭제되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("삭제 중 오류가 발생했습니다.\n\n" + ex.Message);
            }
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

            if (string.IsNullOrWhiteSpace(mycarPath) || !Directory.Exists(mycarPath))
            {
                MessageBox.Show("mycar 폴더 경로를 먼저 지정하세요.");
                return;
            }

            if (string.IsNullOrWhiteSpace(pythonExe))
            {
                MessageBox.Show("Python 실행 파일명을 입력하세요.");
                return;
            }

            if (string.IsNullOrWhiteSpace(trainArgs))
            {
                MessageBox.Show("학습 명령 인자를 입력하세요.");
                return;
            }

            txtLog.Clear();
            AppendLog("학습 시작");
            AppendLog("WorkingDirectory = " + mycarPath);
            AppendLog("Command = " + pythonExe + " " + trainArgs);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = pythonExe,
                Arguments = trainArgs,
                WorkingDirectory = mycarPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                trainProcess = new Process();
                trainProcess.StartInfo = psi;

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
                    "WSL 또는 Conda 환경이면 Python 실행명과 WorkingDirectory를 환경에 맞게 수정해야 합니다.\n\n" +
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

        private void btnRunPilotTest_Click(object? sender, EventArgs e)
        {
            if (currentIndex < 0 || currentIndex >= visibleFrames.Count)
            {
                MessageBox.Show("먼저 Viewer에서 테스트할 프레임을 선택하세요.");
                return;
            }

            DonkeyFrame frame = visibleFrames[currentIndex];

            lblActualAngle.Text = $"실제 Angle: {frame.Angle:F4}";
            lblPredictedAngle.Text = "예측 Angle: Python 연동 필요";

            LoadImageToPictureBox(picPilotTest, Path.Combine(imagesFolderPath, frame.ImageFileName));

            AppendLog("Pilot Test 요청");
            AppendLog("현재 UI는 모델 테스트 자리만 제공하며, 실제 예측은 Python 연동 담당이 구현해야 합니다.");
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

            if (lstCleanerFrames.SelectedIndex >= 0 && lstCleanerFrames.SelectedIndex < visibleFrames.Count)
                ShowFrame(lstCleanerFrames.SelectedIndex);
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
                foreach (string line in File.ReadLines(catalogFilePath))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        DonkeyFrame? frame = JsonSerializer.Deserialize<DonkeyFrame>(line);

                        if (frame != null && !string.IsNullOrWhiteSpace(frame.ImageFileName))
                            allFrames.Add(frame);
                    }
                    catch
                    {
                        AppendLog("catalog 파싱 실패 줄 발견");
                    }
                }

                visibleFrames = allFrames.ToList();

                BindFrameLists();
                SetupTrackBar();

                if (visibleFrames.Count > 0)
                    ShowFrame(0);
                else
                    ClearViewer();

                PopulateTimeline(); // <--- 여기 추가!

                AppendLog($"로드 완료: {visibleFrames.Count}개 프레임");
                UpdateModelStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("catalog 파일을 읽는 중 오류가 발생했습니다.\n\n" + ex.Message);
            }
        }

        private void BindFrameLists()
        {
            isUpdatingSelection = true;

            lstFrames.BeginUpdate();
            lstCleanerFrames.BeginUpdate();

            lstFrames.Items.Clear();
            lstCleanerFrames.Items.Clear();

            foreach (DonkeyFrame frame in visibleFrames)
            {
                string text =
                    $"{frame.Index:D5} | angle={frame.Angle:F3} | throttle={frame.Throttle:F3} | mode={frame.Mode}";

                lstFrames.Items.Add(text);
                lstCleanerFrames.Items.Add(text);
            }

            lstFrames.EndUpdate();
            lstCleanerFrames.EndUpdate();

            isUpdatingSelection = false;
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

            lblFrameInfo.Text = $"Frame: {index + 1} / {visibleFrames.Count}";
            lblAngle.Text = $"Angle: {frame.Angle:F4}";
            lblThrottle.Text = $"Throttle: {frame.Throttle:F4}";
            lblMode.Text = $"Mode: {frame.Mode}";

            lblCleanerInfo.Text =
                $"선택 프레임 정보: index={frame.Index}, angle={frame.Angle:F4}, throttle={frame.Throttle:F4}, mode={frame.Mode}";

            if (trbFrame.Value != index)
                trbFrame.Value = index;

            isUpdatingSelection = true;

            if (lstFrames.SelectedIndex != index)
                lstFrames.SelectedIndex = index;

            // 자동 재생 및 Viewer 조작 시 Cleaner의 다중 선택이 풀리거나 유지·누적되는 문제 방지
            // lstCleanerFrames 리스트의 선택을 강제 동기화하지 않도록 주석(혹은 삭제) 처리합니다.
            // if (lstCleanerFrames.SelectedIndex != index)
            //     lstCleanerFrames.SelectedIndex = index;

            isUpdatingSelection = false;

            // [추가] 썸네일 타임라인에서도 지금 위치를 알 수 있도록 쫓아가기
            HighlightTimelineFrame(index);
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

        // 썸네일 생성 (이미지 축소 로드)
        private async Task<Bitmap?> LoadThumbnailAsync(string path, int width, int height)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (!File.Exists(path)) return null;
                    byte[] bytes = File.ReadAllBytes(path);
                    using MemoryStream ms = new MemoryStream(bytes);
                    using Bitmap original = new Bitmap(ms);
                    return new Bitmap(original, new Size(width, height));
                }
                catch { return null; }
            });
        }

        // 타임라인 그려주기
        private async void PopulateTimeline()
        {
            flpTimeline.SuspendLayout();
            foreach (Control ctrl in flpTimeline.Controls)
            {
                if (ctrl is PictureBox pic && pic.Image != null) pic.Image.Dispose();
                ctrl.Dispose();
            }
            flpTimeline.Controls.Clear();
            flpTimeline.ResumeLayout();

            for (int i = 0; i < visibleFrames.Count; i++)
            {
                int index = i; 
                DonkeyFrame frame = visibleFrames[i];
                string imagePath = Path.Combine(imagesFolderPath, frame.ImageFileName);

                PictureBox picThumb = new PictureBox
                {
                    Width = 80,
                    Height = 60,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.Black, // 기본 배경 겸 기본 얇은 테두리 역할
                    Margin = new Padding(2),
                    Padding = new Padding(2),// 사진과 겉 부분 2픽셀 띄우기
                    Cursor = Cursors.Hand,
                    Tag = index 
                };

                // [추가] 컨트롤 자체가 자신의 겉면에 테두리를 그리도록 하는 이벤트
                picThumb.Paint += (s, e) =>
                {
                    PictureBox pic = (PictureBox)s!;
                    // 이 썸네일이 현재 재생 중인 인덱스라면
                    if ((int)pic.Tag == currentIndex)
                    {
                        // 두께 4픽셀짜리 하얀 테두리(틀)를 그림의 가장자리에 그립니다.
                        // (안쪽 이미지를 절대 덮지 않고 테두리만 두껕게 설정)
                        using (Pen pen = new Pen(Color.White, 4))
                        {
                            // 렌더링 오차 방지를 위해 1픽셀 안쪽으로 당겨서 그립니다.
                            e.Graphics.DrawRectangle(pen, new Rectangle(1, 1, pic.Width - 3, pic.Height - 3));
                        }
                    }
                };

                picThumb.Click += (s, e) => ShowFrame(index);
                flpTimeline.Controls.Add(picThumb);
                picThumb.Image = await LoadThumbnailAsync(imagePath, 80, 60);

                if (i % 20 == 0) await Task.Delay(1);
            }
        }

        private void ApplyFilter()
        {
            visibleFrames = allFrames.ToList();

            BindFrameLists();
            SetupTrackBar();

            isUpdatingSelection = true;
            lstCleanerFrames.BeginUpdate();

            // 단순히 클릭만으로도 선택/해제를 토글할 수 있는 MultiSimple 모드 사용
            lstCleanerFrames.SelectionMode = SelectionMode.MultiSimple;
            lstCleanerFrames.ClearSelected();

            int count = 0;
            bool hasFilter = chkThrottlePositive.Checked || chkExcludeZeroAngle.Checked || chkStopDataOnly.Checked;

            if (hasFilter)
            {
                for (int i = 0; i < visibleFrames.Count; i++)
                {
                    DonkeyFrame f = visibleFrames[i];
                    bool satisfy = true;

                    if (chkThrottlePositive.Checked && f.Throttle <= 0)
                        satisfy = false;

                    if (chkExcludeZeroAngle.Checked && Math.Abs(f.Angle) <= 0.000001)
                        satisfy = false;

                    if (chkStopDataOnly.Checked && Math.Abs(f.Throttle) > 0.000001)
                        satisfy = false;

                    // 조건을 만족하지 못하면 해당 항목을 선택
                    if (!satisfy)
                    {
                        lstCleanerFrames.SetSelected(i, true);
                        count++;
                    }
                }
            }

            lstCleanerFrames.EndUpdate();
            isUpdatingSelection = false;

            if (visibleFrames.Count > 0)
            {
                // 선택된 항목이 있으면 가장 위 선택 항목을 보여줌
                if (lstCleanerFrames.SelectedIndex >= 0)
                    ShowFrame(lstCleanerFrames.SelectedIndex);
                else
                    ShowFrame(0);
            }
            else
            {
                ClearViewer();
            }

            if (hasFilter)
                AppendLog($"필터 적용: 조건 불만족 {count}개 프레임 선택됨");
            else
                AppendLog("필터 적용: 선택된 조건 없음");
        }

        private void ClearViewer()
        {
            currentIndex = -1;

            DisposeCurrentImages();

            lblFrameInfo.Text = "Frame: -";
            lblAngle.Text = "Angle: -";
            lblThrottle.Text = "Throttle: -";
            lblMode.Text = "Mode: -";
            lblCleanerInfo.Text = "선택 프레임 정보: -";
            lblActualAngle.Text = "실제 Angle: -";
            lblPredictedAngle.Text = "예측 Angle: -";

            isUpdatingSelection = true;
            lstFrames.Items.Clear();
            lstCleanerFrames.Items.Clear();
            isUpdatingSelection = false;

            trbFrame.Minimum = 0;
            trbFrame.Maximum = 0;
            trbFrame.Value = 0;
        }

        private void SaveCatalog()
        {
            List<string> lines = new List<string>();

            foreach (DonkeyFrame frame in allFrames)
            {
                Dictionary<string, object?> obj = new Dictionary<string, object?>()
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

            File.WriteAllLines(catalogFilePath, lines);
        }

        private void UpdateModelStatus()
        {
            string mycarPath = txtMycarPath.Text.Trim();

            if (string.IsNullOrWhiteSpace(mycarPath))
            {
                lblModelStatus.Text = "모델 상태: mycar 경로 없음";
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
            if (picFrame.Image != null)
            {
                picFrame.Image.Dispose();
                picFrame.Image = null;
            }

            if (picCleanerPreview.Image != null)
            {
                picCleanerPreview.Image.Dispose();
                picCleanerPreview.Image = null;
            }

            if (picPilotTest.Image != null)
            {
                picPilotTest.Image.Dispose();
                picPilotTest.Image = null;
            }
        }

        private void AppendLog(string message)
        {
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                autoPlayTimer.Stop();
                autoPlayTimer.Dispose();

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

        // 현재 인덱스를 눈에 띄게 테두리로 표시하고, 시야를 벗어날 때 페이지를 통째로 넘깁니다.
        private void HighlightTimelineFrame(int index)
        {
            if (flpTimeline.Controls.Count <= index) return;

            bool needToScroll = false;
            PictureBox? targetPic = null;

            // 1. 모든 썸네일 재갱신(테두리 다시 그리기) 및 현재 썸네일 찾기
            foreach (Control ctrl in flpTimeline.Controls)
            {
                if (ctrl is PictureBox pic)
                {
                    pic.Invalidate();

                    if ((int)pic.Tag == index)
                    {
                        targetPic = pic;
                    }
                }
            }

            if (targetPic != null)
            {
                // [정확한 시야 계산법]
                // targetPic.Bounds 영역이 현재 FlowLayoutPanel의 가시 영역(ClientRectangle) 안에 들어오도록 
                // 강제로 계산된 절대 좌표(flpTimeline 안에서 생성된 진짜 원래 위치)를 뽑아봅니다.
                
                // 컨트롤의 원래 X 위치(스크롤 안 했을 때 기준)는 대략 인덱스를 통해 알 수 있습니다.
                // Control의 Margin, Padding, Width를 모두 더한 한 칸의 실질적 너비 (예: 80(폭) + 2(왼쪽여백) + 2(오른쪽여백) = 84)
                int itemWidth = targetPic.Width + targetPic.Margin.Horizontal; 

                // 현재 targetPic의 절대 X 시작 좌표 (0부터 시작)
                int absoluteX = index * itemWidth;

                // 현재 스크롤 막대가 위치한 X값 (항상 양수로 가져옴)
                int currentScrollX = Math.Abs(flpTimeline.AutoScrollPosition.X);
                
                // 화면의 가로 폭
                int viewWidth = flpTimeline.ClientSize.Width;

                // 만약 사진의 오른쪽 끝(absoluteX + itemWidth)이 화면 오른쪽 밖으로 나갔거나,
                // 사진의 왼쪽 시작(absoluteX)이 화면 왼쪽 밖(과거)으로 나갔다면!
                if (absoluteX + itemWidth > currentScrollX + viewWidth || absoluteX < currentScrollX)
                {
                    needToScroll = true;
                }

                // 스크롤해야 한다면, 이 사진이 화면의 맨 왼쪽(0 지점)에 오도록 스크롤을 점프!
                if (needToScroll)
                {
                    // AutoScrollPosition 설정 시 양수로 지정하면 그 지점으로 스크롤이 이동함
                    // targetPic의 절대 X 좌표(absoluteX)를 스크롤 시작점으로 줌
                    flpTimeline.AutoScrollPosition = new Point(absoluteX, 0);
                }
            }
        }
    }
}

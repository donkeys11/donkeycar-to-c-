using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public MainForm()
        {
            InitializeComponent();
            
            lstCleanerFrames.SelectionMode = SelectionMode.MultiExtended; 
            lstCleanerFrames.DrawMode = DrawMode.OwnerDrawFixed;
            lstCleanerFrames.ItemHeight = 16; 
            lstCleanerFrames.HorizontalScrollbar = false;

            // [추가] 리스트박스 깜빡임(Flickering)을 완벽히 없애는 마법의 코드 (DoubleBuffered 켜기)
            typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)
                           ?.SetValue(lstCleanerFrames, true, null);

            lstCleanerFrames.DrawItem += LstCleanerFrames_DrawItem;

            ConnectEvents();
            
            autoPlayTimer.Interval = 100;
            autoPlayTimer.Tick += AutoPlayTimer_Tick;
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
            if (visibleFrames.Count == 0) return;

            int next = currentIndex + 1;
            int startIndex = next;

            while (true)
            {
                if (next >= visibleFrames.Count) next = 0;

                // 파란색(선택됨)이면 건너뜀
                if (lstCleanerFrames.Items.Count > next && lstCleanerFrames.GetSelected(next))
                {
                    next++;
                    if (next == startIndex || (startIndex >= visibleFrames.Count && next == 0))
                    {
                        autoPlayTimer.Enabled = false;
                        btnAutoPlay.Text = "자동 재생";
                        return;
                    }
                }
                else
                {
                    break;
                }
            }

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

            if (visibleFrames.Count > 0)
                ShowFrame(0);
            else
                ClearViewer();

            AppendLog("필터 해제: 전체 데이터 표시");
        }

        private void btnDeleteFrame_Click(object? sender, EventArgs e)
        {
            if (currentIndex < 0 || currentIndex >= visibleFrames.Count)
            {
                MessageBox.Show("삭제할 프레임을 먼저 선택하세요.");
                return;
            }

            DonkeyFrame frame = visibleFrames[currentIndex];

            DialogResult result = MessageBox.Show(
                $"현재 프레임을 삭제할까요?\n\nIndex: {frame.Index}\nImage: {frame.ImageFileName}",
                "삭제 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                DisposeCurrentImages();

                string imagePath = Path.Combine(imagesFolderPath, frame.ImageFileName);

                // --- 1. backup 폴더 생성 ---
                string backupFolderPath = Path.Combine(dataFolderPath, "backup");
                if (!Directory.Exists(backupFolderPath))
                {
                    Directory.CreateDirectory(backupFolderPath);
                }

                // --- 2. 원본 카탈로그 파일 백업 (삭제를 시작하기 전 최초 1회만 백업본 생성) ---
                string backupCatalogPath = Path.Combine(backupFolderPath, "catalog_0.catalog");
                if (!File.Exists(backupCatalogPath) && File.Exists(catalogFilePath))
                {
                    File.Copy(catalogFilePath, backupCatalogPath);
                }

                // --- 3. 이미지를 backup 폴더로 복사한 후 원본 삭제 ---
                if (File.Exists(imagePath))
                {
                    string backupImagePath = Path.Combine(backupFolderPath, frame.ImageFileName);
                    File.Copy(imagePath, backupImagePath, true); // 백업 폴더로 복사 저장
                    File.Delete(imagePath);                      // 원래 images 폴더 안의 파일은 삭제
                    
                    // 참고: 복사 후 삭제 대신 이동(File.Move)을 사용하려면 위 두 줄 대신 
                    // File.Move(imagePath, backupImagePath, true); 를 사용해도 됩니다.
                }

                // 4. 리스트에서 제거 및 저장
                allFrames.RemoveAll(f =>
                    f.Index == frame.Index &&
                    f.ImageFileName == frame.ImageFileName
                );

                SaveCatalog();
                ApplyFilter();

                AppendLog($"삭제 완료(백업됨): index={frame.Index}, image={frame.ImageFileName}");
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

            // Viewer 탭(lstFrames) 처리 
            if (lstFrames.SelectedIndex != index)
                lstFrames.SelectedIndex = index;

            if (lstCleanerFrames.Items.Count > index)
            {
                // [부활시킨 자동 스크롤 로직]
                // 현재 인덱스가 화면 시야(Top ~ Bottom)를 벗어나면, 스크롤을 따라가게 만듭니다.
                int visibleItemsCount = lstCleanerFrames.ClientSize.Height / lstCleanerFrames.ItemHeight;
                
                // 만약 현재 노란 줄(index)이 화면 맨 위보다 위에 있거나, 화면 맨 아래보다 아래에 있다면
                if (index < lstCleanerFrames.TopIndex || index >= lstCleanerFrames.TopIndex + visibleItemsCount)
                {
                    // 부드럽게 노란 줄이 맨 밑이나 맨 위에 보이도록 스크롤 이동
                    if (index >= lstCleanerFrames.TopIndex + visibleItemsCount)
                        lstCleanerFrames.TopIndex = index - visibleItemsCount + 1; // 화면 맨 밑에 걸치게
                    else
                        lstCleanerFrames.TopIndex = index; // 화면 맨 위에 걸치게
                }


                }
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
            // 전체 프레임을 화면 리스트에 유지해야 하므로 visibleFrames를 필터링되지 않은 상태로 설정
            visibleFrames = allFrames.ToList();
            
            // 리스트 다시 그리기
            BindFrameLists();
            SetupTrackBar();

            // 리스트박스 선택 상태 변경을 시작합니다.
            lstCleanerFrames.BeginUpdate();
            lstCleanerFrames.ClearSelected(); // 기존 선택 해제

            int matchCount = 0;
            
            // 조건에 맞는 인덱스만 하이라이트 되도록 선택(Select)합니다.
            for (int i = 0; i < visibleFrames.Count; i++)
            {
                DonkeyFrame f = visibleFrames[i];
                bool match = true;

                if (chkThrottlePositive.Checked && f.Throttle <= 0)
                    match = false;

                if (chkExcludeZeroAngle.Checked && Math.Abs(f.Angle) <= 0.000001)
                    match = false;

                if (chkStopDataOnly.Checked && Math.Abs(f.Throttle) > 0.000001)
                    match = false;

                // [수정된 부분] 필터가 하나라도 체크되어 있고, 해당 조건을 '만족하지 않는'(!match) 프레임을 선택
                bool isAnyFilterActive = chkThrottlePositive.Checked || 
                                         chkExcludeZeroAngle.Checked || 
                                         chkStopDataOnly.Checked;

                if (isAnyFilterActive && !match)
                {
                    lstCleanerFrames.SetSelected(i, true);
                    matchCount++;
                }
            }
            
            lstCleanerFrames.EndUpdate();
            
            if (visibleFrames.Count > 0)
                ShowFrame(0);
            else
                ClearViewer();

            AppendLog($"필터 적용: {matchCount}개 프레임이 선택됨");
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

        private void LstCleanerFrames_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            bool isCurrentPlaying = (e.Index == currentIndex);

            Color backColor = lstCleanerFrames.BackColor;
            Color foreColor = lstCleanerFrames.ForeColor;

            if (isSelected) 
            {
                backColor = SystemColors.Highlight;
                foreColor = SystemColors.HighlightText; 
            }
            else if (isCurrentPlaying)
            {
                backColor = Color.LightYellow;
                foreColor = Color.Black; 
            }

            // e.DrawBackground() 금지! 100% 수동으로 덮어버리기
            using (SolidBrush bgBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(bgBrush, e.Bounds);
            }

            string text = lstCleanerFrames.Items[e.Index].ToString() ?? "";
            using (SolidBrush textBrush = new SolidBrush(foreColor))
            {
                e.Graphics.DrawString(text, e.Font ?? lstCleanerFrames.Font, textBrush, e.Bounds.X, e.Bounds.Y);
            }

            if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
            {
                e.DrawFocusRectangle();
            }
        }
        
        // MainForm 클래스 멤버로 추가해주세요.
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);
        private const int WM_SETREDRAW = 11;
    }}
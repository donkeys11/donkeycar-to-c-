using System.Drawing;
using System.Windows.Forms;

namespace DonkeycarManager
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private TabControl tabMain;
        private TabPage tabViewer;
        private TabPage tabCleaner;
        private TabPage tabTrainer;
        private TabPage tabPilotTest;

        private Label lblTitleViewer;
        private Button btnOpenDataFolder;
        private Button btnReload;
        private Button btnAutoPlay;
        private Label lblDataPath;
        private PictureBox picFrame;
        private ListBox lstFrames;
        private Label lblFrameInfo;
        private Label lblAngle;
        private Label lblThrottle;
        private Label lblMode;
        private TrackBar trbFrame;

        private Label lblTitleCleaner;
        private GroupBox grpFilters;
        private CheckBox chkThrottlePositive;
        private CheckBox chkExcludeZeroAngle;
        private CheckBox chkStopDataOnly;
        private Button btnApplyFilter;
        private Button btnClearFilter;
        private Button btnDeleteFrame;
        private ListBox lstCleanerFrames;
        private PictureBox picCleanerPreview;
        private Label lblCleanerInfo;

        private GroupBox grpCleanerRangeEditor;
        private Panel pnlCleanerTimeline;
        private Label lblCleanerRangeInfo;
        private Label lblCleanerRangeHint;
        private Button btnDeleteRange;
        private Button btnPlayRange;
        private Button btnClearRange;

        private Label lblTitleTrainer;
        private Label lblMycarPath;
        private TextBox txtMycarPath;
        private Button btnBrowseMycar;
        private Label lblPythonExe;
        private TextBox txtPythonExe;
        private Label lblTrainArgs;
        private TextBox txtTrainArgs;
        private Button btnTrain;
        private Button btnStopTrain;
        private Label lblModelStatus;
        private Label lblTrainInfo;

        private Label lblTitlePilot;
        private Label lblModelPath;
        private TextBox txtModelPath;
        private Button btnBrowseModel;
        private Button btnRunPilotTest;
        private Button btnUseViewerFrame;
        private PictureBox picPilotTest;
        private Label lblPilotImageList;
        private ListBox lstPilotFrames;
        private Label lblActualAngle;
        private Label lblPredictedAngle;
        private Label lblActualThrottle;
        private Label lblPredictedThrottle;
        private Label lblAngleError;
        private Label lblPilotWarning;
        private Label lblPilotNote;

        private TextBox txtLog;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeCurrentImages();

                if (components != null)
                    components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            tabMain = new TabControl();
            tabViewer = new TabPage();
            tabCleaner = new TabPage();
            tabTrainer = new TabPage();
            tabPilotTest = new TabPage();

            lblTitleViewer = new Label();
            btnOpenDataFolder = new Button();
            btnReload = new Button();
            btnAutoPlay = new Button();
            lblDataPath = new Label();
            picFrame = new PictureBox();
            lstFrames = new ListBox();
            lblFrameInfo = new Label();
            lblAngle = new Label();
            lblThrottle = new Label();
            lblMode = new Label();
            trbFrame = new TrackBar();

            lblTitleCleaner = new Label();
            grpFilters = new GroupBox();
            chkThrottlePositive = new CheckBox();
            chkExcludeZeroAngle = new CheckBox();
            chkStopDataOnly = new CheckBox();
            btnApplyFilter = new Button();
            btnClearFilter = new Button();
            btnDeleteFrame = new Button();
            lstCleanerFrames = new ListBox();
            picCleanerPreview = new PictureBox();
            lblCleanerInfo = new Label();

            grpCleanerRangeEditor = new GroupBox();
            pnlCleanerTimeline = new Panel();
            lblCleanerRangeInfo = new Label();
            lblCleanerRangeHint = new Label();
            btnDeleteRange = new Button();
            btnPlayRange = new Button();
            btnClearRange = new Button();

            lblTitleTrainer = new Label();
            lblMycarPath = new Label();
            txtMycarPath = new TextBox();
            btnBrowseMycar = new Button();
            lblPythonExe = new Label();
            txtPythonExe = new TextBox();
            lblTrainArgs = new Label();
            txtTrainArgs = new TextBox();
            btnTrain = new Button();
            btnStopTrain = new Button();
            lblModelStatus = new Label();
            lblTrainInfo = new Label();

            lblTitlePilot = new Label();
            lblModelPath = new Label();
            txtModelPath = new TextBox();
            btnBrowseModel = new Button();
            btnRunPilotTest = new Button();
            btnUseViewerFrame = new Button();
            picPilotTest = new PictureBox();
            lblPilotImageList = new Label();
            lstPilotFrames = new ListBox();
            lblActualAngle = new Label();
            lblPredictedAngle = new Label();
            lblActualThrottle = new Label();
            lblPredictedThrottle = new Label();
            lblAngleError = new Label();
            lblPilotWarning = new Label();
            lblPilotNote = new Label();

            txtLog = new TextBox();

            ((System.ComponentModel.ISupportInitialize)picFrame).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picCleanerPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picPilotTest).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trbFrame).BeginInit();

            SuspendLayout();

            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1400, 900);
            MinimumSize = new Size(1280, 820);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Donkeycar Manager";
            WindowState = FormWindowState.Maximized;

            tabMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabMain.Controls.Add(tabViewer);
            tabMain.Controls.Add(tabCleaner);
            tabMain.Controls.Add(tabTrainer);
            tabMain.Controls.Add(tabPilotTest);
            tabMain.Font = new Font("맑은 고딕", 10F);
            tabMain.Location = new Point(0, 0);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(1400, 720);

            BuildViewerTab();
            BuildCleanerTab();
            BuildTrainerTab();
            BuildPilotTestTab();

            txtLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Font = new Font("Consolas", 9F);
            txtLog.Location = new Point(0, 725);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(1400, 175);

            Controls.Add(tabMain);
            Controls.Add(txtLog);

            ((System.ComponentModel.ISupportInitialize)picFrame).EndInit();
            ((System.ComponentModel.ISupportInitialize)picCleanerPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)picPilotTest).EndInit();
            ((System.ComponentModel.ISupportInitialize)trbFrame).EndInit();

            ResumeLayout(false);
            PerformLayout();
        }

        private void BuildViewerTab()
        {
            tabViewer.BackColor = Color.WhiteSmoke;
            tabViewer.Location = new Point(4, 32);
            tabViewer.Name = "tabViewer";
            tabViewer.Size = new Size(1392, 684);
            tabViewer.Text = "Viewer - 데이터 확인";

            lblTitleViewer = NewLabel("Donkeycar Tub Viewer", 20, 18, 22F, FontStyle.Bold, Color.FromArgb(30, 90, 160));
            btnOpenDataFolder = NewButton("데이터 폴더 열기", 20, 82, 160, 38);
            btnReload = NewButton("새로고침", 190, 82, 110, 38);
            btnAutoPlay = NewButton("자동 재생", 310, 82, 120, 38);
            lblDataPath = NewLabel("Data Folder: -", 450, 90, 10F, FontStyle.Regular, Color.DimGray);

            picFrame = NewPictureBox(20, 140, 830, 390);
            picFrame.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            lstFrames = NewListBox(870, 140, 500, 390);
            lstFrames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;

            lblFrameInfo = NewLabel("Frame: -", 20, 548, 11F, FontStyle.Bold, Color.Black);
            lblAngle = NewLabel("Angle: -", 200, 548, 11F, FontStyle.Regular, Color.Black);
            lblThrottle = NewLabel("Throttle: -", 410, 548, 11F, FontStyle.Regular, Color.Black);
            lblMode = NewLabel("Mode: -", 650, 548, 11F, FontStyle.Regular, Color.Black);

            lblFrameInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblAngle.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblThrottle.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblMode.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            trbFrame.Location = new Point(20, 585);
            trbFrame.Maximum = 0;
            trbFrame.Name = "trbFrame";
            trbFrame.Size = new Size(1350, 56);
            trbFrame.TickStyle = TickStyle.None;
            trbFrame.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            tabViewer.Controls.AddRange(new Control[]
            {
                lblTitleViewer, btnOpenDataFolder, btnReload, btnAutoPlay, lblDataPath,
                picFrame, lstFrames, lblFrameInfo, lblAngle, lblThrottle, lblMode, trbFrame
            });
        }

        private void BuildCleanerTab()
        {
            tabCleaner.BackColor = Color.WhiteSmoke;
            tabCleaner.Location = new Point(4, 32);
            tabCleaner.Name = "tabCleaner";
            tabCleaner.Size = new Size(1392, 684);
            tabCleaner.Text = "Cleaner - 데이터 정리";

            lblTitleCleaner = NewLabel("Tub Cleaner", 20, 18, 22F, FontStyle.Bold, Color.FromArgb(180, 70, 70));

            grpFilters.Location = new Point(20, 90);
            grpFilters.Name = "grpFilters";
            grpFilters.Size = new Size(340, 170);
            grpFilters.Text = "필터 조건";

            chkThrottlePositive.AutoSize = true;
            chkThrottlePositive.Location = new Point(20, 35);
            chkThrottlePositive.Text = "throttle > 0만 보기";

            chkExcludeZeroAngle.AutoSize = true;
            chkExcludeZeroAngle.Location = new Point(20, 75);
            chkExcludeZeroAngle.Text = "angle == 0 제외";

            chkStopDataOnly.AutoSize = true;
            chkStopDataOnly.Location = new Point(20, 115);
            chkStopDataOnly.Text = "정지 데이터만 보기(throttle == 0)";

            grpFilters.Controls.AddRange(new Control[] { chkThrottlePositive, chkExcludeZeroAngle, chkStopDataOnly });

            btnApplyFilter = NewButton("필터 적용", 390, 110, 130, 45);
            btnClearFilter = NewButton("전체 보기", 390, 175, 130, 45);

            btnDeleteFrame = NewButton("선택 프레임 삭제\n(다중 선택 가능)", 550, 110, 170, 110);
            btnDeleteFrame.BackColor = Color.LightCoral;
            btnDeleteFrame.FlatStyle = FlatStyle.Flat;

            grpCleanerRangeEditor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpCleanerRangeEditor.Location = new Point(750, 90);
            grpCleanerRangeEditor.Name = "grpCleanerRangeEditor";
            grpCleanerRangeEditor.Size = new Size(620, 170);
            grpCleanerRangeEditor.Text = "구간 선택 편집";

            lblCleanerRangeInfo = NewLabel("선택 구간: 없음", 15, 28, 10F, FontStyle.Bold, Color.FromArgb(170, 60, 50));
            lblCleanerRangeHint = NewLabel("타임라인에서 드래그하여 구간 선택", 320, 28, 9F, FontStyle.Regular, Color.DimGray);

            pnlCleanerTimeline.BackColor = Color.FromArgb(25, 35, 55);
            pnlCleanerTimeline.BorderStyle = BorderStyle.FixedSingle;
            pnlCleanerTimeline.Location = new Point(15, 58);
            pnlCleanerTimeline.Name = "pnlCleanerTimeline";
            pnlCleanerTimeline.Size = new Size(585, 45);
            pnlCleanerTimeline.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            btnDeleteRange = NewButton("구간 삭제", 15, 115, 120, 38);
            btnDeleteRange.BackColor = Color.FromArgb(180, 60, 50);
            btnDeleteRange.ForeColor = Color.White;
            btnDeleteRange.FlatStyle = FlatStyle.Flat;

            btnPlayRange = NewButton("구간 재생", 150, 115, 120, 38);
            btnPlayRange.BackColor = Color.FromArgb(70, 110, 160);
            btnPlayRange.ForeColor = Color.White;
            btnPlayRange.FlatStyle = FlatStyle.Flat;

            btnClearRange = NewButton("구간 해제", 285, 115, 120, 38);

            grpCleanerRangeEditor.Controls.AddRange(new Control[]
            {
                pnlCleanerTimeline, lblCleanerRangeInfo, lblCleanerRangeHint,
                btnDeleteRange, btnPlayRange, btnClearRange
            });

            lstCleanerFrames = NewListBox(20, 285, 620, 340);
            lstCleanerFrames.SelectionMode = SelectionMode.MultiExtended;
            lstCleanerFrames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;

            picCleanerPreview = NewPictureBox(670, 285, 700, 300);
            picCleanerPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            lblCleanerInfo = NewLabel("선택 프레임 정보: -", 670, 600, 10F, FontStyle.Regular, Color.Black);
            lblCleanerInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            tabCleaner.Controls.AddRange(new Control[]
            {
                lblTitleCleaner, grpFilters, btnApplyFilter, btnClearFilter, btnDeleteFrame,
                grpCleanerRangeEditor, lstCleanerFrames, picCleanerPreview, lblCleanerInfo
            });
        }

        private void BuildTrainerTab()
        {
            tabTrainer.BackColor = Color.WhiteSmoke;
            tabTrainer.Location = new Point(4, 32);
            tabTrainer.Name = "tabTrainer";
            tabTrainer.Size = new Size(1392, 684);
            tabTrainer.Text = "Trainer - 학습 실행";

            lblTitleTrainer = NewLabel("Donkeycar Trainer", 20, 18, 22F, FontStyle.Bold, Color.FromArgb(60, 130, 80));

            lblMycarPath = NewLabel("mycar 경로", 30, 100, 10F, FontStyle.Regular, Color.Black);
            txtMycarPath = NewTextBox(150, 96, 720, 30);
            btnBrowseMycar = NewButton("찾기", 890, 95, 90, 34);

            lblPythonExe = NewLabel("Python 실행명", 30, 155, 10F, FontStyle.Regular, Color.Black);
            txtPythonExe = NewTextBox(150, 151, 300, 30);

            lblTrainArgs = NewLabel("학습 명령 인자", 30, 210, 10F, FontStyle.Regular, Color.Black);
            txtTrainArgs = NewTextBox(150, 206, 830, 30);

            btnTrain = NewButton("학습 시작", 150, 270, 160, 50);
            btnTrain.BackColor = Color.FromArgb(76, 175, 80);
            btnTrain.FlatStyle = FlatStyle.Flat;
            btnTrain.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            btnTrain.ForeColor = Color.White;

            btnStopTrain = NewButton("학습 중지", 330, 270, 160, 50);
            btnStopTrain.BackColor = Color.LightCoral;
            btnStopTrain.FlatStyle = FlatStyle.Flat;
            btnStopTrain.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);

            lblModelStatus = NewLabel("모델 상태: -", 150, 345, 11F, FontStyle.Bold, Color.Black);

            lblTrainInfo = NewLabel(
                "자료 기준 학습 명령 예시:\npython train.py --tub ./data --model ./models/mypilot.h5\n\nC#은 AI를 직접 학습하지 않고 Python 외부 프로세스를 실행합니다.",
                150,
                395,
                10F,
                FontStyle.Regular,
                Color.DimGray
            );

            tabTrainer.Controls.AddRange(new Control[]
            {
                lblTitleTrainer, lblMycarPath, txtMycarPath, btnBrowseMycar,
                lblPythonExe, txtPythonExe, lblTrainArgs, txtTrainArgs,
                btnTrain, btnStopTrain, lblModelStatus, lblTrainInfo
            });
        }

        private void BuildPilotTestTab()
        {
            tabPilotTest.BackColor = Color.WhiteSmoke;
            tabPilotTest.Location = new Point(4, 32);
            tabPilotTest.Name = "tabPilotTest";
            tabPilotTest.Size = new Size(1392, 684);
            tabPilotTest.Text = "Pilot Test - 모델 테스트";

            lblTitlePilot = NewLabel("Pilot Arena / Model Test", 20, 18, 22F, FontStyle.Bold, Color.FromArgb(90, 90, 160));

            lblModelPath = NewLabel("모델 파일", 30, 100, 10F, FontStyle.Regular, Color.Black);
            txtModelPath = NewTextBox(120, 96, 740, 30);
            btnBrowseModel = NewButton("찾기", 880, 95, 90, 34);

            btnRunPilotTest = NewButton("현재 이미지로 예측 테스트", 120, 145, 250, 42);
            btnUseViewerFrame = NewButton("Viewer 선택 이미지 사용", 390, 145, 210, 42);

            picPilotTest = NewPictureBox(30, 220, 650, 340);
            picPilotTest.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;

            lblPilotImageList = NewLabel("테스트 이미지 선택", 30, 570, 10F, FontStyle.Bold, Color.Black);

            lstPilotFrames = NewListBox(30, 600, 650, 70);
            lstPilotFrames.SelectionMode = SelectionMode.One;
            lstPilotFrames.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            lblActualAngle = NewLabel("실제 Angle: -", 710, 230, 12F, FontStyle.Bold, Color.Black);
            lblPredictedAngle = NewLabel("예측 Angle: -", 710, 270, 12F, FontStyle.Bold, Color.Black);
            lblActualThrottle = NewLabel("실제 Throttle: -", 710, 320, 11F, FontStyle.Bold, Color.Black);
            lblPredictedThrottle = NewLabel("예측 Throttle: -", 710, 355, 11F, FontStyle.Bold, Color.Black);
            lblAngleError = NewLabel("Angle Error: -", 710, 405, 12F, FontStyle.Bold, Color.Black);
            lblPilotWarning = NewLabel("판정: -", 710, 450, 12F, FontStyle.Bold, Color.DimGray);

            lblPilotNote = NewLabel(
                "파란선: 실제 angle\n초록선: 예측 angle\n노란 반투명 영역: 실제/예측 차이\n하단 막대: 실제/예측 throttle 비교\n오차가 클수록 경고 색상이 빨간색으로 표시됩니다.",
                710,
                500,
                10F,
                FontStyle.Regular,
                Color.DimGray
            );
            lblPilotNote.Size = new Size(560, 140);

            tabPilotTest.Controls.AddRange(new Control[]
            {
                lblTitlePilot, lblModelPath, txtModelPath, btnBrowseModel,
                btnRunPilotTest, btnUseViewerFrame, picPilotTest,
                lblPilotImageList, lstPilotFrames,
                lblActualAngle, lblPredictedAngle, lblActualThrottle, lblPredictedThrottle,
                lblAngleError, lblPilotWarning, lblPilotNote
            });
        }

        private Label NewLabel(string text, int x, int y, float fontSize, FontStyle style, Color color)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                Location = new Point(x, y),
                Font = new Font("맑은 고딕", fontSize, style),
                ForeColor = color
            };
        }

        private Button NewButton(string text, int x, int y, int w, int h)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                UseVisualStyleBackColor = true
            };
        }

        private TextBox NewTextBox(int x, int y, int w, int h)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(w, h)
            };
        }

        private PictureBox NewPictureBox(int x, int y, int w, int h)
        {
            return new PictureBox
            {
                BackColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(x, y),
                Size = new Size(w, h),
                SizeMode = PictureBoxSizeMode.Zoom,
                TabStop = false
            };
        }

        private ListBox NewListBox(int x, int y, int w, int h)
        {
            return new ListBox
            {
                Font = new Font("Consolas", 9F),
                HorizontalScrollbar = true,
                ItemHeight = 18,
                Location = new Point(x, y),
                Size = new Size(w, h)
            };
        }
    }
}
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
        private HScrollBar hsbCleanerTimeline;
        private Label lblCleanerTimelineScrollInfo;
        private Label lblCleanerRangeInfo;
        private Label lblCleanerRangeHint;
        private Button btnDeleteRange;
        private Button btnPlayRange;
        private Button btnClearRange;
        private Button btnCleanerAutoPlay;
        private Button btnCleanerStop;

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
        private Button btnPilotAutoPlay;
        private Button btnPilotStop;
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
            hsbCleanerTimeline = new HScrollBar();
            lblCleanerTimelineScrollInfo = new Label();
            lblCleanerRangeInfo = new Label();
            lblCleanerRangeHint = new Label();
            btnDeleteRange = new Button();
            btnPlayRange = new Button();
            btnClearRange = new Button();
            btnCleanerAutoPlay = new Button();
            btnCleanerStop = new Button();

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
            btnPilotAutoPlay = new Button();
            btnPilotStop = new Button();
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

            // =========================
            // Viewer Tab
            // =========================
            tabViewer.BackColor = Color.WhiteSmoke;
            tabViewer.Location = new Point(4, 32);
            tabViewer.Name = "tabViewer";
            tabViewer.Padding = new Padding(3);
            tabViewer.Size = new Size(1392, 684);
            tabViewer.Text = "Viewer - 데이터 확인";

            lblTitleViewer.AutoSize = true;
            lblTitleViewer.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitleViewer.ForeColor = Color.FromArgb(30, 90, 160);
            lblTitleViewer.Location = new Point(20, 18);
            lblTitleViewer.Name = "lblTitleViewer";
            lblTitleViewer.Text = "Donkeycar Tub Viewer";

            btnOpenDataFolder.Location = new Point(20, 82);
            btnOpenDataFolder.Name = "btnOpenDataFolder";
            btnOpenDataFolder.Size = new Size(160, 38);
            btnOpenDataFolder.Text = "데이터 폴더 열기";
            btnOpenDataFolder.UseVisualStyleBackColor = true;

            btnReload.Location = new Point(190, 82);
            btnReload.Name = "btnReload";
            btnReload.Size = new Size(110, 38);
            btnReload.Text = "새로고침";
            btnReload.UseVisualStyleBackColor = true;

            btnAutoPlay.Location = new Point(310, 82);
            btnAutoPlay.Name = "btnAutoPlay";
            btnAutoPlay.Size = new Size(120, 38);
            btnAutoPlay.Text = "자동 재생";
            btnAutoPlay.UseVisualStyleBackColor = true;

            lblDataPath.AutoSize = true;
            lblDataPath.ForeColor = Color.DimGray;
            lblDataPath.Location = new Point(450, 90);
            lblDataPath.Name = "lblDataPath";
            lblDataPath.Text = "Data Folder: -";

            picFrame.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picFrame.BackColor = Color.Black;
            picFrame.BorderStyle = BorderStyle.FixedSingle;
            picFrame.Location = new Point(20, 140);
            picFrame.Name = "picFrame";
            picFrame.Size = new Size(830, 390);
            picFrame.SizeMode = PictureBoxSizeMode.Zoom;
            picFrame.TabStop = false;

            lstFrames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lstFrames.Font = new Font("Consolas", 9F);
            lstFrames.HorizontalScrollbar = true;
            lstFrames.ItemHeight = 18;
            lstFrames.Location = new Point(870, 140);
            lstFrames.Name = "lstFrames";
            lstFrames.Size = new Size(500, 390);

            lblFrameInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblFrameInfo.AutoSize = true;
            lblFrameInfo.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblFrameInfo.Location = new Point(20, 548);
            lblFrameInfo.Name = "lblFrameInfo";
            lblFrameInfo.Text = "Frame: -";

            lblAngle.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblAngle.AutoSize = true;
            lblAngle.Font = new Font("맑은 고딕", 11F);
            lblAngle.Location = new Point(200, 548);
            lblAngle.Name = "lblAngle";
            lblAngle.Text = "Angle: -";

            lblThrottle.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblThrottle.AutoSize = true;
            lblThrottle.Font = new Font("맑은 고딕", 11F);
            lblThrottle.Location = new Point(410, 548);
            lblThrottle.Name = "lblThrottle";
            lblThrottle.Text = "Throttle: -";

            lblMode.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblMode.AutoSize = true;
            lblMode.Font = new Font("맑은 고딕", 11F);
            lblMode.Location = new Point(650, 548);
            lblMode.Name = "lblMode";
            lblMode.Text = "Mode: -";

            trbFrame.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trbFrame.Location = new Point(20, 585);
            trbFrame.Maximum = 0;
            trbFrame.Name = "trbFrame";
            trbFrame.Size = new Size(1350, 56);
            trbFrame.TickStyle = TickStyle.None;

            tabViewer.Controls.Add(lblTitleViewer);
            tabViewer.Controls.Add(btnOpenDataFolder);
            tabViewer.Controls.Add(btnReload);
            tabViewer.Controls.Add(btnAutoPlay);
            tabViewer.Controls.Add(lblDataPath);
            tabViewer.Controls.Add(picFrame);
            tabViewer.Controls.Add(lstFrames);
            tabViewer.Controls.Add(lblFrameInfo);
            tabViewer.Controls.Add(lblAngle);
            tabViewer.Controls.Add(lblThrottle);
            tabViewer.Controls.Add(lblMode);
            tabViewer.Controls.Add(trbFrame);

            // =========================
            // Cleaner Tab
            // =========================
            tabCleaner.BackColor = Color.WhiteSmoke;
            tabCleaner.Location = new Point(4, 32);
            tabCleaner.Name = "tabCleaner";
            tabCleaner.Padding = new Padding(3);
            tabCleaner.Size = new Size(1392, 684);
            tabCleaner.Text = "Cleaner - 데이터 정리";

            lblTitleCleaner.AutoSize = true;
            lblTitleCleaner.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitleCleaner.ForeColor = Color.FromArgb(180, 70, 70);
            lblTitleCleaner.Location = new Point(20, 18);
            lblTitleCleaner.Name = "lblTitleCleaner";
            lblTitleCleaner.Text = "Tub Cleaner";

            picCleanerPreview.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            picCleanerPreview.BackColor = Color.Black;
            picCleanerPreview.BorderStyle = BorderStyle.FixedSingle;
            picCleanerPreview.Location = new Point(20, 90);
            picCleanerPreview.Name = "picCleanerPreview";
            picCleanerPreview.Size = new Size(850, 330);
            picCleanerPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picCleanerPreview.TabStop = false;

            lblCleanerInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblCleanerInfo.AutoSize = true;
            lblCleanerInfo.Location = new Point(20, 435);
            lblCleanerInfo.Name = "lblCleanerInfo";
            lblCleanerInfo.Text = "선택 프레임 정보: -";

            grpFilters.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            grpFilters.Location = new Point(900, 75);
            grpFilters.Name = "grpFilters";
            grpFilters.Size = new Size(310, 155);
            grpFilters.TabStop = false;
            grpFilters.Text = "필터 조건";

            chkThrottlePositive.AutoSize = true;
            chkThrottlePositive.Location = new Point(20, 35);
            chkThrottlePositive.Name = "chkThrottlePositive";
            chkThrottlePositive.Text = "throttle > 0만 보기";
            chkThrottlePositive.UseVisualStyleBackColor = true;

            chkExcludeZeroAngle.AutoSize = true;
            chkExcludeZeroAngle.Location = new Point(20, 75);
            chkExcludeZeroAngle.Name = "chkExcludeZeroAngle";
            chkExcludeZeroAngle.Text = "angle == 0 제외";
            chkExcludeZeroAngle.UseVisualStyleBackColor = true;

            chkStopDataOnly.AutoSize = true;
            chkStopDataOnly.Location = new Point(20, 115);
            chkStopDataOnly.Name = "chkStopDataOnly";
            chkStopDataOnly.Text = "정지 데이터만 보기(throttle == 0)";
            chkStopDataOnly.UseVisualStyleBackColor = true;

            grpFilters.Controls.Add(chkThrottlePositive);
            grpFilters.Controls.Add(chkExcludeZeroAngle);
            grpFilters.Controls.Add(chkStopDataOnly);

            btnApplyFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnApplyFilter.Location = new Point(1230, 80);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(130, 40);
            btnApplyFilter.Text = "필터 적용";
            btnApplyFilter.UseVisualStyleBackColor = true;

            btnClearFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearFilter.Location = new Point(1230, 135);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(130, 40);
            btnClearFilter.Text = "전체 보기";
            btnClearFilter.UseVisualStyleBackColor = true;

            btnDeleteFrame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteFrame.BackColor = Color.LightCoral;
            btnDeleteFrame.FlatStyle = FlatStyle.Flat;
            btnDeleteFrame.Location = new Point(1230, 190);
            btnDeleteFrame.Name = "btnDeleteFrame";
            btnDeleteFrame.Size = new Size(130, 50);
            btnDeleteFrame.Text = "선택 프레임 삭제";
            btnDeleteFrame.UseVisualStyleBackColor = false;

            lstCleanerFrames.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lstCleanerFrames.Font = new Font("Consolas", 9F);
            lstCleanerFrames.HorizontalScrollbar = true;
            lstCleanerFrames.ItemHeight = 18;
            lstCleanerFrames.Location = new Point(900, 255);
            lstCleanerFrames.Name = "lstCleanerFrames";
            lstCleanerFrames.SelectionMode = SelectionMode.MultiExtended;
            lstCleanerFrames.Size = new Size(460, 195);

            grpCleanerRangeEditor.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            grpCleanerRangeEditor.Location = new Point(20, 470);
            grpCleanerRangeEditor.Name = "grpCleanerRangeEditor";
            grpCleanerRangeEditor.Size = new Size(1340, 190);
            grpCleanerRangeEditor.TabStop = false;
            grpCleanerRangeEditor.Text = "구간 선택 편집";

            lblCleanerRangeInfo.AutoSize = true;
            lblCleanerRangeInfo.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);
            lblCleanerRangeInfo.ForeColor = Color.FromArgb(170, 60, 50);
            lblCleanerRangeInfo.Location = new Point(15, 28);
            lblCleanerRangeInfo.Name = "lblCleanerRangeInfo";
            lblCleanerRangeInfo.Text = "선택 구간: 없음";

            lblCleanerRangeHint.AutoSize = true;
            lblCleanerRangeHint.ForeColor = Color.DimGray;
            lblCleanerRangeHint.Location = new Point(360, 28);
            lblCleanerRangeHint.Name = "lblCleanerRangeHint";
            lblCleanerRangeHint.Text = "스크롤바로 구간 이동 / 썸네일 1개 = 실제 이미지 1장 / 드래그로 구간 선택";

            pnlCleanerTimeline.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlCleanerTimeline.BackColor = Color.FromArgb(18, 26, 42);
            pnlCleanerTimeline.BorderStyle = BorderStyle.FixedSingle;
            pnlCleanerTimeline.Location = new Point(15, 58);
            pnlCleanerTimeline.Name = "pnlCleanerTimeline";
            pnlCleanerTimeline.Size = new Size(1060, 82);

            hsbCleanerTimeline.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            hsbCleanerTimeline.Location = new Point(15, 146);
            hsbCleanerTimeline.Name = "hsbCleanerTimeline";
            hsbCleanerTimeline.Size = new Size(1060, 22);
            hsbCleanerTimeline.Minimum = 0;
            hsbCleanerTimeline.Maximum = 0;
            hsbCleanerTimeline.SmallChange = 1;
            hsbCleanerTimeline.LargeChange = 1;

            lblCleanerTimelineScrollInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            lblCleanerTimelineScrollInfo.AutoSize = true;
            lblCleanerTimelineScrollInfo.ForeColor = Color.DimGray;
            lblCleanerTimelineScrollInfo.Location = new Point(15, 168);
            lblCleanerTimelineScrollInfo.Name = "lblCleanerTimelineScrollInfo";
            lblCleanerTimelineScrollInfo.Text = "표시 구간: -";

            btnDeleteRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteRange.BackColor = Color.FromArgb(180, 60, 50);
            btnDeleteRange.FlatStyle = FlatStyle.Flat;
            btnDeleteRange.ForeColor = Color.White;
            btnDeleteRange.Location = new Point(1095, 58);
            btnDeleteRange.Name = "btnDeleteRange";
            btnDeleteRange.Size = new Size(105, 34);
            btnDeleteRange.Text = "구간 삭제";
            btnDeleteRange.UseVisualStyleBackColor = false;

            btnPlayRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPlayRange.BackColor = Color.FromArgb(70, 110, 160);
            btnPlayRange.FlatStyle = FlatStyle.Flat;
            btnPlayRange.ForeColor = Color.White;
            btnPlayRange.Location = new Point(1210, 58);
            btnPlayRange.Name = "btnPlayRange";
            btnPlayRange.Size = new Size(105, 34);
            btnPlayRange.Text = "구간 재생";
            btnPlayRange.UseVisualStyleBackColor = false;

            btnClearRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearRange.Location = new Point(1095, 102);
            btnClearRange.Name = "btnClearRange";
            btnClearRange.Size = new Size(105, 34);
            btnClearRange.Text = "구간 해제";
            btnClearRange.UseVisualStyleBackColor = true;

            btnCleanerAutoPlay.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCleanerAutoPlay.BackColor = Color.FromArgb(76, 175, 80);
            btnCleanerAutoPlay.FlatStyle = FlatStyle.Flat;
            btnCleanerAutoPlay.ForeColor = Color.White;
            btnCleanerAutoPlay.Location = new Point(1210, 102);
            btnCleanerAutoPlay.Name = "btnCleanerAutoPlay";
            btnCleanerAutoPlay.Size = new Size(105, 34);
            btnCleanerAutoPlay.Text = "자동 재생";
            btnCleanerAutoPlay.UseVisualStyleBackColor = false;

            btnCleanerStop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCleanerStop.Location = new Point(1095, 146);
            btnCleanerStop.Name = "btnCleanerStop";
            btnCleanerStop.Size = new Size(220, 34);
            btnCleanerStop.Text = "멈춤";
            btnCleanerStop.UseVisualStyleBackColor = true;

            grpCleanerRangeEditor.Controls.Add(lblCleanerRangeInfo);
            grpCleanerRangeEditor.Controls.Add(lblCleanerRangeHint);
            grpCleanerRangeEditor.Controls.Add(pnlCleanerTimeline);
            grpCleanerRangeEditor.Controls.Add(hsbCleanerTimeline);
            grpCleanerRangeEditor.Controls.Add(lblCleanerTimelineScrollInfo);
            grpCleanerRangeEditor.Controls.Add(btnDeleteRange);
            grpCleanerRangeEditor.Controls.Add(btnPlayRange);
            grpCleanerRangeEditor.Controls.Add(btnClearRange);
            grpCleanerRangeEditor.Controls.Add(btnCleanerAutoPlay);
            grpCleanerRangeEditor.Controls.Add(btnCleanerStop);

            tabCleaner.Controls.Add(lblTitleCleaner);
            tabCleaner.Controls.Add(picCleanerPreview);
            tabCleaner.Controls.Add(lblCleanerInfo);
            tabCleaner.Controls.Add(grpFilters);
            tabCleaner.Controls.Add(btnApplyFilter);
            tabCleaner.Controls.Add(btnClearFilter);
            tabCleaner.Controls.Add(btnDeleteFrame);
            tabCleaner.Controls.Add(lstCleanerFrames);
            tabCleaner.Controls.Add(grpCleanerRangeEditor);

            // =========================
            // Trainer Tab
            // =========================
            tabTrainer.BackColor = Color.WhiteSmoke;
            tabTrainer.Location = new Point(4, 32);
            tabTrainer.Name = "tabTrainer";
            tabTrainer.Padding = new Padding(3);
            tabTrainer.Size = new Size(1392, 684);
            tabTrainer.Text = "Trainer - 학습 실행";

            lblTitleTrainer.AutoSize = true;
            lblTitleTrainer.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitleTrainer.ForeColor = Color.FromArgb(60, 130, 80);
            lblTitleTrainer.Location = new Point(20, 18);
            lblTitleTrainer.Name = "lblTitleTrainer";
            lblTitleTrainer.Text = "Donkeycar Trainer";

            lblMycarPath.AutoSize = true;
            lblMycarPath.Location = new Point(30, 100);
            lblMycarPath.Name = "lblMycarPath";
            lblMycarPath.Text = "mycar 경로";

            txtMycarPath.Location = new Point(150, 96);
            txtMycarPath.Name = "txtMycarPath";
            txtMycarPath.Size = new Size(720, 30);

            btnBrowseMycar.Location = new Point(890, 95);
            btnBrowseMycar.Name = "btnBrowseMycar";
            btnBrowseMycar.Size = new Size(90, 34);
            btnBrowseMycar.Text = "찾기";
            btnBrowseMycar.UseVisualStyleBackColor = true;

            lblPythonExe.AutoSize = true;
            lblPythonExe.Location = new Point(30, 155);
            lblPythonExe.Name = "lblPythonExe";
            lblPythonExe.Text = "Python 실행명";

            txtPythonExe.Location = new Point(150, 151);
            txtPythonExe.Name = "txtPythonExe";
            txtPythonExe.Size = new Size(300, 30);

            lblTrainArgs.AutoSize = true;
            lblTrainArgs.Location = new Point(30, 210);
            lblTrainArgs.Name = "lblTrainArgs";
            lblTrainArgs.Text = "학습 명령 인자";

            txtTrainArgs.Location = new Point(150, 206);
            txtTrainArgs.Name = "txtTrainArgs";
            txtTrainArgs.Size = new Size(830, 30);

            btnTrain.BackColor = Color.FromArgb(76, 175, 80);
            btnTrain.FlatStyle = FlatStyle.Flat;
            btnTrain.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            btnTrain.ForeColor = Color.White;
            btnTrain.Location = new Point(150, 270);
            btnTrain.Name = "btnTrain";
            btnTrain.Size = new Size(160, 50);
            btnTrain.Text = "학습 시작";
            btnTrain.UseVisualStyleBackColor = false;

            btnStopTrain.BackColor = Color.LightCoral;
            btnStopTrain.FlatStyle = FlatStyle.Flat;
            btnStopTrain.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            btnStopTrain.Location = new Point(330, 270);
            btnStopTrain.Name = "btnStopTrain";
            btnStopTrain.Size = new Size(160, 50);
            btnStopTrain.Text = "학습 중지";
            btnStopTrain.UseVisualStyleBackColor = false;

            lblModelStatus.AutoSize = true;
            lblModelStatus.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblModelStatus.Location = new Point(150, 345);
            lblModelStatus.Name = "lblModelStatus";
            lblModelStatus.Text = "모델 상태: -";

            lblTrainInfo.AutoSize = true;
            lblTrainInfo.ForeColor = Color.DimGray;
            lblTrainInfo.Location = new Point(150, 395);
            lblTrainInfo.Name = "lblTrainInfo";
            lblTrainInfo.Text =
                "자료 기준 학습 명령 예시:\n" +
                "python train.py --tub ./data --model ./models/mypilot.h5\n\n" +
                "C#은 AI를 직접 학습하지 않고 Python 외부 프로세스를 실행합니다.";

            tabTrainer.Controls.Add(lblTitleTrainer);
            tabTrainer.Controls.Add(txtLog);
            tabTrainer.Controls.Add(lblMycarPath);
            tabTrainer.Controls.Add(txtMycarPath);
            tabTrainer.Controls.Add(btnBrowseMycar);
            tabTrainer.Controls.Add(lblPythonExe);
            tabTrainer.Controls.Add(txtPythonExe);
            tabTrainer.Controls.Add(lblTrainArgs);
            tabTrainer.Controls.Add(txtTrainArgs);
            tabTrainer.Controls.Add(btnTrain);
            tabTrainer.Controls.Add(btnStopTrain);
            tabTrainer.Controls.Add(lblModelStatus);
            tabTrainer.Controls.Add(lblTrainInfo);

            // =========================
            // Pilot Test Tab
            // =========================
            tabPilotTest.BackColor = Color.WhiteSmoke;
            tabPilotTest.Location = new Point(4, 32);
            tabPilotTest.Name = "tabPilotTest";
            tabPilotTest.Padding = new Padding(3);
            tabPilotTest.Size = new Size(1392, 684);
            tabPilotTest.Text = "Pilot Test - 모델 테스트";

            lblTitlePilot.AutoSize = true;
            lblTitlePilot.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitlePilot.ForeColor = Color.FromArgb(90, 90, 160);
            lblTitlePilot.Location = new Point(20, 18);
            lblTitlePilot.Name = "lblTitlePilot";
            lblTitlePilot.Text = "Pilot Arena / Model Test";

            lblModelPath.AutoSize = true;
            lblModelPath.Location = new Point(30, 100);
            lblModelPath.Name = "lblModelPath";
            lblModelPath.Text = "모델 파일";

            txtModelPath.Location = new Point(120, 96);
            txtModelPath.Name = "txtModelPath";
            txtModelPath.Size = new Size(640, 30);

            btnBrowseModel.Location = new Point(780, 95);
            btnBrowseModel.Name = "btnBrowseModel";
            btnBrowseModel.Size = new Size(90, 34);
            btnBrowseModel.Text = "찾기";
            btnBrowseModel.UseVisualStyleBackColor = true;

            btnRunPilotTest.Location = new Point(120, 145);
            btnRunPilotTest.Name = "btnRunPilotTest";
            btnRunPilotTest.Size = new Size(250, 42);
            btnRunPilotTest.Text = "현재 이미지로 예측 테스트";
            btnRunPilotTest.UseVisualStyleBackColor = true;

            btnUseViewerFrame.Location = new Point(390, 145);
            btnUseViewerFrame.Name = "btnUseViewerFrame";
            btnUseViewerFrame.Size = new Size(210, 42);
            btnUseViewerFrame.Text = "Viewer 선택 이미지 사용";
            btnUseViewerFrame.UseVisualStyleBackColor = true;

            btnPilotAutoPlay.BackColor = Color.FromArgb(76, 175, 80);
            btnPilotAutoPlay.FlatStyle = FlatStyle.Flat;
            btnPilotAutoPlay.ForeColor = Color.White;
            btnPilotAutoPlay.Location = new Point(620, 145);
            btnPilotAutoPlay.Name = "btnPilotAutoPlay";
            btnPilotAutoPlay.Size = new Size(120, 42);
            btnPilotAutoPlay.Text = "자동 재생";
            btnPilotAutoPlay.UseVisualStyleBackColor = false;

            btnPilotStop.Location = new Point(755, 145);
            btnPilotStop.Name = "btnPilotStop";
            btnPilotStop.Size = new Size(100, 42);
            btnPilotStop.Text = "멈춤";
            btnPilotStop.UseVisualStyleBackColor = true;

            picPilotTest.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            picPilotTest.BackColor = Color.Black;
            picPilotTest.BorderStyle = BorderStyle.FixedSingle;
            picPilotTest.Location = new Point(30, 220);
            picPilotTest.Name = "picPilotTest";
            picPilotTest.Size = new Size(550, 450);
            picPilotTest.SizeMode = PictureBoxSizeMode.Zoom;
            picPilotTest.TabStop = false;

            lblActualAngle.AutoSize = true;
            lblActualAngle.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblActualAngle.Location = new Point(610, 230);
            lblActualAngle.Name = "lblActualAngle";
            lblActualAngle.Text = "실제 Angle: -";

            lblPredictedAngle.AutoSize = true;
            lblPredictedAngle.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblPredictedAngle.Location = new Point(610, 270);
            lblPredictedAngle.Name = "lblPredictedAngle";
            lblPredictedAngle.Text = "예측 Angle: -";

            lblActualThrottle.AutoSize = true;
            lblActualThrottle.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblActualThrottle.Location = new Point(610, 320);
            lblActualThrottle.Name = "lblActualThrottle";
            lblActualThrottle.Text = "실제 Throttle: -";

            lblPredictedThrottle.AutoSize = true;
            lblPredictedThrottle.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblPredictedThrottle.Location = new Point(610, 355);
            lblPredictedThrottle.Name = "lblPredictedThrottle";
            lblPredictedThrottle.Text = "예측 Throttle: -";

            lblAngleError.AutoSize = true;
            lblAngleError.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblAngleError.Location = new Point(610, 405);
            lblAngleError.Name = "lblAngleError";
            lblAngleError.Text = "Angle Error: -";

            lblPilotWarning.AutoSize = true;
            lblPilotWarning.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblPilotWarning.ForeColor = Color.DimGray;
            lblPilotWarning.Location = new Point(610, 450);
            lblPilotWarning.Name = "lblPilotWarning";
            lblPilotWarning.Text = "판정: -";

            lblPilotNote.ForeColor = Color.DimGray;
            lblPilotNote.Location = new Point(610, 500);
            lblPilotNote.Name = "lblPilotNote";
            lblPilotNote.Size = new Size(240, 150);
            lblPilotNote.Text =
                "파란선: 실제 angle\n" +
                "초록선: 예측 angle\n" +
                "노란 반투명 영역:\n" +
                "실제/예측 차이\n" +
                "하단 막대:\n" +
                "실제/예측 throttle 비교";

            lblPilotImageList.AutoSize = true;
            lblPilotImageList.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblPilotImageList.Location = new Point(880, 200);
            lblPilotImageList.Name = "lblPilotImageList";
            lblPilotImageList.Text = "테스트 이미지 선택";

            lstPilotFrames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstPilotFrames.Font = new Font("Consolas", 10F);
            lstPilotFrames.HorizontalScrollbar = true;
            lstPilotFrames.ItemHeight = 20;
            lstPilotFrames.Location = new Point(880, 230);
            lstPilotFrames.Name = "lstPilotFrames";
            lstPilotFrames.SelectionMode = SelectionMode.One;
            lstPilotFrames.Size = new Size(480, 440);

            tabPilotTest.Controls.Add(lblTitlePilot);
            tabPilotTest.Controls.Add(lblModelPath);
            tabPilotTest.Controls.Add(txtModelPath);
            tabPilotTest.Controls.Add(btnBrowseModel);
            tabPilotTest.Controls.Add(btnRunPilotTest);
            tabPilotTest.Controls.Add(btnUseViewerFrame);
            tabPilotTest.Controls.Add(btnPilotAutoPlay);
            tabPilotTest.Controls.Add(btnPilotStop);
            tabPilotTest.Controls.Add(picPilotTest);
            tabPilotTest.Controls.Add(lblActualAngle);
            tabPilotTest.Controls.Add(lblPredictedAngle);
            tabPilotTest.Controls.Add(lblActualThrottle);
            tabPilotTest.Controls.Add(lblPredictedThrottle);
            tabPilotTest.Controls.Add(lblAngleError);
            tabPilotTest.Controls.Add(lblPilotWarning);
            tabPilotTest.Controls.Add(lblPilotNote);
            tabPilotTest.Controls.Add(lblPilotImageList);
            tabPilotTest.Controls.Add(lstPilotFrames);

            txtLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Font = new Font("Consolas", 9F);
<<<<<<< Updated upstream
            txtLog.Location = new Point(0, 725);
=======
            txtLog.Location = new Point(-4, 418);
            txtLog.Margin = new Padding(2);
>>>>>>> Stashed changes
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(1400, 175);

            Controls.Add(tabMain);
<<<<<<< Updated upstream
            Controls.Add(txtLog);

            ((System.ComponentModel.ISupportInitialize)picFrame).EndInit();
=======
            Margin = new Padding(2);
            MinimumSize = new Size(962, 487);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Donkeycar Manager";
            WindowState = FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)trbBrightness).EndInit();
            ((System.ComponentModel.ISupportInitialize)trbContrast).EndInit();
            tabMain.ResumeLayout(false);
            tabCleaner.ResumeLayout(false);
            tabCleaner.PerformLayout();
>>>>>>> Stashed changes
            ((System.ComponentModel.ISupportInitialize)picCleanerPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)picPilotTest).EndInit();
            ((System.ComponentModel.ISupportInitialize)trbFrame).EndInit();

            ResumeLayout(false);
        }
    }
}
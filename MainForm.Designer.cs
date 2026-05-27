using System.Drawing;
using System.Windows.Forms;

namespace DonkeycarManager
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private ComboBox cmbModelList;
        private Button btnScanModels;
        private Label lblModelList;
        private TrackBar trbBrightness;
        private TrackBar trbContrast;
        private Label lblBrightness;
        private Label lblContrast;


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

        private CheckBox chkFlipHorizontal;
        private CheckBox chkGrayscale;
        private Button btnSaveProcessed;
        private Label lblImageAdjust;

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
            tabMain = new TabControl();
            tabViewer = new TabPage();
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
            tabCleaner = new TabPage();
            lblTitleCleaner = new Label();
            picCleanerPreview = new PictureBox();
            lblCleanerInfo = new Label();
            grpFilters = new GroupBox();
            chkThrottlePositive = new CheckBox();
            chkExcludeZeroAngle = new CheckBox();
            chkStopDataOnly = new CheckBox();
            btnApplyFilter = new Button();
            btnClearFilter = new Button();
            btnDeleteFrame = new Button();
            lstCleanerFrames = new ListBox();
            grpCleanerRangeEditor = new GroupBox();
            lblCleanerRangeInfo = new Label();
            lblCleanerRangeHint = new Label();
            pnlCleanerTimeline = new Panel();
            hsbCleanerTimeline = new HScrollBar();
            lblCleanerTimelineScrollInfo = new Label();
            btnDeleteRange = new Button();
            btnPlayRange = new Button();
            btnClearRange = new Button();
            btnCleanerAutoPlay = new Button();
            btnCleanerStop = new Button();
            tabTrainer = new TabPage();
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
            tabPilotTest = new TabPage();
            lblTitlePilot = new Label();
            lblModelPath = new Label();
            txtModelPath = new TextBox();
            btnBrowseModel = new Button();
            btnRunPilotTest = new Button();
            btnUseViewerFrame = new Button();
            btnPilotAutoPlay = new Button();
            btnPilotStop = new Button();
            picPilotTest = new PictureBox();
            lblActualAngle = new Label();
            lblPredictedAngle = new Label();
            lblActualThrottle = new Label();
            lblPredictedThrottle = new Label();
            lblAngleError = new Label();
            lblPilotWarning = new Label();
            lblPilotNote = new Label();
            lblPilotImageList = new Label();
            lstPilotFrames = new ListBox();
            TbtnPilot = new Button();
            TbtnTrain = new Button();
            TbtnClean = new Button();
            TbtnView = new Button();
            txtLog = new TextBox();
            tabMain.SuspendLayout();
            tabViewer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picFrame).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trbFrame).BeginInit();
            tabCleaner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picCleanerPreview).BeginInit();
            grpFilters.SuspendLayout();
            grpCleanerRangeEditor.SuspendLayout();
            tabTrainer.SuspendLayout();
            tabPilotTest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPilotTest).BeginInit();
            SuspendLayout();
            // 
            // tabMain
            // 
            tabMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabMain.Controls.Add(tabViewer);
            tabMain.Controls.Add(tabCleaner);
            tabMain.Controls.Add(tabTrainer);
            tabMain.Controls.Add(tabPilotTest);
            tabMain.Font = new Font("맑은 고딕", 10F);
            tabMain.ItemSize = new Size(1, 0);
            tabMain.Location = new Point(0, 23);
            tabMain.Margin = new Padding(2);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(1089, 582);
            tabMain.SizeMode = TabSizeMode.Fixed;
            tabMain.TabIndex = 0;
            // 
            // tabViewer
            // 
            tabViewer.BackColor = Color.FromArgb(30, 30, 30);
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
            tabViewer.Location = new Point(4, 26);
            tabViewer.Margin = new Padding(2);
            tabViewer.Name = "tabViewer";
            tabViewer.Padding = new Padding(2);
            tabViewer.Size = new Size(1081, 552);
            tabViewer.TabIndex = 0;
            tabViewer.Text = "Viewer - 데이터 확인";
            // 
            // lblTitleViewer
            // 
            lblTitleViewer.AutoSize = true;
            lblTitleViewer.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitleViewer.ForeColor = Color.DeepSkyBlue;
            lblTitleViewer.Location = new Point(16, 14);
            lblTitleViewer.Margin = new Padding(2, 0, 2, 0);
            lblTitleViewer.Name = "lblTitleViewer";
            lblTitleViewer.Size = new Size(341, 41);
            lblTitleViewer.TabIndex = 0;
            lblTitleViewer.Text = "Donkeycar Tub Viewer";
            // 
            // btnOpenDataFolder
            // 
            btnOpenDataFolder.BackColor = Color.DodgerBlue;
            btnOpenDataFolder.FlatStyle = FlatStyle.Flat;
            btnOpenDataFolder.ForeColor = Color.White;
            btnOpenDataFolder.Location = new Point(16, 62);
            btnOpenDataFolder.Margin = new Padding(2);
            btnOpenDataFolder.Name = "btnOpenDataFolder";
            btnOpenDataFolder.Size = new Size(154, 28);
            btnOpenDataFolder.TabIndex = 1;
            btnOpenDataFolder.Text = "📂 데이터 폴더 열기";
            btnOpenDataFolder.UseVisualStyleBackColor = false;
            // 
            // btnReload
            // 
            btnReload.BackColor = Color.LightGray;
            btnReload.FlatAppearance.BorderColor = Color.DarkGray;
            btnReload.FlatStyle = FlatStyle.Flat;
            btnReload.ForeColor = Color.Black;
            btnReload.Location = new Point(185, 62);
            btnReload.Margin = new Padding(2);
            btnReload.Name = "btnReload";
            btnReload.Size = new Size(99, 28);
            btnReload.TabIndex = 2;
            btnReload.Text = "🔄 새로고침";
            btnReload.UseVisualStyleBackColor = false;
            // 
            // btnAutoPlay
            // 
            btnAutoPlay.BackColor = Color.LightGray;
            btnAutoPlay.FlatAppearance.BorderColor = Color.DarkGray;
            btnAutoPlay.FlatStyle = FlatStyle.Flat;
            btnAutoPlay.ForeColor = Color.Black;
            btnAutoPlay.Location = new Point(301, 62);
            btnAutoPlay.Margin = new Padding(2);
            btnAutoPlay.Name = "btnAutoPlay";
            btnAutoPlay.Size = new Size(103, 28);
            btnAutoPlay.TabIndex = 3;
            btnAutoPlay.Text = "▶️ 자동 재생";
            btnAutoPlay.UseVisualStyleBackColor = false;
            // 
            // lblDataPath
            // 
            lblDataPath.AutoSize = true;
            lblDataPath.ForeColor = Color.DimGray;
            lblDataPath.Location = new Point(418, 68);
            lblDataPath.Margin = new Padding(2, 0, 2, 0);
            lblDataPath.Name = "lblDataPath";
            lblDataPath.Size = new Size(95, 19);
            lblDataPath.TabIndex = 4;
            lblDataPath.Text = "Data Folder: -";
            // 
            // picFrame
            // 
            picFrame.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picFrame.BackColor = Color.Black;
            picFrame.BorderStyle = BorderStyle.FixedSingle;
            picFrame.Location = new Point(16, 105);
            picFrame.Margin = new Padding(2);
            picFrame.Name = "picFrame";
            picFrame.Size = new Size(646, 335);
            picFrame.SizeMode = PictureBoxSizeMode.Zoom;
            picFrame.TabIndex = 5;
            picFrame.TabStop = false;
            // 
            // lstFrames
            // 
            lstFrames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lstFrames.BackColor = Color.FromArgb(20, 20, 20);
            lstFrames.Font = new Font("Consolas", 9F);
            lstFrames.ForeColor = Color.White;
            lstFrames.HorizontalScrollbar = true;
            lstFrames.Location = new Point(677, 105);
            lstFrames.Margin = new Padding(2);
            lstFrames.Name = "lstFrames";
            lstFrames.Size = new Size(390, 270);
            lstFrames.TabIndex = 6;
            // 
            // lblFrameInfo
            // 
            lblFrameInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblFrameInfo.AutoSize = true;
            lblFrameInfo.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblFrameInfo.ForeColor = Color.Gainsboro;
            lblFrameInfo.Location = new Point(16, 453);
            lblFrameInfo.Margin = new Padding(2, 0, 2, 0);
            lblFrameInfo.Name = "lblFrameInfo";
            lblFrameInfo.Size = new Size(68, 20);
            lblFrameInfo.TabIndex = 7;
            lblFrameInfo.Text = "Frame: -";
            // 
            // lblAngle
            // 
            lblAngle.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblAngle.AutoSize = true;
            lblAngle.Font = new Font("맑은 고딕", 11F);
            lblAngle.ForeColor = Color.Gainsboro;
            lblAngle.Location = new Point(156, 453);
            lblAngle.Margin = new Padding(2, 0, 2, 0);
            lblAngle.Name = "lblAngle";
            lblAngle.Size = new Size(63, 20);
            lblAngle.TabIndex = 8;
            lblAngle.Text = "Angle: -";
            // 
            // lblThrottle
            // 
            lblThrottle.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblThrottle.AutoSize = true;
            lblThrottle.Font = new Font("맑은 고딕", 11F);
            lblThrottle.ForeColor = Color.Gainsboro;
            lblThrottle.Location = new Point(319, 453);
            lblThrottle.Margin = new Padding(2, 0, 2, 0);
            lblThrottle.Name = "lblThrottle";
            lblThrottle.Size = new Size(76, 20);
            lblThrottle.TabIndex = 9;
            lblThrottle.Text = "Throttle: -";
            // 
            // lblMode
            // 
            lblMode.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblMode.AutoSize = true;
            lblMode.Font = new Font("맑은 고딕", 11F);
            lblMode.ForeColor = Color.Gainsboro;
            lblMode.Location = new Point(506, 453);
            lblMode.Margin = new Padding(2, 0, 2, 0);
            lblMode.Name = "lblMode";
            lblMode.Size = new Size(63, 20);
            lblMode.TabIndex = 10;
            lblMode.Text = "Mode: -";
            // 
            // trbFrame
            // 
            trbFrame.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trbFrame.Location = new Point(16, 481);
            trbFrame.Margin = new Padding(2);
            trbFrame.Maximum = 0;
            trbFrame.Name = "trbFrame";
            trbFrame.Size = new Size(1050, 45);
            trbFrame.TabIndex = 11;
            trbFrame.TickStyle = TickStyle.None;
            // 
            // tabCleaner
            // 
            tabCleaner.BackColor = Color.FromArgb(30, 30, 30);
            tabCleaner.Controls.Add(lblTitleCleaner);
            tabCleaner.Controls.Add(picCleanerPreview);
            tabCleaner.Controls.Add(lblCleanerInfo);
            tabCleaner.Controls.Add(grpFilters);
            tabCleaner.Controls.Add(btnApplyFilter);
            tabCleaner.Controls.Add(btnClearFilter);
            tabCleaner.Controls.Add(btnDeleteFrame);
            tabCleaner.Controls.Add(lstCleanerFrames);
            tabCleaner.Controls.Add(grpCleanerRangeEditor);
            tabCleaner.Location = new Point(4, 54);
            tabCleaner.Margin = new Padding(2);
            tabCleaner.Name = "tabCleaner";
            tabCleaner.Padding = new Padding(2);
            tabCleaner.Size = new Size(1081, 543);
            tabCleaner.TabIndex = 1;
            tabCleaner.Text = "Cleaner - 데이터 정리";
            // 
            // lblTitleCleaner
            // 
            lblTitleCleaner.AutoSize = true;
            lblTitleCleaner.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitleCleaner.ForeColor = Color.DeepSkyBlue;
            lblTitleCleaner.Location = new Point(16, 14);
            lblTitleCleaner.Margin = new Padding(2, 0, 2, 0);
            lblTitleCleaner.Name = "lblTitleCleaner";
            lblTitleCleaner.Size = new Size(188, 41);
            lblTitleCleaner.TabIndex = 0;
            lblTitleCleaner.Text = "Tub Cleaner";
            // 
            // picCleanerPreview
            // 
            picCleanerPreview.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            picCleanerPreview.BackColor = Color.Black;
            picCleanerPreview.BorderStyle = BorderStyle.FixedSingle;
            picCleanerPreview.Location = new Point(16, 68);
            picCleanerPreview.Margin = new Padding(2);
            picCleanerPreview.Name = "picCleanerPreview";
            picCleanerPreview.Size = new Size(662, 248);
            picCleanerPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picCleanerPreview.TabIndex = 1;
            picCleanerPreview.TabStop = false;
            // 
            // lblCleanerInfo
            // 
            lblCleanerInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblCleanerInfo.AutoSize = true;
            lblCleanerInfo.ForeColor = Color.White;
            lblCleanerInfo.Location = new Point(16, 326);
            lblCleanerInfo.Margin = new Padding(2, 0, 2, 0);
            lblCleanerInfo.Name = "lblCleanerInfo";
            lblCleanerInfo.Size = new Size(131, 19);
            lblCleanerInfo.TabIndex = 2;
            lblCleanerInfo.Text = "선택 프레임 정보: -";
            // 
            // grpFilters
            // 
            grpFilters.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            grpFilters.Controls.Add(chkThrottlePositive);
            grpFilters.Controls.Add(chkExcludeZeroAngle);
            grpFilters.Controls.Add(chkStopDataOnly);
            grpFilters.ForeColor = Color.White;
            grpFilters.Location = new Point(691, 56);
            grpFilters.Margin = new Padding(2);
            grpFilters.Name = "grpFilters";
            grpFilters.Padding = new Padding(2);
            grpFilters.Size = new Size(267, 124);
            grpFilters.TabIndex = 3;
            grpFilters.TabStop = false;
            grpFilters.Text = "필터 조건";
            // 
            // chkThrottlePositive
            // 
            chkThrottlePositive.AutoSize = true;
            chkThrottlePositive.Location = new Point(16, 26);
            chkThrottlePositive.Margin = new Padding(2);
            chkThrottlePositive.Name = "chkThrottlePositive";
            chkThrottlePositive.Size = new Size(149, 23);
            chkThrottlePositive.TabIndex = 0;
            chkThrottlePositive.Text = "throttle > 0만 보기";
            chkThrottlePositive.UseVisualStyleBackColor = true;
            // 
            // chkExcludeZeroAngle
            // 
            chkExcludeZeroAngle.AutoSize = true;
            chkExcludeZeroAngle.Location = new Point(16, 56);
            chkExcludeZeroAngle.Margin = new Padding(2);
            chkExcludeZeroAngle.Name = "chkExcludeZeroAngle";
            chkExcludeZeroAngle.Size = new Size(132, 23);
            chkExcludeZeroAngle.TabIndex = 1;
            chkExcludeZeroAngle.Text = "angle == 0 제외";
            chkExcludeZeroAngle.UseVisualStyleBackColor = true;
            // 
            // chkStopDataOnly
            // 
            chkStopDataOnly.AutoSize = true;
            chkStopDataOnly.Location = new Point(16, 86);
            chkStopDataOnly.Margin = new Padding(2);
            chkStopDataOnly.Name = "chkStopDataOnly";
            chkStopDataOnly.Size = new Size(242, 23);
            chkStopDataOnly.TabIndex = 2;
            chkStopDataOnly.Text = "정지 데이터만 보기(throttle == 0)";
            chkStopDataOnly.UseVisualStyleBackColor = true;
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnApplyFilter.BackColor = Color.FromArgb(20, 20, 20);
            btnApplyFilter.FlatAppearance.BorderColor = Color.Gainsboro;
            btnApplyFilter.FlatAppearance.BorderSize = 0;
            btnApplyFilter.FlatStyle = FlatStyle.Flat;
            btnApplyFilter.ForeColor = Color.Gainsboro;
            btnApplyFilter.Location = new Point(962, 68);
            btnApplyFilter.Margin = new Padding(2);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(101, 30);
            btnApplyFilter.TabIndex = 4;
            btnApplyFilter.Text = "필터 적용";
            btnApplyFilter.UseVisualStyleBackColor = false;
            // 
            // btnClearFilter
            // 
            btnClearFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearFilter.BackColor = Color.FromArgb(20, 20, 20);
            btnClearFilter.FlatAppearance.BorderSize = 0;
            btnClearFilter.FlatStyle = FlatStyle.Flat;
            btnClearFilter.ForeColor = Color.Gainsboro;
            btnClearFilter.Location = new Point(962, 105);
            btnClearFilter.Margin = new Padding(2);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(101, 30);
            btnClearFilter.TabIndex = 5;
            btnClearFilter.Text = "전체 보기";
            btnClearFilter.UseVisualStyleBackColor = false;
            // 
            // btnDeleteFrame
            // 
            btnDeleteFrame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteFrame.BackColor = Color.LightCoral;
            btnDeleteFrame.FlatAppearance.BorderSize = 0;
            btnDeleteFrame.FlatStyle = FlatStyle.Flat;
            btnDeleteFrame.Location = new Point(962, 142);
            btnDeleteFrame.Margin = new Padding(2);
            btnDeleteFrame.Name = "btnDeleteFrame";
            btnDeleteFrame.Size = new Size(101, 38);
            btnDeleteFrame.TabIndex = 6;
            btnDeleteFrame.Text = "선택 프레임 삭제";
            btnDeleteFrame.UseVisualStyleBackColor = false;
            // 
            // lstCleanerFrames
            // 
            lstCleanerFrames.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lstCleanerFrames.BackColor = Color.FromArgb(20, 20, 20);
            lstCleanerFrames.Font = new Font("Consolas", 9F);
            lstCleanerFrames.ForeColor = Color.Gainsboro;
            lstCleanerFrames.HorizontalScrollbar = true;
            lstCleanerFrames.Location = new Point(691, 191);
            lstCleanerFrames.Margin = new Padding(2);
            lstCleanerFrames.Name = "lstCleanerFrames";
            lstCleanerFrames.SelectionMode = SelectionMode.MultiExtended;
            lstCleanerFrames.Size = new Size(368, 144);
            lstCleanerFrames.TabIndex = 7;
            // 
            // grpCleanerRangeEditor
            // 
            grpCleanerRangeEditor.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
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
            grpCleanerRangeEditor.ForeColor = Color.White;
            grpCleanerRangeEditor.Location = new Point(16, 331);
            grpCleanerRangeEditor.Margin = new Padding(2);
            grpCleanerRangeEditor.Name = "grpCleanerRangeEditor";
            grpCleanerRangeEditor.Padding = new Padding(2);
            grpCleanerRangeEditor.Size = new Size(1042, 179);
            grpCleanerRangeEditor.TabIndex = 8;
            grpCleanerRangeEditor.TabStop = false;
            grpCleanerRangeEditor.Text = "구간 선택 편집";
            // 
            // lblCleanerRangeInfo
            // 
            lblCleanerRangeInfo.AutoSize = true;
            lblCleanerRangeInfo.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);
            lblCleanerRangeInfo.ForeColor = Color.FromArgb(170, 60, 50);
            lblCleanerRangeInfo.Location = new Point(12, 21);
            lblCleanerRangeInfo.Margin = new Padding(2, 0, 2, 0);
            lblCleanerRangeInfo.Name = "lblCleanerRangeInfo";
            lblCleanerRangeInfo.Size = new Size(107, 19);
            lblCleanerRangeInfo.TabIndex = 0;
            lblCleanerRangeInfo.Text = "선택 구간: 없음";
            // 
            // lblCleanerRangeHint
            // 
            lblCleanerRangeHint.AutoSize = true;
            lblCleanerRangeHint.ForeColor = Color.Gainsboro;
            lblCleanerRangeHint.Location = new Point(280, 21);
            lblCleanerRangeHint.Margin = new Padding(2, 0, 2, 0);
            lblCleanerRangeHint.Name = "lblCleanerRangeHint";
            lblCleanerRangeHint.Size = new Size(490, 19);
            lblCleanerRangeHint.TabIndex = 1;
            lblCleanerRangeHint.Text = "스크롤바로 구간 이동 / 썸네일 1개 = 실제 이미지 1장 / 드래그로 구간 선택";
            // 
            // pnlCleanerTimeline
            // 
            pnlCleanerTimeline.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlCleanerTimeline.BackColor = Color.FromArgb(18, 26, 42);
            pnlCleanerTimeline.BorderStyle = BorderStyle.FixedSingle;
            pnlCleanerTimeline.Location = new Point(12, 44);
            pnlCleanerTimeline.Margin = new Padding(2);
            pnlCleanerTimeline.Name = "pnlCleanerTimeline";
            pnlCleanerTimeline.Size = new Size(825, 62);
            pnlCleanerTimeline.TabIndex = 2;
            // 
            // hsbCleanerTimeline
            // 
            hsbCleanerTimeline.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            hsbCleanerTimeline.LargeChange = 1;
            hsbCleanerTimeline.Location = new Point(12, 110);
            hsbCleanerTimeline.Maximum = 0;
            hsbCleanerTimeline.Name = "hsbCleanerTimeline";
            hsbCleanerTimeline.Size = new Size(824, 22);
            hsbCleanerTimeline.TabIndex = 3;
            // 
            // lblCleanerTimelineScrollInfo
            // 
            lblCleanerTimelineScrollInfo.AutoSize = true;
            lblCleanerTimelineScrollInfo.ForeColor = Color.Gainsboro;
            lblCleanerTimelineScrollInfo.Location = new Point(12, 145);
            lblCleanerTimelineScrollInfo.Margin = new Padding(2, 0, 2, 0);
            lblCleanerTimelineScrollInfo.Name = "lblCleanerTimelineScrollInfo";
            lblCleanerTimelineScrollInfo.Size = new Size(84, 19);
            lblCleanerTimelineScrollInfo.TabIndex = 4;
            lblCleanerTimelineScrollInfo.Text = "표시 구간: -";
            // 
            // btnDeleteRange
            // 
            btnDeleteRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteRange.BackColor = Color.FromArgb(180, 60, 50);
            btnDeleteRange.FlatStyle = FlatStyle.Flat;
            btnDeleteRange.ForeColor = Color.White;
            btnDeleteRange.Location = new Point(852, 44);
            btnDeleteRange.Margin = new Padding(2);
            btnDeleteRange.Name = "btnDeleteRange";
            btnDeleteRange.Size = new Size(82, 26);
            btnDeleteRange.TabIndex = 5;
            btnDeleteRange.Text = "구간 삭제";
            btnDeleteRange.UseVisualStyleBackColor = false;
            // 
            // btnPlayRange
            // 
            btnPlayRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPlayRange.BackColor = Color.FromArgb(70, 110, 160);
            btnPlayRange.FlatStyle = FlatStyle.Flat;
            btnPlayRange.ForeColor = Color.White;
            btnPlayRange.Location = new Point(941, 44);
            btnPlayRange.Margin = new Padding(2);
            btnPlayRange.Name = "btnPlayRange";
            btnPlayRange.Size = new Size(82, 26);
            btnPlayRange.TabIndex = 6;
            btnPlayRange.Text = "구간 재생";
            btnPlayRange.UseVisualStyleBackColor = false;
            // 
            // btnClearRange
            // 
            btnClearRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearRange.ForeColor = Color.Black;
            btnClearRange.Location = new Point(852, 76);
            btnClearRange.Margin = new Padding(2);
            btnClearRange.Name = "btnClearRange";
            btnClearRange.Size = new Size(82, 26);
            btnClearRange.TabIndex = 7;
            btnClearRange.Text = "구간 해제";
            btnClearRange.UseVisualStyleBackColor = true;
            // 
            // btnCleanerAutoPlay
            // 
            btnCleanerAutoPlay.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCleanerAutoPlay.BackColor = Color.FromArgb(76, 175, 80);
            btnCleanerAutoPlay.FlatStyle = FlatStyle.Flat;
            btnCleanerAutoPlay.ForeColor = Color.White;
            btnCleanerAutoPlay.Location = new Point(941, 76);
            btnCleanerAutoPlay.Margin = new Padding(2);
            btnCleanerAutoPlay.Name = "btnCleanerAutoPlay";
            btnCleanerAutoPlay.Size = new Size(82, 26);
            btnCleanerAutoPlay.TabIndex = 8;
            btnCleanerAutoPlay.Text = "자동 재생";
            btnCleanerAutoPlay.UseVisualStyleBackColor = false;
            // 
            // btnCleanerStop
            // 
            btnCleanerStop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCleanerStop.ForeColor = Color.Black;
            btnCleanerStop.Location = new Point(852, 110);
            btnCleanerStop.Margin = new Padding(2);
            btnCleanerStop.Name = "btnCleanerStop";
            btnCleanerStop.Size = new Size(171, 26);
            btnCleanerStop.TabIndex = 9;
            btnCleanerStop.Text = "멈춤";
            btnCleanerStop.UseVisualStyleBackColor = true;
            // 
            // tabTrainer
            // 
            tabTrainer.BackColor = Color.FromArgb(30, 30, 30);
            tabTrainer.Controls.Add(lblTitleTrainer);
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
            tabTrainer.ForeColor = Color.White;
            tabTrainer.Location = new Point(4, 54);
            tabTrainer.Margin = new Padding(2);
            tabTrainer.Name = "tabTrainer";
            tabTrainer.Padding = new Padding(2);
            tabTrainer.Size = new Size(1081, 543);
            tabTrainer.TabIndex = 2;
            tabTrainer.Text = "Trainer - 학습 실행";
            // 
            // lblTitleTrainer
            // 
            lblTitleTrainer.AutoSize = true;
            lblTitleTrainer.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitleTrainer.ForeColor = Color.LimeGreen;
            lblTitleTrainer.Location = new Point(16, 14);
            lblTitleTrainer.Margin = new Padding(2, 0, 2, 0);
            lblTitleTrainer.Name = "lblTitleTrainer";
            lblTitleTrainer.Size = new Size(279, 41);
            lblTitleTrainer.TabIndex = 0;
            lblTitleTrainer.Text = "Donkeycar Trainer";
            // 
            // lblMycarPath
            // 
            lblMycarPath.AutoSize = true;
            lblMycarPath.ForeColor = Color.Gainsboro;
            lblMycarPath.Location = new Point(23, 75);
            lblMycarPath.Margin = new Padding(2, 0, 2, 0);
            lblMycarPath.Name = "lblMycarPath";
            lblMycarPath.Size = new Size(80, 19);
            lblMycarPath.TabIndex = 1;
            lblMycarPath.Text = "mycar 경로";
            // 
            // txtMycarPath
            // 
            txtMycarPath.BackColor = Color.FromArgb(45, 45, 48);
            txtMycarPath.BorderStyle = BorderStyle.FixedSingle;
            txtMycarPath.ForeColor = Color.White;
            txtMycarPath.Location = new Point(127, 71);
            txtMycarPath.Margin = new Padding(2);
            txtMycarPath.Name = "txtMycarPath";
            txtMycarPath.Size = new Size(561, 25);
            txtMycarPath.TabIndex = 2;
            // 
            // btnBrowseMycar
            // 
            btnBrowseMycar.BackColor = Color.FromArgb(64, 64, 64);
            btnBrowseMycar.FlatAppearance.BorderSize = 0;
            btnBrowseMycar.FlatStyle = FlatStyle.Flat;
            btnBrowseMycar.ForeColor = Color.White;
            btnBrowseMycar.Location = new Point(692, 71);
            btnBrowseMycar.Margin = new Padding(2);
            btnBrowseMycar.Name = "btnBrowseMycar";
            btnBrowseMycar.Size = new Size(70, 26);
            btnBrowseMycar.TabIndex = 3;
            btnBrowseMycar.Text = "🔍 찾기";
            btnBrowseMycar.UseVisualStyleBackColor = false;
            // 
            // lblPythonExe
            // 
            lblPythonExe.AutoSize = true;
            lblPythonExe.ForeColor = Color.Gainsboro;
            lblPythonExe.Location = new Point(23, 116);
            lblPythonExe.Margin = new Padding(2, 0, 2, 0);
            lblPythonExe.Name = "lblPythonExe";
            lblPythonExe.Size = new Size(100, 19);
            lblPythonExe.TabIndex = 4;
            lblPythonExe.Text = "Python 실행명";
            // 
            // txtPythonExe
            // 
            txtPythonExe.BackColor = Color.FromArgb(45, 45, 48);
            txtPythonExe.BorderStyle = BorderStyle.FixedSingle;
            txtPythonExe.ForeColor = Color.White;
            txtPythonExe.Location = new Point(127, 113);
            txtPythonExe.Margin = new Padding(2);
            txtPythonExe.Name = "txtPythonExe";
            txtPythonExe.Size = new Size(234, 25);
            txtPythonExe.TabIndex = 5;
            // 
            // lblTrainArgs
            // 
            lblTrainArgs.AutoSize = true;
            lblTrainArgs.ForeColor = Color.Gainsboro;
            lblTrainArgs.Location = new Point(23, 158);
            lblTrainArgs.Margin = new Padding(2, 0, 2, 0);
            lblTrainArgs.Name = "lblTrainArgs";
            lblTrainArgs.Size = new Size(103, 19);
            lblTrainArgs.TabIndex = 6;
            lblTrainArgs.Text = "학습 명령 인자";
            // 
            // txtTrainArgs
            // 
            txtTrainArgs.BackColor = Color.FromArgb(45, 45, 48);
            txtTrainArgs.BorderStyle = BorderStyle.FixedSingle;
            txtTrainArgs.ForeColor = Color.White;
            txtTrainArgs.Location = new Point(127, 155);
            txtTrainArgs.Margin = new Padding(2);
            txtTrainArgs.Name = "txtTrainArgs";
            txtTrainArgs.Size = new Size(646, 25);
            txtTrainArgs.TabIndex = 7;
            // 
            // btnTrain
            // 
            btnTrain.BackColor = Color.ForestGreen;
            btnTrain.FlatStyle = FlatStyle.Flat;
            btnTrain.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            btnTrain.ForeColor = Color.White;
            btnTrain.Location = new Point(117, 202);
            btnTrain.Margin = new Padding(2);
            btnTrain.Name = "btnTrain";
            btnTrain.Size = new Size(124, 38);
            btnTrain.TabIndex = 8;
            btnTrain.Text = "🚀 학습 시작";
            btnTrain.UseVisualStyleBackColor = false;
            // 
            // btnStopTrain
            // 
            btnStopTrain.BackColor = Color.Crimson;
            btnStopTrain.FlatStyle = FlatStyle.Flat;
            btnStopTrain.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            btnStopTrain.Location = new Point(257, 202);
            btnStopTrain.Margin = new Padding(2);
            btnStopTrain.Name = "btnStopTrain";
            btnStopTrain.Size = new Size(124, 38);
            btnStopTrain.TabIndex = 9;
            btnStopTrain.Text = "\U0001f6d1 학습 중지";
            btnStopTrain.UseVisualStyleBackColor = false;
            // 
            // lblModelStatus
            // 
            lblModelStatus.AutoSize = true;
            lblModelStatus.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblModelStatus.ForeColor = Color.White;
            lblModelStatus.Location = new Point(117, 259);
            lblModelStatus.Margin = new Padding(2, 0, 2, 0);
            lblModelStatus.Name = "lblModelStatus";
            lblModelStatus.Size = new Size(89, 20);
            lblModelStatus.TabIndex = 10;
            lblModelStatus.Text = "모델 상태: -";
            // 
            // lblTrainInfo
            // 
            lblTrainInfo.AutoSize = true;
            lblTrainInfo.ForeColor = Color.Gainsboro;
            lblTrainInfo.Location = new Point(117, 296);
            lblTrainInfo.Margin = new Padding(2, 0, 2, 0);
            lblTrainInfo.Name = "lblTrainInfo";
            lblTrainInfo.Size = new Size(434, 76);
            lblTrainInfo.TabIndex = 11;
            lblTrainInfo.Text = "자료 기준 학습 명령 예시:\npython train.py --tub ./data --model ./models/mypilot.h5\n\nC#은 AI를 직접 학습하지 않고 Python 외부 프로세스를 실행합니다.";
            // 
            // tabPilotTest
            // 
            tabPilotTest.BackColor = Color.FromArgb(30, 30, 30);
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
            tabPilotTest.Location = new Point(4, 54);
            tabPilotTest.Margin = new Padding(2);
            tabPilotTest.Name = "tabPilotTest";
            tabPilotTest.Padding = new Padding(2);
            tabPilotTest.Size = new Size(1081, 543);
            tabPilotTest.TabIndex = 3;
            tabPilotTest.Text = "Pilot Test - 모델 테스트";
            // 
            // lblTitlePilot
            // 
            lblTitlePilot.AutoSize = true;
            lblTitlePilot.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitlePilot.ForeColor = Color.Violet;
            lblTitlePilot.Location = new Point(16, 14);
            lblTitlePilot.Margin = new Padding(2, 0, 2, 0);
            lblTitlePilot.Name = "lblTitlePilot";
            lblTitlePilot.Size = new Size(373, 41);
            lblTitlePilot.TabIndex = 0;
            lblTitlePilot.Text = "Pilot Arena / Model Test";
            // 
            // lblModelPath
            // 
            lblModelPath.AutoSize = true;
            lblModelPath.ForeColor = Color.Gainsboro;
            lblModelPath.Location = new Point(23, 75);
            lblModelPath.Margin = new Padding(2, 0, 2, 0);
            lblModelPath.Name = "lblModelPath";
            lblModelPath.Size = new Size(70, 19);
            lblModelPath.TabIndex = 1;
            lblModelPath.Text = "모델 파일";
            // 
            // txtModelPath
            // 
            txtModelPath.BackColor = Color.FromArgb(45, 45, 48);
            txtModelPath.ForeColor = Color.White;
            txtModelPath.Location = new Point(93, 72);
            txtModelPath.Margin = new Padding(2);
            txtModelPath.Name = "txtModelPath";
            txtModelPath.Size = new Size(499, 25);
            txtModelPath.TabIndex = 2;
            // 
            // btnBrowseModel
            // 
            btnBrowseModel.BackColor = Color.FromArgb(64, 64, 64);
            btnBrowseModel.FlatStyle = FlatStyle.Flat;
            btnBrowseModel.ForeColor = Color.White;
            btnBrowseModel.Location = new Point(607, 71);
            btnBrowseModel.Margin = new Padding(2);
            btnBrowseModel.Name = "btnBrowseModel";
            btnBrowseModel.Size = new Size(81, 26);
            btnBrowseModel.TabIndex = 3;
            btnBrowseModel.Text = "🔍 찾기";
            btnBrowseModel.UseVisualStyleBackColor = false;
            // 
            // btnRunPilotTest
            // 
            btnRunPilotTest.BackColor = Color.RoyalBlue;
            btnRunPilotTest.FlatStyle = FlatStyle.Flat;
            btnRunPilotTest.ForeColor = Color.White;
            btnRunPilotTest.Location = new Point(93, 109);
            btnRunPilotTest.Margin = new Padding(2);
            btnRunPilotTest.Name = "btnRunPilotTest";
            btnRunPilotTest.Size = new Size(194, 32);
            btnRunPilotTest.TabIndex = 4;
            btnRunPilotTest.Text = "🎯 현재 이미지 예측";
            btnRunPilotTest.UseVisualStyleBackColor = false;
            // 
            // btnUseViewerFrame
            // 
            btnUseViewerFrame.BackColor = Color.FromArgb(64, 64, 64);
            btnUseViewerFrame.FlatStyle = FlatStyle.Flat;
            btnUseViewerFrame.ForeColor = Color.White;
            btnUseViewerFrame.Location = new Point(303, 109);
            btnUseViewerFrame.Margin = new Padding(2);
            btnUseViewerFrame.Name = "btnUseViewerFrame";
            btnUseViewerFrame.Size = new Size(177, 32);
            btnUseViewerFrame.TabIndex = 5;
            btnUseViewerFrame.Text = "📋 Viewer 이미지 사용";
            btnUseViewerFrame.UseVisualStyleBackColor = false;
            // 
            // btnPilotAutoPlay
            // 
            btnPilotAutoPlay.BackColor = Color.FromArgb(76, 175, 80);
            btnPilotAutoPlay.FlatStyle = FlatStyle.Flat;
            btnPilotAutoPlay.ForeColor = Color.White;
            btnPilotAutoPlay.Location = new Point(495, 109);
            btnPilotAutoPlay.Margin = new Padding(2);
            btnPilotAutoPlay.Name = "btnPilotAutoPlay";
            btnPilotAutoPlay.Size = new Size(112, 32);
            btnPilotAutoPlay.TabIndex = 6;
            btnPilotAutoPlay.Text = "▶️ 자동 재생";
            btnPilotAutoPlay.UseVisualStyleBackColor = false;
            // 
            // btnPilotStop
            // 
            btnPilotStop.BackColor = Color.Firebrick;
            btnPilotStop.FlatStyle = FlatStyle.Flat;
            btnPilotStop.ForeColor = Color.White;
            btnPilotStop.Location = new Point(618, 109);
            btnPilotStop.Margin = new Padding(2);
            btnPilotStop.Name = "btnPilotStop";
            btnPilotStop.Size = new Size(78, 32);
            btnPilotStop.TabIndex = 7;
            btnPilotStop.Text = "\U0001f6d1 멈춤";
            btnPilotStop.UseVisualStyleBackColor = false;
            // 
            // picPilotTest
            // 
            picPilotTest.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            picPilotTest.BackColor = Color.Black;
            picPilotTest.BorderStyle = BorderStyle.FixedSingle;
            picPilotTest.Location = new Point(23, 165);
            picPilotTest.Margin = new Padding(2);
            picPilotTest.Name = "picPilotTest";
            picPilotTest.Size = new Size(428, 375);
            picPilotTest.SizeMode = PictureBoxSizeMode.Zoom;
            picPilotTest.TabIndex = 8;
            picPilotTest.TabStop = false;
            // 
            // lblActualAngle
            // 
            lblActualAngle.AutoSize = true;
            lblActualAngle.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblActualAngle.ForeColor = Color.White;
            lblActualAngle.Location = new Point(474, 172);
            lblActualAngle.Margin = new Padding(2, 0, 2, 0);
            lblActualAngle.Name = "lblActualAngle";
            lblActualAngle.Size = new Size(109, 21);
            lblActualAngle.TabIndex = 9;
            lblActualAngle.Text = "실제 Angle: -";
            // 
            // lblPredictedAngle
            // 
            lblPredictedAngle.AutoSize = true;
            lblPredictedAngle.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblPredictedAngle.ForeColor = Color.White;
            lblPredictedAngle.Location = new Point(474, 202);
            lblPredictedAngle.Margin = new Padding(2, 0, 2, 0);
            lblPredictedAngle.Name = "lblPredictedAngle";
            lblPredictedAngle.Size = new Size(109, 21);
            lblPredictedAngle.TabIndex = 10;
            lblPredictedAngle.Text = "예측 Angle: -";
            // 
            // lblActualThrottle
            // 
            lblActualThrottle.AutoSize = true;
            lblActualThrottle.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblActualThrottle.ForeColor = Color.White;
            lblActualThrottle.Location = new Point(474, 240);
            lblActualThrottle.Margin = new Padding(2, 0, 2, 0);
            lblActualThrottle.Name = "lblActualThrottle";
            lblActualThrottle.Size = new Size(116, 20);
            lblActualThrottle.TabIndex = 11;
            lblActualThrottle.Text = "실제 Throttle: -";
            // 
            // lblPredictedThrottle
            // 
            lblPredictedThrottle.AutoSize = true;
            lblPredictedThrottle.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblPredictedThrottle.ForeColor = Color.White;
            lblPredictedThrottle.Location = new Point(474, 266);
            lblPredictedThrottle.Margin = new Padding(2, 0, 2, 0);
            lblPredictedThrottle.Name = "lblPredictedThrottle";
            lblPredictedThrottle.Size = new Size(116, 20);
            lblPredictedThrottle.TabIndex = 12;
            lblPredictedThrottle.Text = "예측 Throttle: -";
            // 
            // lblAngleError
            // 
            lblAngleError.AutoSize = true;
            lblAngleError.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblAngleError.ForeColor = Color.White;
            lblAngleError.Location = new Point(474, 304);
            lblAngleError.Margin = new Padding(2, 0, 2, 0);
            lblAngleError.Name = "lblAngleError";
            lblAngleError.Size = new Size(114, 21);
            lblAngleError.TabIndex = 13;
            lblAngleError.Text = "Angle Error: -";
            // 
            // lblPilotWarning
            // 
            lblPilotWarning.AutoSize = true;
            lblPilotWarning.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblPilotWarning.ForeColor = Color.Gainsboro;
            lblPilotWarning.Location = new Point(474, 338);
            lblPilotWarning.Margin = new Padding(2, 0, 2, 0);
            lblPilotWarning.Name = "lblPilotWarning";
            lblPilotWarning.Size = new Size(59, 21);
            lblPilotWarning.TabIndex = 14;
            lblPilotWarning.Text = "판정: -";
            // 
            // lblPilotNote
            // 
            lblPilotNote.ForeColor = Color.Gainsboro;
            lblPilotNote.Location = new Point(474, 375);
            lblPilotNote.Margin = new Padding(2, 0, 2, 0);
            lblPilotNote.Name = "lblPilotNote";
            lblPilotNote.Size = new Size(187, 112);
            lblPilotNote.TabIndex = 15;
            lblPilotNote.Text = "파란선: 실제 angle\n초록선: 예측 angle\n노란 반투명 영역:\n실제/예측 차이\n하단 막대:\n실제/예측 throttle 비교";
            // 
            // lblPilotImageList
            // 
            lblPilotImageList.AutoSize = true;
            lblPilotImageList.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblPilotImageList.ForeColor = Color.Gainsboro;
            lblPilotImageList.Location = new Point(684, 150);
            lblPilotImageList.Margin = new Padding(2, 0, 2, 0);
            lblPilotImageList.Name = "lblPilotImageList";
            lblPilotImageList.Size = new Size(139, 20);
            lblPilotImageList.TabIndex = 16;
            lblPilotImageList.Text = "테스트 이미지 선택";
            // 
            // lstPilotFrames
            // 
            lstPilotFrames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstPilotFrames.BackColor = Color.FromArgb(45, 45, 48);
            lstPilotFrames.BorderStyle = BorderStyle.None;
            lstPilotFrames.Font = new Font("Consolas", 10F);
            lstPilotFrames.ForeColor = Color.Gainsboro;
            lstPilotFrames.HorizontalScrollbar = true;
            lstPilotFrames.Location = new Point(684, 172);
            lstPilotFrames.Margin = new Padding(2);
            lstPilotFrames.Name = "lstPilotFrames";
            lstPilotFrames.Size = new Size(374, 330);
            lstPilotFrames.TabIndex = 17;
            // 
            // TbtnPilot
            // 
            TbtnPilot.BackColor = Color.FromArgb(45, 45, 48);
            TbtnPilot.FlatAppearance.BorderSize = 0;
            TbtnPilot.FlatStyle = FlatStyle.Flat;
            TbtnPilot.ForeColor = Color.White;
            TbtnPilot.Location = new Point(472, 4);
            TbtnPilot.Name = "TbtnPilot";
            TbtnPilot.Size = new Size(150, 40);
            TbtnPilot.TabIndex = 14;
            TbtnPilot.Text = "🏎️ 파일럿 - 모델 테스트";
            TbtnPilot.UseVisualStyleBackColor = false;
            TbtnPilot.Click += TbtnPilot_Click;
            // 
            // TbtnTrain
            // 
            TbtnTrain.BackColor = Color.FromArgb(45, 45, 48);
            TbtnTrain.FlatAppearance.BorderSize = 0;
            TbtnTrain.FlatStyle = FlatStyle.Flat;
            TbtnTrain.ForeColor = Color.White;
            TbtnTrain.Location = new Point(316, 4);
            TbtnTrain.Name = "TbtnTrain";
            TbtnTrain.Size = new Size(150, 40);
            TbtnTrain.TabIndex = 13;
            TbtnTrain.Text = "🏋️ 트레이너 - 학습 실행";
            TbtnTrain.UseVisualStyleBackColor = false;
            TbtnTrain.Click += TbtnTrain_Click;
            // 
            // TbtnClean
            // 
            TbtnClean.BackColor = Color.FromArgb(45, 45, 48);
            TbtnClean.FlatAppearance.BorderSize = 0;
            TbtnClean.FlatStyle = FlatStyle.Flat;
            TbtnClean.ForeColor = Color.White;
            TbtnClean.Location = new Point(160, 4);
            TbtnClean.Name = "TbtnClean";
            TbtnClean.Size = new Size(150, 40);
            TbtnClean.TabIndex = 12;
            TbtnClean.Text = "\U0001f9f9 클리너 - 데이터 정리";
            TbtnClean.UseVisualStyleBackColor = false;
            TbtnClean.Click += TbtnClean_Click;
            // 
            // TbtnView
            // 
            TbtnView.BackColor = Color.FromArgb(45, 45, 48);
            TbtnView.FlatAppearance.BorderSize = 0;
            TbtnView.FlatStyle = FlatStyle.Flat;
            TbtnView.ForeColor = Color.White;
            TbtnView.Location = new Point(4, 4);
            TbtnView.Name = "TbtnView";
            TbtnView.Size = new Size(150, 40);
            TbtnView.TabIndex = 2;
            TbtnView.Text = "👁️ 뷰어 - 데이터 확인";
            TbtnView.UseVisualStyleBackColor = false;
            TbtnView.Click += TbtnView_Click;
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.BackColor = Color.FromArgb(30, 30, 30);
            txtLog.Font = new Font("Consolas", 9F);
            txtLog.Location = new Point(0, 609);
            txtLog.Margin = new Padding(2);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(1090, 67);
            txtLog.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(1089, 675);
            Controls.Add(TbtnView);
            Controls.Add(TbtnClean);
            Controls.Add(TbtnTrain);
            Controls.Add(TbtnPilot);
            Controls.Add(tabMain);
            Controls.Add(txtLog);
            Margin = new Padding(2);
            MinimumSize = new Size(999, 625);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Donkeycar Manager";
            WindowState = FormWindowState.Maximized;
            tabMain.ResumeLayout(false);
            tabViewer.ResumeLayout(false);
            tabViewer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picFrame).EndInit();
            ((System.ComponentModel.ISupportInitialize)trbFrame).EndInit();
            tabCleaner.ResumeLayout(false);
            tabCleaner.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picCleanerPreview).EndInit();
            grpFilters.ResumeLayout(false);
            grpFilters.PerformLayout();
            grpCleanerRangeEditor.ResumeLayout(false);
            grpCleanerRangeEditor.PerformLayout();
            tabTrainer.ResumeLayout(false);
            tabTrainer.PerformLayout();
            tabPilotTest.ResumeLayout(false);
            tabPilotTest.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picPilotTest).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Button TbtnView;
        private Button TbtnPilot;
        private Button TbtnTrain;
        private Button TbtnClean;
    }
}
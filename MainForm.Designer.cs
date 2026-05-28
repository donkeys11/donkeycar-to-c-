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
            cmbModelList = new ComboBox();
            btnScanModels = new Button();
            lblModelList = new Label();
            trbBrightness = new TrackBar();
            trbContrast = new TrackBar();
            lblBrightness = new Label();
            lblContrast = new Label();
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
            lblImageAdjust = new Label();
            chkFlipHorizontal = new CheckBox();
            chkGrayscale = new CheckBox();
            btnSaveProcessed = new Button();
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
            txtLog = new TextBox();
            ((System.ComponentModel.ISupportInitialize)trbBrightness).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trbContrast).BeginInit();
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
            // cmbModelList
            // 
            cmbModelList.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbModelList.Location = new Point(94, 106);
            cmbModelList.Margin = new Padding(2);
            cmbModelList.Name = "cmbModelList";
            cmbModelList.Size = new Size(421, 25);
            cmbModelList.TabIndex = 19;
            // 
            // btnScanModels
            // 
            btnScanModels.Location = new Point(525, 105);
            btnScanModels.Margin = new Padding(2);
            btnScanModels.Name = "btnScanModels";
            btnScanModels.Size = new Size(78, 25);
            btnScanModels.TabIndex = 20;
            btnScanModels.Text = "모델 스캔";
            btnScanModels.UseVisualStyleBackColor = true;
            // 
            // lblModelList
            // 
            lblModelList.AutoSize = true;
            lblModelList.Location = new Point(24, 109);
            lblModelList.Margin = new Padding(2, 0, 2, 0);
            lblModelList.Name = "lblModelList";
            lblModelList.Size = new Size(70, 19);
            lblModelList.TabIndex = 18;
            lblModelList.Text = "모델 목록";
            // 
            // trbBrightness
            // 
            trbBrightness.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trbBrightness.AutoSize = false;
            trbBrightness.LargeChange = 10;
            trbBrightness.Location = new Point(264, 292);
            trbBrightness.Margin = new Padding(2);
            trbBrightness.Maximum = 100;
            trbBrightness.Minimum = -100;
            trbBrightness.Name = "trbBrightness";
            trbBrightness.Size = new Size(124, 25);
            trbBrightness.SmallChange = 5;
            trbBrightness.TabIndex = 5;
            trbBrightness.TickStyle = TickStyle.None;
            // 
            // trbContrast
            // 
            trbContrast.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trbContrast.AutoSize = false;
            trbContrast.LargeChange = 10;
            trbContrast.Location = new Point(264, 328);
            trbContrast.Margin = new Padding(2);
            trbContrast.Maximum = 100;
            trbContrast.Minimum = -100;
            trbContrast.Name = "trbContrast";
            trbContrast.Size = new Size(124, 27);
            trbContrast.SmallChange = 5;
            trbContrast.TabIndex = 7;
            trbContrast.TickStyle = TickStyle.None;
            // 
            // lblBrightness
            // 
            lblBrightness.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblBrightness.AutoSize = true;
            lblBrightness.Location = new Point(216, 292);
            lblBrightness.Margin = new Padding(2, 0, 2, 0);
            lblBrightness.Name = "lblBrightness";
            lblBrightness.Size = new Size(53, 19);
            lblBrightness.TabIndex = 4;
            lblBrightness.Text = "밝기: 0";
            // 
            // lblContrast
            // 
            lblContrast.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblContrast.AutoSize = true;
            lblContrast.Location = new Point(216, 328);
            lblContrast.Margin = new Padding(2, 0, 2, 0);
            lblContrast.Name = "lblContrast";
            lblContrast.Size = new Size(53, 19);
            lblContrast.TabIndex = 6;
            lblContrast.Text = "명암: 0";
            // 
            // tabMain
            // 
            tabMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabMain.Controls.Add(tabViewer);
            tabMain.Controls.Add(tabCleaner);
            tabMain.Controls.Add(tabTrainer);
            tabMain.Controls.Add(tabPilotTest);
            tabMain.Font = new Font("맑은 고딕", 10F);
            tabMain.Location = new Point(0, 0);
            tabMain.Margin = new Padding(2);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(1089, 540);
            tabMain.TabIndex = 0;
            // 
            // tabViewer
            // 
            tabViewer.BackColor = Color.WhiteSmoke;
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
            tabViewer.Size = new Size(1081, 510);
            tabViewer.TabIndex = 0;
            tabViewer.Text = "Viewer - 데이터 확인";
            // 
            // lblTitleViewer
            // 
            lblTitleViewer.AutoSize = true;
            lblTitleViewer.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitleViewer.ForeColor = Color.FromArgb(30, 90, 160);
            lblTitleViewer.Location = new Point(16, 14);
            lblTitleViewer.Margin = new Padding(2, 0, 2, 0);
            lblTitleViewer.Name = "lblTitleViewer";
            lblTitleViewer.Size = new Size(341, 41);
            lblTitleViewer.TabIndex = 0;
            lblTitleViewer.Text = "Donkeycar Tub Viewer";
            // 
            // btnOpenDataFolder
            // 
            btnOpenDataFolder.Location = new Point(16, 61);
            btnOpenDataFolder.Margin = new Padding(2);
            btnOpenDataFolder.Name = "btnOpenDataFolder";
            btnOpenDataFolder.Size = new Size(124, 29);
            btnOpenDataFolder.TabIndex = 1;
            btnOpenDataFolder.Text = "데이터 폴더 열기";
            btnOpenDataFolder.UseVisualStyleBackColor = true;
            // 
            // btnReload
            // 
            btnReload.Location = new Point(148, 61);
            btnReload.Margin = new Padding(2);
            btnReload.Name = "btnReload";
            btnReload.Size = new Size(86, 29);
            btnReload.TabIndex = 2;
            btnReload.Text = "새로고침";
            btnReload.UseVisualStyleBackColor = true;
            // 
            // btnAutoPlay
            // 
            btnAutoPlay.Location = new Point(241, 61);
            btnAutoPlay.Margin = new Padding(2);
            btnAutoPlay.Name = "btnAutoPlay";
            btnAutoPlay.Size = new Size(94, 29);
            btnAutoPlay.TabIndex = 3;
            btnAutoPlay.Text = "자동 재생";
            btnAutoPlay.UseVisualStyleBackColor = true;
            // 
            // lblDataPath
            // 
            lblDataPath.AutoSize = true;
            lblDataPath.ForeColor = Color.DimGray;
            lblDataPath.Location = new Point(350, 68);
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
            picFrame.Size = new Size(646, 293);
            picFrame.SizeMode = PictureBoxSizeMode.Zoom;
            picFrame.TabIndex = 5;
            picFrame.TabStop = false;
            // 
            // lstFrames
            // 
            lstFrames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lstFrames.Font = new Font("Consolas", 9F);
            lstFrames.HorizontalScrollbar = true;
            lstFrames.Location = new Point(676, 105);
            lstFrames.Margin = new Padding(2);
            lstFrames.Name = "lstFrames";
            lstFrames.Size = new Size(390, 284);
            lstFrames.TabIndex = 6;
            // 
            // lblFrameInfo
            // 
            lblFrameInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblFrameInfo.AutoSize = true;
            lblFrameInfo.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblFrameInfo.Location = new Point(16, 411);
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
            lblAngle.Location = new Point(156, 411);
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
            lblThrottle.Location = new Point(319, 411);
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
            lblMode.Location = new Point(506, 411);
            lblMode.Margin = new Padding(2, 0, 2, 0);
            lblMode.Name = "lblMode";
            lblMode.Size = new Size(63, 20);
            lblMode.TabIndex = 10;
            lblMode.Text = "Mode: -";
            // 
            // trbFrame
            // 
            trbFrame.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trbFrame.Location = new Point(16, 439);
            trbFrame.Margin = new Padding(2);
            trbFrame.Maximum = 0;
            trbFrame.Name = "trbFrame";
            trbFrame.Size = new Size(1050, 45);
            trbFrame.TabIndex = 11;
            trbFrame.TickStyle = TickStyle.None;
            // 
            // tabCleaner
            // 
            tabCleaner.BackColor = Color.WhiteSmoke;
            tabCleaner.Controls.Add(lblTitleCleaner);
            tabCleaner.Controls.Add(picCleanerPreview);
            tabCleaner.Controls.Add(lblCleanerInfo);
            tabCleaner.Controls.Add(lblImageAdjust);
            tabCleaner.Controls.Add(lblBrightness);
            tabCleaner.Controls.Add(trbBrightness);
            tabCleaner.Controls.Add(lblContrast);
            tabCleaner.Controls.Add(trbContrast);
            tabCleaner.Controls.Add(chkFlipHorizontal);
            tabCleaner.Controls.Add(chkGrayscale);
            tabCleaner.Controls.Add(btnSaveProcessed);
            tabCleaner.Controls.Add(grpFilters);
            tabCleaner.Controls.Add(btnApplyFilter);
            tabCleaner.Controls.Add(btnClearFilter);
            tabCleaner.Controls.Add(btnDeleteFrame);
            tabCleaner.Controls.Add(lstCleanerFrames);
            tabCleaner.Controls.Add(grpCleanerRangeEditor);
            tabCleaner.Location = new Point(4, 26);
            tabCleaner.Margin = new Padding(2);
            tabCleaner.Name = "tabCleaner";
            tabCleaner.Padding = new Padding(2);
            tabCleaner.Size = new Size(1081, 510);
            tabCleaner.TabIndex = 1;
            tabCleaner.Text = "Cleaner - 데이터 정리";
            // 
            // lblTitleCleaner
            // 
            lblTitleCleaner.AutoSize = true;
            lblTitleCleaner.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitleCleaner.ForeColor = Color.FromArgb(180, 70, 70);
            lblTitleCleaner.Location = new Point(16, 11);
            lblTitleCleaner.Margin = new Padding(2, 0, 2, 0);
            lblTitleCleaner.Name = "lblTitleCleaner";
            lblTitleCleaner.Size = new Size(188, 41);
            lblTitleCleaner.TabIndex = 0;
            lblTitleCleaner.Text = "Tub Cleaner";
            // 
            // picCleanerPreview
            // 
            picCleanerPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picCleanerPreview.BackColor = Color.Black;
            picCleanerPreview.BorderStyle = BorderStyle.FixedSingle;
            picCleanerPreview.Location = new Point(16, 68);
            picCleanerPreview.Margin = new Padding(2);
            picCleanerPreview.Name = "picCleanerPreview";
            picCleanerPreview.Size = new Size(662, 192);
            picCleanerPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picCleanerPreview.TabIndex = 1;
            picCleanerPreview.TabStop = false;
            // 
            // lblCleanerInfo
            // 
            lblCleanerInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblCleanerInfo.AutoSize = true;
            lblCleanerInfo.Location = new Point(16, 48);
            lblCleanerInfo.Margin = new Padding(2, 0, 2, 0);
            lblCleanerInfo.Name = "lblCleanerInfo";
            lblCleanerInfo.Size = new Size(131, 19);
            lblCleanerInfo.TabIndex = 2;
            lblCleanerInfo.Text = "선택 프레임 정보: -";
            // 
            // lblImageAdjust
            // 
            lblImageAdjust.AutoSize = true;
            lblImageAdjust.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
            lblImageAdjust.ForeColor = Color.DimGray;
            lblImageAdjust.Location = new Point(16, 273);
            lblImageAdjust.Margin = new Padding(2, 0, 2, 0);
            lblImageAdjust.Name = "lblImageAdjust";
            lblImageAdjust.Size = new Size(71, 15);
            lblImageAdjust.TabIndex = 3;
            lblImageAdjust.Text = "이미지 조작";
            // 
            // chkFlipHorizontal
            // 
            chkFlipHorizontal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chkFlipHorizontal.AutoSize = true;
            chkFlipHorizontal.Location = new Point(16, 316);
            chkFlipHorizontal.Margin = new Padding(2);
            chkFlipHorizontal.Name = "chkFlipHorizontal";
            chkFlipHorizontal.Size = new Size(201, 23);
            chkFlipHorizontal.TabIndex = 8;
            chkFlipHorizontal.Text = "좌우 반전 (angle 자동 반전)";
            chkFlipHorizontal.UseVisualStyleBackColor = true;
            // 
            // chkGrayscale
            // 
            chkGrayscale.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chkGrayscale.AutoSize = true;
            chkGrayscale.Location = new Point(16, 292);
            chkGrayscale.Margin = new Padding(2);
            chkGrayscale.Name = "chkGrayscale";
            chkGrayscale.Size = new Size(112, 23);
            chkGrayscale.TabIndex = 9;
            chkGrayscale.Text = "그레이스케일";
            chkGrayscale.UseVisualStyleBackColor = true;
            // 
            // btnSaveProcessed
            // 
            btnSaveProcessed.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSaveProcessed.BackColor = Color.FromArgb(70, 110, 160);
            btnSaveProcessed.FlatStyle = FlatStyle.Flat;
            btnSaveProcessed.ForeColor = Color.White;
            btnSaveProcessed.Location = new Point(448, 314);
            btnSaveProcessed.Margin = new Padding(2);
            btnSaveProcessed.Name = "btnSaveProcessed";
            btnSaveProcessed.Size = new Size(124, 31);
            btnSaveProcessed.TabIndex = 10;
            btnSaveProcessed.Text = "조작 데이터 저장";
            btnSaveProcessed.UseVisualStyleBackColor = false;
            // 
            // grpFilters
            // 
            grpFilters.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            grpFilters.Controls.Add(chkThrottlePositive);
            grpFilters.Controls.Add(chkExcludeZeroAngle);
            grpFilters.Controls.Add(chkStopDataOnly);
            grpFilters.Location = new Point(700, 56);
            grpFilters.Margin = new Padding(2);
            grpFilters.Name = "grpFilters";
            grpFilters.Padding = new Padding(2);
            grpFilters.Size = new Size(241, 116);
            grpFilters.TabIndex = 11;
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
            btnApplyFilter.Location = new Point(956, 60);
            btnApplyFilter.Margin = new Padding(2);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(101, 30);
            btnApplyFilter.TabIndex = 12;
            btnApplyFilter.Text = "필터 적용";
            btnApplyFilter.UseVisualStyleBackColor = true;
            // 
            // btnClearFilter
            // 
            btnClearFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearFilter.Location = new Point(956, 101);
            btnClearFilter.Margin = new Padding(2);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(101, 30);
            btnClearFilter.TabIndex = 13;
            btnClearFilter.Text = "전체 보기";
            btnClearFilter.UseVisualStyleBackColor = true;
            // 
            // btnDeleteFrame
            // 
            btnDeleteFrame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteFrame.BackColor = Color.LightCoral;
            btnDeleteFrame.FlatStyle = FlatStyle.Flat;
            btnDeleteFrame.Location = new Point(956, 142);
            btnDeleteFrame.Margin = new Padding(2);
            btnDeleteFrame.Name = "btnDeleteFrame";
            btnDeleteFrame.Size = new Size(101, 38);
            btnDeleteFrame.TabIndex = 14;
            btnDeleteFrame.Text = "선택 프레임 삭제";
            btnDeleteFrame.UseVisualStyleBackColor = false;
            // 
            // lstCleanerFrames
            // 
            lstCleanerFrames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lstCleanerFrames.Font = new Font("Consolas", 9F);
            lstCleanerFrames.HorizontalScrollbar = true;
            lstCleanerFrames.Location = new Point(700, 191);
            lstCleanerFrames.Margin = new Padding(2);
            lstCleanerFrames.Name = "lstCleanerFrames";
            lstCleanerFrames.SelectionMode = SelectionMode.MultiExtended;
            lstCleanerFrames.Size = new Size(358, 130);
            lstCleanerFrames.TabIndex = 15;
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
            grpCleanerRangeEditor.Location = new Point(16, 350);
            grpCleanerRangeEditor.Margin = new Padding(2);
            grpCleanerRangeEditor.Name = "grpCleanerRangeEditor";
            grpCleanerRangeEditor.Padding = new Padding(2);
            grpCleanerRangeEditor.Size = new Size(1042, 150);
            grpCleanerRangeEditor.TabIndex = 16;
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
            lblCleanerRangeHint.ForeColor = Color.DimGray;
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
            pnlCleanerTimeline.Location = new Point(10, 54);
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
            lblCleanerTimelineScrollInfo.ForeColor = Color.DimGray;
            lblCleanerTimelineScrollInfo.Location = new Point(12, 126);
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
            btnDeleteRange.Size = new Size(82, 25);
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
            btnPlayRange.Size = new Size(82, 25);
            btnPlayRange.TabIndex = 6;
            btnPlayRange.Text = "구간 재생";
            btnPlayRange.UseVisualStyleBackColor = false;
            // 
            // btnClearRange
            // 
            btnClearRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearRange.Location = new Point(852, 76);
            btnClearRange.Margin = new Padding(2);
            btnClearRange.Name = "btnClearRange";
            btnClearRange.Size = new Size(82, 25);
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
            btnCleanerAutoPlay.Size = new Size(82, 25);
            btnCleanerAutoPlay.TabIndex = 8;
            btnCleanerAutoPlay.Text = "자동 재생";
            btnCleanerAutoPlay.UseVisualStyleBackColor = false;
            // 
            // btnCleanerStop
            // 
            btnCleanerStop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCleanerStop.Location = new Point(852, 110);
            btnCleanerStop.Margin = new Padding(2);
            btnCleanerStop.Name = "btnCleanerStop";
            btnCleanerStop.Size = new Size(171, 25);
            btnCleanerStop.TabIndex = 9;
            btnCleanerStop.Text = "멈춤";
            btnCleanerStop.UseVisualStyleBackColor = true;
            // 
            // tabTrainer
            // 
            tabTrainer.BackColor = Color.WhiteSmoke;
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
            tabTrainer.Location = new Point(4, 26);
            tabTrainer.Margin = new Padding(2);
            tabTrainer.Name = "tabTrainer";
            tabTrainer.Padding = new Padding(2);
            tabTrainer.Size = new Size(1081, 510);
            tabTrainer.TabIndex = 2;
            tabTrainer.Text = "Trainer - 학습 실행";
            // 
            // lblTitleTrainer
            // 
            lblTitleTrainer.AutoSize = true;
            lblTitleTrainer.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitleTrainer.ForeColor = Color.FromArgb(60, 130, 80);
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
            lblMycarPath.Location = new Point(24, 75);
            lblMycarPath.Margin = new Padding(2, 0, 2, 0);
            lblMycarPath.Name = "lblMycarPath";
            lblMycarPath.Size = new Size(80, 19);
            lblMycarPath.TabIndex = 1;
            lblMycarPath.Text = "mycar 경로";
            // 
            // txtMycarPath
            // 
            txtMycarPath.Location = new Point(116, 72);
            txtMycarPath.Margin = new Padding(2);
            txtMycarPath.Name = "txtMycarPath";
            txtMycarPath.Size = new Size(561, 25);
            txtMycarPath.TabIndex = 2;
            // 
            // btnBrowseMycar
            // 
            btnBrowseMycar.Location = new Point(692, 71);
            btnBrowseMycar.Margin = new Padding(2);
            btnBrowseMycar.Name = "btnBrowseMycar";
            btnBrowseMycar.Size = new Size(70, 25);
            btnBrowseMycar.TabIndex = 3;
            btnBrowseMycar.Text = "찾기";
            btnBrowseMycar.UseVisualStyleBackColor = true;
            // 
            // lblPythonExe
            // 
            lblPythonExe.AutoSize = true;
            lblPythonExe.Location = new Point(24, 116);
            lblPythonExe.Margin = new Padding(2, 0, 2, 0);
            lblPythonExe.Name = "lblPythonExe";
            lblPythonExe.Size = new Size(100, 19);
            lblPythonExe.TabIndex = 4;
            lblPythonExe.Text = "Python 실행명";
            // 
            // txtPythonExe
            // 
            txtPythonExe.Location = new Point(116, 113);
            txtPythonExe.Margin = new Padding(2);
            txtPythonExe.Name = "txtPythonExe";
            txtPythonExe.Size = new Size(234, 25);
            txtPythonExe.TabIndex = 5;
            // 
            // lblTrainArgs
            // 
            lblTrainArgs.AutoSize = true;
            lblTrainArgs.Location = new Point(24, 158);
            lblTrainArgs.Margin = new Padding(2, 0, 2, 0);
            lblTrainArgs.Name = "lblTrainArgs";
            lblTrainArgs.Size = new Size(103, 19);
            lblTrainArgs.TabIndex = 6;
            lblTrainArgs.Text = "학습 명령 인자";
            // 
            // txtTrainArgs
            // 
            txtTrainArgs.Location = new Point(116, 155);
            txtTrainArgs.Margin = new Padding(2);
            txtTrainArgs.Name = "txtTrainArgs";
            txtTrainArgs.Size = new Size(646, 25);
            txtTrainArgs.TabIndex = 7;
            // 
            // btnTrain
            // 
            btnTrain.BackColor = Color.FromArgb(76, 175, 80);
            btnTrain.FlatStyle = FlatStyle.Flat;
            btnTrain.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            btnTrain.ForeColor = Color.White;
            btnTrain.Location = new Point(116, 202);
            btnTrain.Margin = new Padding(2);
            btnTrain.Name = "btnTrain";
            btnTrain.Size = new Size(124, 38);
            btnTrain.TabIndex = 8;
            btnTrain.Text = "학습 시작";
            btnTrain.UseVisualStyleBackColor = false;
            // 
            // btnStopTrain
            // 
            btnStopTrain.BackColor = Color.LightCoral;
            btnStopTrain.FlatStyle = FlatStyle.Flat;
            btnStopTrain.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            btnStopTrain.Location = new Point(256, 202);
            btnStopTrain.Margin = new Padding(2);
            btnStopTrain.Name = "btnStopTrain";
            btnStopTrain.Size = new Size(124, 38);
            btnStopTrain.TabIndex = 9;
            btnStopTrain.Text = "학습 중지";
            btnStopTrain.UseVisualStyleBackColor = false;
            // 
            // lblModelStatus
            // 
            lblModelStatus.AutoSize = true;
            lblModelStatus.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblModelStatus.Location = new Point(116, 259);
            lblModelStatus.Margin = new Padding(2, 0, 2, 0);
            lblModelStatus.Name = "lblModelStatus";
            lblModelStatus.Size = new Size(89, 20);
            lblModelStatus.TabIndex = 10;
            lblModelStatus.Text = "모델 상태: -";
            // 
            // lblTrainInfo
            // 
            lblTrainInfo.AutoSize = true;
            lblTrainInfo.ForeColor = Color.DimGray;
            lblTrainInfo.Location = new Point(116, 296);
            lblTrainInfo.Margin = new Padding(2, 0, 2, 0);
            lblTrainInfo.Name = "lblTrainInfo";
            lblTrainInfo.Size = new Size(434, 76);
            lblTrainInfo.TabIndex = 11;
            lblTrainInfo.Text = "자료 기준 학습 명령 예시:\npython train.py --tub ./data --model ./models/mypilot.h5\n\nC#은 AI를 직접 학습하지 않고 Python 외부 프로세스를 실행합니다.";
            // 
            // tabPilotTest
            // 
            tabPilotTest.BackColor = Color.WhiteSmoke;
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
            tabPilotTest.Controls.Add(lblModelList);
            tabPilotTest.Controls.Add(cmbModelList);
            tabPilotTest.Controls.Add(btnScanModels);
            tabPilotTest.Location = new Point(4, 26);
            tabPilotTest.Margin = new Padding(2);
            tabPilotTest.Name = "tabPilotTest";
            tabPilotTest.Padding = new Padding(2);
            tabPilotTest.Size = new Size(1081, 510);
            tabPilotTest.TabIndex = 3;
            tabPilotTest.Text = "Pilot Test - 모델 테스트";
            // 
            // lblTitlePilot
            // 
            lblTitlePilot.AutoSize = true;
            lblTitlePilot.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitlePilot.ForeColor = Color.FromArgb(90, 90, 160);
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
            lblModelPath.Location = new Point(24, 75);
            lblModelPath.Margin = new Padding(2, 0, 2, 0);
            lblModelPath.Name = "lblModelPath";
            lblModelPath.Size = new Size(70, 19);
            lblModelPath.TabIndex = 1;
            lblModelPath.Text = "모델 파일";
            // 
            // txtModelPath
            // 
            txtModelPath.Location = new Point(94, 72);
            txtModelPath.Margin = new Padding(2);
            txtModelPath.Name = "txtModelPath";
            txtModelPath.Size = new Size(498, 25);
            txtModelPath.TabIndex = 2;
            // 
            // btnBrowseModel
            // 
            btnBrowseModel.Location = new Point(606, 71);
            btnBrowseModel.Margin = new Padding(2);
            btnBrowseModel.Name = "btnBrowseModel";
            btnBrowseModel.Size = new Size(70, 25);
            btnBrowseModel.TabIndex = 3;
            btnBrowseModel.Text = "찾기";
            btnBrowseModel.UseVisualStyleBackColor = true;
            // 
            // btnRunPilotTest
            // 
            btnRunPilotTest.Location = new Point(94, 142);
            btnRunPilotTest.Margin = new Padding(2);
            btnRunPilotTest.Name = "btnRunPilotTest";
            btnRunPilotTest.Size = new Size(194, 31);
            btnRunPilotTest.TabIndex = 4;
            btnRunPilotTest.Text = "현재 이미지로 예측 테스트";
            btnRunPilotTest.UseVisualStyleBackColor = true;
            // 
            // btnUseViewerFrame
            // 
            btnUseViewerFrame.Location = new Point(296, 142);
            btnUseViewerFrame.Margin = new Padding(2);
            btnUseViewerFrame.Name = "btnUseViewerFrame";
            btnUseViewerFrame.Size = new Size(164, 31);
            btnUseViewerFrame.TabIndex = 5;
            btnUseViewerFrame.Text = "Viewer 선택 이미지 사용";
            btnUseViewerFrame.UseVisualStyleBackColor = true;
            // 
            // btnPilotAutoPlay
            // 
            btnPilotAutoPlay.BackColor = Color.FromArgb(76, 175, 80);
            btnPilotAutoPlay.FlatStyle = FlatStyle.Flat;
            btnPilotAutoPlay.ForeColor = Color.White;
            btnPilotAutoPlay.Location = new Point(466, 142);
            btnPilotAutoPlay.Margin = new Padding(2);
            btnPilotAutoPlay.Name = "btnPilotAutoPlay";
            btnPilotAutoPlay.Size = new Size(94, 31);
            btnPilotAutoPlay.TabIndex = 6;
            btnPilotAutoPlay.Text = "자동 재생";
            btnPilotAutoPlay.UseVisualStyleBackColor = false;
            // 
            // btnPilotStop
            // 
            btnPilotStop.Location = new Point(568, 142);
            btnPilotStop.Margin = new Padding(2);
            btnPilotStop.Name = "btnPilotStop";
            btnPilotStop.Size = new Size(78, 31);
            btnPilotStop.TabIndex = 7;
            btnPilotStop.Text = "멈춤";
            btnPilotStop.UseVisualStyleBackColor = true;
            // 
            // picPilotTest
            // 
            picPilotTest.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picPilotTest.BackColor = Color.Black;
            picPilotTest.BorderStyle = BorderStyle.FixedSingle;
            picPilotTest.Location = new Point(24, 188);
            picPilotTest.Margin = new Padding(2);
            picPilotTest.Name = "picPilotTest";
            picPilotTest.Size = new Size(428, 338);
            picPilotTest.SizeMode = PictureBoxSizeMode.Zoom;
            picPilotTest.TabIndex = 8;
            picPilotTest.TabStop = false;
            // 
            // lblActualAngle
            // 
            lblActualAngle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblActualAngle.AutoSize = true;
            lblActualAngle.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblActualAngle.Location = new Point(474, 188);
            lblActualAngle.Margin = new Padding(2, 0, 2, 0);
            lblActualAngle.Name = "lblActualAngle";
            lblActualAngle.Size = new Size(109, 21);
            lblActualAngle.TabIndex = 9;
            lblActualAngle.Text = "실제 Angle: -";
            // 
            // lblPredictedAngle
            // 
            lblPredictedAngle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPredictedAngle.AutoSize = true;
            lblPredictedAngle.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblPredictedAngle.Location = new Point(474, 221);
            lblPredictedAngle.Margin = new Padding(2, 0, 2, 0);
            lblPredictedAngle.Name = "lblPredictedAngle";
            lblPredictedAngle.Size = new Size(109, 21);
            lblPredictedAngle.TabIndex = 10;
            lblPredictedAngle.Text = "예측 Angle: -";
            // 
            // lblActualThrottle
            // 
            lblActualThrottle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblActualThrottle.AutoSize = true;
            lblActualThrottle.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblActualThrottle.Location = new Point(474, 262);
            lblActualThrottle.Margin = new Padding(2, 0, 2, 0);
            lblActualThrottle.Name = "lblActualThrottle";
            lblActualThrottle.Size = new Size(116, 20);
            lblActualThrottle.TabIndex = 11;
            lblActualThrottle.Text = "실제 Throttle: -";
            // 
            // lblPredictedThrottle
            // 
            lblPredictedThrottle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPredictedThrottle.AutoSize = true;
            lblPredictedThrottle.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblPredictedThrottle.Location = new Point(474, 292);
            lblPredictedThrottle.Margin = new Padding(2, 0, 2, 0);
            lblPredictedThrottle.Name = "lblPredictedThrottle";
            lblPredictedThrottle.Size = new Size(116, 20);
            lblPredictedThrottle.TabIndex = 12;
            lblPredictedThrottle.Text = "예측 Throttle: -";
            // 
            // lblAngleError
            // 
            lblAngleError.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblAngleError.AutoSize = true;
            lblAngleError.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblAngleError.Location = new Point(474, 334);
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
            lblPilotWarning.ForeColor = Color.DimGray;
            lblPilotWarning.Location = new Point(474, 338);
            lblPilotWarning.Margin = new Padding(2, 0, 2, 0);
            lblPilotWarning.Name = "lblPilotWarning";
            lblPilotWarning.Size = new Size(59, 21);
            lblPilotWarning.TabIndex = 14;
            lblPilotWarning.Text = "판정: -";
            // 
            // lblPilotNote
            // 
            lblPilotNote.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPilotNote.ForeColor = Color.DimGray;
            lblPilotNote.Location = new Point(474, 405);
            lblPilotNote.Margin = new Padding(2, 0, 2, 0);
            lblPilotNote.Name = "lblPilotNote";
            lblPilotNote.Size = new Size(186, 112);
            lblPilotNote.TabIndex = 15;
            lblPilotNote.Text = "파란선: 실제 angle\n초록선: 예측 angle\n노란 반투명 영역:\n실제/예측 차이\n하단 막대:\n실제/예측 throttle 비교";
            // 
            // lblPilotImageList
            // 
            lblPilotImageList.AutoSize = true;
            lblPilotImageList.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblPilotImageList.Location = new Point(684, 188);
            lblPilotImageList.Margin = new Padding(2, 0, 2, 0);
            lblPilotImageList.Name = "lblPilotImageList";
            lblPilotImageList.Size = new Size(139, 20);
            lblPilotImageList.TabIndex = 16;
            lblPilotImageList.Text = "테스트 이미지 선택";
            // 
            // lstPilotFrames
            // 
            lstPilotFrames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lstPilotFrames.Font = new Font("Consolas", 10F);
            lstPilotFrames.HorizontalScrollbar = true;
            lstPilotFrames.Location = new Point(684, 210);
            lstPilotFrames.Margin = new Padding(2);
            lstPilotFrames.Name = "lstPilotFrames";
            lstPilotFrames.Size = new Size(374, 289);
            lstPilotFrames.TabIndex = 17;
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Font = new Font("Consolas", 9F);
            txtLog.Location = new Point(0, 544);
            txtLog.Margin = new Padding(2);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(1090, 132);
            txtLog.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1089, 666);
            Controls.Add(tabMain);
            Controls.Add(txtLog);
            Margin = new Padding(2);
            MinimumSize = new Size(996, 616);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Donkeycar Manager";
            WindowState = FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)trbBrightness).EndInit();
            ((System.ComponentModel.ISupportInitialize)trbContrast).EndInit();
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
    }
}
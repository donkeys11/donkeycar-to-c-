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
        private Button btnOpenDataFolder;
        private TextBox txtDataPath;
        private TrackBar trbBrightness;
        private TrackBar trbContrast;
        private Label lblBrightness;
        private Label lblContrast;
        //폴더 선택 해제 버튼
        private Button btnClearDataPath;


        private TabControl tabMain;
        private TabPage tabCleaner;
        private TabPage tabTrainer;
        private TabPage tabPilotTest;

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
            btnClearDataPath = new Button();
            cmbModelList = new ComboBox();
            btnScanModels = new Button();
            lblModelList = new Label();
            trbBrightness = new TrackBar();
            trbContrast = new TrackBar();
            btnOpenDataFolder = new Button();
            txtDataPath = new TextBox();
            lblBrightness = new Label();
            lblContrast = new Label();
            tabMain = new TabControl();
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
            tabCleaner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picCleanerPreview).BeginInit();
            grpFilters.SuspendLayout();
            grpCleanerRangeEditor.SuspendLayout();
            tabTrainer.SuspendLayout();
            tabPilotTest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPilotTest).BeginInit();
            SuspendLayout();
            // 
            // btnClearDataPath
            // 
            btnClearDataPath.BackColor = SystemColors.Control;
            btnClearDataPath.FlatAppearance.BorderSize = 0;
            btnClearDataPath.FlatStyle = FlatStyle.Flat;
            btnClearDataPath.Location = new Point(1354, 41);
            btnClearDataPath.Margin = new Padding(4, 2, 4, 2);
            btnClearDataPath.Name = "btnClearDataPath";
            btnClearDataPath.Size = new Size(84, 53);
            btnClearDataPath.TabIndex = 98;
            btnClearDataPath.UseVisualStyleBackColor = false;
            // 
            // cmbModelList
            // 
            cmbModelList.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbModelList.Location = new Point(188, 226);
            cmbModelList.Margin = new Padding(4);
            cmbModelList.Name = "cmbModelList";
            cmbModelList.Size = new Size(838, 45);
            cmbModelList.TabIndex = 19;
            // 
            // btnScanModels
            // 
            btnScanModels.Location = new Point(1050, 224);
            btnScanModels.Margin = new Padding(4);
            btnScanModels.Name = "btnScanModels";
            btnScanModels.Size = new Size(156, 53);
            btnScanModels.TabIndex = 20;
            btnScanModels.Text = "모델 스캔";
            btnScanModels.UseVisualStyleBackColor = true;
            // 
            // lblModelList
            // 
            lblModelList.AutoSize = true;
            lblModelList.Location = new Point(48, 233);
            lblModelList.Margin = new Padding(4, 0, 4, 0);
            lblModelList.Name = "lblModelList";
            lblModelList.Size = new Size(134, 37);
            lblModelList.TabIndex = 18;
            lblModelList.Text = "모델 목록";
            // 
            // trbBrightness
            // 
            trbBrightness.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trbBrightness.AutoSize = false;
            trbBrightness.LargeChange = 10;
            trbBrightness.Location = new Point(544, 607);
            trbBrightness.Margin = new Padding(4);
            trbBrightness.Maximum = 100;
            trbBrightness.Minimum = -100;
            trbBrightness.Name = "trbBrightness";
            trbBrightness.Size = new Size(690, 53);
            trbBrightness.SmallChange = 5;
            trbBrightness.TabIndex = 5;
            trbBrightness.TickStyle = TickStyle.None;
            // 
            // trbContrast
            // 
            trbContrast.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trbContrast.AutoSize = false;
            trbContrast.LargeChange = 10;
            trbContrast.Location = new Point(544, 669);
            trbContrast.Margin = new Padding(4);
            trbContrast.Maximum = 100;
            trbContrast.Minimum = -100;
            trbContrast.Name = "trbContrast";
            trbContrast.Size = new Size(690, 58);
            trbContrast.SmallChange = 5;
            trbContrast.TabIndex = 7;
            trbContrast.TickStyle = TickStyle.None;
            // 
            // btnOpenDataFolder
            // 
            btnOpenDataFolder.BackColor = SystemColors.Control;
            btnOpenDataFolder.FlatAppearance.BorderColor = SystemColors.ControlDark;
            btnOpenDataFolder.FlatAppearance.BorderSize = 0;
            btnOpenDataFolder.FlatStyle = FlatStyle.Flat;
            btnOpenDataFolder.ForeColor = Color.DimGray;
            btnOpenDataFolder.Location = new Point(1268, 41);
            btnOpenDataFolder.Margin = new Padding(6);
            btnOpenDataFolder.Name = "btnOpenDataFolder";
            btnOpenDataFolder.Size = new Size(84, 53);
            btnOpenDataFolder.TabIndex = 2;
            btnOpenDataFolder.UseVisualStyleBackColor = false;
            // 
            // txtDataPath
            // 
            txtDataPath.AllowDrop = true;
            txtDataPath.Font = new Font("맑은 고딕 Semilight", 12F, FontStyle.Regular, GraphicsUnit.Point, 129);
            txtDataPath.ForeColor = Color.DimGray;
            txtDataPath.Location = new Point(498, 45);
            txtDataPath.Margin = new Padding(4, 2, 4, 2);
            txtDataPath.Name = "txtDataPath";
            txtDataPath.ReadOnly = true;
            txtDataPath.Size = new Size(770, 50);
            txtDataPath.TabIndex = 99;
            txtDataPath.Text = "폴더를 선택하거나 끌어오세요";
            // 
            // lblBrightness
            // 
            lblBrightness.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblBrightness.AutoSize = true;
            lblBrightness.Location = new Point(435, 608);
            lblBrightness.Margin = new Padding(4, 0, 4, 0);
            lblBrightness.Name = "lblBrightness";
            lblBrightness.Size = new Size(101, 37);
            lblBrightness.TabIndex = 4;
            lblBrightness.Text = "밝기: 0";
            // 
            // lblContrast
            // 
            lblContrast.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblContrast.AutoSize = true;
            lblContrast.Location = new Point(435, 669);
            lblContrast.Margin = new Padding(4, 0, 4, 0);
            lblContrast.Name = "lblContrast";
            lblContrast.Size = new Size(101, 37);
            lblContrast.TabIndex = 6;
            lblContrast.Text = "명암: 0";
            // 
            // tabMain
            // 
            tabMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabMain.Controls.Add(tabCleaner);
            tabMain.Controls.Add(tabTrainer);
            tabMain.Controls.Add(tabPilotTest);
            tabMain.Font = new Font("맑은 고딕", 10F);
            tabMain.Location = new Point(13, 13);
            tabMain.Margin = new Padding(4);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(2620, 1160);
            tabMain.TabIndex = 0;
            // 
            // tabCleaner
            // 
            tabCleaner.BackColor = Color.WhiteSmoke;
            tabCleaner.Controls.Add(lblTitleCleaner);
            tabCleaner.Controls.Add(picCleanerPreview);
            tabCleaner.Controls.Add(btnOpenDataFolder);
            tabCleaner.Controls.Add(txtDataPath);
            tabCleaner.Controls.Add(btnClearDataPath);
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
            tabCleaner.Location = new Point(8, 51);
            tabCleaner.Margin = new Padding(4);
            tabCleaner.Name = "tabCleaner";
            tabCleaner.Padding = new Padding(4);
            tabCleaner.Size = new Size(2604, 1101);
            tabCleaner.TabIndex = 0;
            tabCleaner.Text = "Cleaner - 데이터 정리";
            // 
            // lblTitleCleaner
            // 
            lblTitleCleaner.AutoSize = true;
            lblTitleCleaner.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitleCleaner.ForeColor = Color.FromArgb(180, 70, 70);
            lblTitleCleaner.Location = new Point(32, 23);
            lblTitleCleaner.Margin = new Padding(4, 0, 4, 0);
            lblTitleCleaner.Name = "lblTitleCleaner";
            lblTitleCleaner.Size = new Size(370, 78);
            lblTitleCleaner.TabIndex = 0;
            lblTitleCleaner.Text = "Tub Cleaner";
            // 
            // picCleanerPreview
            // 
            picCleanerPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picCleanerPreview.BackColor = Color.Black;
            picCleanerPreview.BorderStyle = BorderStyle.FixedSingle;
            picCleanerPreview.Location = new Point(32, 145);
            picCleanerPreview.Margin = new Padding(4);
            picCleanerPreview.Name = "picCleanerPreview";
            picCleanerPreview.Size = new Size(1569, 401);
            picCleanerPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picCleanerPreview.TabIndex = 1;
            picCleanerPreview.TabStop = false;
            // 
            // lblCleanerInfo
            // 
            lblCleanerInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblCleanerInfo.AutoSize = true;
            lblCleanerInfo.Location = new Point(32, 102);
            lblCleanerInfo.Margin = new Padding(4, 0, 4, 0);
            lblCleanerInfo.Name = "lblCleanerInfo";
            lblCleanerInfo.Size = new Size(250, 37);
            lblCleanerInfo.TabIndex = 2;
            lblCleanerInfo.Text = "선택 프레임 정보: -";
            // 
            // lblImageAdjust
            // 
            lblImageAdjust.AutoSize = true;
            lblImageAdjust.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
            lblImageAdjust.ForeColor = Color.DimGray;
            lblImageAdjust.Location = new Point(32, 562);
            lblImageAdjust.Margin = new Padding(4, 0, 4, 0);
            lblImageAdjust.Name = "lblImageAdjust";
            lblImageAdjust.Size = new Size(142, 32);
            lblImageAdjust.TabIndex = 3;
            lblImageAdjust.Text = "이미지 조작";
            // 
            // chkFlipHorizontal
            // 
            chkFlipHorizontal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chkFlipHorizontal.AutoSize = true;
            chkFlipHorizontal.Location = new Point(32, 665);
            chkFlipHorizontal.Margin = new Padding(4);
            chkFlipHorizontal.Name = "chkFlipHorizontal";
            chkFlipHorizontal.Size = new Size(384, 41);
            chkFlipHorizontal.TabIndex = 8;
            chkFlipHorizontal.Text = "좌우 반전 (angle 자동 반전)";
            chkFlipHorizontal.UseVisualStyleBackColor = true;
            // 
            // chkGrayscale
            // 
            chkGrayscale.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chkGrayscale.AutoSize = true;
            chkGrayscale.Location = new Point(32, 607);
            chkGrayscale.Margin = new Padding(4);
            chkGrayscale.Name = "chkGrayscale";
            chkGrayscale.Size = new Size(211, 41);
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
            btnSaveProcessed.Location = new Point(1242, 625);
            btnSaveProcessed.Margin = new Padding(4);
            btnSaveProcessed.Name = "btnSaveProcessed";
            btnSaveProcessed.Size = new Size(248, 66);
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
            grpFilters.Location = new Point(1609, 154);
            grpFilters.Margin = new Padding(4);
            grpFilters.Name = "grpFilters";
            grpFilters.Padding = new Padding(4);
            grpFilters.Size = new Size(712, 247);
            grpFilters.TabIndex = 11;
            grpFilters.TabStop = false;
            grpFilters.Text = "필터 조건";
            // 
            // chkThrottlePositive
            // 
            chkThrottlePositive.AutoSize = true;
            chkThrottlePositive.Location = new Point(32, 55);
            chkThrottlePositive.Margin = new Padding(4);
            chkThrottlePositive.Name = "chkThrottlePositive";
            chkThrottlePositive.Size = new Size(281, 41);
            chkThrottlePositive.TabIndex = 0;
            chkThrottlePositive.Text = "throttle > 0만 보기";
            chkThrottlePositive.UseVisualStyleBackColor = true;
            // 
            // chkExcludeZeroAngle
            // 
            chkExcludeZeroAngle.AutoSize = true;
            chkExcludeZeroAngle.Location = new Point(32, 119);
            chkExcludeZeroAngle.Margin = new Padding(4);
            chkExcludeZeroAngle.Name = "chkExcludeZeroAngle";
            chkExcludeZeroAngle.Size = new Size(250, 41);
            chkExcludeZeroAngle.TabIndex = 1;
            chkExcludeZeroAngle.Text = "angle == 0 제외";
            chkExcludeZeroAngle.UseVisualStyleBackColor = true;
            // 
            // chkStopDataOnly
            // 
            chkStopDataOnly.AutoSize = true;
            chkStopDataOnly.Location = new Point(32, 183);
            chkStopDataOnly.Margin = new Padding(4);
            chkStopDataOnly.Name = "chkStopDataOnly";
            chkStopDataOnly.Size = new Size(460, 41);
            chkStopDataOnly.TabIndex = 2;
            chkStopDataOnly.Text = "정지 데이터만 보기(throttle == 0)";
            chkStopDataOnly.UseVisualStyleBackColor = true;
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnApplyFilter.Location = new Point(2329, 172);
            btnApplyFilter.Margin = new Padding(4);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(202, 64);
            btnApplyFilter.TabIndex = 12;
            btnApplyFilter.Text = "필터 적용";
            btnApplyFilter.UseVisualStyleBackColor = true;
            // 
            // btnClearFilter
            // 
            btnClearFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearFilter.Location = new Point(2329, 244);
            btnClearFilter.Margin = new Padding(4);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(202, 64);
            btnClearFilter.TabIndex = 13;
            btnClearFilter.Text = "전체 보기";
            btnClearFilter.UseVisualStyleBackColor = true;
            // 
            // btnDeleteFrame
            // 
            btnDeleteFrame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteFrame.BackColor = Color.LightCoral;
            btnDeleteFrame.FlatStyle = FlatStyle.Flat;
            btnDeleteFrame.Location = new Point(2329, 316);
            btnDeleteFrame.Margin = new Padding(4);
            btnDeleteFrame.Name = "btnDeleteFrame";
            btnDeleteFrame.Size = new Size(252, 81);
            btnDeleteFrame.TabIndex = 14;
            btnDeleteFrame.Text = "선택 프레임 삭제";
            btnDeleteFrame.UseVisualStyleBackColor = false;
            // 
            // lstCleanerFrames
            // 
            lstCleanerFrames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lstCleanerFrames.Font = new Font("Consolas", 9F);
            lstCleanerFrames.HorizontalScrollbar = true;
            lstCleanerFrames.Location = new Point(1618, 409);
            lstCleanerFrames.Margin = new Padding(4);
            lstCleanerFrames.Name = "lstCleanerFrames";
            lstCleanerFrames.SelectionMode = SelectionMode.MultiExtended;
            lstCleanerFrames.Size = new Size(712, 368);
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
            grpCleanerRangeEditor.Location = new Point(32, 763);
            grpCleanerRangeEditor.Margin = new Padding(4);
            grpCleanerRangeEditor.Name = "grpCleanerRangeEditor";
            grpCleanerRangeEditor.Padding = new Padding(4);
            grpCleanerRangeEditor.Size = new Size(2289, 320);
            grpCleanerRangeEditor.TabIndex = 16;
            grpCleanerRangeEditor.TabStop = false;
            grpCleanerRangeEditor.Text = "구간 선택 편집";
            // 
            // lblCleanerRangeInfo
            // 
            lblCleanerRangeInfo.AutoSize = true;
            lblCleanerRangeInfo.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);
            lblCleanerRangeInfo.ForeColor = Color.FromArgb(170, 60, 50);
            lblCleanerRangeInfo.Location = new Point(24, 45);
            lblCleanerRangeInfo.Margin = new Padding(4, 0, 4, 0);
            lblCleanerRangeInfo.Name = "lblCleanerRangeInfo";
            lblCleanerRangeInfo.Size = new Size(204, 37);
            lblCleanerRangeInfo.TabIndex = 0;
            lblCleanerRangeInfo.Text = "선택 구간: 없음";
            // 
            // lblCleanerRangeHint
            // 
            lblCleanerRangeHint.AutoSize = true;
            lblCleanerRangeHint.ForeColor = Color.DimGray;
            lblCleanerRangeHint.Location = new Point(560, 45);
            lblCleanerRangeHint.Margin = new Padding(4, 0, 4, 0);
            lblCleanerRangeHint.Name = "lblCleanerRangeHint";
            lblCleanerRangeHint.Size = new Size(934, 37);
            lblCleanerRangeHint.TabIndex = 1;
            lblCleanerRangeHint.Text = "스크롤바로 구간 이동 / 썸네일 1개 = 실제 이미지 1장 / 드래그로 구간 선택";
            // 
            // pnlCleanerTimeline
            // 
            pnlCleanerTimeline.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlCleanerTimeline.BackColor = Color.FromArgb(18, 26, 42);
            pnlCleanerTimeline.BorderStyle = BorderStyle.FixedSingle;
            pnlCleanerTimeline.Location = new Point(20, 115);
            pnlCleanerTimeline.Margin = new Padding(4);
            pnlCleanerTimeline.Name = "pnlCleanerTimeline";
            pnlCleanerTimeline.Size = new Size(1853, 130);
            pnlCleanerTimeline.TabIndex = 2;
            // 
            // hsbCleanerTimeline
            // 
            hsbCleanerTimeline.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            hsbCleanerTimeline.LargeChange = 1;
            hsbCleanerTimeline.Location = new Point(24, 235);
            hsbCleanerTimeline.Maximum = 0;
            hsbCleanerTimeline.Name = "hsbCleanerTimeline";
            hsbCleanerTimeline.Size = new Size(1853, 22);
            hsbCleanerTimeline.TabIndex = 3;
            // 
            // lblCleanerTimelineScrollInfo
            // 
            lblCleanerTimelineScrollInfo.AutoSize = true;
            lblCleanerTimelineScrollInfo.ForeColor = Color.DimGray;
            lblCleanerTimelineScrollInfo.Location = new Point(24, 269);
            lblCleanerTimelineScrollInfo.Margin = new Padding(4, 0, 4, 0);
            lblCleanerTimelineScrollInfo.Name = "lblCleanerTimelineScrollInfo";
            lblCleanerTimelineScrollInfo.Size = new Size(160, 37);
            lblCleanerTimelineScrollInfo.TabIndex = 4;
            lblCleanerTimelineScrollInfo.Text = "표시 구간: -";
            // 
            // btnDeleteRange
            // 
            btnDeleteRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteRange.BackColor = Color.FromArgb(180, 60, 50);
            btnDeleteRange.FlatStyle = FlatStyle.Flat;
            btnDeleteRange.ForeColor = Color.White;
            btnDeleteRange.Location = new Point(1905, 115);
            btnDeleteRange.Margin = new Padding(4);
            btnDeleteRange.Name = "btnDeleteRange";
            btnDeleteRange.Size = new Size(164, 53);
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
            btnPlayRange.Location = new Point(2087, 115);
            btnPlayRange.Margin = new Padding(4);
            btnPlayRange.Name = "btnPlayRange";
            btnPlayRange.Size = new Size(164, 53);
            btnPlayRange.TabIndex = 6;
            btnPlayRange.UseVisualStyleBackColor = false;
            // 
            // btnClearRange
            // 
            btnClearRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearRange.Location = new Point(1905, 192);
            btnClearRange.Margin = new Padding(4);
            btnClearRange.Name = "btnClearRange";
            btnClearRange.Size = new Size(164, 53);
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
            btnCleanerAutoPlay.Location = new Point(2087, 192);
            btnCleanerAutoPlay.Margin = new Padding(4);
            btnCleanerAutoPlay.Name = "btnCleanerAutoPlay";
            btnCleanerAutoPlay.Size = new Size(164, 53);
            btnCleanerAutoPlay.TabIndex = 8;
            btnCleanerAutoPlay.Text = "자동 재생";
            btnCleanerAutoPlay.UseVisualStyleBackColor = false;
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
            tabTrainer.Location = new Point(8, 51);
            tabTrainer.Margin = new Padding(4);
            tabTrainer.Name = "tabTrainer";
            tabTrainer.Padding = new Padding(4);
            tabTrainer.Size = new Size(2604, 1101);
            tabTrainer.TabIndex = 1;
            tabTrainer.Text = "Trainer - 학습 실행";
            // 
            // lblTitleTrainer
            // 
            lblTitleTrainer.AutoSize = true;
            lblTitleTrainer.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitleTrainer.ForeColor = Color.FromArgb(60, 130, 80);
            lblTitleTrainer.Location = new Point(32, 30);
            lblTitleTrainer.Margin = new Padding(4, 0, 4, 0);
            lblTitleTrainer.Name = "lblTitleTrainer";
            lblTitleTrainer.Size = new Size(548, 78);
            lblTitleTrainer.TabIndex = 0;
            lblTitleTrainer.Text = "Donkeycar Trainer";
            // 
            // lblMycarPath
            // 
            lblMycarPath.AutoSize = true;
            lblMycarPath.Location = new Point(48, 160);
            lblMycarPath.Margin = new Padding(4, 0, 4, 0);
            lblMycarPath.Name = "lblMycarPath";
            lblMycarPath.Size = new Size(154, 37);
            lblMycarPath.TabIndex = 1;
            lblMycarPath.Text = "mycar 경로";
            // 
            // txtMycarPath
            // 
            txtMycarPath.Location = new Point(232, 154);
            txtMycarPath.Margin = new Padding(4);
            txtMycarPath.Name = "txtMycarPath";
            txtMycarPath.Size = new Size(1118, 43);
            txtMycarPath.TabIndex = 2;
            // 
            // btnBrowseMycar
            // 
            btnBrowseMycar.Location = new Point(1384, 151);
            btnBrowseMycar.Margin = new Padding(4);
            btnBrowseMycar.Name = "btnBrowseMycar";
            btnBrowseMycar.Size = new Size(140, 53);
            btnBrowseMycar.TabIndex = 3;
            btnBrowseMycar.Text = "찾기";
            btnBrowseMycar.UseVisualStyleBackColor = true;
            // 
            // lblPythonExe
            // 
            lblPythonExe.AutoSize = true;
            lblPythonExe.Location = new Point(48, 247);
            lblPythonExe.Margin = new Padding(4, 0, 4, 0);
            lblPythonExe.Name = "lblPythonExe";
            lblPythonExe.Size = new Size(192, 37);
            lblPythonExe.TabIndex = 4;
            lblPythonExe.Text = "Python 실행명";
            // 
            // txtPythonExe
            // 
            txtPythonExe.Location = new Point(232, 241);
            txtPythonExe.Margin = new Padding(4);
            txtPythonExe.Name = "txtPythonExe";
            txtPythonExe.Size = new Size(464, 43);
            txtPythonExe.TabIndex = 5;
            // 
            // lblTrainArgs
            // 
            lblTrainArgs.AutoSize = true;
            lblTrainArgs.Location = new Point(48, 337);
            lblTrainArgs.Margin = new Padding(4, 0, 4, 0);
            lblTrainArgs.Name = "lblTrainArgs";
            lblTrainArgs.Size = new Size(197, 37);
            lblTrainArgs.TabIndex = 6;
            lblTrainArgs.Text = "학습 명령 인자";
            // 
            // txtTrainArgs
            // 
            txtTrainArgs.Location = new Point(232, 331);
            txtTrainArgs.Margin = new Padding(4);
            txtTrainArgs.Name = "txtTrainArgs";
            txtTrainArgs.Size = new Size(1288, 43);
            txtTrainArgs.TabIndex = 7;
            // 
            // btnTrain
            // 
            btnTrain.BackColor = Color.FromArgb(76, 175, 80);
            btnTrain.FlatStyle = FlatStyle.Flat;
            btnTrain.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            btnTrain.ForeColor = Color.White;
            btnTrain.Location = new Point(232, 431);
            btnTrain.Margin = new Padding(4);
            btnTrain.Name = "btnTrain";
            btnTrain.Size = new Size(248, 81);
            btnTrain.TabIndex = 8;
            btnTrain.Text = "학습 시작";
            btnTrain.UseVisualStyleBackColor = false;
            // 
            // btnStopTrain
            // 
            btnStopTrain.BackColor = Color.LightCoral;
            btnStopTrain.FlatStyle = FlatStyle.Flat;
            btnStopTrain.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            btnStopTrain.Location = new Point(512, 431);
            btnStopTrain.Margin = new Padding(4);
            btnStopTrain.Name = "btnStopTrain";
            btnStopTrain.Size = new Size(248, 81);
            btnStopTrain.TabIndex = 9;
            btnStopTrain.Text = "학습 중지";
            btnStopTrain.UseVisualStyleBackColor = false;
            // 
            // lblModelStatus
            // 
            lblModelStatus.AutoSize = true;
            lblModelStatus.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblModelStatus.Location = new Point(232, 553);
            lblModelStatus.Margin = new Padding(4, 0, 4, 0);
            lblModelStatus.Name = "lblModelStatus";
            lblModelStatus.Size = new Size(180, 41);
            lblModelStatus.TabIndex = 10;
            lblModelStatus.Text = "모델 상태: -";
            // 
            // lblTrainInfo
            // 
            lblTrainInfo.AutoSize = true;
            lblTrainInfo.ForeColor = Color.DimGray;
            lblTrainInfo.Location = new Point(232, 631);
            lblTrainInfo.Margin = new Padding(4, 0, 4, 0);
            lblTrainInfo.Name = "lblTrainInfo";
            lblTrainInfo.Size = new Size(832, 148);
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
            tabPilotTest.Location = new Point(8, 51);
            tabPilotTest.Margin = new Padding(4);
            tabPilotTest.Name = "tabPilotTest";
            tabPilotTest.Padding = new Padding(4);
            tabPilotTest.Size = new Size(2604, 1101);
            tabPilotTest.TabIndex = 2;
            tabPilotTest.Text = "Pilot Test - 모델 테스트";
            // 
            // lblTitlePilot
            // 
            lblTitlePilot.AutoSize = true;
            lblTitlePilot.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitlePilot.ForeColor = Color.FromArgb(90, 90, 160);
            lblTitlePilot.Location = new Point(32, 30);
            lblTitlePilot.Margin = new Padding(4, 0, 4, 0);
            lblTitlePilot.Name = "lblTitlePilot";
            lblTitlePilot.Size = new Size(731, 78);
            lblTitlePilot.TabIndex = 0;
            lblTitlePilot.Text = "Pilot Arena / Model Test";
            // 
            // lblModelPath
            // 
            lblModelPath.AutoSize = true;
            lblModelPath.Location = new Point(48, 160);
            lblModelPath.Margin = new Padding(4, 0, 4, 0);
            lblModelPath.Name = "lblModelPath";
            lblModelPath.Size = new Size(134, 37);
            lblModelPath.TabIndex = 1;
            lblModelPath.Text = "모델 파일";
            // 
            // txtModelPath
            // 
            txtModelPath.Location = new Point(188, 154);
            txtModelPath.Margin = new Padding(4);
            txtModelPath.Name = "txtModelPath";
            txtModelPath.Size = new Size(992, 43);
            txtModelPath.TabIndex = 2;
            // 
            // btnBrowseModel
            // 
            btnBrowseModel.Location = new Point(1212, 151);
            btnBrowseModel.Margin = new Padding(4);
            btnBrowseModel.Name = "btnBrowseModel";
            btnBrowseModel.Size = new Size(140, 53);
            btnBrowseModel.TabIndex = 3;
            btnBrowseModel.Text = "찾기";
            btnBrowseModel.UseVisualStyleBackColor = true;
            // 
            // btnRunPilotTest
            // 
            btnRunPilotTest.Location = new Point(188, 303);
            btnRunPilotTest.Margin = new Padding(4);
            btnRunPilotTest.Name = "btnRunPilotTest";
            btnRunPilotTest.Size = new Size(388, 66);
            btnRunPilotTest.TabIndex = 4;
            btnRunPilotTest.Text = "현재 이미지로 예측 테스트";
            btnRunPilotTest.UseVisualStyleBackColor = true;
            // 
            // btnUseViewerFrame
            // 
            btnUseViewerFrame.Location = new Point(592, 303);
            btnUseViewerFrame.Margin = new Padding(4);
            btnUseViewerFrame.Name = "btnUseViewerFrame";
            btnUseViewerFrame.Size = new Size(328, 66);
            btnUseViewerFrame.TabIndex = 5;
            btnUseViewerFrame.Text = "Viewer 선택 이미지 사용";
            btnUseViewerFrame.UseVisualStyleBackColor = true;
            // 
            // btnPilotAutoPlay
            // 
            btnPilotAutoPlay.BackColor = Color.FromArgb(76, 175, 80);
            btnPilotAutoPlay.FlatStyle = FlatStyle.Flat;
            btnPilotAutoPlay.ForeColor = Color.White;
            btnPilotAutoPlay.Location = new Point(932, 303);
            btnPilotAutoPlay.Margin = new Padding(4);
            btnPilotAutoPlay.Name = "btnPilotAutoPlay";
            btnPilotAutoPlay.Size = new Size(188, 66);
            btnPilotAutoPlay.TabIndex = 6;
            btnPilotAutoPlay.Text = "자동 재생";
            btnPilotAutoPlay.UseVisualStyleBackColor = false;
            // 
            // btnPilotStop
            // 
            btnPilotStop.Location = new Point(1136, 303);
            btnPilotStop.Margin = new Padding(4);
            btnPilotStop.Name = "btnPilotStop";
            btnPilotStop.Size = new Size(156, 66);
            btnPilotStop.TabIndex = 7;
            btnPilotStop.Text = "멈춤";
            btnPilotStop.UseVisualStyleBackColor = true;
            // 
            // picPilotTest
            // 
            picPilotTest.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            picPilotTest.BackColor = Color.Black;
            picPilotTest.BorderStyle = BorderStyle.FixedSingle;
            picPilotTest.Location = new Point(48, 401);
            picPilotTest.Margin = new Padding(4);
            picPilotTest.Name = "picPilotTest";
            picPilotTest.Size = new Size(854, 601);
            picPilotTest.SizeMode = PictureBoxSizeMode.Zoom;
            picPilotTest.TabIndex = 8;
            picPilotTest.TabStop = false;
            // 
            // lblActualAngle
            // 
            lblActualAngle.AutoSize = true;
            lblActualAngle.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblActualAngle.Location = new Point(948, 401);
            lblActualAngle.Margin = new Padding(4, 0, 4, 0);
            lblActualAngle.Name = "lblActualAngle";
            lblActualAngle.Size = new Size(216, 45);
            lblActualAngle.TabIndex = 9;
            lblActualAngle.Text = "실제 Angle: -";
            // 
            // lblPredictedAngle
            // 
            lblPredictedAngle.AutoSize = true;
            lblPredictedAngle.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblPredictedAngle.Location = new Point(948, 471);
            lblPredictedAngle.Margin = new Padding(4, 0, 4, 0);
            lblPredictedAngle.Name = "lblPredictedAngle";
            lblPredictedAngle.Size = new Size(216, 45);
            lblPredictedAngle.TabIndex = 10;
            lblPredictedAngle.Text = "예측 Angle: -";
            // 
            // lblActualThrottle
            // 
            lblActualThrottle.AutoSize = true;
            lblActualThrottle.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblActualThrottle.Location = new Point(948, 559);
            lblActualThrottle.Margin = new Padding(4, 0, 4, 0);
            lblActualThrottle.Name = "lblActualThrottle";
            lblActualThrottle.Size = new Size(234, 41);
            lblActualThrottle.TabIndex = 11;
            lblActualThrottle.Text = "실제 Throttle: -";
            // 
            // lblPredictedThrottle
            // 
            lblPredictedThrottle.AutoSize = true;
            lblPredictedThrottle.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblPredictedThrottle.Location = new Point(948, 623);
            lblPredictedThrottle.Margin = new Padding(4, 0, 4, 0);
            lblPredictedThrottle.Name = "lblPredictedThrottle";
            lblPredictedThrottle.Size = new Size(234, 41);
            lblPredictedThrottle.TabIndex = 12;
            lblPredictedThrottle.Text = "예측 Throttle: -";
            // 
            // lblAngleError
            // 
            lblAngleError.AutoSize = true;
            lblAngleError.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblAngleError.Location = new Point(948, 713);
            lblAngleError.Margin = new Padding(4, 0, 4, 0);
            lblAngleError.Name = "lblAngleError";
            lblAngleError.Size = new Size(228, 45);
            lblAngleError.TabIndex = 13;
            lblAngleError.Text = "Angle Error: -";
            // 
            // lblPilotWarning
            // 
            lblPilotWarning.AutoSize = true;
            lblPilotWarning.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblPilotWarning.ForeColor = Color.DimGray;
            lblPilotWarning.Location = new Point(948, 721);
            lblPilotWarning.Margin = new Padding(4, 0, 4, 0);
            lblPilotWarning.Name = "lblPilotWarning";
            lblPilotWarning.Size = new Size(116, 45);
            lblPilotWarning.TabIndex = 14;
            lblPilotWarning.Text = "판정: -";
            // 
            // lblPilotNote
            // 
            lblPilotNote.ForeColor = Color.DimGray;
            lblPilotNote.Location = new Point(948, 864);
            lblPilotNote.Margin = new Padding(4, 0, 4, 0);
            lblPilotNote.Name = "lblPilotNote";
            lblPilotNote.Size = new Size(372, 239);
            lblPilotNote.TabIndex = 15;
            lblPilotNote.Text = "파란선: 실제 angle\n초록선: 예측 angle\n노란 반투명 영역:\n실제/예측 차이\n하단 막대:\n실제/예측 throttle 비교";
            // 
            // lblPilotImageList
            // 
            lblPilotImageList.AutoSize = true;
            lblPilotImageList.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblPilotImageList.Location = new Point(1368, 401);
            lblPilotImageList.Margin = new Padding(4, 0, 4, 0);
            lblPilotImageList.Name = "lblPilotImageList";
            lblPilotImageList.Size = new Size(280, 41);
            lblPilotImageList.TabIndex = 16;
            lblPilotImageList.Text = "테스트 이미지 선택";
            // 
            // lstPilotFrames
            // 
            lstPilotFrames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstPilotFrames.Font = new Font("Consolas", 10F);
            lstPilotFrames.HorizontalScrollbar = true;
            lstPilotFrames.Location = new Point(1368, 448);
            lstPilotFrames.Margin = new Padding(4);
            lstPilotFrames.Name = "lstPilotFrames";
            lstPilotFrames.Size = new Size(1066, 612);
            lstPilotFrames.TabIndex = 17;
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Font = new Font("Consolas", 9F);
            txtLog.Location = new Point(0, 1286);
            txtLog.Margin = new Padding(4);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(2891, 277);
            txtLog.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(14F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(2639, 1186);
            Controls.Add(tabMain);
            Controls.Add(txtLog);
            Margin = new Padding(4);
            MinimumSize = new Size(1908, 995);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Donkeycar Manager";
            WindowState = FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)trbBrightness).EndInit();
            ((System.ComponentModel.ISupportInitialize)trbContrast).EndInit();
            tabMain.ResumeLayout(false);
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
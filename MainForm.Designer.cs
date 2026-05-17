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
        private PictureBox picPilotTest;
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
            picPilotTest = new PictureBox();
            lblActualAngle = new Label();
            lblPredictedAngle = new Label();
            lblActualThrottle = new Label();
            lblPredictedThrottle = new Label();
            lblAngleError = new Label();
            lblPilotWarning = new Label();
            lblPilotNote = new Label();
            txtLog = new TextBox();
            tabMain.SuspendLayout();
            tabViewer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picFrame).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trbFrame).BeginInit();
            tabCleaner.SuspendLayout();
            grpFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picCleanerPreview).BeginInit();
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
            tabMain.Location = new Point(0, 0);
            tabMain.Margin = new Padding(5, 5, 5, 5);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(2178, 1152);
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
            tabViewer.Location = new Point(8, 51);
            tabViewer.Margin = new Padding(5, 5, 5, 5);
            tabViewer.Name = "tabViewer";
            tabViewer.Padding = new Padding(5, 5, 5, 5);
            tabViewer.Size = new Size(2162, 1093);
            tabViewer.TabIndex = 0;
            tabViewer.Text = "Viewer - 데이터 확인";
            // 
            // lblTitleViewer
            // 
            lblTitleViewer.AutoSize = true;
            lblTitleViewer.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitleViewer.ForeColor = Color.Black;
            lblTitleViewer.Location = new Point(31, 29);
            lblTitleViewer.Margin = new Padding(5, 0, 5, 0);
            lblTitleViewer.Name = "lblTitleViewer";
            lblTitleViewer.Size = new Size(670, 78);
            lblTitleViewer.TabIndex = 0;
            lblTitleViewer.Text = "Donkeycar Tub Viewer";
            // 
            // btnOpenDataFolder
            // 
            btnOpenDataFolder.Location = new Point(31, 131);
            btnOpenDataFolder.Margin = new Padding(5, 5, 5, 5);
            btnOpenDataFolder.Name = "btnOpenDataFolder";
            btnOpenDataFolder.Size = new Size(249, 61);
            btnOpenDataFolder.TabIndex = 1;
            btnOpenDataFolder.Text = "데이터 폴더 열기";
            btnOpenDataFolder.UseVisualStyleBackColor = true;
            // 
            // btnReload
            // 
            btnReload.Location = new Point(296, 131);
            btnReload.Margin = new Padding(5, 5, 5, 5);
            btnReload.Name = "btnReload";
            btnReload.Size = new Size(171, 61);
            btnReload.TabIndex = 2;
            btnReload.Text = "새로고침";
            btnReload.UseVisualStyleBackColor = true;
            // 
            // btnAutoPlay
            // 
            btnAutoPlay.Location = new Point(482, 131);
            btnAutoPlay.Margin = new Padding(5, 5, 5, 5);
            btnAutoPlay.Name = "btnAutoPlay";
            btnAutoPlay.Size = new Size(187, 61);
            btnAutoPlay.TabIndex = 3;
            btnAutoPlay.Text = "자동 재생";
            btnAutoPlay.UseVisualStyleBackColor = true;
            // 
            // lblDataPath
            // 
            lblDataPath.AutoSize = true;
            lblDataPath.ForeColor = Color.DimGray;
            lblDataPath.Location = new Point(700, 144);
            lblDataPath.Margin = new Padding(5, 0, 5, 0);
            lblDataPath.Name = "lblDataPath";
            lblDataPath.Size = new Size(184, 37);
            lblDataPath.TabIndex = 4;
            lblDataPath.Text = "Data Folder: -";
            // 
            // picFrame
            // 
            picFrame.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picFrame.BackColor = Color.Black;
            picFrame.BorderStyle = BorderStyle.FixedSingle;
            picFrame.Location = new Point(31, 224);
            picFrame.Margin = new Padding(5, 5, 5, 5);
            picFrame.Name = "picFrame";
            picFrame.Size = new Size(1290, 623);
            picFrame.SizeMode = PictureBoxSizeMode.Zoom;
            picFrame.TabIndex = 5;
            picFrame.TabStop = false;
            // 
            // lstFrames
            // 
            lstFrames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lstFrames.Font = new Font("Consolas", 9F);
            lstFrames.HorizontalScrollbar = true;
            lstFrames.Location = new Point(1353, 224);
            lstFrames.Margin = new Padding(5, 5, 5, 5);
            lstFrames.Name = "lstFrames";
            lstFrames.Size = new Size(776, 620);
            lstFrames.TabIndex = 6;
            // 
            // lblFrameInfo
            // 
            lblFrameInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblFrameInfo.AutoSize = true;
            lblFrameInfo.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblFrameInfo.Location = new Point(31, 877);
            lblFrameInfo.Margin = new Padding(5, 0, 5, 0);
            lblFrameInfo.Name = "lblFrameInfo";
            lblFrameInfo.Size = new Size(137, 41);
            lblFrameInfo.TabIndex = 7;
            lblFrameInfo.Text = "Frame: -";
            // 
            // lblAngle
            // 
            lblAngle.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblAngle.AutoSize = true;
            lblAngle.Font = new Font("맑은 고딕", 11F);
            lblAngle.Location = new Point(311, 877);
            lblAngle.Margin = new Padding(5, 0, 5, 0);
            lblAngle.Name = "lblAngle";
            lblAngle.Size = new Size(126, 41);
            lblAngle.TabIndex = 8;
            lblAngle.Text = "Angle: -";
            // 
            // lblThrottle
            // 
            lblThrottle.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblThrottle.AutoSize = true;
            lblThrottle.Font = new Font("맑은 고딕", 11F);
            lblThrottle.Location = new Point(638, 877);
            lblThrottle.Margin = new Padding(5, 0, 5, 0);
            lblThrottle.Name = "lblThrottle";
            lblThrottle.Size = new Size(153, 41);
            lblThrottle.TabIndex = 9;
            lblThrottle.Text = "Throttle: -";
            // 
            // lblMode
            // 
            lblMode.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblMode.AutoSize = true;
            lblMode.Font = new Font("맑은 고딕", 11F);
            lblMode.Location = new Point(1011, 877);
            lblMode.Margin = new Padding(5, 0, 5, 0);
            lblMode.Name = "lblMode";
            lblMode.Size = new Size(128, 41);
            lblMode.TabIndex = 10;
            lblMode.Text = "Mode: -";
            // 
            // trbFrame
            // 
            trbFrame.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trbFrame.Location = new Point(31, 936);
            trbFrame.Margin = new Padding(5, 5, 5, 5);
            trbFrame.Maximum = 0;
            trbFrame.Name = "trbFrame";
            trbFrame.Size = new Size(2100, 90);
            trbFrame.TabIndex = 11;
            trbFrame.TickStyle = TickStyle.None;
            // 
            // tabCleaner
            // 
            tabCleaner.BackColor = Color.WhiteSmoke;
            tabCleaner.Controls.Add(lblTitleCleaner);
            tabCleaner.Controls.Add(grpFilters);
            tabCleaner.Controls.Add(btnApplyFilter);
            tabCleaner.Controls.Add(btnClearFilter);
            tabCleaner.Controls.Add(btnDeleteFrame);
            tabCleaner.Controls.Add(lstCleanerFrames);
            tabCleaner.Controls.Add(picCleanerPreview);
            tabCleaner.Controls.Add(lblCleanerInfo);
            tabCleaner.Location = new Point(8, 51);
            tabCleaner.Margin = new Padding(5, 5, 5, 5);
            tabCleaner.Name = "tabCleaner";
            tabCleaner.Padding = new Padding(5, 5, 5, 5);
            tabCleaner.Size = new Size(2162, 1093);
            tabCleaner.TabIndex = 1;
            tabCleaner.Text = "Cleaner - 데이터 정리";
            // 
            // lblTitleCleaner
            // 
            lblTitleCleaner.AutoSize = true;
            lblTitleCleaner.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitleCleaner.ForeColor = Color.FromArgb(180, 70, 70);
            lblTitleCleaner.Location = new Point(31, 29);
            lblTitleCleaner.Margin = new Padding(5, 0, 5, 0);
            lblTitleCleaner.Name = "lblTitleCleaner";
            lblTitleCleaner.Size = new Size(370, 78);
            lblTitleCleaner.TabIndex = 0;
            lblTitleCleaner.Text = "Tub Cleaner";
            // 
            // grpFilters
            // 
            grpFilters.Controls.Add(chkThrottlePositive);
            grpFilters.Controls.Add(chkExcludeZeroAngle);
            grpFilters.Controls.Add(chkStopDataOnly);
            grpFilters.Location = new Point(31, 144);
            grpFilters.Margin = new Padding(5, 5, 5, 5);
            grpFilters.Name = "grpFilters";
            grpFilters.Padding = new Padding(5, 5, 5, 5);
            grpFilters.Size = new Size(529, 272);
            grpFilters.TabIndex = 1;
            grpFilters.TabStop = false;
            grpFilters.Text = "필터 조건";
            // 
            // chkThrottlePositive
            // 
            chkThrottlePositive.AutoSize = true;
            chkThrottlePositive.Location = new Point(31, 56);
            chkThrottlePositive.Margin = new Padding(5, 5, 5, 5);
            chkThrottlePositive.Name = "chkThrottlePositive";
            chkThrottlePositive.Size = new Size(281, 41);
            chkThrottlePositive.TabIndex = 0;
            chkThrottlePositive.Text = "throttle > 0만 보기";
            chkThrottlePositive.UseVisualStyleBackColor = true;
            // 
            // chkExcludeZeroAngle
            // 
            chkExcludeZeroAngle.AutoSize = true;
            chkExcludeZeroAngle.Location = new Point(31, 120);
            chkExcludeZeroAngle.Margin = new Padding(5, 5, 5, 5);
            chkExcludeZeroAngle.Name = "chkExcludeZeroAngle";
            chkExcludeZeroAngle.Size = new Size(250, 41);
            chkExcludeZeroAngle.TabIndex = 1;
            chkExcludeZeroAngle.Text = "angle == 0 제외";
            chkExcludeZeroAngle.UseVisualStyleBackColor = true;
            // 
            // chkStopDataOnly
            // 
            chkStopDataOnly.AutoSize = true;
            chkStopDataOnly.Location = new Point(31, 184);
            chkStopDataOnly.Margin = new Padding(5, 5, 5, 5);
            chkStopDataOnly.Name = "chkStopDataOnly";
            chkStopDataOnly.Size = new Size(460, 41);
            chkStopDataOnly.TabIndex = 2;
            chkStopDataOnly.Text = "정지 데이터만 보기(throttle == 0)";
            chkStopDataOnly.UseVisualStyleBackColor = true;
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.Location = new Point(607, 176);
            btnApplyFilter.Margin = new Padding(5, 5, 5, 5);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(202, 72);
            btnApplyFilter.TabIndex = 2;
            btnApplyFilter.Text = "필터 적용";
            btnApplyFilter.UseVisualStyleBackColor = true;
            // 
            // btnClearFilter
            // 
            btnClearFilter.Location = new Point(607, 280);
            btnClearFilter.Margin = new Padding(5, 5, 5, 5);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(202, 72);
            btnClearFilter.TabIndex = 3;
            btnClearFilter.Text = "전체 보기";
            btnClearFilter.UseVisualStyleBackColor = true;
            // 
            // btnDeleteFrame
            // 
            btnDeleteFrame.BackColor = Color.LightCoral;
            btnDeleteFrame.FlatStyle = FlatStyle.Flat;
            btnDeleteFrame.Location = new Point(856, 176);
            btnDeleteFrame.Margin = new Padding(5, 5, 5, 5);
            btnDeleteFrame.Name = "btnDeleteFrame";
            btnDeleteFrame.Size = new Size(264, 176);
            btnDeleteFrame.TabIndex = 4;
            btnDeleteFrame.Text = "선택 프레임 삭제";
            btnDeleteFrame.UseVisualStyleBackColor = false;
            // 
            // lstCleanerFrames
            // 
            lstCleanerFrames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lstCleanerFrames.Font = new Font("Consolas", 9F);
            lstCleanerFrames.HorizontalScrollbar = true;
            lstCleanerFrames.Location = new Point(31, 456);
            lstCleanerFrames.Margin = new Padding(5, 5, 5, 5);
            lstCleanerFrames.Name = "lstCleanerFrames";
            lstCleanerFrames.Size = new Size(962, 536);
            lstCleanerFrames.TabIndex = 5;
            // 
            // picCleanerPreview
            // 
            picCleanerPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picCleanerPreview.BackColor = Color.Black;
            picCleanerPreview.BorderStyle = BorderStyle.FixedSingle;
            picCleanerPreview.Location = new Point(1042, 456);
            picCleanerPreview.Margin = new Padding(5, 5, 5, 5);
            picCleanerPreview.Name = "picCleanerPreview";
            picCleanerPreview.Size = new Size(1088, 479);
            picCleanerPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picCleanerPreview.TabIndex = 6;
            picCleanerPreview.TabStop = false;
            // 
            // lblCleanerInfo
            // 
            lblCleanerInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblCleanerInfo.AutoSize = true;
            lblCleanerInfo.Location = new Point(1042, 960);
            lblCleanerInfo.Margin = new Padding(5, 0, 5, 0);
            lblCleanerInfo.Name = "lblCleanerInfo";
            lblCleanerInfo.Size = new Size(250, 37);
            lblCleanerInfo.TabIndex = 7;
            lblCleanerInfo.Text = "선택 프레임 정보: -";
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
            tabTrainer.Margin = new Padding(5, 5, 5, 5);
            tabTrainer.Name = "tabTrainer";
            tabTrainer.Padding = new Padding(5, 5, 5, 5);
            tabTrainer.Size = new Size(2162, 1093);
            tabTrainer.TabIndex = 2;
            tabTrainer.Text = "Trainer - 학습 실행";
            // 
            // lblTitleTrainer
            // 
            lblTitleTrainer.AutoSize = true;
            lblTitleTrainer.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitleTrainer.ForeColor = Color.FromArgb(60, 130, 80);
            lblTitleTrainer.Location = new Point(31, 29);
            lblTitleTrainer.Margin = new Padding(5, 0, 5, 0);
            lblTitleTrainer.Name = "lblTitleTrainer";
            lblTitleTrainer.Size = new Size(548, 78);
            lblTitleTrainer.TabIndex = 0;
            lblTitleTrainer.Text = "Donkeycar Trainer";
            // 
            // lblMycarPath
            // 
            lblMycarPath.AutoSize = true;
            lblMycarPath.Location = new Point(47, 160);
            lblMycarPath.Margin = new Padding(5, 0, 5, 0);
            lblMycarPath.Name = "lblMycarPath";
            lblMycarPath.Size = new Size(154, 37);
            lblMycarPath.TabIndex = 1;
            lblMycarPath.Text = "mycar 경로";
            // 
            // txtMycarPath
            // 
            txtMycarPath.Location = new Point(233, 154);
            txtMycarPath.Margin = new Padding(5, 5, 5, 5);
            txtMycarPath.Name = "txtMycarPath";
            txtMycarPath.Size = new Size(1118, 43);
            txtMycarPath.TabIndex = 2;
            // 
            // btnBrowseMycar
            // 
            btnBrowseMycar.Location = new Point(1384, 152);
            btnBrowseMycar.Margin = new Padding(5, 5, 5, 5);
            btnBrowseMycar.Name = "btnBrowseMycar";
            btnBrowseMycar.Size = new Size(140, 54);
            btnBrowseMycar.TabIndex = 3;
            btnBrowseMycar.Text = "찾기";
            btnBrowseMycar.UseVisualStyleBackColor = true;
            // 
            // lblPythonExe
            // 
            lblPythonExe.AutoSize = true;
            lblPythonExe.Location = new Point(47, 248);
            lblPythonExe.Margin = new Padding(5, 0, 5, 0);
            lblPythonExe.Name = "lblPythonExe";
            lblPythonExe.Size = new Size(192, 37);
            lblPythonExe.TabIndex = 4;
            lblPythonExe.Text = "Python 실행명";
            // 
            // txtPythonExe
            // 
            txtPythonExe.Location = new Point(233, 242);
            txtPythonExe.Margin = new Padding(5, 5, 5, 5);
            txtPythonExe.Name = "txtPythonExe";
            txtPythonExe.Size = new Size(464, 43);
            txtPythonExe.TabIndex = 5;
            // 
            // lblTrainArgs
            // 
            lblTrainArgs.AutoSize = true;
            lblTrainArgs.Location = new Point(47, 336);
            lblTrainArgs.Margin = new Padding(5, 0, 5, 0);
            lblTrainArgs.Name = "lblTrainArgs";
            lblTrainArgs.Size = new Size(197, 37);
            lblTrainArgs.TabIndex = 6;
            lblTrainArgs.Text = "학습 명령 인자";
            // 
            // txtTrainArgs
            // 
            txtTrainArgs.Location = new Point(233, 330);
            txtTrainArgs.Margin = new Padding(5, 5, 5, 5);
            txtTrainArgs.Name = "txtTrainArgs";
            txtTrainArgs.Size = new Size(1289, 43);
            txtTrainArgs.TabIndex = 7;
            // 
            // btnTrain
            // 
            btnTrain.BackColor = Color.FromArgb(76, 175, 80);
            btnTrain.FlatStyle = FlatStyle.Flat;
            btnTrain.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            btnTrain.ForeColor = Color.White;
            btnTrain.Location = new Point(233, 432);
            btnTrain.Margin = new Padding(5, 5, 5, 5);
            btnTrain.Name = "btnTrain";
            btnTrain.Size = new Size(249, 80);
            btnTrain.TabIndex = 8;
            btnTrain.Text = "학습 시작";
            btnTrain.UseVisualStyleBackColor = false;
            // 
            // btnStopTrain
            // 
            btnStopTrain.BackColor = Color.LightCoral;
            btnStopTrain.FlatStyle = FlatStyle.Flat;
            btnStopTrain.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            btnStopTrain.Location = new Point(513, 432);
            btnStopTrain.Margin = new Padding(5, 5, 5, 5);
            btnStopTrain.Name = "btnStopTrain";
            btnStopTrain.Size = new Size(249, 80);
            btnStopTrain.TabIndex = 9;
            btnStopTrain.Text = "학습 중지";
            btnStopTrain.UseVisualStyleBackColor = false;
            // 
            // lblModelStatus
            // 
            lblModelStatus.AutoSize = true;
            lblModelStatus.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblModelStatus.Location = new Point(233, 552);
            lblModelStatus.Margin = new Padding(5, 0, 5, 0);
            lblModelStatus.Name = "lblModelStatus";
            lblModelStatus.Size = new Size(180, 41);
            lblModelStatus.TabIndex = 10;
            lblModelStatus.Text = "모델 상태: -";
            // 
            // lblTrainInfo
            // 
            lblTrainInfo.AutoSize = true;
            lblTrainInfo.ForeColor = Color.DimGray;
            lblTrainInfo.Location = new Point(233, 632);
            lblTrainInfo.Margin = new Padding(5, 0, 5, 0);
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
            tabPilotTest.Controls.Add(picPilotTest);
            tabPilotTest.Controls.Add(lblActualAngle);
            tabPilotTest.Controls.Add(lblPredictedAngle);
            tabPilotTest.Controls.Add(lblActualThrottle);
            tabPilotTest.Controls.Add(lblPredictedThrottle);
            tabPilotTest.Controls.Add(lblAngleError);
            tabPilotTest.Controls.Add(lblPilotWarning);
            tabPilotTest.Controls.Add(lblPilotNote);
            tabPilotTest.Location = new Point(8, 51);
            tabPilotTest.Margin = new Padding(5, 5, 5, 5);
            tabPilotTest.Name = "tabPilotTest";
            tabPilotTest.Padding = new Padding(5, 5, 5, 5);
            tabPilotTest.Size = new Size(2162, 1093);
            tabPilotTest.TabIndex = 3;
            tabPilotTest.Text = "Pilot Test - 모델 테스트";
            // 
            // lblTitlePilot
            // 
            lblTitlePilot.AutoSize = true;
            lblTitlePilot.Font = new Font("맑은 고딕", 22F, FontStyle.Bold);
            lblTitlePilot.ForeColor = Color.FromArgb(90, 90, 160);
            lblTitlePilot.Location = new Point(31, 29);
            lblTitlePilot.Margin = new Padding(5, 0, 5, 0);
            lblTitlePilot.Name = "lblTitlePilot";
            lblTitlePilot.Size = new Size(731, 78);
            lblTitlePilot.TabIndex = 0;
            lblTitlePilot.Text = "Pilot Arena / Model Test";
            // 
            // lblModelPath
            // 
            lblModelPath.AutoSize = true;
            lblModelPath.Location = new Point(47, 160);
            lblModelPath.Margin = new Padding(5, 0, 5, 0);
            lblModelPath.Name = "lblModelPath";
            lblModelPath.Size = new Size(134, 37);
            lblModelPath.TabIndex = 1;
            lblModelPath.Text = "모델 파일";
            // 
            // txtModelPath
            // 
            txtModelPath.Location = new Point(187, 154);
            txtModelPath.Margin = new Padding(5, 5, 5, 5);
            txtModelPath.Name = "txtModelPath";
            txtModelPath.Size = new Size(1149, 43);
            txtModelPath.TabIndex = 2;
            // 
            // btnBrowseModel
            // 
            btnBrowseModel.Location = new Point(1369, 152);
            btnBrowseModel.Margin = new Padding(5, 5, 5, 5);
            btnBrowseModel.Name = "btnBrowseModel";
            btnBrowseModel.Size = new Size(140, 54);
            btnBrowseModel.TabIndex = 3;
            btnBrowseModel.Text = "찾기";
            btnBrowseModel.UseVisualStyleBackColor = true;
            // 
            // btnRunPilotTest
            // 
            btnRunPilotTest.Location = new Point(187, 232);
            btnRunPilotTest.Margin = new Padding(5, 5, 5, 5);
            btnRunPilotTest.Name = "btnRunPilotTest";
            btnRunPilotTest.Size = new Size(389, 67);
            btnRunPilotTest.TabIndex = 4;
            btnRunPilotTest.Text = "현재 이미지로 예측 테스트";
            btnRunPilotTest.UseVisualStyleBackColor = true;
            // 
            // picPilotTest
            // 
            picPilotTest.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            picPilotTest.BackColor = Color.Black;
            picPilotTest.BorderStyle = BorderStyle.FixedSingle;
            picPilotTest.Location = new Point(47, 352);
            picPilotTest.Margin = new Padding(5, 5, 5, 5);
            picPilotTest.Name = "picPilotTest";
            picPilotTest.Size = new Size(1010, 575);
            picPilotTest.SizeMode = PictureBoxSizeMode.Zoom;
            picPilotTest.TabIndex = 5;
            picPilotTest.TabStop = false;
            // 
            // lblActualAngle
            // 
            lblActualAngle.AutoSize = true;
            lblActualAngle.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblActualAngle.Location = new Point(1104, 368);
            lblActualAngle.Margin = new Padding(5, 0, 5, 0);
            lblActualAngle.Name = "lblActualAngle";
            lblActualAngle.Size = new Size(216, 45);
            lblActualAngle.TabIndex = 6;
            lblActualAngle.Text = "실제 Angle: -";
            // 
            // lblPredictedAngle
            // 
            lblPredictedAngle.AutoSize = true;
            lblPredictedAngle.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblPredictedAngle.Location = new Point(1104, 432);
            lblPredictedAngle.Margin = new Padding(5, 0, 5, 0);
            lblPredictedAngle.Name = "lblPredictedAngle";
            lblPredictedAngle.Size = new Size(216, 45);
            lblPredictedAngle.TabIndex = 7;
            lblPredictedAngle.Text = "예측 Angle: -";
            // 
            // lblActualThrottle
            // 
            lblActualThrottle.AutoSize = true;
            lblActualThrottle.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblActualThrottle.Location = new Point(1104, 512);
            lblActualThrottle.Margin = new Padding(5, 0, 5, 0);
            lblActualThrottle.Name = "lblActualThrottle";
            lblActualThrottle.Size = new Size(234, 41);
            lblActualThrottle.TabIndex = 8;
            lblActualThrottle.Text = "실제 Throttle: -";
            // 
            // lblPredictedThrottle
            // 
            lblPredictedThrottle.AutoSize = true;
            lblPredictedThrottle.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblPredictedThrottle.Location = new Point(1104, 568);
            lblPredictedThrottle.Margin = new Padding(5, 0, 5, 0);
            lblPredictedThrottle.Name = "lblPredictedThrottle";
            lblPredictedThrottle.Size = new Size(234, 41);
            lblPredictedThrottle.TabIndex = 9;
            lblPredictedThrottle.Text = "예측 Throttle: -";
            // 
            // lblAngleError
            // 
            lblAngleError.AutoSize = true;
            lblAngleError.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblAngleError.Location = new Point(1104, 648);
            lblAngleError.Margin = new Padding(5, 0, 5, 0);
            lblAngleError.Name = "lblAngleError";
            lblAngleError.Size = new Size(228, 45);
            lblAngleError.TabIndex = 10;
            lblAngleError.Text = "Angle Error: -";
            // 
            // lblPilotWarning
            // 
            lblPilotWarning.AutoSize = true;
            lblPilotWarning.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblPilotWarning.ForeColor = Color.DimGray;
            lblPilotWarning.Location = new Point(1104, 720);
            lblPilotWarning.Margin = new Padding(5, 0, 5, 0);
            lblPilotWarning.Name = "lblPilotWarning";
            lblPilotWarning.Size = new Size(116, 45);
            lblPilotWarning.TabIndex = 11;
            lblPilotWarning.Text = "판정: -";
            // 
            // lblPilotNote
            // 
            lblPilotNote.ForeColor = Color.DimGray;
            lblPilotNote.Location = new Point(1104, 800);
            lblPilotNote.Margin = new Padding(5, 0, 5, 0);
            lblPilotNote.Name = "lblPilotNote";
            lblPilotNote.Size = new Size(871, 224);
            lblPilotNote.TabIndex = 12;
            lblPilotNote.Text = "파란선: 실제 angle\n초록선: 예측 angle\n노란 반투명 영역: 실제/예측 차이\n하단 막대: 실제/예측 throttle 비교\n오차가 클수록 경고 색상이 빨간색으로 표시됩니다.";
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Font = new Font("Consolas", 9F);
            txtLog.Location = new Point(0, 1160);
            txtLog.Margin = new Padding(5, 5, 5, 5);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(2176, 278);
            txtLog.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(14F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(2178, 1440);
            Controls.Add(tabMain);
            Controls.Add(txtLog);
            Margin = new Padding(5, 5, 5, 5);
            MinimumSize = new Size(1977, 1269);
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
            grpFilters.ResumeLayout(false);
            grpFilters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picCleanerPreview).EndInit();
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
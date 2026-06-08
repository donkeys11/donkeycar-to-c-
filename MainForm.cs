using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
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
        private List<int> cleanerPlayFrames = new();
        private int cleanerPlayIndex = 0;

        private int currentIndex = -1;
        private bool isUpdatingSelection = false;

        private Process? trainProcess;
        private bool trainEndRequested = false;
        private bool isTrainPaused = false;
        private bool currentTrainUsesWsl = false;
        private string currentTrainVersionModelPath = "";
        private string currentRepresentativeModelPath = "./models/mypilot.h5";
        private string currentTrainEpochText = "";
        private int currentWslTrainProcessGroupId = -1;
        private string currentWslTrainPidFilePath = "";
        private bool trainPauseSignalPending = false;
        private readonly List<double> trainLossPoints = new List<double>();
        private readonly List<double> valLossPoints = new List<double>();
        private readonly SemaphoreSlim predictServerSemaphore = new SemaphoreSlim(1, 1);
        private Process? predictServerProcess;
        private string predictServerModelPath = "";
        private bool livePredictInFlight = false;
        private bool livePredictPending = false;
        private int livePredictVersion = 0;
        private int livePredictFailureCount = 0;
        private Label? lblTrainSourceModel;
        private ComboBox? cmbTrainSourceModel;
        private Button? btnScanTrainSourceModels;
        private Button? btnBrowseTrainSourceModel;
        private Label? lblLossGraphTitle;
        private Label? lblLossGraphInfo;
        private Panel? pnlLossGraph;
        private bool trainingExtensionControlsLocked = false;
        private Panel? pnlLogSplitter;
        private int logPanelHeight = LogPanelHeight;
        private bool isDraggingLogSplitter = false;
        private int logSplitterDragStartY = 0;
        private int logSplitterDragStartHeight = 0;
        private bool isAutoRangeSelecting = false;

        private readonly System.Windows.Forms.Timer cleanerRangePlayTimer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer cleanerAutoPlayTimer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer pilotAutoPlayTimer = new System.Windows.Forms.Timer();

        private readonly Dictionary<string, Bitmap> cleanerTimelineThumbCache = new Dictionary<string, Bitmap>();

        private const string WslDistroName = "Ubuntu-22.04";
        private const string CondaEnvName = "e2e_env";
        private const int CatalogChunkSize = 1000;
        private const string WslTrainPidMarker = "__DONKEYCAR_TRAIN_PGID__=";
        private const int LogPanelHeight = 120;
        private const int LogPanelGap = 6;

        private const int CleanerTimelineThumbWidth = 82;
        private const int CleanerTimelineThumbGap = 4;

        private int cleanerTimelineStartIndex = 0;

        private double? overlayActualAngle = null;
        private double? overlayPredictedAngle = null;
        private double? overlayActualThrottle = null;
        private double? overlayPredictedThrottle = null;

        private List<(int Start, int End)> cleanerRanges = new();

        private int pendingRangeStart = -1;
        private int cleanerRangePlayIndex = -1;
        private bool isDraggingCleanerRange = false;
        private List<string> backupFolderPaths = new List<string>();
        private List<int> markedFrameIndices = new List<int>();

        private sealed class MismatchScanResult
        {
            public string[] CatalogFiles { get; set; } = Array.Empty<string>();
            public List<CatalogMismatchEntry> CatalogEntriesWithoutImages { get; } = new();
            public List<string> ImagesWithoutCatalog { get; } = new();
            public int CatalogLineCount { get; set; }
            public int ParseErrorCount { get; set; }
            public bool OrphanImageScanSkipped { get; set; }
            public bool HasMismatch =>
                CatalogEntriesWithoutImages.Count > 0 || ImagesWithoutCatalog.Count > 0;
        }

        private sealed class CatalogMismatchEntry
        {
            public string CatalogPath { get; set; } = "";
            public int LineNumber { get; set; }
            public string ImageReference { get; set; } = "";
            public string ImageFileName { get; set; } = "";
        }

        private sealed class TrainModelChoice
        {
            public string DisplayName { get; set; } = "";
            public string ModelPath { get; set; } = "";

            public override string ToString()
            {
                return DisplayName;
            }
        }

        [System.Runtime.InteropServices.DllImport("ntdll.dll")]
        private static extern int NtSuspendProcess(IntPtr processHandle);

        [System.Runtime.InteropServices.DllImport("ntdll.dll")]
        private static extern int NtResumeProcess(IntPtr processHandle);

        public MainForm()
        {
            InitializeComponent();
            InitializeTrainingExtensions();
            InitializeLogSplitter();
            ConnectEvents();


            tabMain.AllowDrop = true;
            tabMain.DragEnter += MainForm_DragEnter;
            tabMain.DragDrop += MainForm_DragDrop;

            txtMycarPath.Text = "~/mycar";
            txtPythonExe.Text = "wsl";
            txtModelPath.Text = currentRepresentativeModelPath;
            UpdateTrainArgsPreview();

            cleanerRangePlayTimer.Interval = 120;
            cleanerRangePlayTimer.Tick += CleanerRangePlayTimer_Tick;

            cleanerAutoPlayTimer.Interval = 120;
            cleanerAutoPlayTimer.Tick += CleanerAutoPlayTimer_Tick;

            pilotAutoPlayTimer.Interval = 120;
            pilotAutoPlayTimer.Tick += PilotAutoPlayTimer_Tick;

            picPilotTest.Paint += picPilotTest_Paint;
            picPilotTest.Resize += (s, e) => picPilotTest.Invalidate();
            Resize += (s, e) => LayoutMainSections();
            tabCleaner.Resize += (s, e) => LayoutCleanerControls();
            tabTrainer.Resize += (s, e) => LayoutTrainerControls();
            tabPilotTest.Resize += (s, e) => LayoutPilotTestControls();
            LayoutMainSections();
            LayoutCleanerControls();
            LayoutTrainerControls();
            LayoutPilotTestControls();

            // 폴더 아이콘
            btnOpenDataFolder.Text = "📁";
            btnOpenDataFolder.Font = new Font("Segoe UI Emoji", btnOpenDataFolder.Height / 5f);
            btnOpenDataFolder.TextAlign = ContentAlignment.MiddleCenter;

            // 텍스트 없는 클리어 버튼
            btnClearDataPath.Text = "";
            btnClearDataPath.Paint += BtnClearDataPath_Paint;

            btnPlayRange.Text = "";
            btnPlayRange.Paint += BtnPlayRange_Paint;


            btnDeleteFrame.Paint += BtnDelete_Paint;


            btnDeleteRange.Paint += BtnDelete_Paint;



        }

        private void InitializeTrainingExtensions()
        {
            lblTrainInfo.AutoSize = false;

            lblTrainSourceModel = new Label
            {
                AutoSize = true,
                Text = "대표 모델"
            };

            cmbTrainSourceModel = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTrainSourceModel.SelectedIndexChanged += (s, e) => UpdateRepresentativeModelSelection();

            btnScanTrainSourceModels = new Button
            {
                Text = "모델 스캔",
                UseVisualStyleBackColor = true
            };
            btnScanTrainSourceModels.Click += btnScanTrainSourceModels_Click;

            btnBrowseTrainSourceModel = new Button
            {
                Text = "파일 선택",
                UseVisualStyleBackColor = true
            };
            btnBrowseTrainSourceModel.Click += btnBrowseTrainSourceModel_Click;

            lblLossGraphTitle = new Label
            {
                AutoSize = true,
                Font = new Font("맑은 고딕", 11F, FontStyle.Bold),
                Text = "Loss 그래프"
            };

            lblLossGraphInfo = new Label
            {
                ForeColor = Color.DimGray,
                Text = "loss: - / val_loss: -"
            };

            pnlLossGraph = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlLossGraph.Paint += pnlLossGraph_Paint;

            lblTrainArgs.Text = "학습 명령";
            txtTrainArgs.ReadOnly = true;
            lblTrainInfo.Text = "";

            tabTrainer.Controls.Add(lblTrainSourceModel);
            tabTrainer.Controls.Add(cmbTrainSourceModel);
            tabTrainer.Controls.Add(btnScanTrainSourceModels);
            tabTrainer.Controls.Add(btnBrowseTrainSourceModel);
            tabTrainer.Controls.Add(lblLossGraphTitle);
            tabTrainer.Controls.Add(lblLossGraphInfo);
            tabTrainer.Controls.Add(pnlLossGraph);

            ResetTrainSourceModelChoices();
            UpdateTrainSourceModelControls();
        }

        private void InitializeLogSplitter()
        {
            pnlLogSplitter = new Panel
            {
                BackColor = Color.FromArgb(210, 215, 222),
                Cursor = Cursors.HSplit
            };

            pnlLogSplitter.MouseDown += pnlLogSplitter_MouseDown;
            pnlLogSplitter.MouseMove += pnlLogSplitter_MouseMove;
            pnlLogSplitter.MouseUp += pnlLogSplitter_MouseUp;

            Controls.Add(pnlLogSplitter);
            pnlLogSplitter.BringToFront();
        }

        private void LayoutTrainerControls()
        {
            if (tabTrainer == null
                || txtMycarPath == null
                || txtPythonExe == null
                || txtTrainArgs == null
                || btnTrain == null
                || btnStopTrain == null
                || btnEndTrain == null
                || lblTrainSourceModel == null
                || cmbTrainSourceModel == null
                || btnScanTrainSourceModels == null
                || btnBrowseTrainSourceModel == null
                || lblLossGraphTitle == null
                || lblLossGraphInfo == null
                || pnlLossGraph == null)
            {
                return;
            }

            int margin = 28;
            int labelWidth = Math.Max(
                210,
                new[]
                {
                    lblMycarPath.PreferredSize.Width,
                    lblPythonExe.PreferredSize.Width,
                    lblTrainSourceModel.PreferredSize.Width,
                    lblTrainArgs.PreferredSize.Width
                }.Max()
            );
            int rowHeight = 36;
            int rowGap = 52;
            int tabWidth = Math.Max(tabTrainer.ClientSize.Width, 900);
            int tabHeight = Math.Max(tabTrainer.ClientSize.Height, 520);
            int graphX = Math.Max(720, (int)(tabWidth * 0.57));
            int graphWidth = Math.Max(380, tabWidth - graphX - margin);
            int fieldX = margin + labelWidth + 14;
            int fieldWidth = Math.Max(420, graphX - fieldX - 34);
            int y = 24;

            lblTitleTrainer.Location = new Point(margin, y);
            y = lblTitleTrainer.Top + lblTitleTrainer.PreferredSize.Height + 22;

            txtMycarPath.Font = new Font("맑은 고딕", 11F);
            txtPythonExe.Font = new Font("맑은 고딕", 11F);
            txtTrainArgs.Font = new Font("맑은 고딕", 10.5F);
            cmbTrainSourceModel.Font = new Font("맑은 고딕", 10.5F);
            lblLossGraphTitle.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblLossGraphInfo.Font = new Font("맑은 고딕", 10F);

            lblMycarPath.Location = new Point(margin, y + 7);
            txtMycarPath.SetBounds(fieldX, y, Math.Max(260, fieldWidth - 126), rowHeight);
            btnBrowseMycar.SetBounds(fieldX + fieldWidth - 116, y, 116, rowHeight);
            y += rowGap;

            lblPythonExe.Location = new Point(margin, y + 7);
            txtPythonExe.SetBounds(fieldX, y, 220, rowHeight);
            y += rowGap;

            lblTrainSourceModel.Location = new Point(margin, y + 7);
            cmbTrainSourceModel.SetBounds(fieldX, y, Math.Max(260, fieldWidth - 244), rowHeight);
            btnScanTrainSourceModels.SetBounds(fieldX + fieldWidth - 236, y, 112, rowHeight);
            btnBrowseTrainSourceModel.SetBounds(fieldX + fieldWidth - 116, y, 116, rowHeight);
            y += rowGap;

            lblTrainArgs.Location = new Point(margin, y + 7);
            txtTrainArgs.SetBounds(fieldX, y, fieldWidth, rowHeight);
            y += rowGap + 12;

            btnTrain.SetBounds(fieldX, y, 164, 50);
            btnStopTrain.SetBounds(fieldX + 184, y, 164, 50);
            btnEndTrain.SetBounds(fieldX + 368, y, 164, 50);
            y += 72;

            lblTrainProgress.Location = new Point(fieldX, y);
            y += 32;

            prgTrainProgress.SetBounds(fieldX, y, fieldWidth, 24);
            y += 54;

            lblModelStatus.Location = new Point(fieldX, y);
            y += 46;

            lblTrainInfo.SetBounds(fieldX, y, fieldWidth, Math.Max(70, tabHeight - y - margin));

            lblLossGraphTitle.Location = new Point(graphX, 28);
            lblLossGraphInfo.SetBounds(graphX, 58, graphWidth, 34);
            pnlLossGraph.SetBounds(graphX, 98, graphWidth, Math.Max(300, tabHeight - 122));
        }

        private void UpdateTrainSourceModelControls()
        {
            bool canEdit = !trainingExtensionControlsLocked && (trainProcess == null || trainProcess.HasExited);

            if (lblTrainSourceModel != null)
                lblTrainSourceModel.Enabled = canEdit;

            if (cmbTrainSourceModel != null)
                cmbTrainSourceModel.Enabled = canEdit;

            if (btnScanTrainSourceModels != null)
                btnScanTrainSourceModels.Enabled = canEdit;

            if (btnBrowseTrainSourceModel != null)
                btnBrowseTrainSourceModel.Enabled = canEdit;
        }

        private void LayoutCleanerControls()
        {
            if (tabCleaner == null
                || lblTitleCleaner == null
                || txtDataPath == null
                || btnOpenDataFolder == null
                || btnClearDataPath == null
                || btnUndo == null
                || picCleanerPreview == null
                || grpFilters == null
                || btnApplyFilter == null
                || btnClearFilter == null
                || btnCleanMismatch == null
                || btnDeleteFrame == null
                || lstCleanerFrames == null
                || lblImageAdjust == null
                || chkGrayscale == null
                || chkFlipHorizontal == null
                || lblBrightness == null
                || trbBrightness == null
                || lblContrast == null
                || trbContrast == null
                || btnSaveProcessed == null
                || grpCleanerRangeEditor == null
                || pnlCleanerTimeline == null
                || hsbCleanerTimeline == null
                || lblCleanerTimelineScrollInfo == null
                || lblCleanerRangeInfo == null
                || lblCleanerRangeHint == null
                || btnDeleteRange == null
                || btnPlayRange == null
                || btnClearRange == null
                || btnCleanerAutoPlay == null
                || btnCleanerMark == null)
            {
                return;
            }

            int margin = 28;
            int gap = 20;
            int tabWidth = Math.Max(tabCleaner.ClientSize.Width, 980);
            int tabHeight = Math.Max(tabCleaner.ClientSize.Height, 620);
            int sideWidth = Math.Max(300, Math.Min(420, (int)(tabWidth * 0.18)));
            int sideX = tabWidth - margin - sideWidth;
            int mainX = margin;
            int mainWidth = Math.Max(520, sideX - gap - mainX);

            lblTitleCleaner.Location = new Point(mainX, 24);

            int titleRight = lblTitleCleaner.Left + lblTitleCleaner.PreferredSize.Width;
            int titleBottom = lblTitleCleaner.Top + lblTitleCleaner.PreferredSize.Height;
            int pathY = 42;
            int pathX = titleRight + 42;
            int iconSize = 38;
            int pathWidth = Math.Max(260, sideX - pathX - iconSize * 3 - 28);

            if (pathWidth < 360)
            {
                pathY = titleBottom + 14;
                pathX = mainX;
                pathWidth = Math.Max(260, sideX - pathX - iconSize * 3 - 32);
            }

            txtDataPath.SetBounds(pathX, pathY, pathWidth, 34);
            btnOpenDataFolder.SetBounds(pathX + pathWidth + 8, pathY - 1, iconSize, 34);
            btnClearDataPath.SetBounds(pathX + pathWidth + iconSize + 12, pathY - 1, iconSize, 34);
            btnUndo.SetBounds(pathX + pathWidth + iconSize * 2 + 16, pathY - 1, iconSize, 34);

            int rangeHeight = Math.Max(170, Math.Min(230, tabHeight / 5));
            int rangeY = tabHeight - margin - rangeHeight;
            int adjustHeight = 136;
            int topControlsBottom = Math.Max(titleBottom, pathY + 34);
            int infoY = topControlsBottom + 14;
            lblCleanerInfo.Location = new Point(mainX, infoY);

            int previewY = infoY + lblCleanerInfo.PreferredSize.Height + 14;
            int previewHeight = Math.Max(180, rangeY - adjustHeight - gap - previewY);
            picCleanerPreview.SetBounds(mainX, previewY, mainWidth, previewHeight);

            int adjustY = previewY + previewHeight + 14;
            lblImageAdjust.Location = new Point(mainX, adjustY);
            chkGrayscale.SetBounds(mainX, adjustY + 46, 190, 32);
            chkFlipHorizontal.SetBounds(mainX, adjustY + 84, 340, 32);

            int sliderLabelX = mainX + 350;
            int sliderX = sliderLabelX + 84;
            int sliderWidth = Math.Max(260, mainWidth - (sliderX - mainX) - 300);
            lblBrightness.Location = new Point(sliderLabelX, adjustY + 45);
            trbBrightness.SetBounds(sliderX, adjustY + 40, sliderWidth, 34);
            lblContrast.Location = new Point(sliderLabelX, adjustY + 84);
            trbContrast.SetBounds(sliderX, adjustY + 79, sliderWidth, 34);
            btnSaveProcessed.SetBounds(mainX + mainWidth - 230, adjustY + 58, 220, 48);

            grpFilters.SetBounds(sideX, 62, sideWidth, 172);
            chkThrottlePositive.SetBounds(22, 34, sideWidth - 44, 28);
            chkExcludeZeroAngle.SetBounds(22, 66, sideWidth - 44, 28);
            chkExcludeJitterAngle.SetBounds(22, 98, sideWidth - 44, 28);
            chkStopDataOnly.SetBounds(22, 130, sideWidth - 44, 28);

            int sideButtonY = grpFilters.Bottom + 18;
            int sideButtonWidth = sideWidth;
            btnApplyFilter.SetBounds(sideX, sideButtonY, sideButtonWidth, 42);
            btnClearFilter.SetBounds(sideX, sideButtonY + 50, sideButtonWidth, 42);
            btnCleanMismatch.SetBounds(sideX, sideButtonY + 100, sideButtonWidth, 42);
            btnDeleteFrame.SetBounds(sideX, sideButtonY + 150, sideButtonWidth, 42);

            int listY = sideButtonY + 208;
            int listHeight = Math.Max(100, rangeY - listY - gap);
            lstCleanerFrames.SetBounds(sideX, listY, sideWidth, listHeight);

            grpCleanerRangeEditor.SetBounds(mainX, rangeY, tabWidth - margin * 2, rangeHeight);
            LayoutCleanerRangeEditorControls();
        }

        private void LayoutCleanerRangeEditorControls()
        {
            int margin = 18;
            int groupWidth = Math.Max(grpCleanerRangeEditor.ClientSize.Width, 760);
            int groupHeight = Math.Max(grpCleanerRangeEditor.ClientSize.Height, 160);
            int actionWidth = 150;
            int actionGap = 10;
            int actionX = groupWidth - margin - actionWidth * 2 - actionGap;
            int timelineWidth = Math.Max(320, actionX - margin * 2);

            lblCleanerRangeInfo.Location = new Point(margin, 28);
            lblCleanerRangeHint.SetBounds(margin + 220, 28, Math.Max(260, timelineWidth - 240), 28);

            int timelineY = 64;
            int timelineHeight = Math.Max(70, groupHeight - timelineY - 56);
            pnlCleanerTimeline.SetBounds(margin, timelineY, timelineWidth, timelineHeight);
            hsbCleanerTimeline.SetBounds(margin, timelineY + timelineHeight + 6, timelineWidth, 20);
            lblCleanerTimelineScrollInfo.Location = new Point(margin, timelineY + timelineHeight + 32);

            btnDeleteRange.SetBounds(actionX, timelineY, actionWidth, 42);
            btnPlayRange.SetBounds(actionX + actionWidth + actionGap, timelineY, actionWidth, 42);
            btnClearRange.SetBounds(actionX, timelineY + 52, actionWidth, 42);
            btnCleanerAutoPlay.SetBounds(actionX + actionWidth + actionGap, timelineY + 52, actionWidth, 42);
            btnCleanerMark.SetBounds(actionX, timelineY + 104, actionWidth * 2 + actionGap, 42);




        }

        private void SetTrainingExtensionControlsEnabled(bool enabled)
        {
            trainingExtensionControlsLocked = !enabled;

            UpdateTrainSourceModelControls();
        }

        private void ResetTrainSourceModelChoices()
        {
            if (cmbTrainSourceModel == null)
                return;

            cmbTrainSourceModel.Items.Clear();
            cmbTrainSourceModel.Items.Add(new TrainModelChoice
            {
                DisplayName = "./models/mypilot.h5",
                ModelPath = "./models/mypilot.h5"
            });
            cmbTrainSourceModel.SelectedIndex = 0;
        }

        private void UpdateRepresentativeModelSelection()
        {
            string representativeModelPath = GetSelectedRepresentativeModelPath();

            if (string.IsNullOrWhiteSpace(representativeModelPath))
                return;

            currentRepresentativeModelPath = representativeModelPath;
            txtModelPath.Text = representativeModelPath;
            UpdateTrainArgsPreview();
            UpdateModelStatus();
        }

        private void UpdateTrainArgsPreview()
        {
            string representativeModelPath = GetSelectedRepresentativeModelPath();

            if (string.IsNullOrWhiteSpace(representativeModelPath))
                representativeModelPath = "./models/mypilot.h5";

            string outputPreviewPath = BuildSubModelPath(representativeModelPath, "YYYYMMDD_HHMMSS");
            txtTrainArgs.Text =
                $"train_with_transfer.py --tubs=./data --model={QuoteCommandTokenForDisplay(outputPreviewPath)} " +
                $"--transfer={QuoteCommandTokenForDisplay(representativeModelPath)}";
        }

        private void LayoutMainSections()
        {
            if (tabMain == null || txtLog == null || pnlLogSplitter == null)
                return;

            int splitterHeight = 8;
            int logHeight = ClampLogPanelHeight(logPanelHeight);
            int tabHeight = ClientSize.Height - logHeight - splitterHeight;

            if (tabHeight < 360)
            {
                tabHeight = Math.Max(240, ClientSize.Height - 90 - splitterHeight);
                logHeight = Math.Max(60, ClientSize.Height - tabHeight - splitterHeight);
                logPanelHeight = logHeight;
            }

            tabMain.SetBounds(0, 0, ClientSize.Width, tabHeight);
            pnlLogSplitter.SetBounds(0, tabHeight, ClientSize.Width, splitterHeight);
            txtLog.SetBounds(0, tabHeight + splitterHeight, ClientSize.Width, logHeight);
            pnlLogSplitter.BringToFront();
            txtLog.BringToFront();
        }

        private int ClampLogPanelHeight(int requestedHeight)
        {
            int minHeight = 64;
            int maxHeight = Math.Max(minHeight, ClientSize.Height - 360);
            return Math.Max(minHeight, Math.Min(maxHeight, requestedHeight));
        }

        private void pnlLogSplitter_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            isDraggingLogSplitter = true;
            logSplitterDragStartY = PointToClient(Cursor.Position).Y;
            logSplitterDragStartHeight = logPanelHeight;

            if (pnlLogSplitter != null)
                pnlLogSplitter.Capture = true;
        }

        private void pnlLogSplitter_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!isDraggingLogSplitter)
                return;

            int currentY = PointToClient(Cursor.Position).Y;
            int delta = logSplitterDragStartY - currentY;
            logPanelHeight = ClampLogPanelHeight(logSplitterDragStartHeight + delta);
            LayoutMainSections();
        }

        private void pnlLogSplitter_MouseUp(object? sender, MouseEventArgs e)
        {
            isDraggingLogSplitter = false;

            if (pnlLogSplitter != null)
                pnlLogSplitter.Capture = false;
        }

        private void LayoutPilotTestControls()
        {
            if (tabPilotTest == null || picPilotTest == null || lstPilotFrames == null)
                return;

            int margin = 32;
            int gap = 24;
            int tabWidth = Math.Max(tabPilotTest.ClientSize.Width, 900);
            int tabHeight = Math.Max(tabPilotTest.ClientSize.Height, 480);

            lblTitlePilot.Location = new Point(margin, 28);

            int labelWidth = Math.Max(
                150,
                Math.Max(lblModelPath.PreferredSize.Width, lblModelList.PreferredSize.Width)
            );
            int fieldX = margin + labelWidth + 16;
            int topButtonWidth = 110;
            int y = 116;
            int fieldWidth = Math.Max(360, Math.Min(760, tabWidth - fieldX - topButtonWidth - margin * 2));

            lblModelPath.Location = new Point(margin, y + 6);
            txtModelPath.SetBounds(fieldX, y, fieldWidth, 34);
            btnBrowseModel.SetBounds(fieldX + fieldWidth + 16, y - 1, topButtonWidth, 36);

            y += 52;
            lblModelList.Location = new Point(margin, y + 6);
            cmbModelList.SetBounds(fieldX, y, Math.Max(260, fieldWidth - 138), 34);
            btnScanModels.SetBounds(fieldX + fieldWidth - 122, y - 1, 128, 36);

            y += 56;
            btnRunPilotTest.SetBounds(fieldX, y, 230, 44);
            btnUseViewerFrame.SetBounds(fieldX + 246, y, 190, 44);
            btnPilotAutoPlay.SetBounds(fieldX + 452, y, 150, 44);
            btnPilotStop.SetBounds(fieldX + 618, y, 110, 44);

            int contentTop = y + 66;
            int contentWidth = Math.Max(720, tabWidth - margin * 2);
            int contentHeight = Math.Max(260, tabHeight - contentTop - margin);

            int listWidth = Math.Max(260, Math.Min(330, (int)(contentWidth * 0.16)));
            int statWidth = Math.Max(360, Math.Min(420, (int)(contentWidth * 0.2)));
            int listX = tabWidth - margin - listWidth;
            int statX = listX - gap - statWidth;
            int picX = margin;
            int picWidth = Math.Max(480, statX - gap - picX);

            picPilotTest.SetBounds(picX, contentTop, picWidth, contentHeight);

            Label[] statLabels =
            {
                lblActualAngle,
                lblPredictedAngle,
                lblActualThrottle,
                lblPredictedThrottle,
                lblAngleError,
                lblPilotWarning
            };

            foreach (Label label in statLabels)
            {
                label.AutoSize = false;
                label.Width = statWidth;
                label.Height = 32;
            }

            lblActualAngle.SetBounds(statX, contentTop, statWidth, 32);
            lblPredictedAngle.SetBounds(statX, contentTop + 36, statWidth, 32);
            lblActualThrottle.SetBounds(statX, contentTop + 84, statWidth, 32);
            lblPredictedThrottle.SetBounds(statX, contentTop + 120, statWidth, 32);
            lblAngleError.SetBounds(statX, contentTop + 172, statWidth, 32);
            lblPilotWarning.SetBounds(statX, contentTop + 216, statWidth, 36);
            lblPilotNote.SetBounds(statX, contentTop + 268, statWidth, Math.Max(120, contentHeight - 268));

            lblPilotImageList.Location = new Point(listX, contentTop);
            lstPilotFrames.SetBounds(listX, contentTop + 34, listWidth, Math.Max(180, contentHeight - 34));
        }


        private void BtnDelete_Paint(object? sender, PaintEventArgs e)
        {
            Button btn = (Button)sender!;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int s = Math.Min(btn.Width, btn.Height);
            int u = s / 8;
            int cx = btn.Width / 2;
            int cy = btn.Height / 2;

            using Pen pen = new Pen(Color.White, Math.Max(1.5f, u * 0.6f))
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };

            // 뚜껑
            int lidW = u * 4, lidH = u;
            g.DrawLine(pen, cx - lidW / 2, cy - u * 2, cx + lidW / 2, cy - u * 2);
            // 손잡이
            g.DrawLine(pen, cx - u, cy - u * 2, cx - u, cy - u * 3);
            g.DrawLine(pen, cx + u, cy - u * 2, cx + u, cy - u * 3);
            // 몸통
            int bx = cx - u * 2, by = cy - u * 2 + u;
            int bw = u * 4, bh = u * 4;
            using GraphicsPath body = new GraphicsPath();
            body.AddLine(bx + u / 2, by, bx + bw - u / 2, by);
            body.AddLine(bx + bw - u / 2, by, bx + bw - u, by + bh);
            body.AddLine(bx + bw - u, by + bh, bx + u, by + bh);
            body.AddLine(bx + u, by + bh, bx + u / 2, by);
            g.DrawPath(pen, body);
            // 세로선
            g.DrawLine(pen, cx, by + u / 2, cx, by + bh - u / 2);
            g.DrawLine(pen, cx - u, by + u / 2, cx - u, by + bh - u / 2);
            g.DrawLine(pen, cx + u, by + u / 2, cx + u, by + bh - u / 2);
        }




        private void BtnPlayRange_Paint(object? sender, PaintEventArgs e)
        {
            Button btn = (Button)sender!;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int size = Math.Min(btn.Width, btn.Height) - 10;
            int cx = btn.Width / 2;
            int cy = btn.Height / 2;

            if (cleanerRangePlayTimer.Enabled)
            {
                // 일시정지 아이콘 (||)
                int barW = size / 5;
                int barH = size / 2;
                int gap = size / 8;

                using SolidBrush brush = new SolidBrush(Color.White);
                g.FillRectangle(brush, cx - gap - barW, cy - barH / 2, barW, barH);
                g.FillRectangle(brush, cx + gap, cy - barH / 2, barW, barH);
            }
            else
            {
                // 재생 아이콘 (▶)
                int triSize = size / 2;
                PointF[] triangle = new PointF[]
                {
            new PointF(cx - triSize / 2, cy - triSize / 2),
            new PointF(cx - triSize / 2, cy + triSize / 2),
            new PointF(cx + triSize / 2, cy)
                };
                using SolidBrush brush = new SolidBrush(Color.White);
                g.FillPolygon(brush, triangle);
            }
        }





        private void BtnClearDataPath_Paint(object? sender, PaintEventArgs e)
        {
            Button btn = (Button)sender!;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int size = Math.Min(btn.Width, btn.Height) - 16;

            // 폴더 아이콘
            using Font f = new Font("Segoe UI Emoji", size / 3.5f);
            using SolidBrush brush = new SolidBrush(Color.FromArgb(100, 100, 100));
            string folder = "📁";
            SizeF fs = g.MeasureString(folder, f);
            g.DrawString(folder, f, brush, (btn.Width - fs.Width) / 2, (btn.Height - fs.Height) / 2);

            // X 표시
            using Pen xPen = new Pen(Color.FromArgb(200, 60, 60), size / 8f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };
            int xSize = size / 4;
            int cx = btn.Width / 2 + size / 6;
            int cy = btn.Height / 2 + size / 6;
            g.DrawLine(xPen, cx - xSize, cy - xSize, cx + xSize, cy + xSize);
            g.DrawLine(xPen, cx + xSize, cy - xSize, cx - xSize, cy + xSize);
        }

        private void MainForm_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void MainForm_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data == null) return;
            string[]? paths = (string[]?)e.Data.GetData(DataFormats.FileDrop);
            if (paths == null || paths.Length == 0) return;

            string path = paths[0];
            if (Directory.Exists(path))
            {
                imagesFolderPath = Path.Combine(path, "images");
                if (!Directory.Exists(imagesFolderPath))
                {
                    MessageBox.Show(
                        "선택한 폴더 안에 images 폴더가 없습니다.\n\n" +
                        "mycar 폴더가 아니라 mycar/data 폴더를 선택해야 합니다."
                    );
                    imagesFolderPath = "";
                    return;
                }
                dataFolderPath = path;
                txtDataPath.Text = path;
                LoadCatalog();
                LogMismatchSummary();
            }
        }

        private void ConnectEvents()
        {

            // 폴더 선택 해제 버튼 이벤트 연결
            btnClearDataPath.Click += btnClearDataPath_Click;

            // 드래그 앤 드롭 이벤트 연결
            txtDataPath.AllowDrop = true;
            txtDataPath.DragEnter += MainForm_DragEnter;
            txtDataPath.DragDrop += MainForm_DragDrop;

            btnScanModels.Click += btnScanModels_Click;
            trbBrightness.Scroll += trbBrightness_Scroll;
            trbContrast.Scroll += trbContrast_Scroll;
            cmbModelList.SelectedIndexChanged += cmbModelList_SelectedIndexChanged;

            btnOpenDataFolder.Click += btnOpenDataFolder_Click;
            btnUndo.Click += btnUndo_Click;

            btnApplyFilter.Click += btnApplyFilter_Click;
            btnClearFilter.Click += btnClearFilter_Click;
            btnCleanMismatch.Click += btnCleanMismatch_Click;
            btnDeleteFrame.Click += btnDeleteFrame_Click;

            pnlCleanerTimeline.Paint += pnlCleanerTimeline_Paint;
            pnlCleanerTimeline.MouseDown += pnlCleanerTimeline_MouseDown;
            pnlCleanerTimeline.MouseMove += pnlCleanerTimeline_MouseMove;
            pnlCleanerTimeline.MouseUp += pnlCleanerTimeline_MouseUp;

            hsbCleanerTimeline.Scroll += hsbCleanerTimeline_Scroll;

            btnDeleteRange.Click += btnDeleteRange_Click;
            btnPlayRange.Click += btnPlayRange_Click;
            btnClearRange.Click += btnClearRange_Click;

            btnCleanerAutoPlay.Click += btnCleanerAutoPlay_Click;
            

            btnBrowseMycar.Click += btnBrowseMycar_Click;
            btnTrain.Click += btnTrain_Click;
            btnStopTrain.Click += btnStopTrain_Click;
            btnEndTrain.Click += btnEndTrain_Click;

            btnBrowseModel.Click += btnBrowseModel_Click;
            btnRunPilotTest.Click += btnRunPilotTest_Click;
            btnUseViewerFrame.Click += btnUseViewerFrame_Click;

            btnPilotAutoPlay.Click += btnPilotAutoPlay_Click;
            btnPilotStop.Click += btnPilotStop_Click;

            lstCleanerFrames.SelectedIndexChanged += lstCleanerFrames_SelectedIndexChanged;
            lstPilotFrames.SelectedIndexChanged += lstPilotFrames_SelectedIndexChanged;


            // image adjustment events
            btnSaveProcessed.Click += btnSaveProcessed_Click;
            chkFlipHorizontal.CheckedChanged += chkFlipHorizontal_CheckedChanged;
            chkGrayscale.CheckedChanged += chkGrayscale_CheckedChanged;
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
        }

        private void btnUndo_Click(object? sender, EventArgs e)
        {
            UndoLastDelete();
        }

        private void btnClearDataPath_Click(object? sender, EventArgs e)
        {
            dataFolderPath = "";
            imagesFolderPath = "";
            catalogFilePath = "";
            txtDataPath.Text = "폴더를 선택하거나 끌어오세요";
            txtDataPath.ForeColor = Color.DimGray;
            allFrames.Clear();
            visibleFrames.Clear();
            ClearCleanerTimelineThumbnailCache();
            cleanerTimelineStartIndex = 0;
            ResetCleanerRange();
            UpdateCleanerTimelineScrollBar();
            pnlCleanerTimeline.Invalidate();
            ClearViewer();
            AppendLog("데이터 폴더 선택 해제");
        }

        private void trbBrightness_Scroll(object? sender, EventArgs e)
        {
            lblBrightness.Text = $"밝기: {trbBrightness.Value}";
            RefreshCurrentFrame();
        }

        private void trbContrast_Scroll(object? sender, EventArgs e)
        {
            lblContrast.Text = $"명암: {trbContrast.Value}";
            RefreshCurrentFrame();
        }

        private void RefreshCurrentFrame()
        {
            if (currentIndex >= 0 && currentIndex < visibleFrames.Count)
                ShowFrame(currentIndex);
        }

        private void cmbModelList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cmbModelList.SelectedItem != null)
            {
                string selected = cmbModelList.SelectedItem.ToString()!.Trim();
                txtModelPath.Text = $"~/mycar/models/{selected}";
                AppendLog($"모델 선택: {selected}");
            }
        }

        private async void btnScanModels_Click(object? sender, EventArgs e)
        {
            AppendLog("모델 스캔 시작...");
            cmbModelList.Items.Clear();

            string command = "ls ~/mycar/models/*.h5 2>/dev/null | xargs -I{} basename {}";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "wsl.exe",
                Arguments = $"-d {WslDistroName} -- bash -lc {QuoteWindowsArgument(command)}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = new Process();
            process.StartInfo = psi;
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            await Task.Run(() => process.WaitForExit());

            string[] models = output
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (models.Length == 0)
            {
                AppendLog("[경고] ~/mycar/models/ 에 .h5 파일이 없습니다.");
                return;
            }

            foreach (string model in models)
                cmbModelList.Items.Add(model.Trim());

            cmbModelList.SelectedIndex = 0;
            AppendLog($"모델 스캔 완료: {models.Length}개 발견");
        }

        private async void btnScanTrainSourceModels_Click(object? sender, EventArgs e)
        {
            if (cmbTrainSourceModel == null)
                return;

            string mycarPath = txtMycarPath.Text.Trim();
            string pythonExe = txtPythonExe.Text.Trim();
            bool useWsl = IsWslMode(pythonExe);

            ResetTrainSourceModelChoices();
            AppendLog("대표 모델 스캔 시작...");

            try
            {
                List<TrainModelChoice> choices = useWsl
                    ? await ScanTrainSourceModelsInWslAsync(mycarPath)
                    : ScanTrainSourceModelsLocally(mycarPath);

                foreach (TrainModelChoice choice in choices)
                    cmbTrainSourceModel.Items.Add(choice);

                if (cmbTrainSourceModel.Items.Count > 0)
                    cmbTrainSourceModel.SelectedIndex = 0;

                AppendLog($"대표 모델 스캔 완료: {choices.Count}개 발견");
            }
            catch (Exception ex)
            {
                AppendLog("[경고] 대표 모델 스캔 실패: " + ex.Message);
                MessageBox.Show("대표 모델 스캔에 실패했습니다.\n\n" + ex.Message);
            }
        }

        private void btnBrowseTrainSourceModel_Click(object? sender, EventArgs e)
        {
            if (cmbTrainSourceModel == null)
                return;

            using OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "대표 모델 선택";
            dlg.Filter = "Keras Model (*.h5;*.keras)|*.h5;*.keras|All Files (*.*)|*.*";

            string mycarPath = txtMycarPath.Text.Trim();
            string modelDir = Path.Combine(mycarPath, "models");

            if (!IsWslMode(txtPythonExe.Text.Trim()) && Directory.Exists(modelDir))
                dlg.InitialDirectory = modelDir;

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            string selectedModelPath = IsWslMode(txtPythonExe.Text.Trim())
                ? ConvertPathToWslPath(dlg.FileName)
                : dlg.FileName;

            TrainModelChoice choice = new TrainModelChoice
            {
                DisplayName = Path.GetFileName(dlg.FileName),
                ModelPath = selectedModelPath
            };

            cmbTrainSourceModel.Items.Add(choice);
            cmbTrainSourceModel.SelectedItem = choice;
            AppendLog("대표 모델 선택: " + selectedModelPath);
        }

        private async Task<List<TrainModelChoice>> ScanTrainSourceModelsInWslAsync(string mycarPath)
        {
            string wslMycarPath = ConvertPathToWslPath(string.IsNullOrWhiteSpace(mycarPath) ? "~/mycar" : mycarPath);
            string command =
                $"cd {BashCdArgument(wslMycarPath)} && " +
                "if [ -d ./models ]; then " +
                "find ./models -maxdepth 1 -type f \\( -name '*.h5' -o -name '*.keras' \\) -printf '%T@ %p\\n' | sort -nr | sed 's/^[^ ]* //'; " +
                "fi";

            (int exitCode, string output, string error) = await RunWslCommandAsync(command);

            if (exitCode != 0)
                throw new Exception(string.IsNullOrWhiteSpace(error) ? "WSL 모델 스캔 실패" : error.Trim());

            return output
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(path => path.Trim())
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Select(path => new TrainModelChoice
                {
                    DisplayName = $"{Path.GetFileName(path)} ({path})",
                    ModelPath = path
                })
                .ToList();
        }

        private List<TrainModelChoice> ScanTrainSourceModelsLocally(string mycarPath)
        {
            if (string.IsNullOrWhiteSpace(mycarPath) || !Directory.Exists(mycarPath))
                throw new DirectoryNotFoundException("Windows mycar 폴더를 찾을 수 없습니다.");

            string modelDir = Path.Combine(mycarPath, "models");

            if (!Directory.Exists(modelDir))
                return new List<TrainModelChoice>();

            return Directory
                .GetFiles(modelDir, "*.*", SearchOption.TopDirectoryOnly)
                .Where(path =>
                    path.EndsWith(".h5", StringComparison.OrdinalIgnoreCase)
                    || path.EndsWith(".keras", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .Select(path => new TrainModelChoice
                {
                    DisplayName = Path.GetFileName(path),
                    ModelPath = path
                })
                .ToList();
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
            txtDataPath.Text = dataFolderPath;

            LoadCatalog();
            LogMismatchSummary();
        }

        private void btnCleanMismatch_Click(object? sender, EventArgs e)
        {
            if (!EnsureDataFolderReadyForMismatch())
                return;

            try
            {
                MismatchScanResult scan = ScanMismatch();

                if (!scan.HasMismatch)
                {
                    string cleanMessage =
                        scan.ParseErrorCount > 0
                            ? $"정리할 미스매치는 없습니다.\n\n단, catalog 파싱 실패 줄이 {scan.ParseErrorCount}개 있어 수동 확인이 필요합니다."
                            : "미스매치가 없습니다.";

                    MessageBox.Show(cleanMessage, "미스매치 검사");
                    AppendLog("미스매치 검사 완료: 정리할 항목 없음");
                    return;
                }

                DialogResult result = MessageBox.Show(
                    BuildMismatchConfirmMessage(scan),
                    "미스매치 정리 확인",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result != DialogResult.Yes)
                {
                    AppendLog("미스매치 정리 취소");
                    return;
                }

                ApplyMismatchCleanup(scan);
            }
            catch (Exception ex)
            {
                MessageBox.Show("미스매치 정리 중 오류가 발생했습니다.\n\n" + ex.Message);
                AppendLog("미스매치 정리 실패: " + ex.Message);
            }
        }

        private void LogMismatchSummary()
        {
            try
            {
                if (!IsDataFolderReadyForMismatch())
                    return;

                MismatchScanResult scan = ScanMismatch();

                if (scan.HasMismatch)
                {
                    AppendLog(
                        $"미스매치 발견: 이미지 없는 catalog 항목 {scan.CatalogEntriesWithoutImages.Count}개, " +
                        $"catalog에 없는 이미지 {scan.ImagesWithoutCatalog.Count}개"
                    );
                }
                else
                {
                    AppendLog("미스매치 검사: 문제 없음");
                }

                if (scan.ParseErrorCount > 0)
                    AppendLog($"[경고] catalog 파싱 실패 줄: {scan.ParseErrorCount}개");

                if (scan.OrphanImageScanSkipped)
                    AppendLog("[정보] catalog 파싱 실패 줄이 있어 catalog에 없는 이미지 이동 검사는 생략했습니다.");
            }
            catch (Exception ex)
            {
                AppendLog("[경고] 미스매치 검사 실패: " + ex.Message);
            }
        }

        private bool EnsureDataFolderReadyForMismatch()
        {
            if (!IsDataFolderReadyForMismatch())
            {
                MessageBox.Show("먼저 올바른 Donkeycar data 폴더를 열어주세요.");
                return false;
            }

            if (GetCatalogFiles().Length == 0)
            {
                MessageBox.Show("data 폴더 안에 catalog_*.catalog 파일이 없습니다.");
                return false;
            }

            return true;
        }

        private bool IsDataFolderReadyForMismatch()
        {
            if (string.IsNullOrWhiteSpace(dataFolderPath) || !Directory.Exists(dataFolderPath))
                return false;

            imagesFolderPath = Path.Combine(dataFolderPath, "images");
            return Directory.Exists(imagesFolderPath);
        }

        private MismatchScanResult ScanMismatch()
        {
            string[] catalogFiles = GetCatalogFiles();

            if (catalogFiles.Length == 0)
                throw new FileNotFoundException("catalog_*.catalog 파일을 찾을 수 없습니다.");

            Dictionary<string, string> imagePathByFileName = Directory
                .GetFiles(imagesFolderPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(IsSupportedImageFile)
                .Select(path => new
                {
                    Path = path,
                    Name = Path.GetFileName(path)
                })
                .Where(item => !string.IsNullOrWhiteSpace(item.Name))
                .GroupBy(item => item.Name!, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First().Path, StringComparer.OrdinalIgnoreCase);

            HashSet<string> referencedImageFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            MismatchScanResult scan = new MismatchScanResult { CatalogFiles = catalogFiles };

            foreach (string catalogPath in catalogFiles)
            {
                int lineNumber = 0;

                foreach (string line in File.ReadLines(catalogPath))
                {
                    lineNumber++;

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    scan.CatalogLineCount++;

                    try
                    {
                        using JsonDocument doc = JsonDocument.Parse(line);
                        JsonElement root = doc.RootElement;

                        string imageReference = "";

                        if (root.TryGetProperty("cam/image_array", out JsonElement imageElement))
                            imageReference = imageElement.ValueKind == JsonValueKind.String
                                ? imageElement.GetString() ?? ""
                                : imageElement.ToString();

                        string imageFileName = NormalizeCatalogImageFileName(imageReference);

                        if (!string.IsNullOrWhiteSpace(imageFileName))
                            referencedImageFileNames.Add(imageFileName);

                        if (string.IsNullOrWhiteSpace(imageFileName)
                            || !imagePathByFileName.ContainsKey(imageFileName))
                        {
                            scan.CatalogEntriesWithoutImages.Add(new CatalogMismatchEntry
                            {
                                CatalogPath = catalogPath,
                                LineNumber = lineNumber,
                                ImageReference = string.IsNullOrWhiteSpace(imageReference)
                                    ? "(cam/image_array 없음)"
                                    : imageReference,
                                ImageFileName = imageFileName
                            });
                        }
                    }
                    catch (JsonException)
                    {
                        scan.ParseErrorCount++;
                    }
                }
            }

            if (scan.ParseErrorCount > 0)
            {
                scan.OrphanImageScanSkipped = true;
                return scan;
            }

            foreach (string imageFileName in imagePathByFileName.Keys.OrderBy(name => name, StringComparer.OrdinalIgnoreCase))
            {
                if (!referencedImageFileNames.Contains(imageFileName))
                    scan.ImagesWithoutCatalog.Add(imagePathByFileName[imageFileName]);
            }

            return scan;
        }

        private static bool IsSupportedImageFile(string path)
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();
            return extension == ".jpg"
                || extension == ".jpeg"
                || extension == ".png"
                || extension == ".bmp";
        }

        private static string NormalizeCatalogImageFileName(string imageReference)
        {
            if (string.IsNullOrWhiteSpace(imageReference))
                return "";

            string cleaned = imageReference.Trim().Trim('"', '\'').Replace('\\', '/');

            while (cleaned.StartsWith("./", StringComparison.Ordinal))
                cleaned = cleaned.Substring(2);

            int queryIndex = cleaned.IndexOf('?');

            if (queryIndex >= 0)
                cleaned = cleaned.Substring(0, queryIndex);

            int slashIndex = cleaned.LastIndexOf('/');

            if (slashIndex >= 0)
                cleaned = cleaned.Substring(slashIndex + 1);

            return cleaned.Trim();
        }

        private string BuildMismatchConfirmMessage(MismatchScanResult scan)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("미스매치를 정리할까요?");
            builder.AppendLine();
            builder.AppendLine($"catalog 파일: {scan.CatalogFiles.Length}개");
            builder.AppendLine($"검사한 catalog 줄: {scan.CatalogLineCount}개");
            builder.AppendLine($"이미지가 없는 catalog 항목: {scan.CatalogEntriesWithoutImages.Count}개");
            builder.AppendLine($"catalog에 없는 이미지: {scan.ImagesWithoutCatalog.Count}개");

            if (scan.ParseErrorCount > 0)
                builder.AppendLine($"catalog 파싱 실패 줄: {scan.ParseErrorCount}개 (정리하지 않고 보존)");

            if (scan.OrphanImageScanSkipped)
                builder.AppendLine("catalog에 없는 이미지 검사는 파싱 실패 줄 때문에 생략됨");

            builder.AppendLine();
            builder.AppendLine("정리 방식:");
            builder.AppendLine("- catalog 원본과 격리 대상 이미지를 먼저 백업합니다.");
            builder.AppendLine("- catalog에 없는 이미지는 삭제하지 않고 mismatch_trash로 이동합니다.");
            builder.AppendLine("- 이미지가 없는 catalog 줄만 catalog에서 제거합니다.");

            AppendMismatchSamples(builder, scan);

            return builder.ToString();
        }

        private static void AppendMismatchSamples(StringBuilder builder, MismatchScanResult scan)
        {
            if (scan.CatalogEntriesWithoutImages.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("이미지가 없는 catalog 항목 예시:");

                foreach (CatalogMismatchEntry entry in scan.CatalogEntriesWithoutImages.Take(5))
                    builder.AppendLine($"- {Path.GetFileName(entry.CatalogPath)}:{entry.LineNumber} -> {entry.ImageReference}");
            }

            if (scan.ImagesWithoutCatalog.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("catalog에 없는 이미지 예시:");

                foreach (string imagePath in scan.ImagesWithoutCatalog.Take(5))
                    builder.AppendLine($"- {Path.GetFileName(imagePath)}");
            }
        }

        private void ApplyMismatchCleanup(MismatchScanResult scan)
        {
            StopAllAutoPlay("미스매치 정리를 위해 자동 재생 멈춤");
            DisposeCurrentImages();

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupFolder = CreateUniqueDirectory(Path.Combine(dataFolderPath, "backup"), timestamp);
            string catalogBackupDir = Path.Combine(backupFolder, "catalog");
            string imageBackupDir = Path.Combine(backupFolder, "images");
            Directory.CreateDirectory(catalogBackupDir);

            foreach (string catalogFile in scan.CatalogFiles)
            {
                string dest = Path.Combine(catalogBackupDir, Path.GetFileName(catalogFile));
                File.Copy(catalogFile, dest, true);
            }

            if (scan.ImagesWithoutCatalog.Count > 0)
                Directory.CreateDirectory(imageBackupDir);

            foreach (string imagePath in scan.ImagesWithoutCatalog)
            {
                if (!File.Exists(imagePath))
                    continue;

                string backupImagePath = GetUniqueFilePath(Path.Combine(imageBackupDir, Path.GetFileName(imagePath)));
                File.Copy(imagePath, backupImagePath, true);
            }

            backupFolderPaths.Add(backupFolder);

            int removedCatalogLineCount = RewriteCatalogsWithoutMissingImages(scan);
            string trashFolder = "";
            int movedImageCount = 0;

            if (scan.ImagesWithoutCatalog.Count > 0)
            {
                trashFolder = CreateUniqueDirectory(Path.Combine(dataFolderPath, "mismatch_trash"), timestamp);
                movedImageCount = MoveOrphanImagesToTrash(scan, trashFolder);
            }

            ClearCleanerTimelineThumbnailCache();
            LoadCatalog();
            LogMismatchSummary();

            AppendLog(
                $"미스매치 정리 완료: catalog 항목 {removedCatalogLineCount}개 제거, " +
                $"이미지 {movedImageCount}개 격리 이동"
            );

            MessageBox.Show(
                $"미스매치 정리가 완료되었습니다.\n\n" +
                $"제거된 catalog 항목: {removedCatalogLineCount}개\n" +
                $"격리 이동된 이미지: {movedImageCount}개\n\n" +
                $"백업 폴더:\n{backupFolder}\n\n" +
                $"격리 폴더:\n{(string.IsNullOrWhiteSpace(trashFolder) ? "없음" : trashFolder)}",
                "미스매치 정리 완료"
            );
        }

        private int RewriteCatalogsWithoutMissingImages(MismatchScanResult scan)
        {
            int removedCount = 0;

            foreach (var group in scan.CatalogEntriesWithoutImages.GroupBy(entry => entry.CatalogPath))
            {
                HashSet<int> removeLineNumbers = group
                    .Select(entry => entry.LineNumber)
                    .ToHashSet();

                List<string> keptLines = new List<string>();
                int lineNumber = 0;

                foreach (string line in File.ReadLines(group.Key))
                {
                    lineNumber++;

                    if (removeLineNumbers.Contains(lineNumber))
                    {
                        removedCount++;
                        continue;
                    }

                    keptLines.Add(line);
                }

                File.WriteAllLines(group.Key, keptLines);
            }

            return removedCount;
        }

        private int MoveOrphanImagesToTrash(MismatchScanResult scan, string trashFolder)
        {
            if (scan.ImagesWithoutCatalog.Count == 0)
                return 0;

            string trashImageDir = Path.Combine(trashFolder, "images");
            Directory.CreateDirectory(trashImageDir);

            int movedCount = 0;

            foreach (string imagePath in scan.ImagesWithoutCatalog)
            {
                if (!File.Exists(imagePath))
                    continue;

                string targetPath = GetUniqueFilePath(Path.Combine(trashImageDir, Path.GetFileName(imagePath)));
                File.Move(imagePath, targetPath);
                movedCount++;
            }

            return movedCount;
        }

        private static string CreateUniqueDirectory(string root, string preferredName)
        {
            Directory.CreateDirectory(root);

            string path = Path.Combine(root, preferredName);
            int suffix = 1;

            while (Directory.Exists(path))
            {
                path = Path.Combine(root, $"{preferredName}_{suffix:D2}");
                suffix++;
            }

            Directory.CreateDirectory(path);
            return path;
        }

        private static string GetUniqueFilePath(string path)
        {
            if (!File.Exists(path))
                return path;

            string directory = Path.GetDirectoryName(path) ?? "";
            string fileName = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);
            int suffix = 1;

            string candidate;

            do
            {
                candidate = Path.Combine(directory, $"{fileName}_{suffix}{extension}");
                suffix++;
            }
            while (File.Exists(candidate));

            return candidate;
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

        private void btnCleanerAutoPlay_Click(object? sender, EventArgs e)
        {
            if (visibleFrames.Count == 0)
            {
                MessageBox.Show("먼저 데이터 폴더를 열어주세요.");
                return;
            }

            if (IsAutoPlayRunning())
            {
                StopAllAutoPlay("자동 재생 멈춤");
                isAutoRangeSelecting = false;

                return;
            }
            

            cleanerAutoPlayTimer.Start();
            SyncAutoPlayButtons();

            AppendLog("자동 재생 시작");
        }

        
        


        private void btnCleanerStop_Click(object? sender, EventArgs e)
        {
            StopAllAutoPlay("자동 재생 멈춤");
            isAutoRangeSelecting = false;
        }

        private void CleanerAutoPlayTimer_Tick(object? sender, EventArgs e)
        {
            if (visibleFrames.Count == 0)
            {
                StopAllAutoPlay("자동 재생 멈춤");
                return;
            }

            int next = currentIndex + 1;

            if (next < 0 || next >= visibleFrames.Count)
                next = 0;

            ShowFrame(next);
        }

        private void StopCleanerAutoPlay()
        {
            bool wasRunning = cleanerAutoPlayTimer.Enabled;
            cleanerAutoPlayTimer.Stop();
            SyncAutoPlayButtons();

            if (wasRunning)
                AppendLog("자동 재생 멈춤");
        }

        private async void btnPilotAutoPlay_Click(object? sender, EventArgs e)
        {
            if (visibleFrames.Count == 0)
            {
                MessageBox.Show("먼저 데이터 폴더를 열어주세요.");
                return;
            }

            if (IsAutoPlayRunning())
            {
                StopAllAutoPlay("자동 재생 멈춤");
                return;
            }

            string modelPath = GetPilotModelPath();

            try
            {
                btnPilotAutoPlay.Enabled = false;
                lblPredictedAngle.Text = "예측 Angle: 모델 로딩 중...";
                lblPredictedThrottle.Text = "예측 Throttle: 모델 로딩 중...";
                lblPilotWarning.Text = "판정: 예측 서버 준비 중";
                lblPilotWarning.ForeColor = Color.DimGray;

                await EnsurePredictServerRunningAsync(modelPath);
            }
            catch (Exception ex)
            {
                lblPredictedAngle.Text = "예측 Angle: 서버 실패";
                lblPredictedThrottle.Text = "예측 Throttle: 서버 실패";
                lblPilotWarning.Text = "판정: 예측 서버 실패";
                lblPilotWarning.ForeColor = Color.Red;
                AppendLog("실시간 예측 서버 시작 실패: " + ex.Message);
                MessageBox.Show("실시간 예측 서버 시작에 실패했습니다.\n\n" + ex.Message);
                return;
            }
            finally
            {
                btnPilotAutoPlay.Enabled = true;
            }

            livePredictFailureCount = 0;
            pilotAutoPlayTimer.Start();
            SyncAutoPlayButtons();
            QueueLivePredictionForCurrentFrame();

            AppendLog("자동 재생 시작: 실시간 예측 활성화");
        }

        private void btnPilotStop_Click(object? sender, EventArgs e)
        {
            StopAllAutoPlay("자동 재생 멈춤");
        }

        private void PilotAutoPlayTimer_Tick(object? sender, EventArgs e)
        {
            if (visibleFrames.Count == 0)
            {
                StopAllAutoPlay("자동 재생 멈춤");
                return;
            }

            int next = currentIndex + 1;

            if (next < 0 || next >= visibleFrames.Count)
                next = 0;

            ShowFrame(next);
        }

        private void StopPilotAutoPlay()
        {
            bool wasRunning = pilotAutoPlayTimer.Enabled;
            pilotAutoPlayTimer.Stop();
            livePredictPending = false;
            livePredictVersion++;
            SyncAutoPlayButtons();

            if (wasRunning)
                AppendLog("자동 재생 멈춤");
        }

        private bool IsAutoPlayRunning()
        {
            return cleanerAutoPlayTimer.Enabled || pilotAutoPlayTimer.Enabled;
        }

        private void StopAllAutoPlay(string? logMessage = null)
        {
            bool wasRunning = IsAutoPlayRunning();

            cleanerAutoPlayTimer.Stop();
            pilotAutoPlayTimer.Stop();
            SyncAutoPlayButtons();

            if (wasRunning && !string.IsNullOrWhiteSpace(logMessage))
                AppendLog(logMessage);
        }

        private void SyncAutoPlayButtons()
        {
            bool running = IsAutoPlayRunning();
            string text = running ? "재생 중지" : "자동 재생";
            Color backColor = running
                ? Color.FromArgb(220, 80, 80)
                : Color.FromArgb(76, 175, 80);

            if (btnCleanerAutoPlay != null)
            {
                btnCleanerAutoPlay.Text = text;
                btnCleanerAutoPlay.BackColor = backColor;
                btnCleanerAutoPlay.ForeColor = Color.White;
            }

            if (btnPilotAutoPlay != null)
            {
                btnPilotAutoPlay.Text = text;
                btnPilotAutoPlay.BackColor = backColor;
                btnPilotAutoPlay.ForeColor = Color.White;
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
            chkExcludeJitterAngle.Checked = false;
            visibleFrames = allFrames.ToList();

            BindFrameLists();
            ResetCleanerRange();

            if (visibleFrames.Count > 0)
                ShowFrame(0);
            else
                ClearViewer();

            AppendLog("필터 해제: 전체 데이터 표시");
        }

        private void btnDeleteFrame_Click(object? sender, EventArgs e)
        {
            bool hasFilter = chkThrottlePositive.Checked || chkExcludeZeroAngle.Checked || chkStopDataOnly.Checked || chkExcludeJitterAngle.Checked;

            // 1. 필터가 켜져 있어서, 화면에 안 보이고 '걸러진 쓰레기 데이터'가 존재하는 경우의 자동 삭제
            if (hasFilter && visibleFrames.Count < allFrames.Count)
            {
                int trashCount = allFrames.Count - visibleFrames.Count;
                DialogResult filterResult = MessageBox.Show(
                    $"현재 필터링되어 화면에 안 보이는 데이터가 {trashCount}개 있습니다.\n\n" +
                    "이 데이터들을 삭제하시겠습니까?",
                    "데이터 삭제",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question
                );

                if (filterResult == DialogResult.Yes)
                {
                    List<DonkeyFrame> hiddenTrashFrames = new List<DonkeyFrame>();
                    HashSet<string> visibleKeys = new HashSet<string>();

                    foreach (var f in visibleFrames)
                        visibleKeys.Add(MakeFrameKey(f));

                    // 전체 데이터 중에서 화면에 없는(걸러진) 데이터들만 색출
                    foreach (var f in allFrames)
                    {
                        if (!visibleKeys.Contains(MakeFrameKey(f)))
                            hiddenTrashFrames.Add(f);
                    }

                    DeleteFrames(hiddenTrashFrames, "필터 걸러진 데이터 통째로 삭제");
                    return; // 필터 잔연물 삭제 완료 후 종료
                }
                else if (filterResult == DialogResult.Cancel)
                {
                    return; // 취소
                }
                // No를 누를 시, 원래대로 리스트에서 파란색 표기된 개별 선택 삭제 로직으로 넘어감
            }

            // 2. 기본 동작: 필터와 무관하게 사용자가 리스트에서 파란색으로 선택한 항별 수동 삭제
            if (visibleFrames.Count == 0)
            {
                MessageBox.Show("삭제할 데이터가 없습니다.");
                return;
            }

            List<DonkeyFrame> framesToDelete = new List<DonkeyFrame>();

            // lstCleanerFrames에서 파란색으로 선택된 항목들만 가져옴
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
                "삭제 전 백업 폴더가 자동으로 생성됩니다.",
                "선택 프레임 삭제 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                DeleteFrames(framesToDelete, "선택 수동 삭제");
            }
        }

        private void btnDeleteRange_Click(object? sender, EventArgs e)
        {
            if (cleanerRanges.Count == 0)
            {
                MessageBox.Show("먼저 타임라인에서 삭제할 구간을 드래그해서 선택하세요.");
                return;
            }

            List<DonkeyFrame> framesToDelete = new List<DonkeyFrame>();


            foreach (var range in cleanerRanges)
            {
                int start = range.Start;
                int end = range.End;

                for (int i = start; i <= end; i++)
                {
                    if (i >= 0 && i < visibleFrames.Count)
                        framesToDelete.Add(visibleFrames[i]);
                }
            }

            DialogResult result = MessageBox.Show(
                $"{cleanerRanges.Count}개 구간의\n" +
                 $"총 {framesToDelete.Count}개 프레임을 삭제할까요?\n\n" +
                  "이미지 파일과 catalog 데이터가 함께 삭제됩니다.\n" +
                 "삭제 전 백업 폴더가 자동으로 생성됩니다.",
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
                StopCleanerAutoPlay();
                StopPilotAutoPlay();

                DisposeCurrentImages();

                HashSet<string> deleteKeys = new HashSet<string>();

                // !!! 1. 먼저 카탈로그를 정리 및 백업 (합쳐진 백업 폴더 생성) !!!
                // 백업 과정에서 삭제되지 않은 원본 카탈로그 파일들이 백업됩니다.
                int removedCountForCatalog = allFrames.RemoveAll(f =>
                    framesToDelete.Any(delF => MakeFrameKey(delF) == MakeFrameKey(f)));

                // SaveCatalog() 함수가 내부적으로 BackupCatalogFiles()를 호출하여 백업 폴더를 만듭니다.
                // 여기선 그 백업 폴더 경로를 받아와서 쓸 수 있도록 SaveCatalog()의 반환값을 바꾸면 좀 복잡해지므로, 
                // 수동으로 한 번 BackupCatalogFiles()를 호출해서 통합 백업 폴더 경로를 얻겠습니다.
                string backupFolderPath = BackupCatalogFiles();

                // !!! 2. 삭제할 이미지 백업 및 삭제 !!!
                string imgBackupDir = string.Empty;
                if (!string.IsNullOrWhiteSpace(backupFolderPath))
                {
                    imgBackupDir = Path.Combine(backupFolderPath, "images");
                    Directory.CreateDirectory(imgBackupDir);
                }

                foreach (DonkeyFrame frame in framesToDelete)
                {
                    string key = MakeFrameKey(frame);
                    deleteKeys.Add(key);

                    string imagePath = Path.Combine(imagesFolderPath, frame.ImageFileName);

                    if (File.Exists(imagePath))
                    {
                        // 이미지 백업 폴더가 정상적으로 있을 경우 복사
                        if (!string.IsNullOrWhiteSpace(imgBackupDir))
                        {
                            string backupPath = Path.Combine(imgBackupDir, frame.ImageFileName);
                            File.Copy(imagePath, backupPath, true);
                        }

                        File.Delete(imagePath);
                        AppendLog($"{logTitle} 이미지 백업 및 삭제: {frame.ImageFileName}");
                    }
                    else
                    {
                        AppendLog($"{logTitle} 이미지 없음: {frame.ImageFileName}");
                    }
                }

                ClearCleanerTimelineThumbnailCache();

                // !!! 3. 삭제 완료된 새 리스트로 카탈로그 파일을 다시 생성 !!!
                // 단, BackupCatalogFiles()는 위에서 호출했으므로 SaveCatalog 내부의 중복 백업 호출은 막아줘야 합니다.
                // 편의상 이대로 SaveCatalog()를 호출해도 무관합니다. (어차피 1초 안이라 같은 폴더에 덮어씌워짐)
                SaveCatalogWithoutBackup();

                ResetCleanerRange();
                ApplyFilter();

                AppendLog($"{logTitle} 완료: {removedCountForCatalog}개 프레임 삭제");
                MessageBox.Show($"{removedCountForCatalog}개 프레임 삭제 및 백업이 완료되었습니다.\n[백업 폴더: data/backup]");
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
            if (cleanerRanges.Count == 0)
            {
                MessageBox.Show("먼저 타임라인에서 재생할 구간을 선택하세요.");
                return;
            }

            StopCleanerAutoPlay();
            StopPilotAutoPlay();

            // 이미 재생 중이면 중지
            if (cleanerRangePlayTimer.Enabled)
            {
                cleanerRangePlayTimer.Stop();
                btnPlayRange.Text = "";
                btnPlayRange.Invalidate();
                return;
            }

            // 🔥 구간 → 하나의 연속 리스트 생성
            cleanerPlayFrames = cleanerRanges
                .OrderBy(r => r.Start)
                .SelectMany(r => Enumerable.Range(r.Start, r.End - r.Start + 1))
                .ToList();

            if (cleanerPlayFrames.Count == 0)
                return;

            cleanerPlayIndex = 0;

            cleanerRangePlayIndex = cleanerPlayFrames[0];

            ShowFrame(cleanerRangePlayIndex);

            cleanerRangePlayTimer.Start();

            btnPlayRange.Invalidate();

            AppendLog(
                $"구간 통합 재생 시작: {cleanerRanges.Count}개 구간 / {cleanerPlayFrames.Count}프레임"
            );
        }

        private void CleanerRangePlayTimer_Tick(object? sender, EventArgs e)
        {
            if (cleanerPlayFrames == null || cleanerPlayFrames.Count == 0)
            {
                cleanerRangePlayTimer.Stop();
                btnPlayRange.Text = "";
                btnPlayRange.Invalidate();
                return;
            }

            cleanerPlayIndex++;

            if (cleanerPlayIndex >= cleanerPlayFrames.Count)
            {
                cleanerRangePlayTimer.Stop();
                btnPlayRange.Invalidate();
                AppendLog("구간 재생 종료");
                return;
            }

            cleanerRangePlayIndex = cleanerPlayFrames[cleanerPlayIndex];

            ShowFrame(cleanerRangePlayIndex);

            pnlCleanerTimeline.Invalidate();
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


            ShowFrame(index);

            // 🔥 자동 구간 모드 (버튼 기반)
            if (isAutoRangeSelecting)
            {
                AddRangePoint(index);
                return;
            }

            // 🔥 일반 드래그 모드
            isDraggingCleanerRange = true;

            // 시작점 초기화
            pendingRangeStart = index;

            pnlCleanerTimeline.Invalidate();
        }

        private void ToggleMark(int index)
        {
            if (markedFrameIndices.Contains(index))
            {
                markedFrameIndices.Remove(index);
                AppendLog($"마크 제거: {index + 1}번");
            }
            else
            {
                markedFrameIndices.Add(index);
                AppendLog($"마크 추가: {index + 1}번");
            }
            pnlCleanerTimeline.Invalidate();
        }

        private void pnlCleanerTimeline_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!isDraggingCleanerRange || visibleFrames.Count == 0)
                return;

            int index = HitTestCleanerTimelineIndex(e.X);

            if (index < 0)
                return;



            ShowFrame(index);

            UpdateCleanerRangeUi();
            pnlCleanerTimeline.Invalidate();
        }

        private void pnlCleanerTimeline_MouseUp(object? sender, MouseEventArgs e)
        {
            if (isDraggingCleanerRange)
            {
                int index = HitTestCleanerTimelineIndex(e.X);

                AddRangePoint(index);
            }

            isDraggingCleanerRange = false;
        }
        private void hsbCleanerTimeline_Scroll(object? sender, ScrollEventArgs e)
        {
            int maxStart = GetCleanerTimelineMaxStartIndex();
            cleanerTimelineStartIndex = Math.Max(0, Math.Min(e.NewValue, maxStart));

            UpdateCleanerTimelineScrollBar();
            pnlCleanerTimeline.Invalidate();
        }

        private int HitTestCleanerTimelineIndex(int mouseX)
        {
            if (visibleFrames.Count == 0)
                return -1;

            Rectangle rect = GetCleanerTimelineTrackRect();

            if (rect == Rectangle.Empty)
                return -1;

            if (mouseX < rect.Left)
                mouseX = rect.Left;

            if (mouseX > rect.Right)
                mouseX = rect.Right;

            int slotWidth = CleanerTimelineThumbWidth + CleanerTimelineThumbGap;
            int slot = (mouseX - rect.Left) / slotWidth;

            int index = cleanerTimelineStartIndex + slot;

            if (index < 0)
                index = 0;

            if (index >= visibleFrames.Count)
                index = visibleFrames.Count - 1;

            return index;
        }

        private bool TryGetNormalizedCleanerRanges(
     out List<(int Start, int End)> ranges)
        {
            ranges = new();

            if (visibleFrames.Count == 0)
                return false;

            foreach (var range in cleanerRanges)
            {
                int start =
                    Math.Max(0,
                    Math.Min(range.Start, visibleFrames.Count - 1));

                int end =
                    Math.Max(0,
                    Math.Min(range.End, visibleFrames.Count - 1));

                if (start > end)
                    continue;

                ranges.Add((start, end));
            }

            return ranges.Count > 0;
        }



        private void ResetCleanerRange()
        {
            cleanerRanges.Clear();
            markedFrameIndices.Clear();
           
            if (btnCleanerMark != null)
            {
                btnCleanerMark.Text = "구간 마크";
                btnCleanerMark.BackColor = Color.Yellow;
            }

            pendingRangeStart = -1;

            cleanerRangePlayIndex = -1;

            isDraggingCleanerRange = false;

            isAutoRangeSelecting = false;

            cleanerRangePlayTimer.Stop();

            if (btnPlayRange != null)
            {
                btnPlayRange.Text = "";
                btnPlayRange.Invalidate();
            }

            UpdateCleanerRangeUi();

            pnlCleanerTimeline?.Invalidate();

            AppendLog("모든 선택 구간 초기화");
        }

        private void UpdateCleanerRangeUi()
        {
            if (lblCleanerRangeInfo == null)
                return;

            if (cleanerRanges.Count == 0)
            {
                if (pendingRangeStart >= 0)
                {
                    lblCleanerRangeInfo.Text =
                        $"시작 지정됨 : {pendingRangeStart + 1}번 (끝 선택 대기)";
                }
                else
                {
                    lblCleanerRangeInfo.Text = "선택 구간: 없음";
                }

                return;
            }

            int totalSelected = 0;

            foreach (var range in cleanerRanges)
            {
                totalSelected +=
                    (range.End - range.Start + 1);
            }

            lblCleanerRangeInfo.Text =
                $"선택 구간: {cleanerRanges.Count}개 / 총 {totalSelected}장 선택";
        }

        private void pnlCleanerTimeline_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;

            using SolidBrush bgBrush = new SolidBrush(Color.FromArgb(18, 26, 42));
            g.FillRectangle(bgBrush, pnlCleanerTimeline.ClientRectangle);

            Rectangle trackRect = GetCleanerTimelineTrackRect();

            if (visibleFrames.Count == 0)
            {
                using Font font = new Font("맑은 고딕", 10, FontStyle.Bold);
                using SolidBrush brush = new SolidBrush(Color.White);
                g.DrawString("로드된 프레임이 없습니다.", font, brush, 12, 12);
                return;
            }

            DrawCleanerImageTimeline(g, trackRect);
            DrawCleanerSelectedRange(g, trackRect);
            DrawCleanerCurrentFrameMarker(g, trackRect);
            DrawCleanerTimelineBorder(g, trackRect);
        }

        private Rectangle GetCleanerTimelineTrackRect()
        {
            Rectangle rect = pnlCleanerTimeline.ClientRectangle;
            rect.Inflate(-8, -8);

            if (rect.Width < 10 || rect.Height < 10)
                return Rectangle.Empty;

            return rect;
        }

        private int GetCleanerTimelineVisibleSlotCount()
        {
            Rectangle rect = GetCleanerTimelineTrackRect();

            if (rect == Rectangle.Empty)
                return 1;

            int slotWidth = CleanerTimelineThumbWidth + CleanerTimelineThumbGap;
            int count = Math.Max(1, rect.Width / slotWidth);

            return count;
        }

        private int GetCleanerTimelineMaxStartIndex()
        {
            if (visibleFrames.Count == 0)
                return 0;

            int visibleSlotCount = GetCleanerTimelineVisibleSlotCount();
            return Math.Max(0, visibleFrames.Count - visibleSlotCount);
        }

        private void UpdateCleanerTimelineScrollBar()
        {
            if (hsbCleanerTimeline == null)
                return;

            int visibleSlotCount = GetCleanerTimelineVisibleSlotCount();
            int maxStart = GetCleanerTimelineMaxStartIndex();

            cleanerTimelineStartIndex = Math.Max(0, Math.Min(cleanerTimelineStartIndex, maxStart));

            hsbCleanerTimeline.Enabled = visibleFrames.Count > visibleSlotCount;
            hsbCleanerTimeline.Minimum = 0;
            hsbCleanerTimeline.SmallChange = 1;
            hsbCleanerTimeline.LargeChange = Math.Max(1, visibleSlotCount);
            hsbCleanerTimeline.Maximum = maxStart + hsbCleanerTimeline.LargeChange - 1;

            int safeValue = Math.Max(hsbCleanerTimeline.Minimum, Math.Min(cleanerTimelineStartIndex, hsbCleanerTimeline.Maximum));

            if (hsbCleanerTimeline.Value != safeValue)
                hsbCleanerTimeline.Value = safeValue;

            if (lblCleanerTimelineScrollInfo != null)
            {
                if (visibleFrames.Count == 0)
                {
                    lblCleanerTimelineScrollInfo.Text = "표시 구간: -";
                }
                else
                {
                    int viewStart = cleanerTimelineStartIndex + 1;
                    int viewEnd = Math.Min(visibleFrames.Count, cleanerTimelineStartIndex + visibleSlotCount);
                    lblCleanerTimelineScrollInfo.Text =
                        $"표시 구간: {viewStart} ~ {viewEnd} / {visibleFrames.Count}";
                }
            }
        }

        private void EnsureCleanerTimelineFrameVisible(int index)
        {
            if (index < 0 || index >= visibleFrames.Count)
                return;

            int visibleSlotCount = GetCleanerTimelineVisibleSlotCount();

            if (index < cleanerTimelineStartIndex)
                cleanerTimelineStartIndex = index;
            else if (index >= cleanerTimelineStartIndex + visibleSlotCount)
                cleanerTimelineStartIndex = index - visibleSlotCount + 1;

            cleanerTimelineStartIndex = Math.Max(0, Math.Min(cleanerTimelineStartIndex, GetCleanerTimelineMaxStartIndex()));
            UpdateCleanerTimelineScrollBar();
        }

        private void DrawCleanerImageTimeline(Graphics g, Rectangle trackRect)
        {
            if (trackRect == Rectangle.Empty || visibleFrames.Count == 0)
                return;

            int labelHeight = 18;
            int thumbHeight = Math.Max(30, trackRect.Height - labelHeight);
            int slotWidth = CleanerTimelineThumbWidth + CleanerTimelineThumbGap;

            int visibleSlotCount = GetCleanerTimelineVisibleSlotCount();
            int maxSlotCount = Math.Min(visibleSlotCount, visibleFrames.Count - cleanerTimelineStartIndex);

            using Font indexFont = new Font("Consolas", 7F, FontStyle.Bold);
            using SolidBrush textBrush = new SolidBrush(Color.White);
            using SolidBrush missingBrush = new SolidBrush(Color.FromArgb(55, 65, 85));
            using Pen normalPen = new Pen(Color.FromArgb(80, 255, 255, 255), 1);

            for (int slot = 0; slot < maxSlotCount; slot++)
            {
                int frameIndex = cleanerTimelineStartIndex + slot;

                if (frameIndex < 0 || frameIndex >= visibleFrames.Count)
                    continue;

                DonkeyFrame frame = visibleFrames[frameIndex];

                int x = trackRect.Left + slot * slotWidth;
                Rectangle thumbRect = new Rectangle(x, trackRect.Top, CleanerTimelineThumbWidth, thumbHeight);

                try
                {
                    Bitmap? thumb = GetCleanerTimelineThumbnail(frame, thumbRect.Size);

                    if (thumb != null)
                        g.DrawImage(thumb, thumbRect);
                    else
                    {
                        g.FillRectangle(missingBrush, thumbRect);
                        g.DrawString("No Image", indexFont, textBrush, thumbRect.Left + 8, thumbRect.Top + 16);
                    }
                }
                catch
                {
                    g.FillRectangle(missingBrush, thumbRect);
                    g.DrawString("Error", indexFont, textBrush, thumbRect.Left + 20, thumbRect.Top + 16);
                }

                g.DrawRectangle(normalPen, thumbRect);

                string indexText = frame.Index.ToString("D4");
                g.DrawString(indexText, indexFont, textBrush, thumbRect.Left + 4, thumbRect.Bottom + 2);
            }
            // 마크 표시 (▼ 삼각형)
            using SolidBrush markBrush = new SolidBrush(Color.OrangeRed);
            foreach (int markedIndex in markedFrameIndices)
            {
                int slot = markedIndex - cleanerTimelineStartIndex;
                if (slot < 0 || slot >= maxSlotCount) continue;
                int mx = trackRect.Left + slot * slotWidth + CleanerTimelineThumbWidth / 2;
                PointF[] triangle = new PointF[]
                {
                    new PointF(mx - 7, trackRect.Top - 1),
                    new PointF(mx + 7, trackRect.Top - 1),
                    new PointF(mx, trackRect.Top + 12)
                };
                g.FillPolygon(markBrush, triangle);
            }
        }

        private Bitmap? GetCleanerTimelineThumbnail(DonkeyFrame frame, Size targetSize)
        {
            if (string.IsNullOrWhiteSpace(imagesFolderPath))
                return null;

            string imagePath = Path.Combine(imagesFolderPath, frame.ImageFileName);

            if (!File.Exists(imagePath))
                return null;

            string cacheKey = imagePath + "|" + targetSize.Width + "x" + targetSize.Height;

            if (cleanerTimelineThumbCache.TryGetValue(cacheKey, out Bitmap? cached))
                return cached;

            if (cleanerTimelineThumbCache.Count > 350)
                ClearCleanerTimelineThumbnailCache();

            byte[] bytes = File.ReadAllBytes(imagePath);

            using MemoryStream ms = new MemoryStream(bytes);
            using Bitmap source = new Bitmap(ms);

            Bitmap thumb = new Bitmap(targetSize.Width, targetSize.Height);

            using (Graphics tg = Graphics.FromImage(thumb))
            {
                tg.Clear(Color.Black);
                tg.SmoothingMode = SmoothingMode.AntiAlias;
                tg.InterpolationMode = InterpolationMode.HighQualityBilinear;
                tg.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Rectangle dest = GetImageContainRectangle(
                    new Size(source.Width, source.Height),
                    new Rectangle(0, 0, targetSize.Width, targetSize.Height)
                );

                tg.DrawImage(source, dest);
            }

            cleanerTimelineThumbCache[cacheKey] = thumb;
            return thumb;
        }

        private Rectangle GetImageContainRectangle(Size imageSize, Rectangle box)
        {
            if (imageSize.Width <= 0 || imageSize.Height <= 0 || box.Width <= 0 || box.Height <= 0)
                return Rectangle.Empty;

            double imageRatio = imageSize.Width / (double)imageSize.Height;
            double boxRatio = box.Width / (double)box.Height;

            int width;
            int height;

            if (imageRatio > boxRatio)
            {
                width = box.Width;
                height = (int)(box.Width / imageRatio);
            }
            else
            {
                height = box.Height;
                width = (int)(box.Height * imageRatio);
            }

            int x = box.Left + (box.Width - width) / 2;
            int y = box.Top + (box.Height - height) / 2;

            return new Rectangle(x, y, width, height);
        }

        private void DrawCleanerSelectedRange(Graphics g, Rectangle trackRect)
        {
            if (cleanerRanges.Count == 0)
                return;

            if (visibleFrames.Count == 0)
                return;

            int visibleSlotCount = GetCleanerTimelineVisibleSlotCount();
            int viewStart = cleanerTimelineStartIndex;
            int viewEnd = Math.Min(visibleFrames.Count - 1, viewStart + visibleSlotCount - 1);

            using SolidBrush rangeBrush = new SolidBrush(Color.FromArgb(105, 255, 190, 40));
            using Pen rangePen = new Pen(Color.FromArgb(255, 190, 40), 2);
            using Pen handlePen = new Pen(Color.FromArgb(255, 230, 80), 3);

            foreach (var range in cleanerRanges)
            {
                int start = range.Start;
                int end = range.End;

                if (end < viewStart || start > viewEnd)
                    continue;

                int drawStart = Math.Max(start, viewStart);
                int drawEnd = Math.Min(end, viewEnd);

                Rectangle startRect = GetCleanerFrameSlotRectangle(drawStart, trackRect);
                Rectangle endRect = GetCleanerFrameSlotRectangle(drawEnd, trackRect);

                int x = startRect.Left;
                int right = endRect.Right;
                int width = Math.Max(4, right - x);

                Rectangle fillRect = new Rectangle(x, trackRect.Top, width, trackRect.Height);

                g.FillRectangle(rangeBrush, fillRect);
                g.DrawRectangle(rangePen, fillRect);

                g.DrawLine(handlePen, x, trackRect.Top - 4, x, trackRect.Bottom + 4);
                g.DrawLine(handlePen, right, trackRect.Top - 4, right, trackRect.Bottom + 4);
            }
        }



        private void DrawCleanerCurrentFrameMarker(Graphics g, Rectangle trackRect)
        {
            if (currentIndex < 0 || currentIndex >= visibleFrames.Count)
                return;

            int visibleSlotCount = GetCleanerTimelineVisibleSlotCount();
            int viewStart = cleanerTimelineStartIndex;
            int viewEnd = Math.Min(visibleFrames.Count - 1, cleanerTimelineStartIndex + visibleSlotCount - 1);

            if (currentIndex < viewStart || currentIndex > viewEnd)
                return;

            Rectangle frameRect = GetCleanerFrameSlotRectangle(currentIndex, trackRect);
            float x = frameRect.Left + frameRect.Width / 2f;

            using Pen currentPen = new Pen(Color.White, 3);
            using SolidBrush markerBrush = new SolidBrush(Color.White);

            g.DrawLine(currentPen, x, trackRect.Top - 5, x, trackRect.Bottom + 5);
            g.FillEllipse(markerBrush, x - 5, trackRect.Top - 8, 10, 10);
        }

        private Rectangle GetCleanerFrameSlotRectangle(int frameIndex, Rectangle trackRect)
        {
            int slot = frameIndex - cleanerTimelineStartIndex;
            int slotWidth = CleanerTimelineThumbWidth + CleanerTimelineThumbGap;

            int labelHeight = 18;
            int thumbHeight = Math.Max(30, trackRect.Height - labelHeight);

            int x = trackRect.Left + slot * slotWidth;

            return new Rectangle(x, trackRect.Top, CleanerTimelineThumbWidth, thumbHeight);
        }

        private void DrawCleanerTimelineBorder(Graphics g, Rectangle trackRect)
        {
            using Pen borderPen = new Pen(Color.FromArgb(160, 255, 255, 255), 1);
            g.DrawRectangle(borderPen, trackRect);
        }

        private void ClearCleanerTimelineThumbnailCache()
        {
            foreach (Bitmap bmp in cleanerTimelineThumbCache.Values)
                bmp.Dispose();

            cleanerTimelineThumbCache.Clear();
        }

        private void btnBrowseMycar_Click(object? sender, EventArgs e)
        {
            DialogResult modeResult = MessageBox.Show(
                "WSL 기본 학습 폴더(~/mycar)를 사용하시겠습니까?\n\n" +
                "예: ~/mycar를 바로 입력합니다.\n" +
                "아니요: Windows 폴더 선택창에서 직접 선택합니다.",
                "학습 폴더 선택",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );

            if (modeResult == DialogResult.Cancel)
                return;

            if (modeResult == DialogResult.Yes)
            {
                txtMycarPath.Text = "~/mycar";
                UpdateModelStatus();
                AppendLog("학습 폴더 선택: WSL 기본 경로 ~/mycar");
                return;
            }

            using FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Windows에서 접근 가능한 mycar 폴더를 선택하세요. WSL을 쓰면 취소 후 ~/mycar를 사용하세요.";
            dlg.ShowNewFolderButton = false;

            if (!string.IsNullOrWhiteSpace(txtMycarPath.Text) && Directory.Exists(txtMycarPath.Text))
                dlg.SelectedPath = txtMycarPath.Text;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtMycarPath.Text = dlg.SelectedPath;
                UpdateModelStatus();
                AppendLog("학습 폴더 선택: " + dlg.SelectedPath);
            }
        }

        private async void btnTrain_Click(object? sender, EventArgs e)
        {
            if (trainProcess != null && !trainProcess.HasExited)
            {
                MessageBox.Show("이미 학습이 진행 중입니다.");
                return;
            }

            string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string representativeModelPath = GetSelectedRepresentativeModelPath();

            if (string.IsNullOrWhiteSpace(representativeModelPath))
                representativeModelPath = "./models/mypilot.h5";

            string outputModelPath = BuildSubModelPath(representativeModelPath, timeStamp);
            txtModelPath.Text = representativeModelPath;

            string mycarPath = txtMycarPath.Text.Trim();
            string pythonExe = txtPythonExe.Text.Trim();
            string trainArgs = txtTrainArgs.Text.Trim();

            if (string.IsNullOrWhiteSpace(trainArgs))
                trainArgs =
                    $"train_with_transfer.py --tubs=./data --model={QuoteCommandTokenForDisplay(outputModelPath)} " +
                    $"--transfer={QuoteCommandTokenForDisplay(representativeModelPath)}";

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
            bool hasSelectedDataFolder = !string.IsNullOrWhiteSpace(dataFolderPath) && Directory.Exists(dataFolderPath);

            if (!useWsl && !Directory.Exists(mycarPath))
            {
                MessageBox.Show(
                    "Windows 경로의 mycar 폴더를 찾을 수 없습니다.\n" +
                    "WSL을 사용할 경우 Python 실행명에 wsl을 입력하고 mycar 경로는 ~/mycar로 입력하세요."
                );
                return;
            }

            txtLog.Clear();
            ResetLossGraph();

            try
            {
                trainArgs = await PrepareTrainArgsForStartAsync(
                    trainArgs,
                    outputModelPath,
                    representativeModelPath,
                    useWsl,
                    mycarPath,
                    hasSelectedDataFolder
                );
                txtTrainArgs.Text = trainArgs;

                await EnsureTrainWithTransferScriptAsync(useWsl, mycarPath);

                if (hasSelectedDataFolder)
                    ConvertCatalogToCsv(dataFolderPath);
            }
            catch (Exception ex)
            {
                AppendLog("학습 준비 실패: " + ex.Message);
                MessageBox.Show("학습 준비에 실패했습니다.\n\n" + ex.Message);
                return;
            }

            string versionModelPath = GetModelPathFromTrainArgs(trainArgs);

            trainEndRequested = false;
            isTrainPaused = false;
            currentTrainUsesWsl = useWsl;
            currentTrainVersionModelPath = versionModelPath;
            currentRepresentativeModelPath = representativeModelPath;
            currentTrainEpochText = "";
            currentWslTrainProcessGroupId = -1;
            currentWslTrainPidFilePath = "";
            trainPauseSignalPending = false;

            ResetTrainingProgress("진행도: 준비 중");
            btnTrain.Enabled = false;
            btnStopTrain.Enabled = !useWsl;
            btnStopTrain.Text = "일시정지";
            btnEndTrain.Enabled = true;
            btnEndTrain.Text = "학습 종료";
            SetTrainingExtensionControlsEnabled(false);
            lblModelStatus.Text = "모델 상태: 학습 준비 중";

            AppendLog("학습 시작");
            AppendLog("실행 방식: " + (useWsl ? "WSL + Conda" : "Windows Python"));
            AppendLog("mycar 경로 = " + mycarPath);
            AppendLog("학습 인자 = " + trainArgs);
            AppendLog("대표 모델 = " + representativeModelPath);
            AppendLog("저장 모델 = " + versionModelPath);
            AppendLog("학습 완료 후 대표 모델에 반영됩니다.");

            if (useWsl)
                AppendLog("WSL 학습 프로세스 PID를 확인한 뒤 일시정지가 활성화됩니다.");

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
                        BeginInvoke(new Action(() => HandleTrainingOutput(ev.Data)));
                };

                trainProcess.ErrorDataReceived += (s, ev) =>
                {
                    if (!string.IsNullOrWhiteSpace(ev.Data))
                        BeginInvoke(new Action(() => HandleTrainingOutput(ev.Data, true)));
                };

                trainProcess.Start();
                trainProcess.BeginOutputReadLine();
                trainProcess.BeginErrorReadLine();
                SetTrainingProgress(0, "진행도: 학습 중");
                lblModelStatus.Text = "모델 상태: 학습 중";

                await Task.Run(() => trainProcess.WaitForExit());

                // trainProcess가 null이 되지 않았는지 확인 (경쟁 조건 방지)
                if (trainProcess == null)
                {
                    AppendLog("[오류] 학습 프로세스가 예상치 않게 null이 되었습니다.");
                    return;
                }

                int exitCode = trainProcess.ExitCode;
                bool endedByUser = trainEndRequested;

                AppendLog((endedByUser ? "학습 종료 요청 완료. ExitCode = " : "학습 종료. ExitCode = ") + exitCode);

                if (exitCode == 0 || endedByUser)
                {
                    try
                    {
                        string? savedModelPath = exitCode == 0
                            ? versionModelPath
                            : await FindBestSavedModelPathAsync(useWsl, mycarPath, versionModelPath);

                        if (!string.IsNullOrWhiteSpace(savedModelPath)
                            && !await SavedModelExistsAsync(useWsl, mycarPath, savedModelPath))
                        {
                            AppendLog("[경고] 지정 저장 모델을 찾지 못해 최신 저장 모델을 다시 탐색합니다: " + savedModelPath);
                            savedModelPath = await FindBestSavedModelPathAsync(useWsl, mycarPath, versionModelPath);
                        }

                        if (!string.IsNullOrWhiteSpace(savedModelPath))
                        {
                            await PromoteTrainedModelAsync(useWsl, mycarPath, savedModelPath, currentRepresentativeModelPath);
                            txtModelPath.Text = currentRepresentativeModelPath;

                            SetTrainingProgress(
                                exitCode == 0 ? 100 : prgTrainProgress.Value,
                                exitCode == 0
                                    ? "진행도: 완료 - 대표 모델 적용 완료"
                                    : "진행도: 종료됨 - 저장된 최신 모델 적용 완료"
                            );

                            lblModelStatus.Text = "모델 상태: 대표 모델 갱신 완료";
                        }
                        else
                        {
                            AppendLog("[경고] 종료 시점까지 저장된 모델 파일이 없어 대표 모델을 갱신하지 않았습니다.");
                            SetTrainingProgress(prgTrainProgress.Value, "진행도: 종료됨 - 저장된 모델 없음");
                            lblModelStatus.Text = "모델 상태: 저장된 중간 모델 없음";
                        }
                    }
                    catch (Exception promoteEx)
                    {
                        AppendLog("[경고] 대표 모델 갱신 실패: " + promoteEx.Message);
                        MessageBox.Show(
                            "저장된 모델 파일은 있지만 대표 모델 갱신에 실패했습니다.\n\n" +
                            "시점별 모델 파일은 그대로 남아 있습니다.\n\n" +
                            promoteEx.Message
                        );
                        lblModelStatus.Text = "모델 상태: 대표 모델 갱신 실패";
                    }
                }
                else
                {
                    AppendLog("[경고] 학습이 실패하여 대표 모델을 갱신하지 않았습니다.");
                    SetTrainingProgress(prgTrainProgress.Value, "진행도: 실패");
                    lblModelStatus.Text = "모델 상태: 학습 실패";
                }

                if (exitCode == 0 || endedByUser)
                    UpdateModelStatus();
            }
            catch (Exception ex)
            {
                AppendLog("학습 실행 실패: " + ex.Message);
                SetTrainingProgress(prgTrainProgress.Value, "진행도: 실행 실패");
                lblModelStatus.Text = "모델 상태: 실행 실패";
                MessageBox.Show(
                    "학습 실행에 실패했습니다.\n\n" +
                    "확인할 것:\n" +
                    "1. WSL 이름이 맞는지 확인\n" +
                    "2. Conda 환경 이름이 맞는지 확인\n" +
                    "3. ~/mycar 폴더 안에 data 폴더가 있는지 확인\n" +
                    "4. ~/mycar/data 안에 manifest.json이 있는지 확인\n" +
                    "5. Ubuntu 터미널에서 직접 학습 명령이 되는지 확인\n\n" +
                    ex.Message
                );
            }
            finally
            {
                trainProcess?.Dispose();
                trainProcess = null;
                trainEndRequested = false;
                isTrainPaused = false;
                currentTrainUsesWsl = false;
                currentTrainVersionModelPath = "";
                currentRepresentativeModelPath = GetSelectedRepresentativeModelPath();
                currentTrainEpochText = "";
                currentWslTrainProcessGroupId = -1;
                trainPauseSignalPending = false;

                // PID 파일 정리
                if (!string.IsNullOrWhiteSpace(currentWslTrainPidFilePath) && File.Exists(currentWslTrainPidFilePath))
                {
                    try { File.Delete(currentWslTrainPidFilePath); }
                    catch { }
                }
                currentWslTrainPidFilePath = "";

                btnTrain.Enabled = true;
                btnStopTrain.Enabled = false;
                btnStopTrain.Text = "일시정지";
                btnEndTrain.Enabled = false;
                btnEndTrain.Text = "학습 종료";
                SetTrainingExtensionControlsEnabled(true);
            }
        }
        private void ConvertCatalogToCsv(string dataPath)
        {
            try
            {
                var catalogFiles = Directory.GetFiles(
                    dataPath,
                    "catalog_*.catalog",
                    SearchOption.TopDirectoryOnly
                );

                if (catalogFiles.Length == 0)
                {
                    AppendLog("[경고] catalog 파일 없음");
                    return;
                }

                string csvPath = Path.Combine(dataPath, "training_data.csv");

                using StreamWriter writer = new StreamWriter(
                    csvPath,
                    false,
                    Encoding.UTF8
                );

                // 헤더
                writer.WriteLine("image_path,angle,throttle");

                foreach (var file in catalogFiles)
                {
                    foreach (string line in File.ReadLines(file))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        try
                        {
                            using JsonDocument doc =
                                JsonDocument.Parse(line);

                            var root = doc.RootElement;

                            string img =
                                root.GetProperty("cam/image_array").GetString()
                                ?? "";

                            float angle =
                                root.GetProperty("user/angle").GetSingle();

                            float throttle =
                                root.GetProperty("user/throttle").GetSingle();

                            writer.WriteLine(
                                $"{img},{angle},{throttle}"
                            );
                        }
                        catch
                        {
                            AppendLog("[경고] catalog 파싱 실패");
                        }
                    }
                }

                AppendLog($"CSV 저장 완료: {csvPath}");
            }
            catch (Exception ex)
            {
                AppendLog("CSV 변환 실패: " + ex.Message);
            }
        }

        private async Task EnsurePredictOneScriptAsync()
        {
            string scriptContent =
                "import os\n" +
                "os.environ[\"TF_CPP_MIN_LOG_LEVEL\"] = \"2\"\n" +
                "import argparse\n" +
                "import json\n" +
                "from pathlib import Path\n" +
                "import numpy as np\n" +
                "from PIL import Image\n" +
                "from tensorflow.keras.models import load_model\n" +
                "\n" +
                "def prepare_image(image_path):\n" +
                "    img = Image.open(image_path).convert(\"RGB\")\n" +
                "    img = img.resize((160, 120))\n" +
                "    arr = np.asarray(img, dtype=np.float32) / 255.0\n" +
                "    arr = arr.reshape((1, 120, 160, 3))\n" +
                "    return arr\n" +
                "\n" +
                "def parse_prediction(pred):\n" +
                "    if isinstance(pred, list):\n" +
                "        angle = float(np.squeeze(pred[0]))\n" +
                "        if len(pred) > 1:\n" +
                "            throttle = float(np.squeeze(pred[1]))\n" +
                "        else:\n" +
                "            throttle = 0.0\n" +
                "        return angle, throttle\n" +
                "    arr = np.asarray(pred).reshape(-1)\n" +
                "    if arr.size >= 2:\n" +
                "        return float(arr[0]), float(arr[1])\n" +
                "    if arr.size == 1:\n" +
                "        return float(arr[0]), 0.0\n" +
                "    return 0.0, 0.0\n" +
                "\n" +
                "def main():\n" +
                "    parser = argparse.ArgumentParser()\n" +
                "    parser.add_argument(\"--model\", required=True)\n" +
                "    parser.add_argument(\"--image\", required=True)\n" +
                "    args = parser.parse_args()\n" +
                "    model_path = Path(args.model).expanduser()\n" +
                "    image_path = Path(args.image).expanduser()\n" +
                "    if not model_path.exists():\n" +
                "        print(json.dumps({\"ok\": False, \"error\": f\"Model file not found: {model_path}\"}))\n" +
                "        return\n" +
                "    if not image_path.exists():\n" +
                "        print(json.dumps({\"ok\": False, \"error\": f\"Image file not found: {image_path}\"}))\n" +
                "        return\n" +
                "    model = load_model(model_path, compile=False)\n" +
                "    x = prepare_image(image_path)\n" +
                "    pred = model.predict(x, verbose=0)\n" +
                "    angle, throttle = parse_prediction(pred)\n" +
                "    print(json.dumps({\"ok\": True, \"angle\": angle, \"throttle\": throttle, \"model\": str(model_path), \"image\": str(image_path)}))\n" +
                "\n" +
                "if __name__ == \"__main__\":\n" +
                "    main()\n";

            string serverScriptContent =
                "import os\n" +
                "os.environ[\"TF_CPP_MIN_LOG_LEVEL\"] = \"2\"\n" +
                "import argparse\n" +
                "import json\n" +
                "import sys\n" +
                "import traceback\n" +
                "from pathlib import Path\n" +
                "import numpy as np\n" +
                "from PIL import Image\n" +
                "from tensorflow.keras.models import load_model\n" +
                "\n" +
                "def write_json(obj):\n" +
                "    print(json.dumps(obj, ensure_ascii=False), flush=True)\n" +
                "\n" +
                "def prepare_image(image_path):\n" +
                "    img = Image.open(image_path).convert(\"RGB\")\n" +
                "    img = img.resize((160, 120))\n" +
                "    arr = np.asarray(img, dtype=np.float32) / 255.0\n" +
                "    arr = arr.reshape((1, 120, 160, 3))\n" +
                "    return arr\n" +
                "\n" +
                "def parse_prediction(pred):\n" +
                "    if isinstance(pred, list):\n" +
                "        angle = float(np.squeeze(pred[0]))\n" +
                "        throttle = float(np.squeeze(pred[1])) if len(pred) > 1 else 0.0\n" +
                "        return angle, throttle\n" +
                "    arr = np.asarray(pred).reshape(-1)\n" +
                "    if arr.size >= 2:\n" +
                "        return float(arr[0]), float(arr[1])\n" +
                "    if arr.size == 1:\n" +
                "        return float(arr[0]), 0.0\n" +
                "    return 0.0, 0.0\n" +
                "\n" +
                "def main():\n" +
                "    parser = argparse.ArgumentParser()\n" +
                "    parser.add_argument(\"--model\", required=True)\n" +
                "    args = parser.parse_args()\n" +
                "    model_path = Path(args.model).expanduser()\n" +
                "    if not model_path.exists():\n" +
                "        write_json({\"ok\": False, \"ready\": False, \"error\": f\"Model file not found: {model_path}\"})\n" +
                "        return\n" +
                "    try:\n" +
                "        model = load_model(model_path, compile=False)\n" +
                "        write_json({\"ok\": True, \"ready\": True, \"model\": str(model_path)})\n" +
                "    except Exception as ex:\n" +
                "        write_json({\"ok\": False, \"ready\": False, \"error\": str(ex)})\n" +
                "        return\n" +
                "    for raw in sys.stdin:\n" +
                "        raw = raw.strip()\n" +
                "        if not raw:\n" +
                "            continue\n" +
                "        try:\n" +
                "            req = json.loads(raw)\n" +
                "            image_path = Path(req.get(\"image\", \"\")).expanduser()\n" +
                "            if not image_path.exists():\n" +
                "                write_json({\"ok\": False, \"error\": f\"Image file not found: {image_path}\"})\n" +
                "                continue\n" +
                "            x = prepare_image(image_path)\n" +
                "            pred = model.predict(x, verbose=0)\n" +
                "            angle, throttle = parse_prediction(pred)\n" +
                "            write_json({\"ok\": True, \"angle\": angle, \"throttle\": throttle, \"image\": str(image_path)})\n" +
                "        except Exception as ex:\n" +
                "            write_json({\"ok\": False, \"error\": str(ex), \"traceback\": traceback.format_exc(limit=2)})\n" +
                "\n" +
                "if __name__ == \"__main__\":\n" +
                "    main()\n";

            string tempPath = Path.Combine(Path.GetTempPath(), "predict_one.py");
            string tempServerPath = Path.Combine(Path.GetTempPath(), "predict_server.py");
            File.WriteAllText(tempPath, scriptContent, new System.Text.UTF8Encoding(false));
            File.WriteAllText(tempServerPath, serverScriptContent, new System.Text.UTF8Encoding(false));

            string wslTempPath = ConvertPathToWslPath(tempPath);
            string wslTempServerPath = ConvertPathToWslPath(tempServerPath);
            string command =
                $"cp {BashQuote(wslTempPath)} ~/mycar/predict_one.py && " +
                $"cp {BashQuote(wslTempServerPath)} ~/mycar/predict_server.py";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "wsl.exe",
                Arguments = $"-d {WslDistroName} -- bash -lc {QuoteWindowsArgument(command)}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = new Process();
            process.StartInfo = psi;
            process.Start();

            await Task.Run(() => process.WaitForExit());

            if (process.ExitCode == 0)
                AppendLog("[정보] predict_one.py / predict_server.py 자동 생성 완료");
            else
                AppendLog("[경고] 예측 스크립트 자동 생성 실패 - 수동으로 넣어주세요");
        }

        private async Task EnsureTrainWithTransferScriptAsync(bool useWsl, string mycarPath)
        {
            string scriptContent = BuildTrainWithTransferScriptContent();

            if (useWsl)
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "train_with_transfer.py");
                File.WriteAllText(tempPath, scriptContent, new System.Text.UTF8Encoding(false));

                string wslTempPath = ConvertPathToWslPath(tempPath);
                string wslMycarPath = ConvertPathToWslPath(string.IsNullOrWhiteSpace(mycarPath) ? "~/mycar" : mycarPath);
                string command =
                    $"cd {BashCdArgument(wslMycarPath)} && " +
                    $"cp {BashQuote(wslTempPath)} ./train_with_transfer.py && " +
                    "chmod +x ./train_with_transfer.py";

                (int exitCode, _, string error) = await RunWslCommandAsync(command);

                if (exitCode != 0)
                    throw new Exception("train_with_transfer.py 자동 생성 실패: " + error.Trim());

                AppendLog("[정보] train_with_transfer.py 자동 생성 완료");
                return;
            }

            if (string.IsNullOrWhiteSpace(mycarPath))
                throw new InvalidOperationException("mycar 경로를 확인할 수 없습니다.");

            Directory.CreateDirectory(mycarPath);
            string localScriptPath = Path.Combine(mycarPath, "train_with_transfer.py");
            File.WriteAllText(localScriptPath, scriptContent, new System.Text.UTF8Encoding(false));
            AppendLog("[정보] train_with_transfer.py 자동 생성 완료: " + localScriptPath);
        }

        private string BuildTrainWithTransferScriptContent()
        {
            return
                "#!/usr/bin/env python3\n" +
                "\"\"\"\n" +
                "Scripts to train a keras model using tensorflow with transfer support.\n" +
                "\n" +
                "Usage:\n" +
                "    train_with_transfer.py [--tubs=tubs] (--model=<model>)\n" +
                "    [--transfer=<transfer>]\n" +
                "    [--type=(linear|inferred|tensorrt_linear|tflite_linear)]\n" +
                "    [--comment=<comment>]\n" +
                "\n" +
                "Options:\n" +
                "    -h --help              Show this screen.\n" +
                "\"\"\"\n" +
                "\n" +
                "from docopt import docopt\n" +
                "import os\n" +
                "import sys\n" +
                "import donkeycar as dk\n" +
                "from donkeycar.pipeline.training import train\n" +
                "\n" +
                "\n" +
                "def expand_path(value):\n" +
                "    if value is None:\n" +
                "        return None\n" +
                "    return os.path.expanduser(value)\n" +
                "\n" +
                "\n" +
                "def require_transfer_model(transfer):\n" +
                "    if not transfer:\n" +
                "        print('[DONKEYCAR_TRANSFER] loading=<none>', flush=True)\n" +
                "        return\n" +
                "    if not os.path.isfile(transfer):\n" +
                "        print(f'[DONKEYCAR_TRANSFER_ERROR] model file not found: {transfer}', flush=True)\n" +
                "        sys.exit(2)\n" +
                "    print(f'[DONKEYCAR_TRANSFER] loading={transfer}', flush=True)\n" +
                "\n" +
                "\n" +
                "def main():\n" +
                "    args = docopt(__doc__)\n" +
                "    cfg = dk.load_config()\n" +
                "    tubs = args['--tubs'] or './data'\n" +
                "    model = expand_path(args['--model'])\n" +
                "    model_type = args['--type']\n" +
                "    transfer = expand_path(args['--transfer'])\n" +
                "    comment = args['--comment']\n" +
                "    require_transfer_model(transfer)\n" +
                "    print(f'[DONKEYCAR_OUTPUT] model={model}', flush=True)\n" +
                "    train(cfg, tubs, model=model, model_type=model_type, transfer=transfer, comment=comment)\n" +
                "\n" +
                "\n" +
                "if __name__ == \"__main__\":\n" +
                "    main()\n";
        }

        private async void btnStopTrain_Click(object? sender, EventArgs e)
        {
            try
            {
                await ToggleTrainingPauseAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("학습 일시정지/재시작 실패\n\n" + ex.Message);
            }
        }

        private async void btnEndTrain_Click(object? sender, EventArgs e)
        {
            try
            {
                await RequestTrainingEndAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("학습 종료 실패\n\n" + ex.Message);
            }
        }

        private async Task ToggleTrainingPauseAsync()
        {
            if (trainProcess == null || trainProcess.HasExited)
            {
                MessageBox.Show("진행 중인 학습이 없습니다.");
                return;
            }

            if (trainEndRequested)
                return;

            btnStopTrain.Enabled = false;

            try
            {
                if (!isTrainPaused)
                {
                    trainPauseSignalPending = true;
                    isTrainPaused = true;
                    btnStopTrain.Text = "재시작";
                    lblModelStatus.Text = "모델 상태: 일시정지 요청";
                    AppendLog("학습 일시정지 요청");

                    bool paused = await PauseTrainingProcessAsync();

                    if (!paused)
                    {
                        trainPauseSignalPending = false;
                        isTrainPaused = false;
                        btnStopTrain.Text = "일시정지";
                        lblModelStatus.Text = "모델 상태: 학습 중";
                        AppendLog("[경고] 학습 일시정지에 실패했습니다.");
                        return;
                    }

                    trainPauseSignalPending = false;
                    lblModelStatus.Text = "모델 상태: 일시정지";
                    AppendLog("학습 일시정지");
                }
                else
                {
                    lblModelStatus.Text = "모델 상태: 재시작 요청";

                    bool resumed = await ResumeTrainingProcessAsync();

                    if (!resumed)
                    {
                        lblModelStatus.Text = "모델 상태: 일시정지";
                        AppendLog("[경고] 학습 재시작에 실패했습니다.");
                        return;
                    }

                    isTrainPaused = false;
                    trainPauseSignalPending = false;
                    btnStopTrain.Text = "일시정지";
                    lblModelStatus.Text = "모델 상태: 학습 중";
                    AppendLog("학습 재시작");
                }
            }
            finally
            {
                if (trainProcess != null && !trainProcess.HasExited && !trainEndRequested)
                    btnStopTrain.Enabled = true;
            }
        }

        private async Task RequestTrainingEndAsync()
        {
            if (trainProcess == null || trainProcess.HasExited)
            {
                MessageBox.Show("진행 중인 학습이 없습니다.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "학습을 종료할까요?\n\n" +
                "종료 신호를 보낸 뒤 현재까지 저장된 최신 모델을 대표 모델로 적용합니다.\n" +
                "일시정지 중이면 먼저 재시작한 뒤 종료합니다.",
                "학습 종료",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            trainEndRequested = true;
            btnStopTrain.Enabled = false;
            btnEndTrain.Enabled = false;
            btnEndTrain.Text = "종료 중...";
            lblModelStatus.Text = "모델 상태: 학습 종료 요청";

            if (!string.IsNullOrWhiteSpace(currentTrainVersionModelPath))
                AppendLog("저장 확인 대상 모델 = " + currentTrainVersionModelPath);

            if (isTrainPaused)
            {
                bool resumed = await ResumeTrainingProcessAsync();
                isTrainPaused = false;
                trainPauseSignalPending = false;
                btnStopTrain.Text = "일시정지";

                if (resumed)
                    AppendLog("종료 처리를 위해 일시정지된 학습을 재시작했습니다.");
            }

            if (currentTrainUsesWsl)
                await RequestWslTrainingEndAsync();
            else
                await RequestLocalTrainingEndAsync();
        }

        private async Task<bool> PauseTrainingProcessAsync()
        {
            if (currentTrainUsesWsl)
                return await SendWslTrainingSignalAsync("STOP");

            return SuspendLocalTrainingProcess();
        }

        private async Task<bool> ResumeTrainingProcessAsync()
        {
            if (currentTrainUsesWsl)
                return await SendWslTrainingSignalAsync("CONT");

            return ResumeLocalTrainingProcess();
        }

        private bool SuspendLocalTrainingProcess()
        {
            if (trainProcess == null || trainProcess.HasExited)
                return false;

            return NtSuspendProcess(trainProcess.Handle) == 0;
        }

        private bool ResumeLocalTrainingProcess()
        {
            if (trainProcess == null || trainProcess.HasExited)
                return false;

            return NtResumeProcess(trainProcess.Handle) == 0;
        }

        private async Task RequestWslTrainingEndAsync()
        {
            if (currentWslTrainProcessGroupId <= 0)
            {
                AppendLog("[경고] WSL 학습 PID를 알 수 없어 래퍼 프로세스를 종료합니다.");
                trainProcess?.Kill(true);
                return;
            }

            AppendLog("학습 종료 신호 전송: SIGINT");
            await SendWslTrainingSignalAsync("INT");

            if (await WaitForTrainingExitAsync(120000))
                return;

            AppendLog("[경고] 학습이 120초 안에 종료되지 않아 SIGTERM을 전송합니다.");
            await SendWslTrainingSignalAsync("TERM");

            if (await WaitForTrainingExitAsync(30000))
                return;

            AppendLog("[경고] 학습이 계속 실행 중이라 마지막으로 SIGKILL을 전송합니다.");
            await SendWslTrainingSignalAsync("KILL");
        }

        private async Task RequestLocalTrainingEndAsync()
        {
            AppendLog("[경고] Windows Python 직접 실행은 안전 종료 신호를 보낼 수 없어 현재 저장된 모델만 적용합니다.");

            Process? process = trainProcess;

            if (process != null && !process.HasExited)
                await Task.Run(() => process.Kill(true));
        }

        private async Task<bool> WaitForTrainingExitAsync(int milliseconds)
        {
            Process? process = trainProcess;

            if (process == null)
                return true;

            if (process.HasExited)
                return true;

            return await Task.Run(() => process.WaitForExit(milliseconds));
        }

        private async Task<bool> SendWslTrainingSignalAsync(string signal)
        {
            if (currentWslTrainProcessGroupId <= 0)
            {
                AppendLog("[경고] WSL 학습 PID를 아직 찾지 못했습니다.");
                return false;
            }

            string command = BuildWslTrainingSignalCommand(signal, currentWslTrainProcessGroupId);

            (int exitCode, string output, string error) = await RunWslCommandAsync(command);

            if (!string.IsNullOrWhiteSpace(output))
                AppendLog(output.Trim());

            if (exitCode != 0)
            {
                AppendLog("[경고] WSL 신호 전송 실패: " + error.Trim());
                return false;
            }

            return true;
        }

        private string BuildWslTrainingSignalCommand(string signal, int rootPid)
        {
            signal = signal.ToUpperInvariant();

            if (signal == "STOP")
                return
                    $"kill -STOP -- -{rootPid} 2>/dev/null || kill -STOP {rootPid} 2>/dev/null; " +
                    $"kill -0 {rootPid} 2>/dev/null";

            if (signal == "CONT")
                return
                    $"kill -CONT -- -{rootPid} 2>/dev/null || kill -CONT {rootPid} 2>/dev/null; " +
                    $"kill -0 {rootPid} 2>/dev/null";

            return
                $"kill -{signal} -- -{rootPid} 2>/dev/null || " +
                $"kill -{signal} {rootPid} 2>/dev/null || true";
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
                MessageBox.Show("먼저 Cleaner 탭에서 사용할 이미지를 선택하세요.");
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
                    await PredictImageWithServerAsync(modelPath, imagePath);

                ApplyPilotPrediction(frame, predictedAngle, predictedThrottle);

                AppendLog($"실제 Angle = {frame.Angle:F4}");
                AppendLog($"예측 Angle = {predictedAngle:F4}");
                AppendLog($"Angle Error = {Math.Abs(frame.Angle - predictedAngle):F4}");
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
                    "1. ~/mycar/predict_server.py 파일이 있는지 확인\n" +
                    "2. ~/mycar/models/mypilot.h5 파일이 있는지 확인\n" +
                    "3. 현재 선택한 이미지 파일이 실제로 존재하는지 확인\n" +
                    "4. WSL 이름과 Conda 환경 이름이 맞는지 확인\n\n" +
                    ex.Message
                );
            }
        }

        private string GetPilotModelPath()
        {
            string modelPath = txtModelPath.Text.Trim();

            if (string.IsNullOrWhiteSpace(modelPath))
            {
                modelPath = "~/mycar/models/mypilot.h5";
                txtModelPath.Text = modelPath;
            }

            return modelPath;
        }

        private void QueueLivePredictionForCurrentFrame()
        {
            if (!pilotAutoPlayTimer.Enabled)
                return;

            if (currentIndex < 0 || currentIndex >= visibleFrames.Count)
                return;

            livePredictVersion++;

            if (livePredictInFlight)
            {
                livePredictPending = true;
                return;
            }

            livePredictInFlight = true;
            _ = RunLivePredictionAsync(livePredictVersion);
        }

        private async Task RunLivePredictionAsync(int requestVersion)
        {
            try
            {
                while (true)
                {
                    livePredictPending = false;

                    if (!pilotAutoPlayTimer.Enabled || currentIndex < 0 || currentIndex >= visibleFrames.Count)
                        return;

                    int frameIndex = currentIndex;
                    DonkeyFrame frame = visibleFrames[frameIndex];
                    string imagePath = Path.Combine(imagesFolderPath, frame.ImageFileName);
                    string modelPath = GetPilotModelPath();

                    if (requestVersion == livePredictVersion && currentIndex == frameIndex)
                    {
                        lblPredictedAngle.Text = "예측 Angle: 계산 중...";
                        lblPredictedThrottle.Text = "예측 Throttle: 계산 중...";
                        lblAngleError.Text = "Angle Error: 계산 중...";
                    }

                    try
                    {
                        (double predictedAngle, double predictedThrottle) =
                            await PredictImageWithServerAsync(modelPath, imagePath);

                        if (requestVersion == livePredictVersion
                            && pilotAutoPlayTimer.Enabled
                            && currentIndex == frameIndex)
                        {
                            livePredictFailureCount = 0;
                            ApplyPilotPrediction(frame, predictedAngle, predictedThrottle);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (requestVersion == livePredictVersion && pilotAutoPlayTimer.Enabled)
                            ApplyLivePredictionFailure(ex);
                    }

                    if (!livePredictPending || !pilotAutoPlayTimer.Enabled)
                        return;

                    requestVersion = livePredictVersion;
                }
            }
            finally
            {
                livePredictInFlight = false;

                if (livePredictPending && pilotAutoPlayTimer.Enabled)
                    QueueLivePredictionForCurrentFrame();
            }
        }

        private void ApplyPilotPrediction(DonkeyFrame frame, double predictedAngle, double predictedThrottle)
        {
            double angleError = Math.Abs(frame.Angle - predictedAngle);

            lblPredictedAngle.Text = $"예측 Angle: {predictedAngle:F4}";
            lblPredictedThrottle.Text = $"예측 Throttle: {predictedThrottle:F4}";
            lblAngleError.Text = $"Angle Error: {angleError:F4}";

            lblPilotWarning.ForeColor = GetErrorColor(angleError);
            lblPilotWarning.Text = "판정: " + GetErrorMessage(angleError);

            overlayActualAngle = frame.Angle;
            overlayPredictedAngle = predictedAngle;
            overlayActualThrottle = frame.Throttle;
            overlayPredictedThrottle = predictedThrottle;

            picPilotTest.Invalidate();
        }

        private void ApplyLivePredictionFailure(Exception ex)
        {
            livePredictFailureCount++;

            lblPredictedAngle.Text = "예측 Angle: 실패";
            lblPredictedThrottle.Text = "예측 Throttle: 실패";
            lblAngleError.Text = "Angle Error: 실패";
            lblPilotWarning.Text = "판정: 실시간 예측 실패";
            lblPilotWarning.ForeColor = Color.Red;

            if (livePredictFailureCount <= 3)
                AppendLog("실시간 예측 실패: " + ex.Message);
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

        private void LoadCatalog()
        {
            allFrames.Clear();
            ClearCleanerTimelineThumbnailCache();

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
                ResetCleanerRange();

                if (visibleFrames.Count > 0)
                    ShowFrame(0);
                else
                    ClearViewer();

                AppendLog(
                    $"로드 완료: {visibleFrames.Count}개 프레임 / catalog 파일 {catalogFiles.Length}개 / 전체 줄 {totalLines}개"
                );
                AppendLog(DataSummary.Calculate(visibleFrames).ToString());

                if (parseErrorCount > 0)
                    AppendLog($"catalog 파싱 실패 줄: {parseErrorCount}개");

                UpdateModelStatus();
                ScanAndRestoreBackupFolders();
            }
            catch (Exception ex)
            {
                MessageBox.Show("catalog 파일을 읽는 중 오류가 발생했습니다.\n\n" + ex.Message);
            }
        }

        private void ScanAndRestoreBackupFolders()
        {
            if (string.IsNullOrWhiteSpace(dataFolderPath))
                return;

            string backupRoot = Path.Combine(dataFolderPath, "backup");

            if (!Directory.Exists(backupRoot))
                return;

            backupFolderPaths = Directory
                .GetDirectories(backupRoot)
                .OrderBy(path => path)
                .ToList();
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

        private string BackupCatalogFiles()
        {
            try
            {
                string[] catalogFiles = GetCatalogFiles();

                if (catalogFiles.Length == 0)
                    return string.Empty;

                // 통합 백업 폴더 생성
                string backupRoot = Path.Combine(dataFolderPath, "backup");
                Directory.CreateDirectory(backupRoot);

                string backupFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFolder = Path.Combine(backupRoot, backupFolderName);
                Directory.CreateDirectory(backupFolder);

                // 카탈로그 전용 폴더 생성
                string catalogBackupDir = Path.Combine(backupFolder, "catalog");
                Directory.CreateDirectory(catalogBackupDir);

                // 카탈로그 복사
                foreach (string catalogFile in catalogFiles)
                {
                    string dest = Path.Combine(catalogBackupDir, Path.GetFileName(catalogFile));
                    File.Copy(catalogFile, dest, true);
                }

                AppendLog($"통합 백업 폴더 생성 완료: {backupFolder}");
                backupFolderPaths.Add(backupFolder);
                return backupFolder; // 백업된 통합 폴더 경로를 반환!
            }
            catch (Exception ex)
            {
                AppendLog("통합 백업 실패: " + ex.Message);
                return string.Empty;
            }
        }


        private void UndoLastDelete()
        {
            if (backupFolderPaths.Count == 0)
            {
                MessageBox.Show("되돌릴 내용이 없습니다.");
                return;
            }

            // 백업 목록을 드롭다운으로 보여주기
            using Form selectForm = new Form();
            selectForm.Text = "되돌릴 시점 선택";
            selectForm.Size = new Size(500, 300);
            selectForm.StartPosition = FormStartPosition.CenterParent;
            selectForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            selectForm.MaximizeBox = false;

            ListBox lst = new ListBox();
            lst.Dock = DockStyle.Fill;
            lst.Font = new Font("맑은 고딕", 10F);

            foreach (string path in backupFolderPaths)
            {
                string folderName = Path.GetFileName(path);
                // yyyyMMdd_HHmmss → yyyy-MM-dd HH:mm:ss
                string display = folderName.Length == 15
                    ? $"{folderName.Substring(0, 4)}-{folderName.Substring(4, 2)}-{folderName.Substring(6, 2)} {folderName.Substring(9, 2)}:{folderName.Substring(11, 2)}:{folderName.Substring(13, 2)}"
                    : folderName;
                lst.Items.Add(display);
            }

            lst.SelectedIndex = lst.Items.Count - 1;

            Button btnOk = new Button();
            btnOk.Text = "복원";
            btnOk.Dock = DockStyle.Bottom;
            btnOk.Height = 50;
            btnOk.DialogResult = DialogResult.OK;

            selectForm.Controls.Add(lst);
            selectForm.Controls.Add(btnOk);
            selectForm.AcceptButton = btnOk;

            if (selectForm.ShowDialog() != DialogResult.OK)
                return;

            if (lst.SelectedIndex < 0)
                return;

            DialogResult warn = MessageBox.Show(
            "선택한 시점으로 복원하면 현재 상태로 다시 돌아올 수 없습니다.\n\n계속하시겠습니까?",
            "복원 확인",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
            );

            if (warn != DialogResult.Yes)
                return;

            string selectedPath = backupFolderPaths[lst.SelectedIndex];

            if (!Directory.Exists(selectedPath))
            {
                MessageBox.Show("선택한 백업 폴더가 존재하지 않습니다.");
                return;
            }

            try
            {
                string catalogBackupDir = Path.Combine(selectedPath, "catalog");
                if (Directory.Exists(catalogBackupDir))
                {
                    foreach (string file in Directory.GetFiles(catalogBackupDir))
                    {
                        string dest = Path.Combine(dataFolderPath, Path.GetFileName(file));
                        File.Copy(file, dest, true);
                    }
                }

                string imgBackupDir = Path.Combine(selectedPath, "images");
                if (Directory.Exists(imgBackupDir))
                {
                    foreach (string file in Directory.GetFiles(imgBackupDir))
                    {
                        string dest = Path.Combine(imagesFolderPath, Path.GetFileName(file));
                        File.Copy(file, dest, true);
                    }
                }

                backupFolderPaths.Clear();
                LoadCatalog();

                AppendLog("되돌리기 완료");
                MessageBox.Show("이전 상태로 복원되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("되돌리기 실패\n\n" + ex.Message);
            }
        }



        private void BindFrameLists()
        {
            isUpdatingSelection = true;

            lstCleanerFrames.BeginUpdate();
            lstPilotFrames.BeginUpdate();

            lstCleanerFrames.Items.Clear();
            lstPilotFrames.Items.Clear();

            foreach (DonkeyFrame frame in visibleFrames)
            {
                string text =
                    $"{frame.Index:D5} | angle={frame.Angle:F3} | throttle={frame.Throttle:F3} | mode={frame.Mode}";

                lstCleanerFrames.Items.Add(text);
                lstPilotFrames.Items.Add(text);
            }

            lstCleanerFrames.EndUpdate();
            lstPilotFrames.EndUpdate();

            isUpdatingSelection = false;

            cleanerTimelineStartIndex = 0;
            UpdateCleanerTimelineScrollBar();

            pnlCleanerTimeline.Invalidate();
            UpdateCleanerRangeUi();
        }

        private void ShowFrame(int index)
        {
            if (index < 0 || index >= visibleFrames.Count)
                return;

            currentIndex = index;
            DonkeyFrame frame = visibleFrames[index];

            string imagePath = Path.Combine(imagesFolderPath, frame.ImageFileName);

            LoadImageToPictureBox(picCleanerPreview, imagePath);
            LoadImageToPictureBox(picPilotTest, imagePath);

            overlayActualAngle = null;
            overlayPredictedAngle = null;
            overlayActualThrottle = null;
            overlayPredictedThrottle = null;
            picPilotTest.Invalidate();

            lblCleanerInfo.Text =
                $"선택 프레임 정보: index={frame.Index}, angle={frame.Angle:F4}, throttle={frame.Throttle:F4}, mode={frame.Mode}";

            lblActualAngle.Text = $"실제 Angle: {frame.Angle:F4}";
            lblActualThrottle.Text = $"실제 Throttle: {frame.Throttle:F4}";
            lblPredictedAngle.Text = "예측 Angle: -";
            lblPredictedThrottle.Text = "예측 Throttle: -";
            lblAngleError.Text = "Angle Error: -";
            lblPilotWarning.Text = "판정: -";
            lblPilotWarning.ForeColor = Color.DimGray;

            isUpdatingSelection = true;

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

            EnsureCleanerTimelineFrameVisible(index);
            pnlCleanerTimeline.Invalidate();

            if (pilotAutoPlayTimer.Enabled)
                QueueLivePredictionForCurrentFrame();
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

                    if (pictureBox == picCleanerPreview)
                    {
                        Bitmap adjusted = ApplyBrightnessContrast(temp, trbBrightness.Value, trbContrast.Value);

                        if (chkFlipHorizontal != null && chkFlipHorizontal.Checked)
                            adjusted.RotateFlip(RotateFlipType.RotateNoneFlipX);

                        if (chkGrayscale != null && chkGrayscale.Checked)
                        {
                            Bitmap gray = new Bitmap(adjusted.Width, adjusted.Height);
                            using Graphics g = Graphics.FromImage(gray);
                            System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix(new float[][]
                            {
                                new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
                                new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
                                new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
                                new float[] { 0, 0, 0, 1, 0 },
                                new float[] { 0, 0, 0, 0, 1 }
                            });
                            using System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();
                            ia.SetColorMatrix(cm);
                            g.DrawImage(adjusted, new Rectangle(0, 0, adjusted.Width, adjusted.Height),
                                0, 0, adjusted.Width, adjusted.Height, GraphicsUnit.Pixel, ia);
                            adjusted = gray;
                        }

                        pictureBox.Image = adjusted;
                    }
                    else
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
            visibleFrames = allFrames.ToList();



            isUpdatingSelection = true;
            lstCleanerFrames.BeginUpdate();

            lstCleanerFrames.SelectionMode = SelectionMode.MultiSimple;
            lstCleanerFrames.ClearSelected(); int count = 0;
            bool hasFilter = chkThrottlePositive.Checked || chkExcludeZeroAngle.Checked || chkStopDataOnly.Checked || chkExcludeJitterAngle.Checked;

            if (hasFilter)
            {
                List<DonkeyFrame> cleanFrames = new List<DonkeyFrame>();

                foreach (DonkeyFrame f in allFrames)
                {
                    bool isGood = true;

                    // 살릴 데이터의 조건 (조건 불만족시 걸러냄)
                    if (chkThrottlePositive.Checked && f.Throttle <= 0)
                        isGood = false;

                    if (chkExcludeZeroAngle.Checked && Math.Abs(f.Angle) <= 0.000001)
                        isGood = false;

                    if (chkStopDataOnly.Checked && Math.Abs(f.Throttle) > 0.000001)
                        isGood = false;
                    if (chkExcludeJitterAngle.Checked && Math.Abs(f.Angle) <= 0.2)
                        isGood = false;

                    if (isGood)
                        cleanFrames.Add(f);  // 보여줄 정상 데이터만 추가
                    else
                        count++;             // 숨겨질 쓰레기 데이터 카운트
                }

                // 화면엔 깨끗한 정상 데이터만 보임!
                visibleFrames = cleanFrames;
            }
            else
            {
                visibleFrames = allFrames.ToList();
            }

            BindFrameLists();
            ResetCleanerRange();

            if (visibleFrames.Count > 0)
                ShowFrame(0);
            else
                ClearViewer();

            if (hasFilter)
                AppendLog($"필터 적용:필터링된 데이터 {count}개가 화면에서 숨겨졌습니다.");
            else
                AppendLog("필터 해제: 전체 데이터 표시");
        }




        private void ClearViewer()
        {
            currentIndex = -1;

            DisposeCurrentImages();

            overlayActualAngle = null;
            overlayPredictedAngle = null;
            overlayActualThrottle = null;
            overlayPredictedThrottle = null;

            lblCleanerInfo.Text = "선택 프레임 정보: -";

            lblActualAngle.Text = "실제 Angle: -";
            lblPredictedAngle.Text = "예측 Angle: -";
            lblActualThrottle.Text = "실제 Throttle: -";
            lblPredictedThrottle.Text = "예측 Throttle: -";
            lblAngleError.Text = "Angle Error: -";
            lblPilotWarning.Text = "판정: -";
            lblPilotWarning.ForeColor = Color.DimGray;

            isUpdatingSelection = true;
            lstCleanerFrames.Items.Clear();
            lstPilotFrames.Items.Clear();
            isUpdatingSelection = false;

            cleanerTimelineStartIndex = 0;
            UpdateCleanerTimelineScrollBar();
            ResetCleanerRange();
        }

        private void SaveCatalogWithoutBackup()
        {
            try
            {
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
                    $"catalog 재작성 완료: {writtenCount}개 프레임 / catalog 파일 {catalogIndex}개로 분할 저장"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("catalog 재작성 중 오류가 발생했습니다.\n\n" + ex.Message);
            }
        }

        private async Task<string> PrepareTrainArgsForStartAsync(
            string trainArgs,
            string outputModelPath,
            string representativeModelPath,
            bool useWsl,
            string mycarPath,
            bool hasSelectedDataFolder)
        {
            List<string> tokens = SplitCommandLine(trainArgs);
            NormalizeTrainCommandTokens(tokens);
            tokens[0] = "train_with_transfer.py";
            SetOptionAssignment(tokens, "--model", outputModelPath);

            if (hasSelectedDataFolder)
            {
                string dataPathForTrain = useWsl ? ConvertPathToWslPath(dataFolderPath) : dataFolderPath;
                RemoveOptionTokens(tokens, "--tub", "--tubs");
                SetOptionAssignment(tokens, "--tubs", dataPathForTrain);
                AppendLog($"[정보] --tubs 경로 자동 변환: {dataPathForTrain}");
            }
            else
            {
                string tubsPath = GetOptionValue(tokens, "--tubs")
                    ?? GetOptionValue(tokens, "--tub")
                    ?? "./data";

                RemoveOptionTokens(tokens, "--tub", "--tubs");
                SetOptionAssignment(tokens, "--tubs", tubsPath);
                AppendLog($"[정보] 선택된 데이터 폴더가 없어 --tubs 경로를 사용합니다: {tubsPath}");
            }

            RemoveOptionTokens(tokens, "--transfer", "--checkpoint");
            RemoveFlagToken(tokens, "--resume");

            if (string.IsNullOrWhiteSpace(representativeModelPath))
                throw new InvalidOperationException("대표 모델을 선택하세요.");

            await EnsureTrainSourceModelExistsAsync(useWsl, mycarPath, representativeModelPath);

            SetOptionAssignment(tokens, "--transfer", representativeModelPath);

            AppendLog("학습 방식 = 대표 모델 중첩학습");
            AppendLog("대표 모델 입력 = " + representativeModelPath);
            AppendLog("서브 모델 출력 = " + outputModelPath);

            return JoinCommandLineForDisplay(tokens);
        }

        private string GetSelectedRepresentativeModelPath()
        {
            if (cmbTrainSourceModel?.SelectedItem is TrainModelChoice choice)
                return choice.ModelPath;

            return currentRepresentativeModelPath;
        }

        private string BuildSubModelPath(string representativeModelPath, string timeStamp)
        {
            string normalized = string.IsNullOrWhiteSpace(representativeModelPath)
                ? "./models/mypilot.h5"
                : representativeModelPath.Trim().Trim('"');

            normalized = normalized.Replace('\\', '/');

            int slashIndex = normalized.LastIndexOf('/');
            string directory = slashIndex >= 0 ? normalized.Substring(0, slashIndex + 1) : "";
            string fileName = slashIndex >= 0 ? normalized.Substring(slashIndex + 1) : normalized;
            string extension = Path.GetExtension(fileName);

            if (string.IsNullOrWhiteSpace(extension))
                extension = ".h5";

            string baseName = fileName.Substring(0, fileName.Length - extension.Length);

            if (string.IsNullOrWhiteSpace(baseName))
                baseName = "mypilot";

            return $"{directory}{baseName}_{timeStamp}{extension}";
        }

        private string GetUnixDirectoryName(string path)
        {
            string normalized = string.IsNullOrWhiteSpace(path)
                ? "."
                : path.Trim().Trim('"').Replace('\\', '/');

            int slashIndex = normalized.LastIndexOf('/');

            if (slashIndex < 0)
                return ".";

            if (slashIndex == 0)
                return "/";

            return normalized.Substring(0, slashIndex);
        }

        private string GetUnixFileNameWithoutExtension(string path)
        {
            string normalized = string.IsNullOrWhiteSpace(path)
                ? ""
                : path.Trim().Trim('"').Replace('\\', '/');

            int slashIndex = normalized.LastIndexOf('/');
            string fileName = slashIndex >= 0 ? normalized.Substring(slashIndex + 1) : normalized;
            string extension = Path.GetExtension(fileName);

            return string.IsNullOrEmpty(extension)
                ? fileName
                : fileName.Substring(0, fileName.Length - extension.Length);
        }

        private string ChangeUnixExtension(string path, string extension)
        {
            string normalized = string.IsNullOrWhiteSpace(path)
                ? ""
                : path.Trim().Trim('"').Replace('\\', '/');

            string directory = GetUnixDirectoryName(normalized);
            string baseName = GetUnixFileNameWithoutExtension(normalized);

            if (directory == ".")
                return baseName + extension;

            if (directory == "/")
                return "/" + baseName + extension;

            return directory + "/" + baseName + extension;
        }

        private async Task EnsureTrainSourceModelExistsAsync(bool useWsl, string mycarPath, string modelPath)
        {
            if (useWsl)
            {
                string wslMycarPath = ConvertPathToWslPath(mycarPath);
                string command =
                    $"cd {BashCdArgument(wslMycarPath)} && " +
                    $"test -f {BashQuote(modelPath)}";

                (int exitCode, _, string error) = await RunWslCommandAsync(command);

                if (exitCode != 0)
                    throw new FileNotFoundException(
                        "WSL에서 대표 모델을 찾을 수 없습니다."
                        + (string.IsNullOrWhiteSpace(error) ? "" : "\n" + error.Trim()),
                        modelPath
                    );

                return;
            }

            string localModelPath = ResolveLocalModelPath(mycarPath, modelPath);

            if (!File.Exists(localModelPath))
                throw new FileNotFoundException("대표 모델을 찾을 수 없습니다.", localModelPath);
        }

        private List<string> SplitCommandLine(string commandLine)
        {
            List<string> tokens = new List<string>();
            StringBuilder current = new StringBuilder();
            char quote = '\0';
            bool escaping = false;

            foreach (char ch in commandLine)
            {
                if (escaping)
                {
                    current.Append(ch);
                    escaping = false;
                    continue;
                }

                if (ch == '\\' && quote == '"')
                {
                    escaping = true;
                    continue;
                }

                if (quote != '\0')
                {
                    if (ch == quote)
                        quote = '\0';
                    else
                        current.Append(ch);

                    continue;
                }

                if (ch == '\'' || ch == '"')
                {
                    quote = ch;
                    continue;
                }

                if (char.IsWhiteSpace(ch))
                {
                    if (current.Length > 0)
                    {
                        tokens.Add(current.ToString());
                        current.Clear();
                    }

                    continue;
                }

                current.Append(ch);
            }

            if (escaping)
                current.Append('\\');

            if (current.Length > 0)
                tokens.Add(current.ToString());

            return tokens;
        }

        private void NormalizeTrainCommandTokens(List<string> tokens)
        {
            while (tokens.Count > 0
                && (tokens[0].Equals("python", StringComparison.OrdinalIgnoreCase)
                    || tokens[0].Equals("python3", StringComparison.OrdinalIgnoreCase)))
            {
                tokens.RemoveAt(0);
            }

            if (tokens.Count == 0)
                tokens.Add("train.py");

            if (!tokens[0].EndsWith(".py", StringComparison.OrdinalIgnoreCase)
                && !tokens[0].Equals("donkey", StringComparison.OrdinalIgnoreCase))
            {
                tokens.Insert(0, "train.py");
            }
        }

        private string JoinCommandLineForDisplay(IEnumerable<string> tokens)
        {
            return string.Join(" ", tokens.Select(QuoteCommandTokenForDisplay));
        }

        private string QuoteCommandTokenForDisplay(string token)
        {
            if (string.IsNullOrEmpty(token))
                return "\"\"";

            bool needsQuote = token.Any(char.IsWhiteSpace)
                || token.Contains('"')
                || token.Contains('\'');

            if (!needsQuote)
                return token;

            return "\"" + token.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
        }

        private string? GetOptionValue(List<string> tokens, string option)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                string token = tokens[i];

                if (token.Equals(option, StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < tokens.Count && !tokens[i + 1].StartsWith("--", StringComparison.Ordinal))
                        return tokens[i + 1];

                    return "";
                }

                string prefix = option + "=";

                if (token.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return token.Substring(prefix.Length);
            }

            return null;
        }

        private void SetOptionAssignment(List<string> tokens, string option, string value)
        {
            RemoveOptionTokens(tokens, option);
            tokens.Add(option + "=" + value);
        }

        private void RemoveOptionTokens(List<string> tokens, params string[] options)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                string token = tokens[i];
                string? matchedOption = options.FirstOrDefault(option =>
                    token.Equals(option, StringComparison.OrdinalIgnoreCase)
                    || token.StartsWith(option + "=", StringComparison.OrdinalIgnoreCase));

                if (matchedOption == null)
                    continue;

                tokens.RemoveAt(i);

                if (token.Equals(matchedOption, StringComparison.OrdinalIgnoreCase)
                    && i < tokens.Count
                    && !tokens[i].StartsWith("--", StringComparison.Ordinal))
                {
                    tokens.RemoveAt(i);
                }

                i--;
            }
        }

        private void RemoveFlagToken(List<string> tokens, string option)
        {
            tokens.RemoveAll(token => token.Equals(option, StringComparison.OrdinalIgnoreCase));
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

        private string CreateWslTrainWrapperScript(string mycarPath, string trainArgs, out string pidFilePath, out string scriptFilePath)
        {
            string wslMycarPath = ConvertPathToWslPath(mycarPath);
            scriptFilePath = Path.Combine(Path.GetTempPath(), $"train_wrapper_{Guid.NewGuid():N}.sh");
            pidFilePath = Path.Combine(Path.GetTempPath(), $"train_pid_{Guid.NewGuid():N}.txt");
            string wslPidFilePath = ConvertPathToWslPath(pidFilePath);

            // trainArgs를 파싱해서 각 인자를 bash-safe하게 감싸기
            // 예: train_with_transfer.py --tubs="/path with spaces" --model=./file.h5
            string escapedArgs = EscapeTrainArgsForBash(trainArgs);
            string innerTrainCommand =
                $"echo \"$$\" > {BashQuote(wslPidFilePath)}; " +
                $"echo \"{WslTrainPidMarker}$$\"; " +
                "exec python \"$@\"";

            string script =
                "#!/bin/bash\n" +
                "set -e\n" +
                "\n" +
                "# 소스 conda 초기화\n" +
                "source ~/miniconda3/etc/profile.d/conda.sh\n" +
                "\n" +
                "# conda 환경 활성화\n" +
                $"conda activate {CondaEnvName}\n" +
                "\n" +
                $"# mycar 폴더로 이동\n" +
                $"cd {BashCdArgument(wslMycarPath)}\n" +
                "\n" +
                "# 학습 프로세스를 새 세션에서 foreground로 실행하고 실제 PID 저장\n" +
                "setsid bash -c " + BashQuote(innerTrainCommand) + " train-session " + escapedArgs + "\n";

            // BOM 없는 UTF-8로 저장 (WSL bash 호환성)
            var utf8NoBom = new UTF8Encoding(false);
            File.WriteAllText(scriptFilePath, script, utf8NoBom);
            AppendLog($"[정보] WSL 학습 스크립트 생성: {scriptFilePath}");

            return scriptFilePath;
        }

        private string EscapeTrainArgsForBash(string trainArgs)
        {
            List<string> tokens = SplitCommandLine(trainArgs);
            NormalizeTrainCommandTokens(tokens);
            return string.Join(" ", tokens.Select(BashQuote));
        }

        private ProcessStartInfo CreateWslTrainProcessStartInfo(string mycarPath, string trainArgs)
        {
            string scriptPath = CreateWslTrainWrapperScript(mycarPath, trainArgs, out string pidFilePath, out string scriptFilePath);
            string wslScriptPath = ConvertPathToWslPath(scriptPath);

            // PID 파일 경로 저장
            currentWslTrainPidFilePath = pidFilePath;

            string command = $"bash {wslScriptPath}";

            AppendLog("WSL Train Command = " + command);
            AppendLog("Train PID File = " + pidFilePath);

            return new ProcessStartInfo
            {
                FileName = "wsl.exe",
                Arguments = $"-d {WslDistroName} -- bash {QuoteWindowsArgument(wslScriptPath)}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        private async Task<(int ExitCode, string Output, string Error)> RunWslCommandAsync(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "wsl.exe",
                Arguments = $"-d {WslDistroName} -- bash -lc {QuoteWindowsArgument(command)}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = new Process();
            process.StartInfo = psi;
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
            string error = await process.StandardError.ReadToEndAsync().ConfigureAwait(false);
            await Task.Run(() => process.WaitForExit()).ConfigureAwait(false);

            return (process.ExitCode, output, error);
        }

        private string GetModelPathFromTrainArgs(string trainArgs)
        {
            List<string> tokens = SplitCommandLine(trainArgs);
            return GetOptionValue(tokens, "--model") ?? "./models/mypilot.h5";
        }

        private async Task<bool> SavedModelExistsAsync(bool useWsl, string mycarPath, string modelPath)
        {
            if (string.IsNullOrWhiteSpace(modelPath))
                return false;

            if (useWsl)
            {
                string wslMycarPath = ConvertPathToWslPath(mycarPath);
                string command =
                    $"cd {BashCdArgument(wslMycarPath)} && " +
                    $"test -f {BashQuote(modelPath)}";

                (int exitCode, _, _) = await RunWslCommandAsync(command);
                return exitCode == 0;
            }

            return File.Exists(ResolveLocalModelPath(mycarPath, modelPath));
        }

        private async Task<string?> FindBestSavedModelPathAsync(bool useWsl, string mycarPath, string versionModelPath)
        {
            if (useWsl)
                return await FindBestSavedModelPathInWslAsync(mycarPath, versionModelPath);

            return FindBestSavedModelPathLocally(mycarPath, versionModelPath);
        }

        private string? FindBestSavedModelPathLocally(string mycarPath, string versionModelPath)
        {
            string exactPath = ResolveLocalModelPath(mycarPath, versionModelPath);

            if (File.Exists(exactPath))
                return versionModelPath;

            string? directory = Path.GetDirectoryName(exactPath);

            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
                return null;

            string baseName = Path.GetFileNameWithoutExtension(exactPath);
            string? latest = Directory
                .GetFiles(directory, baseName + "*.h5", SearchOption.TopDirectoryOnly)
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(latest))
                return null;

            AppendLog("저장된 최신 모델 발견: " + latest);
            return latest;
        }

        private async Task<string?> FindBestSavedModelPathInWslAsync(string mycarPath, string versionModelPath)
        {
            string wslMycarPath = ConvertPathToWslPath(mycarPath);
            string modelDir = GetUnixDirectoryName(versionModelPath);
            string modelBaseName = GetUnixFileNameWithoutExtension(versionModelPath);
            string modelPattern = modelBaseName + "*.h5";

            string command =
                $"cd {BashCdArgument(wslMycarPath)} && " +
                $"if [ -f {BashQuote(versionModelPath)} ]; then printf '%s\\n' {BashQuote(versionModelPath)}; exit 0; fi; " +
                $"if [ ! -d {BashQuote(modelDir)} ]; then exit 0; fi; " +
                $"find {BashQuote(modelDir)} -maxdepth 1 -type f -name {BashQuote(modelPattern)} -printf '%T@ %p\\n' 2>/dev/null | " +
                "sort -nr | head -n 1 | sed 's/^[^ ]* //'";

            (int exitCode, string output, string error) = await RunWslCommandAsync(command);

            if (exitCode != 0)
            {
                AppendLog("[경고] 저장 모델 탐색 실패: " + error.Trim());
                return null;
            }

            string modelPath = output
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault()?
                .Trim() ?? "";

            if (string.IsNullOrWhiteSpace(modelPath))
                return null;

            AppendLog("저장된 최신 모델 발견: " + modelPath);
            return modelPath;
        }

        private async Task PromoteTrainedModelAsync(
            bool useWsl,
            string mycarPath,
            string versionModelPath,
            string representativeModelPath)
        {
            if (useWsl)
            {
                await PromoteTrainedModelInWslAsync(mycarPath, versionModelPath, representativeModelPath);
                return;
            }

            PromoteTrainedModelLocally(mycarPath, versionModelPath, representativeModelPath);
        }

        private async Task PromoteTrainedModelInWslAsync(
            string mycarPath,
            string versionModelPath,
            string representativeModelPath)
        {
            string wslMycarPath = ConvertPathToWslPath(mycarPath);
            string representativeDir = GetUnixDirectoryName(representativeModelPath);
            string versionPngPath = ChangeUnixExtension(versionModelPath, ".png");
            string representativePngPath = ChangeUnixExtension(representativeModelPath, ".png");
            string versionTflitePath = ChangeUnixExtension(versionModelPath, ".tflite");
            string representativeTflitePath = ChangeUnixExtension(representativeModelPath, ".tflite");

            string command =
                $"cd {BashCdArgument(wslMycarPath)} && " +
                $"mkdir -p {BashQuote(representativeDir)} && " +
                $"cp -f {BashQuote(versionModelPath)} {BashQuote(representativeModelPath)} && " +
                $"if [ -f {BashQuote(versionPngPath)} ]; then cp -f {BashQuote(versionPngPath)} {BashQuote(representativePngPath)}; fi && " +
                $"if [ -f {BashQuote(versionTflitePath)} ]; then cp -f {BashQuote(versionTflitePath)} {BashQuote(representativeTflitePath)}; fi";

            AppendLog("대표 모델 갱신 명령 = " + command);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "wsl.exe",
                Arguments = $"-d {WslDistroName} -- bash -lc {QuoteWindowsArgument(command)}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = new Process();
            process.StartInfo = psi;
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            await Task.Run(() => process.WaitForExit());

            if (!string.IsNullOrWhiteSpace(output))
                AppendLog(output.Trim());

            if (process.ExitCode != 0)
                throw new Exception(error.Trim());

            AppendLog($"대표 모델 갱신 완료: {versionModelPath} -> {representativeModelPath}");
        }

        private void PromoteTrainedModelLocally(string mycarPath, string versionModelPath, string representativeModelPath)
        {
            string sourceModelPath = ResolveLocalModelPath(mycarPath, versionModelPath);
            string mainModelPath = ResolveLocalModelPath(mycarPath, representativeModelPath);
            string? mainModelDir = Path.GetDirectoryName(mainModelPath);

            if (string.IsNullOrWhiteSpace(mainModelDir))
                throw new Exception("대표 모델 저장 폴더를 확인할 수 없습니다.");

            Directory.CreateDirectory(mainModelDir);

            if (!File.Exists(sourceModelPath))
                throw new FileNotFoundException("학습 결과 모델 파일을 찾을 수 없습니다.", sourceModelPath);

            File.Copy(sourceModelPath, mainModelPath, true);
            CopyOptionalModelCompanionFiles(sourceModelPath, mainModelPath);

            AppendLog($"대표 모델 갱신 완료: {sourceModelPath} -> {mainModelPath}");
        }

        private string ResolveLocalModelPath(string mycarPath, string modelPath)
        {
            modelPath = modelPath.Trim().Trim('"');
            modelPath = modelPath.Replace("/", "\\");

            if (modelPath.StartsWith(".\\"))
                modelPath = modelPath.Substring(2);

            if (Path.IsPathRooted(modelPath))
                return modelPath;

            return Path.Combine(mycarPath, modelPath);
        }

        private void CopyOptionalModelCompanionFiles(string sourceModelPath, string mainModelPath)
        {
            string sourceBasePath = Path.Combine(
                Path.GetDirectoryName(sourceModelPath) ?? "",
                Path.GetFileNameWithoutExtension(sourceModelPath)
            );

            string mainBasePath = Path.Combine(
                Path.GetDirectoryName(mainModelPath) ?? "",
                Path.GetFileNameWithoutExtension(mainModelPath)
            );

            foreach (string extension in new[] { ".png", ".tflite" })
            {
                string sourcePath = sourceBasePath + extension;

                if (File.Exists(sourcePath))
                    File.Copy(sourcePath, mainBasePath + extension, true);
            }
        }

        private async Task EnsurePredictServerRunningAsync(string modelPath)
        {
            string wslModelPath = ConvertPathToWslPath(modelPath);

            await predictServerSemaphore.WaitAsync();

            try
            {
                await EnsurePredictServerRunningUnderLockAsync(wslModelPath);
            }
            finally
            {
                predictServerSemaphore.Release();
            }
        }

        private async Task<(double angle, double throttle)> PredictImageWithServerAsync(string modelPath, string imagePath)
        {
            string wslModelPath = ConvertPathToWslPath(modelPath);
            string wslImagePath = ConvertPathToWslPath(imagePath);

            await predictServerSemaphore.WaitAsync();

            try
            {
                Process process = await EnsurePredictServerRunningUnderLockAsync(wslModelPath);

                string requestJson = JsonSerializer.Serialize(new Dictionary<string, string>
                {
                    ["image"] = wslImagePath
                });

                await process.StandardInput.WriteLineAsync(requestJson);
                await process.StandardInput.FlushAsync();

                string response = await ReadPredictServerJsonLineAsync(process, 30000);
                return ParsePredictServerPrediction(response);
            }
            catch
            {
                StopPredictServer();
                throw;
            }
            finally
            {
                predictServerSemaphore.Release();
            }
        }

        private async Task<Process> EnsurePredictServerRunningUnderLockAsync(string wslModelPath)
        {
            if (predictServerProcess != null
                && !predictServerProcess.HasExited
                && predictServerModelPath.Equals(wslModelPath, StringComparison.Ordinal))
            {
                return predictServerProcess;
            }

            StopPredictServer();
            await EnsurePredictOneScriptAsync();

            string command =
                "source ~/miniconda3/etc/profile.d/conda.sh && " +
                $"conda activate {CondaEnvName} && " +
                "cd ~/mycar && " +
                $"python -u predict_server.py --model {BashQuote(wslModelPath)}";

            AppendLog("실시간 예측 서버 시작 = " + command);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "wsl.exe",
                Arguments = $"-d {WslDistroName} -- bash -lc {QuoteWindowsArgument(command)}",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process();
            process.StartInfo = psi;
            process.EnableRaisingEvents = true;
            process.ErrorDataReceived += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(ev.Data) || IsDisposed || !IsHandleCreated)
                    return;

                try
                {
                    BeginInvoke(new Action(() => AppendLog("[PRED ERR] " + ev.Data)));
                }
                catch { }
            };

            process.Start();
            process.BeginErrorReadLine();

            try
            {
                string readyLine = await ReadPredictServerJsonLineAsync(process, 180000);

                using JsonDocument doc = JsonDocument.Parse(readyLine);
                JsonElement root = doc.RootElement;
                bool ok = root.TryGetProperty("ok", out JsonElement okElement) && okElement.GetBoolean();
                bool ready = root.TryGetProperty("ready", out JsonElement readyElement) && readyElement.GetBoolean();

                if (!ok || !ready)
                {
                    string error = root.TryGetProperty("error", out JsonElement errorElement)
                        ? errorElement.GetString() ?? "Unknown error"
                        : "Unknown error";
                    throw new Exception(error);
                }
            }
            catch
            {
                try
                {
                    if (!process.HasExited)
                        process.Kill(true);
                }
                catch { }

                process.Dispose();
                throw;
            }

            predictServerProcess = process;
            predictServerModelPath = wslModelPath;
            AppendLog("실시간 예측 서버 준비 완료: " + wslModelPath);

            return process;
        }

        private async Task<string> ReadPredictServerJsonLineAsync(Process process, int timeoutMilliseconds)
        {
            DateTime deadline = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);

            while (DateTime.UtcNow < deadline)
            {
                if (process.HasExited)
                    throw new Exception("실시간 예측 서버가 종료되었습니다. ExitCode = " + process.ExitCode);

                int remainingMilliseconds = Math.Max(1, (int)(deadline - DateTime.UtcNow).TotalMilliseconds);
                Task<string?> readTask = process.StandardOutput.ReadLineAsync();
                Task completed = await Task.WhenAny(readTask, Task.Delay(remainingMilliseconds));

                if (completed != readTask)
                    throw new TimeoutException("실시간 예측 서버 응답 시간이 초과되었습니다.");

                string? line = await readTask;

                if (line == null)
                    throw new Exception("실시간 예측 서버 출력이 종료되었습니다.");

                line = line.Trim();

                if (line.StartsWith("{", StringComparison.Ordinal))
                    return line;

                if (!string.IsNullOrWhiteSpace(line))
                    AppendLog("[PRED] " + line);
            }

            throw new TimeoutException("실시간 예측 서버 응답 시간이 초과되었습니다.");
        }

        private (double angle, double throttle) ParsePredictServerPrediction(string jsonLine)
        {
            using JsonDocument doc = JsonDocument.Parse(jsonLine);
            JsonElement root = doc.RootElement;
            bool ok = root.TryGetProperty("ok", out JsonElement okElement) && okElement.GetBoolean();

            if (!ok)
            {
                string error = root.TryGetProperty("error", out JsonElement errorElement)
                    ? errorElement.GetString() ?? "Unknown error"
                    : "Unknown error";
                throw new Exception(error);
            }

            double angle = root.GetProperty("angle").GetDouble();
            double throttle = root.GetProperty("throttle").GetDouble();

            return (angle, throttle);
        }

        private void StopPredictServer()
        {
            Process? process = predictServerProcess;
            predictServerProcess = null;
            predictServerModelPath = "";

            if (process == null)
                return;

            try
            {
                if (!process.HasExited)
                {
                    try { process.StandardInput.Close(); }
                    catch { }

                    if (!process.WaitForExit(1200))
                        process.Kill(true);
                }
            }
            catch { }
            finally
            {
                process.Dispose();
            }
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
                    BeginInvoke(new Action(() => AppendLog(ev.Data)));
                }
            };

            process.ErrorDataReceived += (s, ev) =>
            {
                if (!string.IsNullOrWhiteSpace(ev.Data))
                {
                    errorBuilder.AppendLine(ev.Data);
                    BeginInvoke(new Action(() => AppendLog("[ERR] " + ev.Data)));
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
                    "Python 예측 스크립트가 실패했습니다.\n\n" + stderr
                );
            }

            string? jsonLine = stdout
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault(line => line.TrimStart().StartsWith("{"));

            if (string.IsNullOrWhiteSpace(jsonLine))
            {
                throw new Exception(
                    "Python 예측 결과 JSON을 찾지 못했습니다.\n\n출력:\n" + stdout
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
            string representativeModelPath = GetSelectedRepresentativeModelPath();

            if (string.IsNullOrWhiteSpace(mycarPath))
            {
                lblModelStatus.Text = "모델 상태: mycar 경로 없음";
                return;
            }

            if (mycarPath.StartsWith("~/"))
            {
                lblModelStatus.Text = "모델 상태: WSL 대표 모델 사용";
                return;
            }

            string modelFileName = "mypilot.h5";

            if (!string.IsNullOrWhiteSpace(representativeModelPath))
                modelFileName = Path.GetFileName(representativeModelPath);

            string modelPath = ResolveLocalModelPath(mycarPath, representativeModelPath);

            if (File.Exists(modelPath))
                lblModelStatus.Text = $"모델 상태: {modelFileName} 존재";
            else
                lblModelStatus.Text = $"모델 상태: {modelFileName} 없음";
        }

        private void DisposeCurrentImages()
        {
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

        private void HandleTrainingOutput(string message, bool isError = false)
        {
            if (TryCaptureWslTrainProcessGroupId(message))
                return;

            // PID 파일에서 읽기 시도 (출력 기반 캡처 실패 시)
            if (currentTrainUsesWsl && currentWslTrainProcessGroupId <= 0 &&
                !string.IsNullOrWhiteSpace(currentWslTrainPidFilePath) && File.Exists(currentWslTrainPidFilePath))
            {
                try
                {
                    string pidContent = File.ReadAllText(currentWslTrainPidFilePath).Trim();
                    if (int.TryParse(pidContent, out int pidFromFile) && pidFromFile > 0)
                    {
                        currentWslTrainProcessGroupId = pidFromFile;
                        AppendLog($"[정보] PID 파일에서 읽음: PID {pidFromFile}");
                        if (trainProcess != null && !trainProcess.HasExited && !trainEndRequested)
                            btnStopTrain.Enabled = true;
                        return;
                    }
                }
                catch { }
            }

            AppendLog(isError ? "[ERR] " + message : message);
        }

        private bool TryCaptureWslTrainProcessGroupId(string message)
        {
            int markerIndex = message.IndexOf(WslTrainPidMarker, StringComparison.Ordinal);

            if (markerIndex < 0)
                return false;

            string value = message.Substring(markerIndex + WslTrainPidMarker.Length).Trim();
            string pidText = value.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault() ?? "";

            if (int.TryParse(pidText, out int pid))
            {
                if (pid > 0)
                {
                    currentWslTrainProcessGroupId = pid;
                    AppendLog($"WSL 학습 프로세스 제어 준비 완료: PID {pid}");

                    if (trainProcess != null && !trainProcess.HasExited && !trainEndRequested)
                        btnStopTrain.Enabled = true;
                }
                else
                {
                    AppendLog("[경고] WSL 학습 프로세스 PID 캡처 실패 (PID = -1). 프로세스는 백그라운드에서 실행 중입니다. 일시정지 기능은 사용할 수 없습니다.");
                    currentWslTrainProcessGroupId = -1;
                }
            }
            else
            {
                AppendLog("[경고] WSL 학습 프로세스 PID를 읽지 못했습니다. pidText = '" + pidText + "'");
            }

            return true;
        }

        private void AppendLog(string message)
        {
            if (txtLog == null)
                return;

            message = SanitizeLogMessage(message);

            if (string.IsNullOrWhiteSpace(message))
                return;

            if (trainProcess != null)
                UpdateTrainingProgressFromLog(message);

            if (txtLog.Lines.Length > 500)
                txtLog.Text = string.Join(Environment.NewLine,
                    txtLog.Lines.Skip(txtLog.Lines.Length - 500));

            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }

        private void ResetTrainingProgress(string text)
        {
            currentTrainEpochText = "";
            SetTrainingProgress(0, text);
        }

        private void ResetLossGraph()
        {
            trainLossPoints.Clear();
            valLossPoints.Clear();

            if (lblLossGraphInfo != null)
                lblLossGraphInfo.Text = "loss: - / val_loss: -";

            pnlLossGraph?.Invalidate();
        }

        private void SetTrainingProgress(int percent, string text)
        {
            if (prgTrainProgress == null || lblTrainProgress == null)
                return;

            percent = Math.Max(0, Math.Min(100, percent));
            prgTrainProgress.Value = percent;
            lblTrainProgress.Text = text;
        }

        private void UpdateTrainingProgressFromLog(string message)
        {
            if (trainPauseSignalPending || isTrainPaused)
                return;

            RecordLossFromLog(message);

            var epochMatch = System.Text.RegularExpressions.Regex.Match(
                message,
                @"(?:^|\s)Epoch\s+(?<current>\d+)\s*/\s*(?<total>\d+)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            if (epochMatch.Success)
            {
                currentTrainEpochText =
                    $"Epoch {epochMatch.Groups["current"].Value}/{epochMatch.Groups["total"].Value}";
            }

            var stepMatch = System.Text.RegularExpressions.Regex.Match(
                message,
                @"(?:^|\s)(?<current>\d+)\s*/\s*(?<total>\d+)\s+\["
            );

            if (!stepMatch.Success)
                return;

            if (!int.TryParse(stepMatch.Groups["current"].Value, out int currentStep))
                return;

            if (!int.TryParse(stepMatch.Groups["total"].Value, out int totalSteps) || totalSteps <= 0)
                return;

            int percent = (int)Math.Round(currentStep * 100.0 / totalSteps);
            string epochText = string.IsNullOrWhiteSpace(currentTrainEpochText)
                ? ""
                : currentTrainEpochText + " / ";

            SetTrainingProgress(
                percent,
                $"진행도: {epochText}{currentStep}/{totalSteps} ({percent}%)"
            );
        }

        private void RecordLossFromLog(string message)
        {
            var matches = System.Text.RegularExpressions.Regex.Matches(
                message,
                @"(?<key>[A-Za-z0-9_/]*loss)\s*:\s*(?<value>[-+]?(?:\d+(?:\.\d*)?|\.\d+)(?:[eE][-+]?\d+)?)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            if (matches.Count == 0)
                return;

            double? loss = null;
            double? valLoss = null;
            double? fallbackLoss = null;
            double? fallbackValLoss = null;

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                string key = match.Groups["key"].Value.Replace("/", "_").ToLowerInvariant();

                if (!double.TryParse(
                    match.Groups["value"].Value,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double value))
                {
                    continue;
                }

                if (key == "loss")
                    loss = value;
                else if (key == "val_loss")
                    valLoss = value;
                else if (key.StartsWith("val_", StringComparison.Ordinal) && key.EndsWith("_loss", StringComparison.Ordinal))
                    fallbackValLoss ??= value;
                else if (key.EndsWith("_loss", StringComparison.Ordinal))
                    fallbackLoss ??= value;
            }

            loss ??= fallbackLoss;
            valLoss ??= fallbackValLoss;

            if (!loss.HasValue && !valLoss.HasValue)
                return;

            if (loss.HasValue)
                AddLossPoint(trainLossPoints, loss.Value);

            if (valLoss.HasValue)
                AddLossPoint(valLossPoints, valLoss.Value);

            string lossText = trainLossPoints.Count > 0
                ? trainLossPoints.Last().ToString("0.#####", System.Globalization.CultureInfo.InvariantCulture)
                : "-";
            string valLossText = valLossPoints.Count > 0
                ? valLossPoints.Last().ToString("0.#####", System.Globalization.CultureInfo.InvariantCulture)
                : "-";

            if (lblLossGraphInfo != null)
                lblLossGraphInfo.Text = $"loss: {lossText} / val_loss: {valLossText}";

            pnlLossGraph?.Invalidate();
        }

        private static void AddLossPoint(List<double> points, double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return;

            points.Add(value);

            if (points.Count > 500)
                points.RemoveAt(0);
        }

        private void pnlLossGraph_Paint(object? sender, PaintEventArgs e)
        {
            if (pnlLossGraph == null)
                return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(250, 251, 253));

            Rectangle bounds = pnlLossGraph.ClientRectangle;

            if (bounds.Width <= 20 || bounds.Height <= 20)
                return;

            Rectangle plot = new Rectangle(
                bounds.Left + 64,
                bounds.Top + 28,
                bounds.Width - 86,
                bounds.Height - 72
            );

            using Brush plotBrush = new SolidBrush(Color.White);
            using Pen axisPen = new Pen(Color.FromArgb(95, 105, 120), 1.2f);
            using Pen gridPen = new Pen(Color.FromArgb(226, 231, 238), 1);
            using Brush textBrush = new SolidBrush(Color.FromArgb(82, 92, 108));
            using Font smallFont = new Font("맑은 고딕", 9F);
            using Font emptyFont = new Font("맑은 고딕", 10F);

            g.FillRectangle(plotBrush, plot);
            for (int i = 0; i <= 4; i++)
            {
                int y = plot.Top + (plot.Height * i / 4);
                g.DrawLine(gridPen, plot.Left, y, plot.Right, y);
            }

            g.DrawRectangle(axisPen, plot);

            List<double> allPoints = trainLossPoints
                .Concat(valLossPoints)
                .Where(value => !double.IsNaN(value) && !double.IsInfinity(value))
                .ToList();

            if (allPoints.Count == 0)
            {
                string emptyText = "학습 로그에서 loss 값을 기다리는 중";
                SizeF size = g.MeasureString(emptyText, emptyFont);
                g.DrawString(
                    emptyText,
                    emptyFont,
                    textBrush,
                    plot.Left + (plot.Width - size.Width) / 2,
                    plot.Top + (plot.Height - size.Height) / 2
                );
                DrawLossLegend(g, bounds);
                return;
            }

            double min = allPoints.Min();
            double max = allPoints.Max();

            if (Math.Abs(max - min) < 0.000001)
            {
                max += 0.5;
                min = Math.Max(0, min - 0.5);
            }
            else
            {
                double padding = (max - min) * 0.12;
                max += padding;
                min = Math.Max(0, min - padding);
            }

            double mid = (min + max) / 2.0;
            g.DrawString(max.ToString("0.####", System.Globalization.CultureInfo.InvariantCulture), smallFont, textBrush, 8, plot.Top - 8);
            g.DrawString(mid.ToString("0.####", System.Globalization.CultureInfo.InvariantCulture), smallFont, textBrush, 8, plot.Top + plot.Height / 2 - 8);
            g.DrawString(min.ToString("0.####", System.Globalization.CultureInfo.InvariantCulture), smallFont, textBrush, 8, plot.Bottom - 12);

            using Pen trainPen = new Pen(Color.FromArgb(42, 116, 232), 2.6f);
            using Pen valPen = new Pen(Color.FromArgb(239, 142, 35), 2.6f);

            DrawLossLine(g, plot, trainLossPoints, min, max, trainPen);
            DrawLossLine(g, plot, valLossPoints, min, max, valPen);
            DrawLossLegend(g, bounds);
        }

        private static void DrawLossLine(Graphics g, Rectangle plot, List<double> points, double min, double max, Pen pen)
        {
            if (points.Count == 0)
                return;

            if (points.Count == 1)
            {
                PointF point = MapLossPoint(plot, 0, 1, points[0], min, max);
                using Brush brush = new SolidBrush(pen.Color);
                g.FillEllipse(brush, point.X - 4, point.Y - 4, 8, 8);
                return;
            }

            PointF[] mapped = points
                .Select((value, index) => MapLossPoint(plot, index, points.Count, value, min, max))
                .ToArray();

            g.DrawLines(pen, mapped);

            using Brush lastBrush = new SolidBrush(pen.Color);
            PointF last = mapped[mapped.Length - 1];
            g.FillEllipse(lastBrush, last.X - 3.5f, last.Y - 3.5f, 7, 7);
        }

        private static PointF MapLossPoint(Rectangle plot, int index, int count, double value, double min, double max)
        {
            float x = count <= 1
                ? plot.Left
                : plot.Left + (float)(index * plot.Width / (double)(count - 1));
            float y = plot.Bottom - (float)((value - min) * plot.Height / (max - min));

            return new PointF(x, y);
        }

        private static void DrawLossLegend(Graphics g, Rectangle bounds)
        {
            using Font font = new Font("맑은 고딕", 9F);
            using Brush textBrush = new SolidBrush(Color.FromArgb(82, 92, 108));
            using Pen trainPen = new Pen(Color.FromArgb(42, 116, 232), 2.6f);
            using Pen valPen = new Pen(Color.FromArgb(239, 142, 35), 2.6f);

            int x = bounds.Left + 66;
            int y = bounds.Bottom - 32;

            g.DrawLine(trainPen, x, y + 8, x + 22, y + 8);
            g.DrawString("loss", font, textBrush, x + 28, y);

            x += 96;
            g.DrawLine(valPen, x, y + 8, x + 22, y + 8);
            g.DrawString("val_loss", font, textBrush, x + 28, y);
        }

        private string SanitizeLogMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return "";

            message = System.Text.RegularExpressions.Regex.Replace(
                message,
                @"\x1B\[[0-?]*[ -/]*[@-~]",
                ""
            );

            StringBuilder builder = new StringBuilder(message.Length);

            foreach (char ch in message)
            {
                if (ch == '\b')
                    continue;

                if (ch == '\r')
                    continue;

                if (char.IsControl(ch) && ch != '\t')
                    continue;

                builder.Append(ch);
            }

            return builder.ToString().TrimEnd();
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
                DrawSteeringOverlay(e.Graphics, imgRect, overlayActualAngle.Value, Color.DeepSkyBlue, 5f, "Actual");

            if (overlayPredictedAngle.HasValue)
                DrawSteeringOverlay(e.Graphics, imgRect, overlayPredictedAngle.Value, Color.LimeGreen, 5f, "Pred");

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
            path.AddPolygon(new PointF[] { startA, endA, endP });

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

            int drawWidth, drawHeight, drawX, drawY;

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
            if (error <= 0.05) return Color.LimeGreen;
            if (error <= 0.15) return Color.Orange;
            return Color.Red;
        }

        private string GetErrorMessage(double error)
        {
            if (error <= 0.05) return "Good";
            if (error <= 0.15) return "Warning";
            return "High Error";
        }

        private Bitmap ApplyBrightnessContrast(Bitmap source, int brightnessVal = 0, int contrastVal = 0)
        {
            float brightness = brightnessVal / 100f;
            float contrast = (contrastVal + 100f) / 100f;

            Bitmap result = new Bitmap(source.Width, source.Height);

            using Graphics g = Graphics.FromImage(result);

            float t = (1f - contrast) / 2f;

            System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix(new float[][]
            {
                new float[] { contrast, 0, 0, 0, 0 },
                new float[] { 0, contrast, 0, 0, 0 },
                new float[] { 0, 0, contrast, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { t + brightness, t + brightness, t + brightness, 0, 1 }
            });

            using System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();
            ia.SetColorMatrix(cm);

            g.DrawImage(source,
                new Rectangle(0, 0, source.Width, source.Height),
                0, 0, source.Width, source.Height,
                GraphicsUnit.Pixel, ia);

            return result;
        }

        private void chkFlipHorizontal_CheckedChanged(object? sender, EventArgs e)
        {
            if (currentIndex >= 0 && currentIndex < visibleFrames.Count)
                ShowFrame(currentIndex);
        }

        private void chkGrayscale_CheckedChanged(object? sender, EventArgs e)
        {
            if (currentIndex >= 0 && currentIndex < visibleFrames.Count)
                ShowFrame(currentIndex);
        }

        private async void btnSaveProcessed_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(dataFolderPath))
            {
                MessageBox.Show("먼저 Cleaner 탭에서 data 폴더를 열어주세요.");
                return;
            }

            string processedFolder = Path.Combine(dataFolderPath, "processed");
            string processedImagesFolder = Path.Combine(processedFolder, "images");

            btnSaveProcessed.Enabled = false;
            btnSaveProcessed.Text = "저장 중...";

            bool flipHorizontal = chkFlipHorizontal.Checked;
            bool grayscale = chkGrayscale.Checked;
            int brightnessVal = trbBrightness.Value;
            int contrastVal = trbContrast.Value;

            try
            {
                int savedCount = await Task.Run(() =>
                {
                    Directory.CreateDirectory(processedImagesFolder);

                    int count = 0;

                    foreach (DonkeyFrame frame in allFrames)
                    {
                        string srcPath = Path.Combine(imagesFolderPath, frame.ImageFileName);

                        if (!File.Exists(srcPath))
                            continue;

                        byte[] bytes = File.ReadAllBytes(srcPath);

                        using MemoryStream ms = new MemoryStream(bytes);
                        using Bitmap src = new Bitmap(ms);
                        using Bitmap processed = ApplyBrightnessContrast(src, brightnessVal, contrastVal);

                        Bitmap final = processed;

                        if (flipHorizontal)
                        {
                            Bitmap flipped = new Bitmap(processed);
                            flipped.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            final = flipped;
                        }

                        if (grayscale)
                        {
                            Bitmap gray = new Bitmap(final.Width, final.Height);
                            using Graphics g = Graphics.FromImage(gray);
                            System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix(new float[][]
                            {
                                new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
                                new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
                                new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
                                new float[] { 0, 0, 0, 1, 0 },
                                new float[] { 0, 0, 0, 0, 1 }
                            });
                            using System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();
                            ia.SetColorMatrix(cm);
                            g.DrawImage(final, new Rectangle(0, 0, final.Width, final.Height),
                                0, 0, final.Width, final.Height, GraphicsUnit.Pixel, ia);
                            final = gray;
                        }

                        string dstPath = Path.Combine(processedImagesFolder, frame.ImageFileName);
                        final.Save(dstPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        count++;
                    }

                    string[] catalogFiles = Directory.GetFiles(dataFolderPath, "catalog_*.catalog");

                    foreach (string catalogFile in catalogFiles)
                    {
                        string dest = Path.Combine(processedFolder, Path.GetFileName(catalogFile));

                        if (flipHorizontal)
                        {
                            var lines = File.ReadAllLines(catalogFile);
                            var flippedLines = new List<string>();

                            foreach (string line in lines)
                            {
                                if (string.IsNullOrWhiteSpace(line)) continue;
                                try
                                {
                                    using JsonDocument doc = JsonDocument.Parse(line);
                                    var root = doc.RootElement;
                                    var obj = new Dictionary<string, object?>();
                                    foreach (var prop in root.EnumerateObject())
                                        obj[prop.Name] = prop.Value.ValueKind == JsonValueKind.Number
                                            ? (object?)prop.Value.GetDouble()
                                            : prop.Value.GetString();

                                    if (obj.ContainsKey("user/angle") && obj["user/angle"] is double angle)
                                        obj["user/angle"] = -angle;

                                    flippedLines.Add(JsonSerializer.Serialize(obj));
                                }
                                catch { flippedLines.Add(line); }
                            }
                            File.WriteAllLines(dest, flippedLines);
                        }
                        else
                        {
                            File.Copy(catalogFile, dest, true);
                        }
                    }

                    return count;
                });

                AppendLog($"조작 데이터 저장 완료: {savedCount}개 이미지 → {processedFolder}");
                MessageBox.Show($"{savedCount}개 이미지가 저장되었습니다.\n\n{processedFolder}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("저장 중 오류가 발생했습니다.\n\n" + ex.Message);
            }
            finally
            {
                btnSaveProcessed.Enabled = true;
                btnSaveProcessed.Text = "조작 데이터 저장";
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                cleanerRangePlayTimer.Stop();
                cleanerRangePlayTimer.Dispose();

                cleanerAutoPlayTimer.Stop();
                cleanerAutoPlayTimer.Dispose();

                pilotAutoPlayTimer.Stop();
                pilotAutoPlayTimer.Dispose();

                StopPredictServer();
                predictServerSemaphore.Dispose();

                if (trainProcess != null && !trainProcess.HasExited)
                {
                    if (currentTrainUsesWsl && currentWslTrainProcessGroupId > 0)
                    {
                        string command =
                            $"kill -KILL -- -{currentWslTrainProcessGroupId} 2>/dev/null || " +
                            $"kill -KILL {currentWslTrainProcessGroupId}";
                        RunWslCommandAsync(command).GetAwaiter().GetResult();
                    }

                    trainProcess.Kill(true);
                }

                trainProcess?.Dispose();

                ClearCleanerTimelineThumbnailCache();
                DisposeCurrentImages();
            }
            catch { }

            base.OnFormClosed(e);
        }

        private void TbtnClean_Click(object sender, EventArgs e)
        {
            tabMain.SelectedIndex = 0;
        }

        private void TbtnTrain_Click(object sender, EventArgs e)
        {
            tabMain.SelectedIndex = 1;
        }

        private void TbtnPilot_Click(object sender, EventArgs e)
        {
            tabMain.SelectedIndex = 2;
        }

        public void SelectTab(int index)
        {
            if (tabMain.TabCount > index)
                tabMain.SelectedIndex = index;
        }

        private void btnCleanerMark_Click(object? sender, EventArgs e)
        {
            if (isAutoRangeSelecting || cleanerRanges.Count > 0 || pendingRangeStart >= 0)
            {
                ResetCleanerRange();
            }
            else
            {
                isAutoRangeSelecting = true;
                btnCleanerMark.Text = "구간 마크 해제";
                btnCleanerMark.BackColor = Color.OrangeRed;
                AppendLog("구간 마크 대기 모드 - 자동 재생 후 스페이스바로 마크 찍기");
            }
        }
        private void AddRangePoint(int index)
        {
            if (index < 0 || index >= visibleFrames.Count)
                return;

            // 첫 번째 클릭(또는 버튼)
            if (pendingRangeStart < 0)
            {
                pendingRangeStart = index;
                markedFrameIndices.Add(index); // ← 시작점 마크 추가
                AppendLog($"구간 시작 지정 : {index + 1}");
                pnlCleanerTimeline.Invalidate();
                return;
            }

            // 두 번째 클릭 -> 구간 생성
            int start = Math.Min(pendingRangeStart, index);
            int end = Math.Max(pendingRangeStart, index);

            cleanerRanges.Add((start, end));
            markedFrameIndices.Add(index); // ← 끝점 마크 추가

            AppendLog($"구간 추가 : {start + 1} ~ {end + 1}");

            pendingRangeStart = -1;

            UpdateCleanerRangeUi();
            pnlCleanerTimeline.Invalidate();


        }
        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && isAutoRangeSelecting && currentIndex >= 0)
            {
                AddRangePoint(currentIndex);
                e.Handled = true;
            }
        }
    }
}

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DonkeycarManager
{
    partial class SplashForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        static readonly Color C_BG      = Color.FromArgb(248, 249, 250);
        static readonly Color C_SURFACE = Color.FromArgb(255, 255, 255);
        static readonly Color C_BORDER  = Color.FromArgb(220, 225, 230);
        static readonly Color C_TEXT1   = Color.FromArgb(26,  26,  46);
        static readonly Color C_TEXT2   = Color.FromArgb(107, 114, 128);
        static readonly Color C_BLUE    = Color.FromArgb(56,  139, 253);
        static readonly Color C_GREEN   = Color.FromArgb(63,  185, 80);
        static readonly Color C_AMBER   = Color.FromArgb(210, 153, 34);
        static readonly Color C_RED     = Color.FromArgb(248, 81,  73);

        private Button btnDataView, btnCleaner, btnTraining, btnPilotTest;

        private void InitializeComponent()
        {
            SuspendLayout();

            Text = "Donkeycar Manager";
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
          
            BackColor = C_BG;
            DoubleBuffered = true;

            this.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

               

                using var divPen = new Pen(Color.FromArgb(33, 38, 45), 1f);
                g.DrawLine(divPen, 24, 80, ClientSize.Width - 24, 80);

                using var sbBrush = new SolidBrush(Color.FromArgb(235, 237, 240));
                g.FillRectangle(sbBrush, 0, ClientSize.Height - 30, ClientSize.Width, 30);
                using var sbPen = new Pen(Color.FromArgb(33, 38, 45), 1f);
                g.DrawLine(sbPen, 0, ClientSize.Height - 30, ClientSize.Width, ClientSize.Height - 30);

                using var fStat = new Font("Segoe UI", 8.5f);
                using var bGreen = new SolidBrush(C_GREEN);
                using var bGray = new SolidBrush(C_TEXT2);
                g.DrawString("●  ReaLTaiizor 연결됨", fStat, bGreen,
                    new PointF(14, ClientSize.Height - 20));
                var repo = "v1.0 · donkeys11/donkeycar-to-c-";
                var tw = g.MeasureString(repo, fStat);
                g.DrawString(repo, fStat, bGray,
                    new PointF(ClientSize.Width - tw.Width - 14, ClientSize.Height - 20));

                using var fTitle = new Font("Segoe UI", 17f, FontStyle.Bold);
                using var fSub = new Font("Segoe UI", 9f);
                using var bW = new SolidBrush(C_TEXT1);
                g.DrawString("Donkeycar Manager", fTitle, bW, new PointF(22, 16));
               

                using var fLabel = new Font("Segoe UI", 8.5f);
                g.DrawString("무엇을 할까요?", fLabel, bGray, new PointF(24, 90));
            };


            btnDataView = MakeBtn("chart", "데이터 확인", "Viewer 탭  ·  프레임 탐색 및 확인", C_BLUE, Color.FromArgb(238, 244, 255));
            btnCleaner = MakeBtn("adjust", "데이터 정리", "Cleaner 탭  ·  밝기·필터·삭제", C_GREEN, Color.FromArgb(238, 250, 243));
            btnTraining = MakeBtn("brain", "학습 실행", "Training 탭  ·  모델 학습", C_AMBER, Color.FromArgb(255, 248, 238));
            btnPilotTest = MakeBtn("wheel", "Pilot Test", "PilotTest 탭  ·  자율주행 테스트", C_RED, Color.FromArgb(255, 240, 240));




            btnDataView.Click += btnDataView_Click;
            btnCleaner.Click += btnCleaner_Click;
            btnTraining.Click += btnTraining_Click;
            btnPilotTest.Click += btnPilotTest_Click;

            Controls.AddRange(new Control[]
            {
                btnDataView, btnCleaner, btnTraining, btnPilotTest 
            });
            
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ResumeLayout(false);
        }

        private Button MakeBtn(string icon, string title, string sub, Color accent, Color bgColor)
        {
            var b = new Button
            {
                FlatStyle = FlatStyle.Flat,
                BackColor = bgColor,
                Cursor = Cursors.Hand,
                TabStop = false,
                Tag = new object[] { icon, title, sub, accent }
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(
                Math.Min(BackColor.R + 14, 255),
                Math.Min(BackColor.G + 14, 255),
                Math.Min(BackColor.B + 16, 255));
            b.Paint += MenuBtn_Paint;
            return b;
        }

        private void MenuBtn_Paint(object sender, PaintEventArgs e)
        {
            if (sender is not Button b) return;
            var info = (object[])b.Tag;
            string icon = (string)info[0];
            string title = (string)info[1];
            string sub = (string)info[2];
            var accent = (Color)info[3];

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using var bgPath = RoundRect(b.ClientRectangle, 8);
            using var bgBrush = new SolidBrush(b.BackColor);
            g.FillPath(bgBrush, bgPath);

            using var sPath = RoundRect(new Rectangle(0, 0, b.Width, 3), 2);
            using var sBrush = new SolidBrush(accent);
            g.FillPath(sBrush, sPath);

            using var borderPen = new Pen(C_BORDER, 1f);
            g.DrawPath(borderPen, bgPath);

            // 아이콘 (상단 중앙)
            int iconSz = 44;
            var iconBg = new Rectangle((b.Width - iconSz) / 2, b.Height / 2 - iconSz - 10, iconSz, iconSz);
            using var iconBgBrush = new SolidBrush(Color.FromArgb(30, accent));
            g.FillEllipse(iconBgBrush, iconBg);

            string sym = icon switch
            {
                "chart" => "▦",
                "adjust" => "⚙",
                "brain" => "◈",
                "wheel" => "◎",
                _ => "●"
            };
            using var fIcon = new Font("Segoe UI Symbol", 16f, FontStyle.Bold);
            using var bAccent = new SolidBrush(accent);
            var centerFmt = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(sym, fIcon, bAccent,
                new RectangleF(iconBg.X, iconBg.Y, iconBg.Width, iconBg.Height), centerFmt);

            // 제목 (아이콘 아래 중앙)
            using var fTitle = new Font("Segoe UI", 11.5f, FontStyle.Bold);
            using var bText1 = new SolidBrush(C_TEXT1);
            g.DrawString(title, fTitle, bText1,
                new RectangleF(0, iconBg.Bottom + 8, b.Width, 30), centerFmt);

            // 설명 (제목 아래 중앙)
            using var fSub = new Font("Segoe UI", 7.5f);
            using var bText2 = new SolidBrush(C_TEXT2);
            g.DrawString(sub, fSub, bText2,
                new RectangleF(4, iconBg.Bottom + 36, b.Width - 8, 30), centerFmt);
        }

        private static GraphicsPath RoundRect(Rectangle r, int rad)
        {
            int d = rad * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}
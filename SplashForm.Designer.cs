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

        static readonly Color C_BG = Color.FromArgb(248, 249, 250);
        static readonly Color C_SURFACE = Color.FromArgb(255, 255, 255);
        static readonly Color C_BORDER = Color.FromArgb(220, 225, 230);
        static readonly Color C_TEXT1 = Color.FromArgb(26, 26, 46);
        static readonly Color C_TEXT2 = Color.FromArgb(107, 114, 128);
        static readonly Color C_GREEN = Color.FromArgb(63, 185, 80);
        static readonly Color C_AMBER = Color.FromArgb(210, 153, 34);
        static readonly Color C_RED = Color.FromArgb(248, 81, 73);

        private Button btnCleaner, btnTraining, btnPilotTest;

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

                

                using var sbBrush = new SolidBrush(Color.FromArgb(235, 237, 240));
                g.FillRectangle(sbBrush, 0, ClientSize.Height - 30, ClientSize.Width, 30);
                using var sbPen = new Pen(Color.FromArgb(220, 225, 230), 1f);
                g.DrawLine(sbPen, 0, ClientSize.Height - 30, ClientSize.Width, ClientSize.Height - 30);

                using var fStat = new Font("Segoe UI", 8.5f);
                using var bGreen = new SolidBrush(C_GREEN);
                using var bGray = new SolidBrush(C_TEXT2);
                g.DrawString("●  WinForms UI 준비됨", fStat, bGreen,
                    new PointF(14, ClientSize.Height - 20));
                var repo = "v1.0 · donkeys11/donkeycar-to-c-";
                var tw = g.MeasureString(repo, fStat);
                g.DrawString(repo, fStat, bGray,
                    new PointF(ClientSize.Width - tw.Width - 14, ClientSize.Height - 20));

               

               
            };

            btnCleaner = MakeBtn("adjust", "데이터 정리", "밝기·필터·삭제", C_GREEN, Color.FromArgb(210, 240, 220));
            btnTraining = MakeBtn("brain", "학습 실행", "모델 학습", C_AMBER, Color.FromArgb(255, 235, 210));
            btnPilotTest = MakeBtn("wheel", "모델 테스트", "자율주행 테스트", C_RED, Color.FromArgb(255, 215, 215));

            btnCleaner.Click += btnCleaner_Click;
            btnTraining.Click += btnTraining_Click;
            btnPilotTest.Click += btnPilotTest_Click;

            Controls.AddRange(new Control[] { btnCleaner, btnTraining, btnPilotTest });

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
                Tag = new object[] { icon, title, sub, accent, false }
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = bgColor;
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
            bool hovered = info[4] is true;

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

            int iconSz = 44;
            var iconBg = new Rectangle((b.Width - iconSz) / 2, b.Height / 2 - iconSz - 10, iconSz, iconSz);
            using var iconBgBrush = new SolidBrush(Color.FromArgb(30, accent));
            g.FillEllipse(iconBgBrush, iconBg);

            string sym = icon switch
            {
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

            using var fTitle = new Font("Segoe UI", 11.5f, FontStyle.Bold);
            using var bText1 = new SolidBrush(C_TEXT1);
            g.DrawString(title, fTitle, bText1,
                new RectangleF(0, iconBg.Bottom + 8, b.Width, 30), centerFmt);

            using var fSub = new Font("Segoe UI", 7.5f);
            using var bText2 = new SolidBrush(C_TEXT2);
            g.DrawString(sub, fSub, bText2,
                new RectangleF(4, iconBg.Bottom + 36, b.Width - 8, 30), centerFmt);

            // 호버 화살표
            if (hovered)
            {
                int arrowW = 36;
                int arrowH = 28;
                int ax = b.Width - arrowW - 14;
                int ay = b.Height / 2 - arrowH / 2;

                using var arrowBrush = new SolidBrush(Color.FromArgb(180, 180, 190));
                using var arrowPath = new GraphicsPath();

                // 꼬리 없는 둥근 화살표 (> 모양)
                float cx = ax + arrowW * 0.5f;
                float cy = ay + arrowH / 2f;
                float tipX = ax + arrowW;
                float topX = ax;
                float topY = ay;
                float botY = ay + arrowH;

                arrowPath.AddLine(topX + 4, topY, tipX - 4, cy);
                arrowPath.AddLine(tipX - 4, cy, topX + 4, botY);

                using var arrowPen = new Pen(Color.FromArgb(160, 160, 175), 10f)
                {
                    StartCap = LineCap.Round,
                    EndCap = LineCap.Round,
                    LineJoin = LineJoin.Round
                };
                g.DrawPath(arrowPen, arrowPath);
            }
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

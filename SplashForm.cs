using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using System.Windows.Forms;

namespace DonkeycarManager
{
    public partial class SplashForm : Form
    {
        

        public SplashForm()
        {
            InitializeComponent();
        }

       
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int margin = 150;
            int gapX = 60;
            int totalGap = gapX * 3 + margin * 2;
            int bw = (ClientSize.Width - totalGap) / 4;
            int bh = (int)(ClientSize.Height * 0.35);
            int sx = margin;
            int sy = ClientSize.Height - bh - 60;

            btnDataView.Location  = new Point(sx, sy);
            btnCleaner.Location   = new Point(sx + (bw + gapX), sy);
            btnTraining.Location  = new Point(sx + (bw + gapX) * 2, sy);
            btnPilotTest.Location = new Point(sx + (bw + gapX) * 3, sy);

            foreach (var b in new[] { btnDataView, btnCleaner, btnTraining, btnPilotTest })
            {
                b.Size = new Size(bw, bh);

                var timer = new System.Windows.Forms.Timer { Interval = 15 };
                float[] alpha = { 0f };
                bool[] hovering = { false };
                var btn = b;
                var originalColor = b.BackColor;

                btn.MouseEnter += (s, ev) => { hovering[0] = true; timer.Start(); };
                btn.MouseLeave += (s, ev) => { hovering[0] = false; timer.Start(); };

                timer.Tick += (s, ev) =>
                {
                    if (hovering[0])
                        alpha[0] = Math.Min(1f, alpha[0] + 0.08f);
                    else
                        alpha[0] = Math.Max(0f, alpha[0] - 0.08f);

                    int r  = (int)(originalColor.R - 20 * alpha[0]);
                    int g2 = (int)(originalColor.G - 20 * alpha[0]);
                    int bl = (int)(originalColor.B - 20 * alpha[0]);
                    btn.BackColor = Color.FromArgb(Math.Max(0, r), Math.Max(0, g2), Math.Max(0, bl));

                    if (alpha[0] <= 0f || alpha[0] >= 1f)
                        timer.Stop();

                    btn.Invalidate();
                };
            }
        }

        private void OpenMainForm(int tabIndex)
        {
            var main = new MainForm();
            main.Show();
            main.SelectTab(tabIndex);
            this.Hide();
            main.FormClosed += (s, e) => this.Close();
        }

        private void btnDataView_Click(object sender, EventArgs e) => OpenMainForm(0);
        private void btnCleaner_Click(object sender, EventArgs e) => OpenMainForm(1);
        private void btnTraining_Click(object sender, EventArgs e) => OpenMainForm(2);
        private void btnPilotTest_Click(object sender, EventArgs e) => OpenMainForm(3);
    }
}
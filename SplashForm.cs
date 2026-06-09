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
            int totalGap = gapX * 2 + margin * 2;
            int bw = (ClientSize.Width - totalGap) / 3;
            int bh = (int)(ClientSize.Height * 0.35);
            int sx = margin;
            int sy = ClientSize.Height - bh - 60;

            btnCleaner.Location = new Point(sx, sy);
            btnTraining.Location = new Point(sx + (bw + gapX), sy);
            btnPilotTest.Location = new Point(sx + (bw + gapX) * 2, sy);

            foreach (var b in new[] { btnCleaner, btnTraining, btnPilotTest })
            {
                b.Size = new Size(bw, bh);

                var timer = new System.Windows.Forms.Timer { Interval = 15 };
                float[] alpha = { 0f };
                bool[] hovering = { false };
                var btn = b;
                var originalColor = b.BackColor;

                btn.MouseEnter += (s, ev) =>
                {
                    hovering[0] = true;
                    if (btn.Tag is object[] t) t[4] = true;
                    timer.Start();
                    btn.Invalidate();
                };
                btn.MouseLeave += (s, ev) =>
                {
                    hovering[0] = false;
                    if (btn.Tag is object[] t) t[4] = false;
                    timer.Start();
                    btn.Invalidate();
                };

                timer.Tick += (s, ev) =>
                {
                    if (hovering[0])
                        alpha[0] = Math.Min(1f, alpha[0] + 0.08f);
                    else
                        alpha[0] = Math.Max(0f, alpha[0] - 0.08f);

                    int r = (int)(originalColor.R - 20 * alpha[0]);
                    int g2 = (int)(originalColor.G - 20 * alpha[0]);
                    int bl = (int)(originalColor.B - 20 * alpha[0]);
                    btn.BackColor = Color.FromArgb(Math.Max(0, r), Math.Max(0, g2), Math.Max(0, bl));

                    if (alpha[0] <= 0f || alpha[0] >= 1f)
                        timer.Stop();

                    btn.Invalidate();
                };
            }
            // ── 상단 영역: 이미지(왼) + 텍스트(오) ───────────
            int areaTop = 60;
            int areaBot = sy - 30;
            int areaH = areaBot - areaTop;
            int halfW = ClientSize.Width / 2;

            // 왼쪽 이미지
            var pb = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(248, 249, 250),
                Location = new Point(80, areaTop),
                Size = new Size(halfW - 100, areaH)
            };

            string imgPath = Path.Combine(Application.StartupPath, "img", "splash_donkey.png");
            if (File.Exists(imgPath))
            {
                var original = new Bitmap(imgPath);
                var result = new Bitmap(original.Width, original.Height,
                                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                for (int y = 0; y < original.Height; y++)
                {
                    for (int x = 0; x < original.Width; x++)
                    {
                        var pixel = original.GetPixel(x, y);
                        if (pixel.R > 240 && pixel.G > 240 && pixel.B > 240)
                            result.SetPixel(x, y, Color.Transparent);
                        else
                            result.SetPixel(x, y, pixel);
                    }
                }
                pb.Image = result;
            }

            // 오른쪽 텍스트
            float fontSize = ClientSize.Height * 0.04f;
            int lineGap = (int)(ClientSize.Height * 0.13f);
            int textBlockH = lineGap * 3;
            int textY = areaTop + (areaH - textBlockH) / 2;
            int textX = halfW + 60;

            var lblData = new Label
            {
                Text = "Data",
                Font = new Font("Segoe UI", fontSize, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 26, 46),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(textX, textY)
            };

            var lblManager = new Label
            {
                Text = "Manager",
                Font = new Font("Segoe UI", fontSize, FontStyle.Bold),
                ForeColor = Color.FromArgb(63, 185, 80),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(textX, textY + lineGap)
            };

            var lblUI = new Label
            {
                Text = "UI",
                Font = new Font("Segoe UI", fontSize, FontStyle.Regular),
                ForeColor = Color.FromArgb(180, 190, 200),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(textX, textY + lineGap * 2)
            };

            Controls.AddRange(new Control[] { pb, lblData, lblManager, lblUI });
        }

        private void OpenMainForm(int tabIndex)
        {
            var main = new MainForm();
            main.Show();
            main.SelectTab(tabIndex);
            this.Hide();
            main.FormClosed += (s, e) => this.Close();
        }

        private void btnCleaner_Click(object sender, EventArgs e) => OpenMainForm(0);
        private void btnTraining_Click(object sender, EventArgs e) => OpenMainForm(1);
        private void btnPilotTest_Click(object sender, EventArgs e) => OpenMainForm(2);
    }
}
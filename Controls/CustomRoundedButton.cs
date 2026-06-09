using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DonkeycarManager.Controls
{
    public class CustomRoundedButton : Button
    {
        private bool isHover = false;
        private Color originalBack;
        private Color hoverBack;

        public CustomRoundedButton()
        {
            DoubleBuffered = true;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            originalBack = BackColor;
            UpdateHoverColor();
            Resize += (s, e) => UpdateRegion();
        }

        private void UpdateHoverColor()
        {
            originalBack = BackColor;
            try
            {
                hoverBack = ControlPaint.Light(originalBack, 0.08f);
            }
            catch
            {
                hoverBack = originalBack;
            }
        }

        private void UpdateRegion()
        {
            using var gp = CreateRoundRectangle(new Rectangle(0, 0, Width, Height), CornerRadius);
            Region = new Region(gp);
        }

        [Category("Appearance")]
        [Description("모서리 반경 (픽셀)")]
        public int CornerRadius { get; set; } = 10;

        [Category("Appearance")]
        [Description("테두리 두께")]
        public int BorderSize { get; set; } = 2;

        Color? borderColorCache = null;
        [Category("Appearance")]
        [Description("테두리 색 - 지정하지 않으면 BackColor에서 계산")]
        public Color BorderColor
        {
            get => borderColorCache ?? ControlPaint.Dark(BackColor);
            set { borderColorCache = value; Invalidate(); }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            UpdateHoverColor();
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHover = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHover = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width, Height);

            using (var path = CreateRoundRectangle(rect, CornerRadius))
            {
                // background
                using var b = new SolidBrush(isHover ? hoverBack : BackColor);
                g.FillPath(b, path);

                // border
                using var p = new Pen(BorderColor, Math.Max(1, BorderSize));
                g.DrawPath(p, path);
            }

            // text
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            Rectangle textRect = rect;
            // respect padding
            textRect.X += Padding.Left;
            textRect.Y += Padding.Top;
            textRect.Width -= Padding.Left + Padding.Right;
            textRect.Height -= Padding.Top + Padding.Bottom;

            Color textColor = ForeColor;
            TextRenderer.DrawText(g, Text, Font, textRect, textColor, flags);
        }

        private GraphicsPath CreateRoundRectangle(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = Math.Max(0, radius) * 2;
            if (d > 0)
            {
                path.AddArc(r.Left, r.Top, d, d, 180, 90);
                path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
                path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
                path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            }
            else
            {
                path.AddRectangle(r);
            }

            path.CloseAllFigures();
            return path;
        }
    }
}

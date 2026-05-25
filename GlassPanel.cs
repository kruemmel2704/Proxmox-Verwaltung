using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProxmoxVEGui
{
    public class GlassPanel : Panel
    {
        private int borderRadius = 16;
        public int BorderRadius
        {
            get => borderRadius;
            set
            {
                borderRadius = value;
                UpdateRoundedRegion();
                this.Invalidate();
            }
        }

        private int borderSize = 1;
        public int BorderSize
        {
            get => borderSize;
            set
            {
                borderSize = value;
                this.Invalidate();
            }
        }

        private Color borderColor = Color.FromArgb(55, 65, 81);
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                this.Invalidate();
            }
        }

        private bool enableGlassSheen = true;
        public bool EnableGlassSheen
        {
            get => enableGlassSheen;
            set
            {
                enableGlassSheen = value;
                this.Invalidate();
            }
        }

        public GlassPanel()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw,
                true
            );
            this.BackColor = Color.FromArgb(17, 24, 39);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            UpdateRoundedRegion();
        }

        private void UpdateRoundedRegion()
        {
            if (this.Width > 0 && this.Height > 0)
            {
                using (GraphicsPath path = RoundedButton.GetRoundedPath(new Rectangle(0, 0, this.Width, this.Height), BorderRadius))
                {
                    this.Region = new Region(path);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);

            // Clean background corners using the parent's background color
            Color parentBg = Color.Transparent;
            if (this.Parent != null)
            {
                parentBg = this.Parent.BackColor;
                if (parentBg == Color.Transparent || parentBg.A == 0 || parentBg == SystemColors.Control)
                {
                    Control p = this.Parent.Parent;
                    while (p != null)
                    {
                        if (p.BackColor != Color.Transparent && p.BackColor.A != 0 && p.BackColor != SystemColors.Control)
                        {
                            parentBg = p.BackColor;
                            break;
                        }
                        p = p.Parent;
                    }
                }
            }
            if (parentBg == Color.Transparent || parentBg.A == 0 || parentBg == SystemColors.Control)
            {
                parentBg = Color.FromArgb(10, 15, 25);
            }

            using (SolidBrush parentBgBrush = new SolidBrush(parentBg))
            {
                e.Graphics.FillRectangle(parentBgBrush, rect);
            }

            Rectangle shrinkRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            if (shrinkRect.Width <= 0 || shrinkRect.Height <= 0) return;

            using (GraphicsPath path = RoundedButton.GetRoundedPath(shrinkRect, BorderRadius))
            {
                // Translucent background
                Color bgCol = this.BackColor;
                int alpha = (bgCol == Color.Transparent || bgCol.A == 0) ? 0 : 160;
                Color glassBg = Color.FromArgb(alpha, bgCol.R, bgCol.G, bgCol.B);

                using (SolidBrush bgBrush = new SolidBrush(glassBg))
                {
                    e.Graphics.FillPath(bgBrush, path);
                }

                // Specular highlights
                if (enableGlassSheen && this.Height > 16)
                {
                    RectangleF highlightRect = new RectangleF(1, 1, this.Width - 2, (this.Height - 2) * 0.35f);
                    if (highlightRect.Height > 0)
                    {
                        using (GraphicsPath highlightPath = RoundedButton.GetRoundedPath(Rectangle.Round(highlightRect), BorderRadius - 1))
                        {
                            using (LinearGradientBrush highlightBrush = new LinearGradientBrush(
                                new PointF(0, highlightRect.Top),
                                new PointF(0, highlightRect.Bottom),
                                Color.FromArgb(40, 255, 255, 255),
                                Color.FromArgb(2, 255, 255, 255)))
                            {
                                e.Graphics.FillPath(highlightBrush, highlightPath);
                            }
                        }
                    }
                }

                // Glass refracting border
                Color topBorderColor = Color.FromArgb(90, 255, 255, 255);
                Color bottomBorderColor = Color.FromArgb(25, 255, 255, 255);

                if (BorderColor != Color.Transparent && BorderColor.A > 0)
                {
                    topBorderColor = Color.FromArgb(130, BorderColor.R, BorderColor.G, BorderColor.B);
                    bottomBorderColor = Color.FromArgb(35, BorderColor.R, BorderColor.G, BorderColor.B);
                }

                using (LinearGradientBrush borderBrush = new LinearGradientBrush(
                    shrinkRect, topBorderColor, bottomBorderColor, LinearGradientMode.Vertical))
                using (Pen borderPen = new Pen(borderBrush, BorderSize))
                {
                    e.Graphics.DrawPath(borderPen, path);
                }
            }
        }
    }
}

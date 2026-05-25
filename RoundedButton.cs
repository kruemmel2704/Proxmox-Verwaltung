using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProxmoxVEGui
{
    public class RoundedButton : Button
    {
        private int borderRadius = 10;
        public int BorderRadius
        {
            get => borderRadius;
            set
            {
                borderRadius = value;
                this.Invalidate();
            }
        }

        private bool isHover = false;
        private bool isDown = false;

        private Color? hoverColor = null;
        private Color? downColor = null;

        public Color HoverColor
        {
            get => hoverColor ?? GetHoverColor(this.BackColor);
            set => hoverColor = value;
        }

        public Color DownColor
        {
            get => downColor ?? GetDownColor(this.BackColor);
            set => downColor = value;
        }

        public RoundedButton()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw,
                true
            );

            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHover = true;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHover = false;
            isDown = false;
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            isDown = true;
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            isDown = false;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            // Resolve parent background color for corner blending
            Color parentBg = Color.Transparent;
            if (this.Parent != null)
            {
                parentBg = this.Parent.BackColor;
                // If parent background is transparent, empty, or inherits the default SystemColors.Control color,
                // traverse up the parent tree to find the custom themed background color.
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
                parentBg = Color.FromArgb(10, 15, 25); // Fallback to the main panel dark background
            }

            // Set up high quality anti-aliasing for the corners
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pevent.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Draw parent background color on the entire client rectangle to clear corners
            using (SolidBrush bgBrush = new SolidBrush(parentBg))
            {
                pevent.Graphics.FillRectangle(bgBrush, this.ClientRectangle);
            }

            // Determine base color for the button
            Color baseColor;
            Color textColor;

            if (!this.Enabled)
            {
                baseColor = Color.FromArgb(31, 41, 55); // Muted dark background for disabled state (Slate 800)
                textColor = Color.FromArgb(156, 163, 175); // Muted gray text for disabled state (Slate 400)
            }
            else
            {
                baseColor = isDown ? DownColor : isHover ? HoverColor : this.BackColor;
                textColor = this.ForeColor;
            }

            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            if (rect.Width > 0 && rect.Height > 0)
            {
                DrawLiquidGlass(pevent.Graphics, rect, baseColor, BorderRadius, isHover, isDown, this.Enabled);
            }

            // Render Text with GDI+ to prevent pixelated/fringed text rendering on alpha-blended backgrounds
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.FormatFlags = StringFormatFlags.NoWrap;

                pevent.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                using (SolidBrush textBrush = new SolidBrush(textColor))
                {
                    pevent.Graphics.DrawString(this.Text, this.Font, textBrush, new RectangleF(0, 0, this.Width, this.Height), sf);
                }
            }
        }

        public static void DrawLiquidGlass(Graphics g, Rectangle rect, Color baseColor, int borderRadius, bool isHover, bool isDown, bool enabled)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            if (baseColor == Color.Transparent || baseColor.A == 0) return; // Keep transparent buttons transparent

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Lighten/Darken helper values for 3D liquid gel depth
            Color topBgColor = Lighten(baseColor, 0.22f);
            Color bottomBgColor = Darken(baseColor, 0.18f);

            if (!enabled)
            {
                topBgColor = Color.FromArgb(45, 55, 72);
                bottomBgColor = Color.FromArgb(26, 32, 44);
            }
            else
            {
                if (isHover)
                {
                    topBgColor = Lighten(baseColor, 0.32f);
                    bottomBgColor = Darken(baseColor, 0.08f);
                }
                if (isDown)
                {
                    topBgColor = Darken(baseColor, 0.25f);
                    bottomBgColor = Lighten(baseColor, 0.05f); // Inverted-feel compression
                }
            }

            using (GraphicsPath path = GetRoundedPath(rect, borderRadius))
            {
                // 1. Fill base vertical gradient
                using (LinearGradientBrush bgBrush = new LinearGradientBrush(rect, topBgColor, bottomBgColor, LinearGradientMode.Vertical))
                {
                    g.FillPath(bgBrush, path);
                }

                // 2. Specular highlight (Gel-like overlay on the top 45%)
                if (rect.Height > 10)
                {
                    Rectangle highlightRect = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, (int)(rect.Height * 0.45f));
                    if (highlightRect.Width > 0 && highlightRect.Height > 0)
                    {
                        using (GraphicsPath highlightPath = GetRoundedPath(highlightRect, borderRadius - 1))
                        {
                            int topAlpha = !enabled ? 40 : isDown ? 90 : isHover ? 190 : 130;
                            int bottomAlpha = !enabled ? 5 : isDown ? 5 : isHover ? 35 : 15;

                            using (LinearGradientBrush highlightBrush = new LinearGradientBrush(
                                new Point(0, highlightRect.Top),
                                new Point(0, highlightRect.Bottom),
                                Color.FromArgb(topAlpha, 255, 255, 255),
                                Color.FromArgb(bottomAlpha, 255, 255, 255)))
                            {
                                g.FillPath(highlightBrush, highlightPath);
                            }
                        }
                    }
                }

                // 3. Glass refracting border outline (Bright on top, softer on bottom)
                Color topPenColor = Color.FromArgb(160, 255, 255, 255);
                Color bottomPenColor = Color.FromArgb(30, 255, 255, 255);

                if (isDown)
                {
                    topPenColor = Color.FromArgb(90, 0, 0, 0); // Shadows cast down
                    bottomPenColor = Color.FromArgb(40, 255, 255, 255);
                }

                using (LinearGradientBrush borderBrush = new LinearGradientBrush(rect, topPenColor, bottomPenColor, LinearGradientMode.Vertical))
                using (Pen borderPen = new Pen(borderBrush, 1f))
                {
                    g.DrawPath(borderPen, path);
                }
            }
        }

        public static GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            int diameter = radius * 2;

            if (diameter > rect.Width) diameter = rect.Width;
            if (diameter > rect.Height) diameter = rect.Height;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private static Color GetHoverColor(Color color)
        {
            if (color == Color.Transparent || color.A == 0) return Color.Transparent;
            float brightness = color.GetBrightness();
            float factor = brightness < 0.2f ? 0.25f : brightness > 0.8f ? -0.15f : 0.15f;
            return AdjustBrightness(color, factor);
        }

        private static Color GetDownColor(Color color)
        {
            if (color == Color.Transparent || color.A == 0) return Color.Transparent;
            float brightness = color.GetBrightness();
            float factor = brightness < 0.2f ? 0.40f : -0.20f;
            return AdjustBrightness(color, factor);
        }

        private static Color Lighten(Color color, float percent)
        {
            return AdjustBrightness(color, percent);
        }

        private static Color Darken(Color color, float percent)
        {
            return AdjustBrightness(color, -percent);
        }

        private static Color AdjustBrightness(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)Math.Min(255, Math.Max(0, red)), (int)Math.Min(255, Math.Max(0, green)), (int)Math.Min(255, Math.Max(0, blue)));
        }
    }
}

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
                parentBg = Color.FromArgb(17, 24, 39); // Fallback to the main panel dark background
            }

            // Set up high quality anti-aliasing for the corners
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pevent.Graphics.PixelOffsetMode = PixelOffsetMode.Default; // Use default to prevent half-pixel shifting at boundaries

            // Draw parent background color on the entire client rectangle to clear corners
            using (SolidBrush bgBrush = new SolidBrush(parentBg))
            {
                pevent.Graphics.FillRectangle(bgBrush, this.ClientRectangle);
            }

            // Fill rounded path with button background color
            Color fillColor;
            Color textColor;

            if (!this.Enabled)
            {
                fillColor = Color.FromArgb(31, 41, 55); // Muted dark background for disabled state (Slate 800)
                textColor = Color.FromArgb(156, 163, 175); // Muted gray text for disabled state (Slate 400)
            }
            else
            {
                fillColor = isDown ? DownColor : isHover ? HoverColor : this.BackColor;
                textColor = this.ForeColor;
            }

            // Use the full width and height to prevent unpainted gaps at the edges (which cause dark crescent outlines)
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);

            using (GraphicsPath path = GetRoundedPath(rect, BorderRadius))
            using (SolidBrush brush = new SolidBrush(fillColor))
            {
                pevent.Graphics.FillPath(brush, path);
            }

            // Render Text (shifted down by 1 pixel to center perfectly vertically with Segoe UI)
            TextRenderer.DrawText(
                pevent.Graphics,
                this.Text,
                this.Font,
                new Rectangle(0, 1, this.Width, this.Height),
                textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
            );
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

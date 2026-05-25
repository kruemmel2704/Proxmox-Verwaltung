using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProxmoxVEGui
{
    public class UsageChart : Control
    {
        private readonly List<double> _history = new List<double>();
        private readonly int _maxPoints = 60;

        public Color ChartColor { get; set; } = Color.FromArgb(34, 197, 94); // Default Green
        public string Title { get; set; } = "Usage";
        public string Suffix { get; set; } = "%";

        public UsageChart()
        {
            // Set styles for high-quality, flicker-free rendering
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw, true);
            this.BackColor = Color.FromArgb(17, 24, 39); // slate-900 card bg (login panel)
            
            // Initialize with zeroes
            for (int i = 0; i < _maxPoints; i++)
            {
                _history.Add(0.0);
            }
        }

        public void AddValue(double val)
        {
            _history.Add(val);
            if (_history.Count > _maxPoints)
            {
                _history.RemoveAt(0);
            }
            this.Invalidate(); // Request repaint
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int width = this.Width;
            int height = this.Height;

            int borderRadius = 12;
            Rectangle rect = new Rectangle(0, 0, width - 1, height - 1);

            if (width <= 0 || height <= 0) return;

            using (GraphicsPath path = RoundedButton.GetRoundedPath(rect, borderRadius))
            {
                // Set clip to keep charts inside rounded corners
                g.SetClip(path);

                // Fill background with semi-transparent slot bg
                using (var bgBrush = new SolidBrush(Color.FromArgb(110, 12, 18, 31)))
                {
                    g.FillPath(bgBrush, path);
                }

                // Draw grid lines inside clip
                using (var gridPen = new Pen(Color.FromArgb(15, 255, 255, 255), 1))
                {
                    for (int i = 1; i < 4; i++)
                    {
                        float y = height * (i / 4f);
                        g.DrawLine(gridPen, 0, y, width, y);
                    }
                }

                if (_history.Count >= 2)
                {
                    // Map values to screen coordinates
                    var points = new PointF[_history.Count];
                    float xStep = (float)width / (_maxPoints - 1);

                    for (int i = 0; i < _history.Count; i++)
                    {
                        double val = _history[i];
                        val = Math.Max(0.0, Math.Min(100.0, val));
                        float x = i * xStep;
                        float y = height - (float)(val / 100.0 * (height - 35)) - 10;
                        points[i] = new PointF(x, y);
                    }

                    // Draw gradient area
                    using (var fillPath = new GraphicsPath())
                    {
                        fillPath.AddLine(0, height, points[0].X, points[0].Y);
                        for (int i = 1; i < points.Length; i++)
                        {
                            fillPath.AddLine(points[i - 1].X, points[i - 1].Y, points[i].X, points[i].Y);
                        }
                        fillPath.AddLine(points[points.Length - 1].X, points[points.Length - 1].Y, width, height);
                        fillPath.CloseFigure();

                        using (var fillBrush = new LinearGradientBrush(
                            new Point(0, 0),
                            new Point(0, height),
                            Color.FromArgb(80, ChartColor.R, ChartColor.G, ChartColor.B),
                            Color.FromArgb(0, ChartColor.R, ChartColor.G, ChartColor.B)
                        ))
                        {
                            g.FillPath(fillBrush, fillPath);
                        }
                    }

                    // Draw glowing trace line
                    using (var glowPen = new Pen(Color.FromArgb(60, ChartColor.R, ChartColor.G, ChartColor.B), 4f))
                    {
                        g.DrawLines(glowPen, points);
                    }

                    // Draw solid trace line
                    using (var linePen = new Pen(ChartColor, 2f))
                    {
                        g.DrawLines(linePen, points);
                    }
                }

                // Reset clip for border painting
                g.ResetClip();

                // Draw subtle Fluent card border
                using (LinearGradientBrush borderBrush = new LinearGradientBrush(
                    rect, Color.FromArgb(40, 255, 255, 255), Color.FromArgb(15, 255, 255, 255), LinearGradientMode.Vertical))
                using (Pen borderPen = new Pen(borderBrush, 1f))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            // Draw overlay text
            double curVal = _history[_history.Count - 1];
            string labelText = $"{Title}: {Math.Round(curVal, 1)}{Suffix}";

            using (var font = new Font("Segoe UI Semibold", 10f, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.FromArgb(241, 245, 249)))
            {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                g.DrawString(labelText, font, textBrush, 12, 10);
            }
        }
    }
}

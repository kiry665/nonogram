using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace nonogram.Controls
{
    public class Switch : Control
    {
        Rectangle rect;
        
        int TogglePosX_ON;
        int TogglePosX_OFF;
        public bool Checked { get; set; } = false;
         
        public Switch()
        {
            Size = new Size(50, 20);
            rect = new Rectangle(0, 0, Width - 1, Height - 1);
            TogglePosX_OFF = rect.X;
            TogglePosX_ON = rect.X + rect.Width - rect.Height;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            rect = new Rectangle(0, 0, Width - 1, Height - 1);
            TogglePosX_OFF = rect.X;
            TogglePosX_ON = rect.X + rect.Width - rect.Height;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            Pen contour = new Pen(Color.Gray);
            
            GraphicsPath rectGP = RoundedRectangle(rect, rect.Height);
            Rectangle rectToggle = new Rectangle(0, 0, rect.Height, rect.Height);

            if (Checked)
            {
                rectToggle.Location = new Point(TogglePosX_ON, rect.Y);
                g.FillEllipse(new SolidBrush(Color.FromArgb(255, 52, 72, 97)), rectToggle);
            }
            else
            {
                rectToggle.Location = new Point(TogglePosX_OFF, rect.Y);
                g.FillEllipse(new SolidBrush(Color.White), rectToggle);
            }

            g.DrawEllipse(contour, rectToggle);
            g.DrawPath(contour, rectGP);
        }

        private GraphicsPath RoundedRectangle(Rectangle rect, int RoundedSize)
        {
            GraphicsPath gp = new GraphicsPath();

            gp.AddArc(rect.X, rect.Y, RoundedSize, RoundedSize, 180, 90);
            gp.AddArc(rect.X + rect.Width - RoundedSize, rect.Y, RoundedSize, RoundedSize, 270, 90);
            gp.AddArc(rect.X + rect.Width - RoundedSize, rect.Y + rect.Height - RoundedSize, RoundedSize, RoundedSize, 0, 90);
            gp.AddArc(rect.X, rect.Y + rect.Height - RoundedSize, RoundedSize, RoundedSize, 90, 90);

            gp.CloseFigure();

            return gp;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Checked = !Checked;

            Invalidate();
        }
    }
}

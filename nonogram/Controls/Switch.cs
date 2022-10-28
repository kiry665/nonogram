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
        Rectangle rect = new Rectangle();
        int TogglePosX_ON;
        int TogglePosX_OFF;
        public bool Checked { get; set; } = false;
        public Switch()
        {
            Size = new Size(40, 15);
            BackColor = Color.White;
            rect = new Rectangle(0, 0, Width - 1, Height - 1);
            TogglePosX_OFF = rect.X;
            TogglePosX_ON = rect.Width - rect.Height;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.Clear(Parent.BackColor);

            Pen pen_contur = new Pen(Color.Gray, 1);

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            Rectangle rectToggle = new Rectangle(0, 0, Height - 1, Height - 1);
            GraphicsPath rectGP = RoundedRect(rect, rect.Height);

            g.FillPath(new SolidBrush(BackColor), rectGP);
            g.DrawPath(pen_contur, rectGP);

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

            g.DrawEllipse(pen_contur, rectToggle);
        }

        private GraphicsPath RoundedRect(Rectangle rect, int RoundSize)
        {
            GraphicsPath path = new GraphicsPath();

            path.AddArc(rect.X, rect.Y, RoundSize, RoundSize, 180, 90);
            path.AddArc(rect.X + rect.Width - RoundSize, rect.Y, RoundSize, RoundSize, 270, 90);
            path.AddArc(rect.X + rect.Width - RoundSize, rect.Y + rect.Height - RoundSize, RoundSize, RoundSize, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - RoundSize, RoundSize, RoundSize, 90, 90);

            path.CloseFigure();

            return path;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            SwitchToggle();
        }

        private void SwitchToggle()
        {
            Checked = !Checked;

            Invalidate();
        }
    }
}

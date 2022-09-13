using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nonogram
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        //переменные и постоянные
        const int x0 = 10, y0 = 10, col = 10, row = 10, row_up = 2, col_left = 2, line_big_block = 2, line_small_block = 1, size_block = 50;
        bool flag, flag_H = false, flag_V = false, color = true;
        string s;
        StreamReader f = new StreamReader("fish.txt");
        int[,] nonogram = new int[col, row];
        int x_start, y_start, c, col_time, row_time;

        Pen black = new Pen(Color.FromArgb(255, 0, 0, 0), line_big_block);
        Pen gray = new Pen(Color.FromArgb(255, 192, 196, 207), line_small_block);
        Pen blue = new Pen(Color.FromArgb(255, 20, 40, 65), line_small_block);
        SolidBrush light_blue = new SolidBrush(Color.FromArgb(255, 52, 72, 97));
        SolidBrush white = new SolidBrush(Color.FromArgb(255, 255, 255, 255));


        private void Form2_Load(object sender, EventArgs e)
        {
            button1.BackColor = Color.Black;
            //читаем построчно файл с "картинкой"
            while ((s = f.ReadLine()) != null)
            {
                int[]? test = s.Split(' ').Select(int.Parse).ToArray();
                for (int i = 0; i < 10; i++)
                {
                    nonogram[c, i] = test[i];
                }
                c++;
            }
            f.Close();
            //можно убрать
            dataGridView1.RowCount = row;
            dataGridView1.ColumnCount = col;
            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                    dataGridView1.Rows[i].Cells[j].Value = nonogram[i, j];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            color = !color;
            if (color)
                button1.BackColor = Color.Black;
            else
                button1.BackColor = Color.White;
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            this.BackColor = Color.White;

            //левый блок
            x_start = x0 + col_left * size_block + line_big_block; y_start = y0;
            g.DrawRectangle(black, x_start, y_start, col * size_block + line_big_block, row_up * size_block + line_big_block);
            for (int i = 0; i < row_up; i++)
                for (int j = 0; j < col; j++)
                {
                    int x = x_start + size_block * j;
                    int y = y_start + size_block * i;
                    g.DrawRectangle(gray, x + 1, y + 1, size_block - 1, size_block - 1);
                }

            //верхний блок
            x_start = x0; y_start = y0 + row_up * size_block + line_big_block;
            g.DrawRectangle(black, x_start, y_start, size_block * col_left + line_big_block, size_block * row + line_big_block);
            for (int i = 0; i < row; i++)
                for (int j = 0; j < col_left; j++)
                {
                    int x = x_start + size_block * j;
                    int y = y_start + size_block * i;
                    g.DrawRectangle(gray, x + 1, y + 1, size_block - 1, size_block - 1);
                }

            //игровое поле
            x_start = x0 + col_left * size_block + line_big_block; y_start = y0 + row_up * size_block + line_big_block;
            g.DrawRectangle(black, x_start, y_start, size_block * col + line_big_block, size_block * row + line_big_block);
            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                {
                    int x = x_start + size_block * j;
                    int y = y_start + size_block * i;
                    if (nonogram[i, j] == 0)
                        g.DrawRectangle(gray, x + 1, y + 1, size_block - 1, size_block - 1);
                    else
                    {
                        g.FillRectangle(light_blue, x + 1, y + 1, size_block - 1, size_block - 1);
                        g.DrawRectangle(blue, x + 1, y + 1, size_block - 1, size_block - 1);
                    }


                }

            for (int i = 1; i < col; i++) //жирные вертикальные линии
                if (i % 5 == 0)
                {
                    int x1 = x_start + size_block * i + 1;
                    int y1 = y0;
                    int y2 = y_start + size_block * row;
                    g.DrawLine(black, x1, y1, x1, y2 + 1);
                }

            for (int i = 1; i < row; i++) //жирные горизонтальные линии
                if (i % 5 == 0)
                {
                    int x1 = x0;
                    int x2 = x_start + size_block * col;
                    int y1 = y_start + size_block * i + 1;
                    g.DrawLine(black, x1, y1, x2 + 1, y1);
                }

        }

        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            flag = true;
            flag_H = false;
            flag_V = false;
            x_y_mouse(e);
        }

        private void Form2_MouseMove_1(object sender, MouseEventArgs e)
        {
            x_y_mouse(e);
        }

        private void Form2_MouseUp(object sender, MouseEventArgs e)
        {
            flag = false;
        }



        private void redraw(int row_r, int col_r)
        {
            Graphics g = CreateGraphics();
            int x = x_start + size_block * col_r;
            int y = y_start + size_block * row_r;
            
            if (color)
            {
                g.FillRectangle(light_blue, x + 1, y + 1, size_block - 1, size_block - 1);
                g.DrawRectangle(blue, x + 1, y + 1, size_block - 1, size_block - 1);
            }
            else
            {
                g.FillRectangle(white, x + 1, y + 1, size_block - 1, size_block - 1);
                g.DrawRectangle(gray, x + 1, y + 1, size_block - 1, size_block - 1);
            }

            for (int i = 1; i < col; i++) //жирные вертикальные линии
                if (i % 5 == 0)
                {
                    int x1 = x_start + size_block * i + 1;
                    int y1 = y0;
                    int y2 = y_start + size_block * row;
                    g.DrawLine(black, x1, y1, x1, y2 + 1);
                }

            for (int i = 1; i < row; i++) //жирные горизонтальные линии
                if (i % 5 == 0)
                {
                    int x1 = x0;
                    int x2 = x_start + size_block * col;
                    int y1 = y_start + size_block * i + 1;
                    g.DrawLine(black, x1, y1, x2 + 1, y1);
                }
        }

        private void x_y_mouse(MouseEventArgs e)
        {
            x_start = x0 + col_left * size_block + line_big_block; y_start = y0 + row_up * size_block + line_big_block;
            if ((e.Button == MouseButtons.Left) && (flag))
            {

                int col_mouse = (e.X - x_start) / size_block;
                int row_mouse = (e.Y - y_start) / size_block;
                row_time = row_mouse;
                col_time = col_mouse;

                textBox1.Text = row_mouse.ToString();
                textBox2.Text = col_mouse.ToString();

                if ((row_mouse >= 0 && row_mouse <= 9) && (col_mouse >= 0 && col_mouse <= 9))
                {
                    if (color)
                        nonogram[row_mouse, col_mouse] = 1;
                    else
                        nonogram[row_mouse, col_mouse] = 0;

                    dataGridView1.Rows[row_mouse].Cells[col_mouse].Value = nonogram[row_mouse, col_mouse];
                    redraw(row_mouse, col_mouse);
                    flag = false;
                }
            }

            if ((e.Button == MouseButtons.Left) && (!flag))
            {
                int col_mouse = (e.X - x_start) / size_block;
                int row_mouse = (e.Y - y_start) / size_block;

                if ((row_time != row_mouse) && (!flag_H))
                    flag_V = true;

                if ((col_time != col_mouse) && (!flag_V))
                    flag_H = true;

                textBox1.Text = row_mouse.ToString();
                textBox2.Text = col_mouse.ToString();

                if ((row_mouse >= 0 && row_mouse <= 9) && (col_mouse >= 0 && col_mouse <= 9))
                {
                    if (color)
                        nonogram[row_mouse, col_mouse] = 1;
                    else
                        nonogram[row_mouse, col_mouse] = 0;
                    if (flag_V)
                    {
                        redraw(row_mouse, col_time);
                        dataGridView1.Rows[row_mouse].Cells[col_time].Value = nonogram[row_mouse, col_mouse];
                    }
                        
                    if (flag_H)
                    {
                        redraw(row_time, col_mouse);
                        dataGridView1.Rows[row_mouse].Cells[col_mouse].Value = nonogram[row_mouse, col_mouse];
                    }
                        
                }
            }
        }

    }
}

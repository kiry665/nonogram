using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nonogram
{
    public partial class Form2 : Form
    {
        Form form1 = new Form1();
        const int x0 = 10, y0 = 10, line_big_block = 2, line_small_block = 1, size_block = 50;
        bool flag, flag_move_mouse, flag_H = false, flag_V = false, color = true;
        int col, row, max_left, max_up, row_up, col_left, mode, act, number_level;
        int x_start, y_start, col_time = 0, row_time = 0;
        int[,] answer, nonogram;
        int[][] left, up;
        Pen black = new Pen(Color.FromArgb(255, 0, 0, 0), line_big_block);
        Pen gray = new Pen(Color.FromArgb(255, 192, 196, 207), line_small_block);
        Pen blue = new Pen(Color.FromArgb(255, 20, 40, 65), line_small_block);
        Pen light_blue_pen = new Pen(Color.FromArgb(255, 20, 40, 65), 4);
        SolidBrush light_blue = new SolidBrush(Color.FromArgb(255, 52, 72, 97));
        SolidBrush white = new SolidBrush(Color.FromArgb(255, 255, 255, 255));

        public Form2(Form1 f1, int row, int col, int[,] nonogram, int number_level)
        {
            InitializeComponent();
            form1 = f1;
            this.nonogram = nonogram;
            answer = new int[row, col];
            this.row = row;
            this.col = col;
            this.number_level = number_level;
            counter(nonogram);
            this.Width = 2 * x0 + (col_left + col) * size_block + 6 + 15;
            this.Height = 2 * y0 + (row_up + row) * size_block + 6 + 45;
            this.Text = "Level " + number_level;
            button1.BackColor = Color.FromArgb(255, 20, 40, 65);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            color = !color;
            if (color)
                button1.BackColor = Color.FromArgb(255, 20, 40, 65);
            else
                button1.BackColor = Color.White;
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            this.BackColor = Color.White;

            //верхний блок
            x_start = x0 + col_left * size_block + line_big_block; y_start = y0;
            g.DrawRectangle(black, x_start, y_start, col * size_block + line_big_block, row_up * size_block + line_big_block);
            for (int i = 0; i < col; i++)
                for (int j = 0; j < row_up; j++)
                {
                    int x = x_start + size_block * i;
                    int y = y_start + size_block * j;
                    g.DrawRectangle(gray, x + 1, y + 1, size_block - 1, size_block - 1);
                    if (row_up - j - 1 < up[i].Length)
                        DrawString(x, y, up[i][up[i].Length - (row_up - j - 1) - 1]);
                }

            //левый блок
            x_start = x0; y_start = y0 + row_up * size_block + line_big_block;
            g.DrawRectangle(black, x_start, y_start, size_block * col_left + line_big_block, size_block * row + line_big_block);
            for (int i = 0; i < row; i++)
                for (int j = 0; j < col_left; j++)
                {
                    int x = x_start + size_block * j;
                    int y = y_start + size_block * i;
                    g.DrawRectangle(gray, x + 1, y + 1, size_block - 1, size_block - 1);
                    if (col_left - j - 1 < left[i].Length)
                        DrawString(x, y, left[i][left[i].Length - (col_left - j - 1) - 1]);
                }

            //игровое поле
            x_start = x0 + col_left * size_block + line_big_block; y_start = y0 + row_up * size_block + line_big_block;
            g.DrawRectangle(black, x_start, y_start, size_block * col + line_big_block, size_block * row + line_big_block);
            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                {
                    int x = x_start + size_block * j;
                    int y = y_start + size_block * i;
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

        }//полная отрисовка 

        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Middle)
            {
                color = !color;
                if (color)
                    button1.BackColor = Color.FromArgb(255, 20, 40, 65);
                else
                    button1.BackColor = Color.White;
            }
            if(e.Button == MouseButtons.Left)
            {
                flag = true;
                flag_move_mouse = true;
                flag_H = false;
                flag_V = false;
                x_y_mouse(e);
            }
        }

        private void Form2_MouseMove_1(object sender, MouseEventArgs e)
        {
            //if (flag_move_mouse)
                x_y_mouse(e);
        }

        private void x_y_mouse(MouseEventArgs e)
        {
            bool priority = false;
            if ((e.Button == MouseButtons.Left) && (flag))
            {
                int col_mouse = (e.X - x_start) / size_block;
                int row_mouse = (e.Y - y_start) / size_block;
                row_time = row_mouse;
                col_time = col_mouse;
                act = 3;
                if ((row_mouse >= 0 && row_mouse <= row - 1) && (col_mouse >= 0 && col_mouse <= col - 1))
                {
                    if (color && answer[row_mouse, col_mouse] == 0)
                    {
                        act = 1;
                        mode = 0;
                    } 
                    else 
                    if (color && answer[row_mouse, col_mouse] == 1) // mode 1
                    {
                        act = 0;
                        mode = 1;
                    }  
                    
                    if (!color && answer[row_mouse, col_mouse] == 0) // mode 2
                    {
                        act = 2;
                        mode = 2;
                    } 
                    else
                    if (!color && answer[row_mouse, col_mouse] == 2)// mode 3
                    {
                        act = 0;
                        mode = 3;
                    }
                    else 
                    if (!color && answer[row_mouse, col_mouse] == 1)
                    {
                        act = 2;
                        mode = 4;
                        priority = true;
                    }
                    if (act != 3)
                        redraw(row_mouse, col_mouse, act, priority);
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

                if ((flag_V) && (row_mouse >= 0 && row_mouse <= row - 1) && (col_time >= 0 && col_time <= col - 1))
                {
                    switch (mode)
                    {
                        case 0:
                            act = 1;
                            break;
                        case 1:
                            act = 0;
                            break;
                        case 2:
                            act = 2;
                            break;
                        case 3:
                            act = 0;
                            break;
                        case 4:
                            act = 2;
                            break;

                    }
                        
                    redraw(row_mouse, col_time, act, priority);
                    
                }

                if ((flag_H) && (row_time >= 0 && row_time <= row - 1) && (col_mouse >= 0 && col_mouse <= col - 1))
                {
                    switch (mode)
                    {
                        case 0:
                            act = 1;
                            break;
                        case 1:
                            act = 0;
                            break;
                        case 2:
                            act = 2;
                            break;
                        case 3:
                            act = 0;
                            break;
                        case 4:
                            act = 2;
                            break;
                    }
                    redraw(row_time, col_mouse, act, priority);
                    
                }
            }
        } 

        private void redraw(int row_r, int col_r, int act, bool priority)
        {
            Graphics g = CreateGraphics();
            int x = x_start + size_block * col_r;
            int y = y_start + size_block * row_r;

            if ((act == 0) && (answer[row_r, col_r] != act))
            {
                g.FillRectangle(white, x + 1, y + 1, size_block - 1, size_block - 1);
                g.DrawRectangle(gray, x + 1, y + 1, size_block - 1, size_block - 1);
                answer[row_r, col_r] = act;
            }

            if ((act == 1) && (answer[row_r, col_r] != act))
            {
                g.FillRectangle(light_blue, x + 1, y + 1, size_block - 1, size_block - 1);
                g.DrawRectangle(blue, x + 1, y + 1, size_block - 1, size_block - 1);
                answer[row_r, col_r] = act;
            }

            if ((act == 2) && (answer[row_r, col_r] != act) && ((answer[row_r,col_r] != 1) || priority))
            {
                g.FillRectangle(white, x + 1, y + 1, size_block - 1, size_block - 1);
                g.DrawRectangle(gray, x + 1, y + 1, size_block - 1, size_block - 1);
                g.DrawLine(light_blue_pen, x + 10, y + 10, x + size_block - 10, y + size_block - 10);
                g.DrawLine(light_blue_pen, x + 10, y + size_block - 10, x + size_block - 10, y + 10);
                answer[row_r, col_r] = act;
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
            cross(answer, row_r, col_r);
            check(answer, row_r, col_r);
        }

        private void counter(int[,] patern)
        {
            int count;
            left = new int[row][];
            up = new int[col][];
            List<int> temporary = new List<int> { };

            //массив левого блока
            for (int i = 0; i < row; i++)
            {
                count = 0;
                temporary.Clear();
                for (int j = 0; j < col; j++)
                {
                    if (patern[i, j] == 1)
                        count++;
                    if ((patern[i,j] == 0) && (count > 0))
                    {
                        temporary.Add(count);
                        count = 0;
                    }
                    if((j == col - 1) && (count > 0))
                    {
                        temporary.Add(count);
                    }
                }
                left[i] = temporary.ToArray();
                if (left[i].Length > max_left)
                    max_left = left[i].Length;
            }
            col_left = max_left;

            //массив верхнего блока
            for (int j = 0; j < col; j++)
            {
                count = 0;
                temporary.Clear();
                for (int i = 0; i < row; i++)
                {
                    if (patern[i, j] == 1)
                        count++;
                    if ((patern[i, j] == 0) && (count > 0))
                    {
                        temporary.Add(count);
                        count = 0;
                    }
                    if ((i == col - 1) && (count > 0))
                    {
                        temporary.Add(count);
                    }
                }
                up[j] = temporary.ToArray();
                if (up[j].Length > max_up)
                    max_up = up[j].Length;
            }
            row_up = max_up;
        }

        private void DrawString(int x, int y, int number)
        {
            string drawString = number.ToString();
            Graphics formGraphics = this.CreateGraphics();
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            Font drawFont = new Font("Arial", 16); 
            Size len = TextRenderer.MeasureText(drawString, drawFont);
            int coef_x = ((size_block - len.Width) / 2) + 4 / drawString.Length;
            int coef_y = ((size_block - len.Height) / 2) + 1;
            StringFormat drawFormat = new StringFormat();
            formGraphics.DrawString(drawString, drawFont, drawBrush, x + coef_x, y + coef_y, drawFormat);
        }

        private void check(int[,] patern, int row, int col)
        {
            bool check = true;

            if (patern[row, col] != nonogram[row, col])
                check = false;

            for (int i = 0; i < this.row; i++)
            {
                if (!check)
                    break;
                for (int j = 0; j < this.col; j++)
                {
                   if ((patern[i,j] == 1) && (patern[i,j] != nonogram[i, j]))
                    {
                        check = false;
                        break;
                    }
                   if (((patern[i, j] == 0) || (patern[i, j] == 2)) && (nonogram[i,j] != 0))
                    {
                        check = false;
                        break;
                    }
                }
            }

            if (check)
            {
                MessageBox.Show("Победа");
                SQL_Change_Passed();
            }
                
        }

        private void cross(int[,] patern, int row, int col)
        {
            List<int> temporary = new List<int> { };
            Boolean cross_flag_H = false, cross_flag_V = false;
            int count = 0;

            for (int i = 0; i < this.col; i++)
            {
                if (patern[row, i] == 1)
                    count++;
                if (patern[row, i] != 1 && count > 0)
                {
                    temporary.Add(count);
                    count = 0;
                }
                if ((i == this.col - 1) && (count != 0))
                    temporary.Add(count);
            }
            
            if (left[row].Length == temporary.Count)
            {
                cross_flag_H = true;
                for (int i = 0; i < left[row].Length; i++)
                {
                    if (left[row][i] != temporary[i])
                        cross_flag_H = false;
                }
            }

            if (cross_flag_H)
            {
                for (int i = 0; i < this.col; i++)
                {
                    if (patern[row,i] == 0)
                    {
                        flag_H = false;
                        redraw(row, i, 2, false);
                    }
                }
            }

            count = 0;
            temporary.Clear();
            for (int i = 0; i < this.row; i++)
            {
                if (patern[i, col] == 1)
                    count++;
                if (patern[i, col] != 1 && count > 0)
                {
                    temporary.Add(count);
                    count = 0;
                }
                if ((i == this.row - 1) && (count != 0))
                    temporary.Add(count);
            }

            if (up[col].Length == temporary.Count)
            {
                cross_flag_V = true;
                for (int i = 0; i < up[col].Length; i++)
                {
                    if (up[col][i] != temporary[i])
                        cross_flag_V = false;
                }
            }

            if (cross_flag_V)
            {
                for (int i = 0; i < this.row; i++)
                {
                    if (patern[i, col] == 0)
                    {
                        flag_V = false;
                        redraw(i, col, 2, false);
                    }
                }
            }

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            form1.Visible = true;
        }

        private void SQL_Change_Passed()
        {
            try
            {
                using (var connection = new SQLiteConnection(@"Data Source = db.sqlite"))
                {
                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand("UPDATE Levels SET Passed = 1 WHERE Number_Level = " + number_level, connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {

            }
        }

    }

    
}

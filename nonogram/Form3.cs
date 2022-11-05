using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nonogram
{
    public partial class Form3 : Form
    {
        const int x0 = 60, y0 = 60, size_block = 50, line_big_block = 2, line_small_block = 1;
        int x_start, y_start, row = 0, col = 0;
        bool flag, flag_move_mouse, flag_H = false, flag_V = false;
        Form1 form1 = new Form1();
        int mode, act, row_time, col_time;
        int[,] answer;
        Pen black = new Pen(Color.FromArgb(255, 0, 0, 0), line_big_block);
        Pen gray = new Pen(Color.FromArgb(255, 192, 196, 207), line_small_block);
        Pen blue = new Pen(Color.FromArgb(255, 20, 40, 65), line_small_block);
        Pen light_blue_pen = new Pen(Color.FromArgb(255, 20, 40, 65), 4);
        SolidBrush light_blue = new SolidBrush(Color.FromArgb(255, 52, 72, 97));
        SolidBrush white = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
        public Form3(Form1 f1)
        {
            InitializeComponent();
            form1 = f1;
            domainUpDown1.SelectedIndex = 0;
            numericUpDown1.Maximum = SQL_Count();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Вы точно хотите внести изменения?", "Внесение изменений в БД", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                string s = "";
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        s += answer[i, j].ToString();
                        if (j != col - 1)
                            s += ",";
                    }
                    s += ";";
                }
                SQL_UPDATE(s);
                MessageBox.Show(s);
                
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            SQL_Levels();
            textBox1.Text = row.ToString();
            textBox2.Text = col.ToString();
            this.Width = (2 * x0 + col * size_block < 1000) ? 1000 : 2 * x0 + col * size_block;
            this.Height = 3 * y0 + row * size_block;
            Graphics g = this.CreateGraphics();
            g.Clear(Color.White);

            Invalidate();
        }

        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {
            numericUpDown1.Maximum = SQL_Count();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Вы точно хотите внести изменения?", "Внесение изменений в БД", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                string s = "";
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        s += answer[i, j].ToString();
                        if (j != col - 1)
                            s += ",";
                    }
                    s += ";";
                }
                SQL_INSERT(s);
                MessageBox.Show(s);
                numericUpDown1.Maximum = SQL_Count();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            row = Convert.ToInt32(textBox1.Text);
            col = Convert.ToInt32(textBox2.Text);
            answer = new int[row, col];
            this.Width = (2 * x0 + col * size_block < 1000) ? 1000 : 2 * x0 + col * size_block;
            this.Height = 3 * y0 + row * size_block;

            Graphics g = this.CreateGraphics();
            g.Clear(Color.White);

            Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SQL_Levels();
            textBox1.Text = row.ToString();
            textBox2.Text = col.ToString();
            this.Width = (2 * x0 + col * size_block < 1000) ? 1000 : 2 * x0 + col * size_block;
            this.Height = 3 * y0 + row * size_block;
            Graphics g = this.CreateGraphics();
            g.Clear(Color.White);

            Invalidate();
        }

        private void Form3_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            this.BackColor = Color.White;

            x_start = x0 ; y_start = y0;
            g.DrawRectangle(black, x_start, y_start, size_block * col + line_big_block, size_block * row + line_big_block);
            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                {
                    int x = x_start + size_block * j;
                    int y = y_start + size_block * i;
                    g.DrawRectangle(gray, x + 1, y + 1, size_block - 1, size_block - 1);
                    if (answer[i, j] == 1)
                        redraw(i, j, 1);
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

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            form1.Visible = true;
        }

        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
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
            x_y_mouse(e);
        }

        private void x_y_mouse(MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (flag))
            {
                int col_mouse = (e.X - x_start) / size_block;
                int row_mouse = (e.Y - y_start) / size_block;
                row_time = row_mouse;
                col_time = col_mouse;

                if ((row_mouse >= 0 && row_mouse <= row - 1) && (col_mouse >= 0 && col_mouse <= col - 1))
                {
                    act = 3;

                    switch (answer[row_mouse, col_mouse])
                    {
                        case 0:
                            act = 1;
                            mode = 0;
                            break;
                        case 1:
                            act = 0;
                            mode = 1;
                            break;
                    }
                    
                    redraw(row_mouse, col_mouse, act);
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
                    if (answer[row_mouse, col_time] == mode)
                        redraw(row_mouse, col_time, act);
                    else
                        flag_V = false;
                }

                if ((flag_H) && (row_time >= 0 && row_time <= row - 1) && (col_mouse >= 0 && col_mouse <= col - 1))
                {
                    if (answer[row_time, col_mouse] == mode)
                        redraw(row_time, col_mouse, act);

                }
            }
        }

        private void redraw(int row_r, int col_r, int act)
        {
            Graphics g = CreateGraphics();
            int x = x_start + size_block * col_r;
            int y = y_start + size_block * row_r;

            if (act == 0)
            {
                g.FillRectangle(white, x + 1, y + 1, size_block - 1, size_block - 1);
                g.DrawRectangle(gray, x + 1, y + 1, size_block - 1, size_block - 1);
                answer[row_r, col_r] = act;
            }

            if (act == 1)
            {
                g.FillRectangle(light_blue, x + 1, y + 1, size_block - 1, size_block - 1);
                g.DrawRectangle(blue, x + 1, y + 1, size_block - 1, size_block - 1);
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
        }

        private void SQL_INSERT(string s)
        {
            try
            {
                using (var connection = new SQLiteConnection(@"Data Source = db.sqlite"))
                {
                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand("INSERT INTO "+ domainUpDown1.Text +"(Array, Row, Column) Values(\"" + s + "\", " + row + ", " + col + ")", connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {

            }
        }

        private int SQL_Count()
        {
            try
            {
                using (var connection = new SQLiteConnection(@"Data Source = db.sqlite"))
                {
                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand(@"SELECT COUNT(*) FROM " + domainUpDown1.Text, connection);
                    object count = cmd.ExecuteScalar();
                    return (Convert.ToInt32(count));
                }
            }
            catch
            {
                return 0;
            }
        }

        private void SQL_Levels()
        {
            try
            {
                using (var connection = new SQLiteConnection(@"Data Source = db.sqlite"))
                {
                    int number_level = Convert.ToInt32(numericUpDown1.Value);
                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM " + domainUpDown1.Text + " WHERE Number_Level = " + number_level, connection);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())   // построчно считываем данные
                        {
                            var s = reader.GetValue(1);
                            row = reader.GetInt32(2);
                            col = reader.GetInt32(3);
                            answer = new int[row, col];
                            string[] words = s.ToString().Split(';');

                            for (int i = 0; i < row; i++)
                            {
                                string[] symbols = words[i].Split(',');
                                for (int j = 0; j < col; j++)
                                {
                                    answer[i, j] = Convert.ToInt32(symbols[j]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SQL_UPDATE(string s)
        {
            try
            {
                using (var connection = new SQLiteConnection(@"Data Source = db.sqlite"))
                {
                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand("UPDATE " + domainUpDown1.Text + " SET Array = \"" + s + "\", Row = " + row + ", Column = " + col + " WHERE Number_Level = " + numericUpDown1.Value, connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {

            }
        }
    }
}

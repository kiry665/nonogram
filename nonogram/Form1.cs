using System.Windows.Forms;
using System.Data.SQLite;
using Microsoft.VisualBasic.ApplicationServices;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace nonogram
{
    public partial class Form1 : Form
    {
        #region Переменные
        int row = 0, col = 0, number_level = 0;
        int[,] patern = new int[1, 1];
        List<string> difficult = new List<string>
        {
            "Easy", "Medium"
        };
        List<string> difficult_RU = new List<string>
        {
            "Легкий", "Средний"
        };
        #endregion                                                                                             //блок переменных

        public Form1()
        {
            InitializeComponent();
            domainUpDown1.Items.AddRange(difficult_RU);                                                        //наполнение DomainUpDown
            domainUpDown1.SelectedIndex = Convert.ToInt32(SQL_GET("Last_Difficult"));                          //отображение уровня сложности
            numericUpDown1.Maximum = SQL_Count();                                                              //ограничение максимального кол-ва уровней
            numericUpDown1.Value = Convert.ToInt32(SQL_GET("Last_" + difficult[domainUpDown1.SelectedIndex])); //отображение уровня сложности
            Check_Label();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SQL_Levels();

            Form2 form2 = new Form2(this, row, col, patern, Convert.ToInt32(numericUpDown1.Value), difficult[domainUpDown1.SelectedIndex]);
            form2.Show();                                                                                      // отображение form2
            this.Visible = false;                                                                              // отключение видимости form1
        }

        protected override void OnKeyDown(KeyEventArgs e)                                                      // событие нажатия кнопок на клавиатуре
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Z && e.Alt)
            {
                Form3 form3 = new Form3(this);
                form3.Show();
                this.Visible = false;
                e.Handled = true;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)                                   //событие изменения значения numericUpDown1
        {
                                                                                                               //number_level = Convert.ToInt32(numericUpDown1.Value);
            SQL_SET();
            Check_Label();
        }

        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)                             //событие изменения значения domainUpDown1
        {
                                                                                                               //SQL_SET();
            numericUpDown1.Maximum = SQL_Count();
            numericUpDown1.Value = Convert.ToInt32(SQL_GET("Last_" + difficult[domainUpDown1.SelectedIndex]));
                                                                                                               //number_level = Convert.ToInt32(numericUpDown1.Value);
            Check_Label();
        }

        private void Form1_Activated(object sender, EventArgs e)                                               //событие активации form1
        {
                                                                                                               //number_level = Convert.ToInt32(numericUpDown1.Value);
            Check_Label();
        }

        private void Check_Label()                                                                             // событие отображение информации в label1 с проверкой
        {

            if (SQL_Passed(difficult[domainUpDown1.SelectedIndex], number_level))
            {
                label2.Text = "✓";
                label2.ForeColor = Color.Green;
            }
            else
            {
                label2.Text = "X";
                label2.ForeColor = Color.Red;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)                                  //событие закрытия form1
        {
            SQL_SET();
        }

        #region SQL
        private void SQL_Levels()
        {
            try
            {
                using (var connection = new SQLiteConnection(@"Data Source = db.sqlite"))
                {
                    number_level = Convert.ToInt32(numericUpDown1.Value);
                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM " + difficult[domainUpDown1.SelectedIndex] + " WHERE Number_Level = " + number_level, connection);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())                                                                  // построчно считываем данные
                        {
                            var s = reader.GetValue(1);
                            row = reader.GetInt32(2);
                            col = reader.GetInt32(3);
                            patern = new int[row, col];
                            string[] words = s.ToString().Split(';');

                            for (int i = 0; i < row; i++)
                            {
                                string[] symbols = words[i].Split(',');
                                for (int j = 0; j < col; j++)
                                {
                                    patern[i, j] = Convert.ToInt32(symbols[j]);
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
        private int SQL_Count()
        {
            try
            {
                using (var connection = new SQLiteConnection(@"Data Source = db.sqlite"))
                {
                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand(@"SELECT COUNT(*) FROM " + difficult[domainUpDown1.SelectedIndex], connection);
                    object count = cmd.ExecuteScalar();
                    return (Convert.ToInt32(count));
                }
            }
            catch
            {
                return 0;
            }
        }
        private bool SQL_Passed(string table, int num)
        {
            try
            {
                using(var connection = new SQLiteConnection(@"Data Source = db.sqlite"))
                {
                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT Passed FROM " + table + " WHERE Number_Level = " + num, connection);
                    object pass = cmd.ExecuteScalar();
                    return (Convert.ToBoolean(pass));
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        private object SQL_GET(string cell)
        {
            try
            {
                using (var connection = new SQLiteConnection(@"Data Source = db.sqlite"))
                {
                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT " + cell + " FROM Settings WHERE KEY = 1", connection);
                    object last_level = cmd.ExecuteScalar();
                    return last_level;
                }
            }
            catch
            {
                return "";
            }
        }

        private void SQL_SET()
        {
            try
            {
                using (var connection = new SQLiteConnection(@"Data Source = db.sqlite"))
                {
                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand("UPDATE Settings SET Last_" + difficult[domainUpDown1.SelectedIndex] + " =" + numericUpDown1.Value, connection);
                    cmd.ExecuteNonQuery();
                    cmd = new SQLiteCommand("UPDATE Settings SET Last_difficult = " + domainUpDown1.SelectedIndex, connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {

            }
        }
        #endregion                                                                                             // блок SQL запросов для БД

    }
}

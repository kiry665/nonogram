using System.Windows.Forms;
using System.Data.SQLite;
using Microsoft.VisualBasic.ApplicationServices;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace nonogram
{
    public partial class Form1 : Form
    {
        int row = 0, col = 0, number_level = 0;
        int[,] patern = new int[1, 1];

        public Form1()
        {
            InitializeComponent();
            numericUpDown1.Maximum = SQL_Count();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SQL_Levels();

            Form2 form2 = new Form2(this, row, col, patern, number_level);
            form2.Show();
            this.Visible = false;
        }

        private void SQL_Levels()
        {
            try
            {
                using (var connection = new SQLiteConnection(@"Data Source = db.sqlite"))
                {
                    number_level = Convert.ToInt32(numericUpDown1.Value);
                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Levels WHERE Number_Level = " + number_level, connection);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())   // ��������� ��������� ������
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
                    SQLiteCommand cmd = new SQLiteCommand(@"SELECT COUNT(*) FROM Levels;", connection);
                    object count = cmd.ExecuteScalar();

                    return (Convert.ToInt32(count));
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        
    }
}
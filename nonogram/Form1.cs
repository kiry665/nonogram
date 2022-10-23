using System.Windows.Forms;
using System.Data.SQLite;
namespace nonogram
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int row = 0, col = 0;
            int[,] patern = new int[1,1]; 
            try
            {
                using (var connection = new SQLiteConnection(@"Data Source = db.sqlite"))
                {
                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Levels WHERE Number_Level = 1", connection);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())   // построчно считываем данные
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

            Form2 form2 = new Form2(this, row, col, patern);
            form2.Show();
            this.Visible = false;
        }

        
    }
}
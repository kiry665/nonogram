using System.Data.SQLite;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (var connection = new SQLiteConnection(@"Data Source = db.sqlite"))
                {
                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Levels WHERE Number_Level = 13", connection);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())   // построчно считываем данные
                        {
                            var array = reader.GetValue(1);
                            int row = reader.GetInt32(2);
                            int col = reader.GetInt32(3);
                            int[,] patern = new int[row, col];
                            string[] rows = array.ToString().Split(';');

                            dataGridView1.RowCount = row;
                            dataGridView1.ColumnCount = col;

                            for (int i = 0; i < row; i++)
                            {
                                string[] cells = rows[i].Split(',');
                                for (int j = 0; j < col; j++)
                                {
                                    patern[i, j] = Convert.ToInt32(cells[j]);
                                    dataGridView1.Rows[i].Cells[j].Value = patern[i, j];
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
    }
}
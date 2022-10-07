namespace nonogram
{
    public partial class Form1 : Form
    {
        //public List<string> txt = new List<string> {"oak.txt"};
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            DirectoryInfo d = new DirectoryInfo("easy");
            FileInfo[] f = d.GetFiles("*.txt");

            List<string> files = new List<string> { };
            foreach(FileInfo file in f)
            {
                files.Add(file.Name);
            }

            Form2 form2 = new Form2("easy/" + files[rnd.Next(0,files.Count)], this);
            form2.Show();
            this.Visible = false;
        }
    }
}
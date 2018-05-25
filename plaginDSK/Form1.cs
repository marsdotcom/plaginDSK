using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;


namespace plaginDSK
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            data2 = new BindingList<RowPiket>();
        }

        public BindingList<RowPiket> data2;
        public double X, Y, YBase, Max, Min;

        private void Button1_Click(object sender, EventArgs e)
        {

            data2.Clear();

            string[] str = Clipboard.GetText().Split('\n');

            for (int i = 0; i < str.Length; i++)
            {
                string[] s = str[i].Split('\t');
                if (s.Length > 2)
                {
                    data2.Add(new RowPiket(s[0], s[1], s[2]));
                }
                else break;
            }

            if (data2.Count > 1)
            {
                Max = data2.Max(RowPiket => RowPiket.Project);
                Min = data2.Min(RowPiket => RowPiket.Project);
                double h = Max - Min; if (h == 0) h = 1;
                double k = 100 / h;
                textBox2.Text = k.ToString("N0");
                textBox3.Text = (200 - Max * k).ToString("N0");
            }                     

            dataGridView1.DataSource = data2;
        }

        private void Button2_Click(object sender, EventArgs e)
        {    
            if (!double.TryParse(textBox1.Text, out X)) X = 1;
            if (!double.TryParse(textBox2.Text, out Y)) Y = 1;
            if(!double.TryParse(textBox3.Text, out YBase)) YBase = 0;
        }

    }

    public class RowPiket
    {
        public string Piket { get; set; }
        public double Project { get; set; }
        public double Fact { get; set; }
        public double Delta;

        public RowPiket(string Piket, string Project, string Fact)
        {
            this.Piket = Piket;
            Parsestr(Project, Fact);
            Delta = this.Project - this.Fact;
        }

        private void Parsestr(string Project, string Fact)
        {
            if (double.TryParse(Project, out double d)) this.Project = d;
            if (double.TryParse(Fact, out d)) this.Fact = d;
        }

        public RowPiket(string Piket, double Project, double Fact)
        {
            this.Piket = Piket;
            this.Project = Project;
            this.Fact = Fact;
            Delta = this.Project - this.Fact;
        }

    }
}

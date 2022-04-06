using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Globalization;


namespace Zadanie_3
{
    public partial class Form1 : Form
    {

        internal static Form2 form2;
        internal static Form1 form1;


        public static List<string> lista = new List<string>();

        public Form1()
        {
            new JObject();
            InitializeComponent();
            form1 = this;
            timer1.Interval = 30000;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            form2 = new Form2(this);
            form2.Show();
        }

        public void add_waluta (string waluta)
        {
            try
            {
                string url = "http://api.nbp.pl/api/exchangerates/rates/a/" + waluta + "/?format=json";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var status = response.StatusCode;
                string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
                dynamic date = JObject.Parse(content);

                bool jest = false;

                foreach (string element in lista)
                {
                    if(element == waluta)
                    {
                        jest = true;
                        MessageBox.Show("Waluta znajduje się już na liście");
                    }
                }

                if (jest == false)
                {
                    dodaj(waluta, (string)date.rates[0].effectiveDate, (string)date.rates[0].mid);
                }

            }
            catch(Exception zmienna)
            {

                MessageBox.Show(zmienna.Message);

            }


        }

        private void dodaj (string curr, string data, string kurs)
        {
            lista.Add(curr);
            string name = "Currency/" + curr + ".txt";
            DateTime date1 = DateTime.Now;
            using (StreamWriter save = new StreamWriter(name, true))
            {
                save.WriteLine(curr + " | " + data + " | " + kurs + " | " + date1);
            }
            
        }

         public void show_lista()
        {
            textBox1.Text = "";
            foreach (string element in lista)
            {
                textBox1.Text += element + ", ";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for(int i=0; i<lista.Count(); i++)
            {
                string url = "http://api.nbp.pl/api/exchangerates/rates/a/" + lista[i] + "/?format=json";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var status = response.StatusCode;
                string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
                dynamic date = JObject.Parse(content);

                string name = "Currency/" + lista[i] + ".txt";
                string curr = lista[i];
                string data = (string)date.rates[0].effectiveDate;
                string kurs = (string)date.rates[0].mid;

                DateTime date1 = DateTime.Now;
                using (StreamWriter save = new StreamWriter(name, true))
                {
                    save.WriteLine(curr + " | " + data + " | " + kurs + " | " + date1);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string path = textBox2.Text;
                textBox3.Text = read(path).ToString();
            }
            catch
            {
                MessageBox.Show("Błędna waluta");
            }

            
        }

        private double read (string path)
        {
            double avr = 0;
            int amount = 0;

            using (StreamReader sr = new StreamReader("Currency/" + path + ".txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!(line == ""))
                    {
                        string[] red = line.Split('|');
                        double result = double.Parse(red[2], CultureInfo.InvariantCulture);
                        avr += result;
                        amount++;
                    }
                }
            }
            return (avr / amount);
        }

    }


}
 

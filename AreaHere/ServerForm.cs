using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AreaHere
{
    public partial class ServerForm : Form
    {
        public string addres;
        public int port = 0;
        public bool isOK = false;

        public ServerForm(string lastAddres, int lastport)
        {
            InitializeComponent();
            textBox1.Text = addres = lastAddres;
            if (lastport != 0)
                textBox2.Text = (port = lastport).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool ba = checkAddres(textBox1.Text);
            bool bp = checkPort(textBox2.Text);
            if (!ba && !bp)
                MessageBox.Show("Порт и адресс указаны не верно");
            else if (!ba)
                MessageBox.Show("Адресс указан не верно");
            else if (!bp)
                MessageBox.Show("Порт указан не верно");
            else
            {
                addres = textBox1.Text;
                port = Convert.ToInt32(textBox2.Text);
                isOK = true;
                Close();
            }
        }

        private bool checkAddres(string addr)
        {
            var t = addr.Split('.');
            if (t.Length != 4) return false;
            for(int i = 0; i < 4; i++)
            {
                if (t[i] == "") return false;
                int num;
                if (!int.TryParse(t[i], out num)) return false;
                if (num < 0 || num > 255) return false;
            }
            return true;
        }
        private bool checkPort(string port)
        {
            if (port == "") return false;
            int num;
            if (!int.TryParse(port, out num)) return false;
            if (num < 1) return false;
            return true;
        }
    }
}

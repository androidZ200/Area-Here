using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AreaHere
{
    public partial class ConnectForm : Form
    {
        public string addr;
        public string name;
        public int port = 0;
        public bool isOK = false;

        public ConnectForm(string lastAddres, string lastName, int Lastport)
        {
            InitializeComponent();
            textBox1.Text = name = lastName;
            textBox2.Text = addr = lastAddres;
            if (Lastport != 0) textBox3.Text = Lastport.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool ba = checkAddres(textBox2.Text);
            bool bp = checkPort(textBox3.Text);
            if (!ba && !bp)
                MessageBox.Show("Порт и адресс указаны не верно");
            else if (!ba)
                MessageBox.Show("Адресс указан не верно");
            else if (!bp)
                MessageBox.Show("Порт указан не верно");
            else
            {
                addr = textBox2.Text;
                port = Convert.ToInt32(textBox3.Text);
                name = textBox1.Text;
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(addr), port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    socket.Connect(ipPoint);
                } catch
                {
                    MessageBox.Show("Не удалось установить соединение");
                    return;
                }
                socket.Send(BitConverter.GetBytes(Encoding.Unicode.GetBytes(name).Length));
                socket.Send(Encoding.Unicode.GetBytes(name));
                GameFormOnline form = new GameFormOnline(socket);
                form.Show();
                isOK = true;
                Close();
            }
        }

        private bool checkAddres(string addr)
        {
            var t = addr.Split('.');
            if (t.Length != 4) return false;
            for (int i = 0; i < 4; i++)
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AreaHere
{
    public partial class WaitPlayersForm : Form
    {
        public Socket server;
        public Socket client = null;

        public WaitPlayersForm(Socket serv)
        {
            InitializeComponent();
            server = serv;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void WaitPlayersForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.Close();
        }
        private void WaitPlayersForm_Shown(object sender, EventArgs e)
        {
            Thread thr = new Thread(WaitPlayer);
            thr.Start();
        }

        private void WaitPlayer()
        {
            try { client = server.Accept(); }
            catch { return; }
            Invoke((Action)(() => Close()));
        }
    }
}

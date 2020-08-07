using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AreaHere
{
    public partial class MainForm : Form
    {
        string lastName = "onlin";
        string lastAddres = "127.0.0.1";
        int lastPort = 10000;

        public MainForm()
        {
            InitializeComponent();
            ComboBox[] lb = { listBox1, listBox2, listBox3, listBox4 };
            for (int i = 0; i < lb.Length; i++)
                lb[i].DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        }

        private IPlayer[] GetPlayers()
        {
            bool isOnline = false;
            int count = 0;
            ComboBox[] lb = { listBox1, listBox2, listBox3, listBox4 };
            TextBox[] tb = { textBox1, textBox2, textBox3, textBox4 };
            Color[] colorPlayer = { Color.Red, Color.Blue, Color.Orange, Color.Green };
            for (int i = 0; i < 4; i++)
            {
                if (lb[i].SelectedIndex != 0) count++;
                if (lb[i].SelectedIndex == 3) isOnline = true;
            }
            if (count < 2) throw new Exception("Игроков не может быть меньше двух");
            IPlayer[] players = new IPlayer[count];
            Socket server = null;
            if (isOnline)
            {
                ServerForm form = new ServerForm(lastAddres, lastPort);
                form.ShowDialog();
                if (!form.isOK) throw new Exception("Отмена создания сервера");
                string addr = lastAddres = form.addres;
                int port = lastPort = form.port;
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(addr), port);
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try { server.Bind(ipPoint); }
                catch { throw new Exception("Не удалось создать сервер"); }
                server.Listen(1);
            }
            int j = 0;
            for (int i = 0; i < lb.Length; i++, j++)
                switch (lb[i].SelectedIndex)
                {
                    case 0:
                        j--;
                        break;
                    case 1:
                        players[j] = new Player(tb[i].Text, colorPlayer[i]);
                        break;
                    case 2:
                        players[j] = new Bot(tb[i].Text, colorPlayer[i]);
                        break;
                    case 3:
                        WaitPlayersForm waitForm = new WaitPlayersForm(server);
                        waitForm.ShowDialog();
                        Socket soc = waitForm.client;
                        if (soc == null) throw new Exception("Отмена ожидания игроков");
                        players[j] = new OnlinePlayer(soc, players, trackBarX.Value, trackBarY.Value, colorPlayer[i]);
                        break;
                }
            if (isOnline)
            {
                server.Close();
            }
            return players;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            try
            {
                Game game;
                switch (comboBoxMode.SelectedIndex)
                {
                    case 0:
                        game = new Game(trackBarX.Value, trackBarY.Value, GetPlayers(), Game.Mode.Normal);
                        break;
                    case 1:
                        game = new Game(trackBarX.Value, trackBarY.Value, GetPlayers(), Game.Mode.Wall);
                        break;
                    default:
                        game = new Game(trackBarX.Value, trackBarY.Value, GetPlayers());
                        break;
                }
                GameForm form = new GameForm(game);
                form.Show();
            }
            catch (Exception mes) { MessageBox.Show(mes.Message); }
        }
        private void trackBarX_Scroll(object sender, EventArgs e)
        {
            labelX.Text = trackBarX.Value.ToString();
        }
        private void trackBarY_Scroll(object sender, EventArgs e)
        {
            labelY.Text = trackBarY.Value.ToString();
        }
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            ConnectForm form = new ConnectForm(lastAddres, lastName, lastPort);
            form.ShowDialog();
            if (form.isOK)
            {
                lastAddres = form.addr;
                lastName = form.name;
                lastPort = form.port;
            }
        }
    }
}

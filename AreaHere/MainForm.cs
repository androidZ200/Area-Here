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
    public partial class MainForm : Form
    {
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
            int count = 0;
            ComboBox[] lb = { listBox1, listBox2, listBox3, listBox4 };
            TextBox[] tb = { textBox1, textBox2, textBox3, textBox4 };
            Color[] colorPlayer = { Color.Red, Color.Blue, Color.Orange, Color.Green };
            for (int i = 0; i < 4; i++) if (lb[i].SelectedIndex != 0) count++;
            if (count < 2) throw new Exception();
            IPlayer[] players = new IPlayer[count];
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
                }
            return players;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            try
            {
                Game game;
                switch(comboBoxMode.SelectedIndex)
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
            catch { MessageBox.Show("Игроков не может быть меньше двух"); }
        }
        private void trackBarX_Scroll(object sender, EventArgs e)
        {
            labelX.Text = trackBarX.Value.ToString();
        }
        private void trackBarY_Scroll(object sender, EventArgs e)
        {
            labelY.Text = trackBarY.Value.ToString();
        }
    }
}

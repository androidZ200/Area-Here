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
        }

        private IPlayer[] GetPlayers()
        {
            int count = 0;
            ComboBox[] lb = { listBox1, listBox2, listBox3, listBox4 };
            TextBox[] tb = { textBox1, textBox2, textBox3, textBox4 };
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
                        players[j] = new Player(tb[i].Text);
                        break;
                    case 2:
                        players[j] = new Bot(tb[i].Text);
                        break;
                }
            return players;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            try
            {
                Game game = new Game(trackBarX.Value, trackBarY.Value, GetPlayers());
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

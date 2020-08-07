using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;

namespace AreaHere
{
    class OnlinePlayer : IPlayer
    {
        Socket socket;
        IPlayer[] players;

        public OnlinePlayer(Socket soc, IPlayer[] players, int w, int h, Color c)
        {
            socket = soc;
            myColor = c;
            this.players = players;
            byte[] t = new byte[4];
            WaitData(4); soc.Receive(t);
            int size = BitConverter.ToInt32(t, 0);
            byte[] byteName = new byte[size];
            WaitData(size); soc.Receive(byteName);
            Name = Encoding.Unicode.GetString(byteName);
            socket.Send(BitConverter.GetBytes(w));
            socket.Send(BitConverter.GetBytes(h));
        }

        public override Rectangle GetMove(Field field, int a, int b)
        {
            CheckConnect();
            if (!inGame) return null;
            socket.Send(BitConverter.GetBytes(2));
            socket.Send(BitConverter.GetBytes(a));
            socket.Send(BitConverter.GetBytes(b));

            byte[] t = new byte[4];
            WaitData(4); 
            if (!inGame) return null;
            socket.Receive(t);

            int l = BitConverter.ToInt32(t, 0);
            WaitData(4); socket.Receive(t);
            int u = BitConverter.ToInt32(t, 0);
            WaitData(4); socket.Receive(t);
            int r = BitConverter.ToInt32(t, 0);
            WaitData(4); socket.Receive(t);
            int d = BitConverter.ToInt32(t, 0);
            Rectangle rec = new Rectangle(l, u, r, d);
            return rec;
        }
        public override void UpdateField(Field newField)
        {
            CheckConnect();
            if (!inGame) return;
            socket.Send(BitConverter.GetBytes(1));
            int[] arr = toData(newField);
            for (int i = 0; i < arr.Length; i++)
                socket.Send(BitConverter.GetBytes(arr[i]));
        }
        public override void EndGame(IPlayer winer)
        {
            CheckConnect();
            if (!inGame) return;
            socket.Send(BitConverter.GetBytes(3));
            socket.Send(BitConverter.GetBytes(Encoding.Unicode.GetBytes(winer.Name).Length));
            socket.Send(Encoding.Unicode.GetBytes(winer.Name));
            socket.Close();
        }
        public override void UpdateParametrs(int a, int b)
        {
            CheckConnect();
            if (!inGame) return;
            socket.Send(BitConverter.GetBytes(5));
            socket.Send(BitConverter.GetBytes(a));
            socket.Send(BitConverter.GetBytes(b));
        }
        public override void UpdatePlayerMove(IPlayer player)
        {
            CheckConnect();
            if (!inGame) return;
            socket.Send(BitConverter.GetBytes(4));
            socket.Send(BitConverter.GetBytes(Encoding.Unicode.GetBytes(player.Name).Length));
            socket.Send(Encoding.Unicode.GetBytes(player.Name));
        }
        public void WaitGenerate()
        {
            CheckConnect();
            if (!inGame) return;
            socket.Send(BitConverter.GetBytes(6));
            byte[] t = new byte[4];
            WaitData(4);
            if (!inGame) return;
            socket.Receive(t);
        }

        private int[] toData(Field f)
        {
            int[] data = new int[f.Width * f.Height];
            for (int j = 0; j < f.Height; j++)
                for (int i = 0; i < f.Width; i++)
                {
                    data[f.Width * j + i] = Color.Transparent.ToArgb();
                    for (int k = 0; k < players.Length; k++)
                        if (f.field[i, j] != null)
                            data[f.Width * j + i] = f.field[i, j].myColor.ToArgb();
                }
            return data;
        }
        private void CheckConnect()
        {
            bool part1 = socket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (socket.Available == 0);
            if (part1 && part2)
                if (inGame)
                    PlayerDisconnect();

        }
        private void PlayerDisconnect()
        {
            MessageBox.Show(Name + " отсоединился");
            inGame = false;
        }
        private void WaitData(int length)
        {
            while (socket.Available < length && inGame)
            {
                Thread.Sleep(10);
                CheckConnect();
            }
        }
    }
}

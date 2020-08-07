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
    public partial class GameFormOnline : Form
    {
        Socket soc;
        bool isMyMove = false;
        bool click = false;
        Thread listenSocet;
        int width;
        int height;
        int[] data;
        Point beg, end;
        int a, b;

        private Bitmap DrawField()
        {
            if (pictureBox1.Width > 0)
            {
                Bitmap bmp = new Bitmap(width * 50, height * 50);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawRectangle(new Pen(Color.Gray, 1), 0, 0, bmp.Width - 1, bmp.Height - 1);
                for (int j = 0; j < height; j++)
                    for (int i = 0; i < width; i++)
                        g.FillRectangle(new SolidBrush(Color.FromArgb(data[width * j + i])), i * 50, j * 50, 49, 49);
                bmp = new Bitmap(bmp, GetSizeImage());
                Invoke((Action)(()=> { pictureBox1.Image = bmp; }));
                return bmp;
            }
            return null;
        }
        private Bitmap DrawField(Rectangle move)
        {
            if (pictureBox1.Width > 0)
            {
                Bitmap b = DrawField();
                Graphics g1 = Graphics.FromImage(b);
                Bitmap bmp = new Bitmap(width * 50, height * 50);
                Graphics g = Graphics.FromImage(bmp);
                for (int j = move.Up; j <= move.Down; j++)
                    for (int i = move.Left; i <= move.Right; i++)
                        g.DrawImage(GetBlock(move, i, j), i * 50, j * 50);
                bmp = new Bitmap(bmp, GetSizeImage());
                g1.DrawImage(bmp, 0, 0);
                Invoke((Action)(() => { pictureBox1.Image = b; }));
                return b;
            }
            return null;
        }
        private Size GetSizeImage()
        {
            double w = pictureBox1.Width * 1.0 / width;
            double h = pictureBox1.Height * 1.0 / height;
            if (w > h)
                return new Size(width * pictureBox1.Height / height, pictureBox1.Height);
            else
                return new Size(pictureBox1.Width, height * pictureBox1.Width / width);
        }
        private Bitmap GetBlock(Rectangle r, int x, int y)
        {
            Bitmap bmp = new Bitmap(49, 49);
            Graphics g = Graphics.FromImage(bmp);
            if (x == r.Left) g.FillRectangle(new SolidBrush(Color.Gray), 0, 0, 10, 49);
            if (x == r.Right) g.FillRectangle(new SolidBrush(Color.Gray), 39, 0, 10, 49);
            if (y == r.Up) g.FillRectangle(new SolidBrush(Color.Gray), 0, 0, 49, 10);
            if (y == r.Down) g.FillRectangle(new SolidBrush(Color.Gray), 0, 39, 49, 10);
            return bmp;
        }
        private Point CoordinateMouse(Point location)
        {
            Size size = GetSizeImage();
            double step = size.Width * 1.0 / width;
            Point coordinate = new Point(-1, -1);
            location.X -= (pictureBox1.Width - size.Width) / 2;
            location.Y -= (pictureBox1.Height - size.Height) / 2;
            for (int i = 0; i < width; i++)
                if (i * step <= location.X && (i + 1) * step > location.X) coordinate.X = i;
            for (int i = 0; i < height; i++)
                if (i * step <= location.Y && (i + 1) * step > location.Y) coordinate.Y = i;
            return coordinate;
        }
        private void WaitData(int lengthData)
        {
            while (soc.Available < lengthData)
            {
                Thread.Sleep(10);
                if (!CheckConnect())
                {
                    MessageBox.Show("Соединение потеряно");
                    Invoke((Action)(() => { Close(); }));
                }
                
            }
        }
        private bool CheckConnect()
        {
            bool part1 = soc.Poll(1000, SelectMode.SelectRead);
            bool part2 = (soc.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }

        private void Loop()
        {
            byte[] t = new byte[4];
            while (true)
            {
                WaitData(4); soc.Receive(t);
                int num = BitConverter.ToInt32(t, 0);

                switch (num)
                {
                    case 1:
                        WaitData(data.Length);
                        for (int i = 0; i < data.Length; i++)
                        {
                            soc.Receive(t);
                            data[i] = BitConverter.ToInt32(t, 0);
                        }
                        DrawField();
                        isMyMove = false;
                        Invoke((Action)(() => {
                            labelA.Text = "a";
                            labelB.Text = "b";
                        }));
                        break;
                    case 2:
                        WaitData(4); soc.Receive(t);
                        a = BitConverter.ToInt32(t, 0);
                        WaitData(4); soc.Receive(t);
                        b = BitConverter.ToInt32(t, 0);
                        break;
                    case 3:
                        WaitData(4); soc.Receive(t);
                        int size = BitConverter.ToInt32(t, 0);
                        byte[] byteName = new byte[size];
                        WaitData(size); soc.Receive(byteName);
                        string winName = Encoding.Unicode.GetString(byteName);
                        MessageBox.Show(winName + " Win!");
                        soc.Close();
                        goto end_point;
                    case 4:
                        WaitData(4); soc.Receive(t);
                        size = BitConverter.ToInt32(t, 0);
                        byteName = new byte[size];
                        WaitData(size); soc.Receive(byteName);
                        string playerName = Encoding.Unicode.GetString(byteName);
                        Invoke((Action)(() => { labelName.Text = playerName; }));
                        break;
                    case 5:
                        WaitData(4); soc.Receive(t);
                        Invoke((Action)(() => { labelA.Text = BitConverter.ToInt32(t, 0).ToString(); }));
                        WaitData(4); soc.Receive(t);
                        Invoke((Action)(() => { labelB.Text = BitConverter.ToInt32(t, 0).ToString(); }));
                        break;
                    case 6:
                        if (!isMyMove) Invoke((Action)(() => { buttonGenerate.Enabled = true; }));
                        break;
                }
            }
            end_point:
            Invoke((Action)(() => { Close(); }));
        }

        public GameFormOnline(Socket socket)
        {
            InitializeComponent();
            soc = socket;

            byte[] t = new byte[4];
            WaitData(4); soc.Receive(t);
            width = BitConverter.ToInt32(t, 0);
            WaitData(4); soc.Receive(t);
            height = BitConverter.ToInt32(t, 0);
            data = new int[width * height];
            WaitData(4); soc.Receive(t);

            listenSocet = new Thread(Loop);
            listenSocet.Start();
        }
        private void GameFormOnline_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (listenSocet != null) listenSocet.Abort();
            soc.Close();
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (isMyMove) click = true;
            end = beg = CoordinateMouse(e.Location);
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            click = false;
            if (isMyMove)
            {
                soc.Send(BitConverter.GetBytes(Math.Min(beg.X, end.X)));
                soc.Send(BitConverter.GetBytes(Math.Min(beg.Y, end.Y)));
                soc.Send(BitConverter.GetBytes(Math.Max(beg.X, end.X)));
                soc.Send(BitConverter.GetBytes(Math.Max(beg.Y, end.Y)));
                DrawField();
            }
        }
        private void GameFormOnline_SizeChanged(object sender, EventArgs e)
        {
            DrawField();
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (click)
            {
                Point prevEnd = end;
                end = CoordinateMouse(e.Location);
                int l = Math.Min(beg.X, end.X);
                int u = Math.Min(beg.Y, end.Y);
                int r = Math.Max(beg.X, end.X);
                int d = Math.Max(beg.Y, end.Y);

                if (end != prevEnd)
                    DrawField(new Rectangle(l, u, r, d));
            }
        }
        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            labelA.Text = a.ToString();
            labelB.Text = b.ToString();
            isMyMove = true;
            buttonGenerate.Enabled = false;
            soc.Send(BitConverter.GetBytes(0));
        }
    }
}

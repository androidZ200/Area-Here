using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AreaHere
{
    public partial class GameForm : Form
    {
        Game game;
        Thread gameThread;
        object lockNewMove = new object();
        object lockGetMove = new object();
        bool waitMove = false;
        bool click = false;
        Rectangle moveRectangle = new Rectangle(0, 0, 0, 0);
        Point beginningRectangle = new Point(0, 0);

        private void Win(IPlayer player)
        {
            Invoke(new Action(() => { MessageBox.Show(player.Name + " Win!"); Close(); }));
        }
        private void NewMove(IPlayer player, int a, int b)
        {
            if (player is Player)
            {
                Invoke(new Action(() =>
                {
                    StartButton.Enabled = true;
                    PlayerNameLabel.Text = player.Name;
                    labelA.Text = "a";
                    labelB.Text = "b";
                }));
                Monitor.Enter(lockNewMove);
                Monitor.Exit(lockNewMove);
            }
            Invoke(new Action(() =>
            {
                labelA.Text = a.ToString();
                labelB.Text = b.ToString();
            }));
        }
        private Rectangle GetMove()
        {
            waitMove = true;
            Monitor.Enter(lockGetMove);
            Monitor.Exit(lockGetMove);
            waitMove = false;
            return moveRectangle;
        }
        private Bitmap DrawField()
        {
            Color[] colorPlayer = { Color.Red, Color.Blue, Color.Orange, Color.Green };
            if (pictureBox1.Width > 0)
            {
                Bitmap bmp = new Bitmap(game.field.Width * 50, game.field.Height * 50);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawRectangle(new Pen(Color.Gray, 1), 0, 0, bmp.Width - 1, bmp.Height - 1);
                for (int j = 0; j < game.field.Height; j++)
                    for (int i = 0; i < game.field.Width; i++)
                        for (int l = 0; l < game.players.Length; l++)
                            if (game.field.field[i, j] == game.players[l])
                                g.FillRectangle(new SolidBrush(colorPlayer[l]), i * 50, j * 50, 49, 49);
                bmp = new Bitmap(bmp, GetSizeImage());
                pictureBox1.Image = bmp;
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
                Bitmap bmp = new Bitmap(game.field.Width * 50, game.field.Height * 50);
                Graphics g = Graphics.FromImage(bmp);
                for (int j = move.Up; j <= move.Down; j++)
                    for (int i = move.Left; i <= move.Right; i++)
                        g.DrawImage(GetBlock(move, i, j), i * 50, j * 50);
                bmp = new Bitmap(bmp, GetSizeImage());
                g1.DrawImage(bmp, 0, 0);
                pictureBox1.Image = b;
                return b;
            }
            return null;
        }
        private Size GetSizeImage()
        {
            int width = game.field.Width;
            int height = game.field.Height;
            double w = pictureBox1.Width * 1.0 / width;
            double h = pictureBox1.Height * 1.0 / height;
            if (w > h)
                return new Size(width * pictureBox1.Height / height, pictureBox1.Height);
            else
                return new Size(pictureBox1.Width, height * pictureBox1.Width / width);
        }
        private Point CoordinateMouse(Point location)
        {
            int width = game.field.Width;
            int height = game.field.Height;
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

        public GameForm(Game game)
        {
            InitializeComponent();
            this.game = game;
            game.Win += Win;
            game.NewMove += NewMove;
            game.DoneMove += new Action( () => { DrawField(); });
            for (int i = 0; i < game.players.Length; i++)
                if (game.players[i] is Player) ((Player)game.players[i]).GetMoveRectangle += GetMove;
            gameThread = new Thread(game.Start);
            Monitor.Enter(lockNewMove);
            Monitor.Enter(lockGetMove);
            DrawField();
        }
        private void StartButton_Click(object sender, EventArgs e)
        {
            Monitor.Exit(lockNewMove);
            StartButton.Enabled = false;
            Monitor.Enter(lockNewMove);
        }
        private void GameForm_Load(object sender, EventArgs e)
        {
            gameThread.Start();
        }
        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            gameThread.Abort();
        }
        private void GameForm_SizeChanged(object sender, EventArgs e)
        {
            DrawField();
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (waitMove)
            {
                click = true;
                beginningRectangle = CoordinateMouse(e.Location);
                if (beginningRectangle.X == -1 || beginningRectangle.Y == -1) click = false;
                else
                {
                    moveRectangle.Down = moveRectangle.Up = beginningRectangle.Y;
                    moveRectangle.Left = moveRectangle.Right = beginningRectangle.X;
                    DrawField(moveRectangle);
                }
            }
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (click && waitMove)
            {
                Point coordinate = CoordinateMouse(e.Location);
                if (coordinate.X == -1 || coordinate.Y == -1)
                {
                    click = false;
                    DrawField();
                }
                else
                {
                    Rectangle t = moveRectangle.Copy();
                    moveRectangle.Up = beginningRectangle.Y;
                    moveRectangle.Down = coordinate.Y;
                    moveRectangle.Left = beginningRectangle.X;
                    moveRectangle.Right = coordinate.X;
                    if (t != moveRectangle)
                        DrawField(moveRectangle);
                }
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (waitMove && click)
            {
                click = false;
                Monitor.Exit(lockGetMove);
                Monitor.Enter(lockGetMove);
                DrawField();
            }
        }
    }
}

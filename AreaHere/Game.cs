using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AreaHere
{
    public class Game
    {
        public enum Mode { Normal, Wall }
        public Field field { get; private set; }
        public IPlayer[] players { get; private set; }
        public event Action<IPlayer, int, int> NewMove;
        public event Action<IPlayer> Win;
        public event Action DoneMove;
        private Random rand = new Random();

        public Game(int Width, int Height, IPlayer[] players)
        {
            if (players.Length < 2 || players.Length > 4) throw new Exception();
            field = new Field(Width, Height);
            this.players = players;
        }
        public Game(int Width, int Height, IPlayer[] players, Mode mode)
        {
            if (players.Length < 2 || players.Length > 4) throw new Exception();
            switch (mode)
            {
                case Mode.Normal:
                    field = new Field(Width, Height);
                    break;
                case Mode.Wall:
                    field = new Field2(Width, Height);
                    break;
            }
            this.players = players;
        }
        public void Start()
        {
            int counter = 0;
            bool[] firstMove = new bool[players.Length];
            for (int i = 0; i < players.Length; i++) firstMove[i] = false;
            while (field.CountVoidCell() > 0)
            {
                foreach (var p in players) p.UpdateField(field);
                HashSet<Rectangle> PossibleMoves;
                int a = 0, b = 0;
                do
                {
                    if (a == 1 && b == 1) counter = (counter + 1) % players.Length;
                    a = rand.Next(1, 7);
                    b = rand.Next(1, 7);
                    if (!firstMove[counter])
                    {
                        PossibleMoves = GetPossibleFirstMoves(counter, a, b);
                        firstMove[counter] = true;
                    }
                    else PossibleMoves = field.GetStartPoint(players[counter], a, b);
                } while (PossibleMoves.Count == 0);
                foreach (var x in players) x.UpdatePlayerMove(players[counter]);
                NewMove(players[counter], a, b);
                foreach (var x in players) x.UpdateParametrs(a, b);
                while (true)
                {
                    Rectangle rectangle = players[counter].GetMove(field, a, b);
                    if ((object)rectangle == null) break;
                    bool isRight = false;
                    foreach (var r in PossibleMoves)
                        if (rectangle == r) isRight = true;
                    if (isRight)
                    {
                        field.SetRectangle(rectangle, players[counter]);
                        break;
                    }
                }
                counter = (counter + 1) % players.Length;
                if (firstMove[players.Length - 1]) field.Update();
                DoneMove();
            }
            foreach (var p in players) p.UpdateField(field);
            int[] score = new int[players.Length];
            for (int i = 0; i < score.Length; i++)
                score[i] = field.CountPlayr(players[i]);
            int maxIndex = 0;
            int max = score[0];
            for (int i = 1; i < score.Length; i++)
                if (score[i] > max)
                {
                    max = score[i];
                    maxIndex = i;
                }
            for (int i = 0; i < players.Length; i++)
                players[i].EndGame(players[maxIndex]);
            Win(players[maxIndex]);
        }

        private HashSet<Rectangle> GetPossibleFirstMoves(int index, int a, int b)
        {
            HashSet<Rectangle> set = new HashSet<Rectangle>();
            if (index == 0)
            {
                set.Add(new Rectangle(0, 0, a - 1, b - 1));
                set.Add(new Rectangle(0, 0, b - 1, a - 1));
            }
            else if (index == 2 || players.Length == 2)
            {
                set.Add(new Rectangle(field.Width - a, field.Height - b, field.Width - 1, field.Height - 1));
                set.Add(new Rectangle(field.Width - b, field.Height - a, field.Width - 1, field.Height - 1));
            }
            else if (index == 1)
            {
                set.Add(new Rectangle(field.Width - a, b - 1, field.Width - 1, 0));
                set.Add(new Rectangle(field.Width - b, a - 1, field.Width - 1, 0));
            }
            else
            {
                set.Add(new Rectangle(0, field.Height - b, a - 1, field.Height - 1));
                set.Add(new Rectangle(0, field.Height - a, b - 1, field.Height - 1));
            }
            return set;
        }
    }
}

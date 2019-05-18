using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace AreaHere
{
    public class Field
    {
        protected enum Fill { Void, Player, Time, None }
        protected enum Direction { Up, Right, Down, Left }
        public IPlayer[,] field { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        protected bool FieldChanged = false;

        public Field(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            field = new IPlayer[Width, Height];
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    field[i, j] = null;
        }

        public virtual int CountVoidCell()
        {
            int count = 0;
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    if (field[i, j] == null) count++;
            return count;

        }
        public virtual bool IsVoidRectangle(Rectangle r)
        {
            if (r.Left < 0 || r.Up < 0 || r.Right >= Width || r.Down >= Height) return false;
            for (int i = r.Left; i <= r.Right; i++)
                for (int j = r.Up; j <= r.Down; j++)
                    if (field[i, j] != null) return false;
            return true;
        }
        public virtual void SetRectangle(Rectangle r, IPlayer p)
        {
            FieldChanged = true;
            for (int i = r.Left; i <= r.Right; i++)
                for (int j = r.Up; j <= r.Down; j++)
                    field[i, j] = p;
        }
        public virtual void Update()
        {
            if (!FieldChanged) return;
            FieldChanged = false;
            Fill[,] points = new Fill[Width, Height];
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                    if (field[i, j] == null) points[i, j] = Fill.Void;
                    else points[i, j] = Fill.Player;
            HashSet<IPlayer> neighboard = new HashSet<IPlayer>();
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                    if (points[i, j] == Fill.Void)
                    {
                        Point coordinate = new Point(i, j);
                        Point startPoint = new Point(i, j);
                        Direction angle = Direction.Up;
                        int countRotate = 0;
                        while (true)
                        {
                            points[coordinate.X, coordinate.Y] = Fill.Time;
                            angle = Rotate(angle, false);
                            --countRotate;
                            Point n = Neighboard(coordinate, angle);
                            while (!InsideField(n.X, n.Y))
                            {
                                angle = Rotate(angle, true);
                                ++countRotate;
                                n = Neighboard(coordinate, angle);
                            }
                            if (points[n.X, n.Y] == Fill.Void || points[n.X, n.Y] == Fill.Time)
                                coordinate = n;
                            else if (points[n.X, n.Y] == Fill.Player)
                            {
                                neighboard.Add(field[n.X, n.Y]);
                                angle = Rotate(Rotate(angle, true), true);
                                countRotate += 2;
                                if (coordinate == startPoint && countRotate > 3)
                                {
                                    if (neighboard.Count == 1)
                                    {
                                        FillField(neighboard.ElementAt(0), points);
                                        FillTime(points, Fill.Player);
                                    }
                                    else if (neighboard.Count > 1)
                                        FillTime(points, Fill.None);
                                    break;
                                }
                            }
                            else if (points[n.X, n.Y] == Fill.None)
                            {
                                FillTime(points, Fill.None);
                                break;
                            }
                        }
                        neighboard.Clear();
                    }
        }
        public virtual int CountPlayr(IPlayer p)
        {
            int count = 0;
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    if (field[i, j] == p) count++;
            return count;
        }
        public virtual HashSet<Rectangle> GetStartPoint(IPlayer p, int a, int b)
        {
            a--; b--;
            HashSet<Point> set = GetStartPoint(p);
            HashSet<Rectangle> rec = new HashSet<Rectangle>();
            foreach (var s in set)
            {
                if (s.X - a >= 0 && s.Y - b >= 0 && IsVoidRectangle(new Rectangle(s.X - a, s.Y - b, s.X, s.Y)))
                    rec.Add(new Rectangle(s.X - a, s.Y - b, s.X, s.Y));
                if (s.X - b >= 0 && s.Y - a >= 0 && IsVoidRectangle(new Rectangle(s.X - b, s.Y - a, s.X, s.Y)))
                    rec.Add(new Rectangle(s.X - b, s.Y - a, s.X, s.Y));

                if (s.X + a < field.GetLength(0) && s.Y - b >= 0 && IsVoidRectangle(new Rectangle(s.X + a, s.Y - b, s.X, s.Y)))
                    rec.Add(new Rectangle(s.X + a, s.Y - b, s.X, s.Y));
                if (s.X + b < field.GetLength(0) && s.Y - a >= 0 && IsVoidRectangle(new Rectangle(s.X + b, s.Y - a, s.X, s.Y)))
                    rec.Add(new Rectangle(s.X + b, s.Y - a, s.X, s.Y));

                if (s.X - a >= 0 && s.Y + b < field.GetLength(1) && IsVoidRectangle(new Rectangle(s.X - a, s.Y + b, s.X, s.Y)))
                    rec.Add(new Rectangle(s.X - a, s.Y + b, s.X, s.Y));
                if (s.X - b >= 0 && s.Y + a < field.GetLength(1) && IsVoidRectangle(new Rectangle(s.X - b, s.Y + a, s.X, s.Y)))
                    rec.Add(new Rectangle(s.X - b, s.Y + a, s.X, s.Y));

                if (s.X + a < field.GetLength(0) && s.Y + b < field.GetLength(1) &&
                    IsVoidRectangle(new Rectangle(s.X + a, s.Y + b, s.X, s.Y)))
                    rec.Add(new Rectangle(s.X + a, s.Y + b, s.X, s.Y));
                if (s.X + b < field.GetLength(0) && s.Y + a < field.GetLength(1) &&
                    IsVoidRectangle(new Rectangle(s.X + b, s.Y + a, s.X, s.Y)))
                    rec.Add(new Rectangle(s.X + b, s.Y + a, s.X, s.Y));
            }
            return rec;
        }
        public virtual HashSet<IPlayer> GetPlayers()
        {
            HashSet<IPlayer> p = new HashSet<IPlayer>();
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    if (field[i, j] != null) p.Add(field[i, j]);
            return p;
        }

        protected HashSet<Point> GetStartPoint(IPlayer p)
        {
            HashSet<Point> set = new HashSet<Point>();
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    if (field[i, j] == null)
                    {
                        bool isPlayer = false;
                        bool isSpace = false;
                        Func<int, int, bool> VoidSpace = (x, y) => (x >= 0 && y >= 0 &&
                        x < Width && y < Height && field[x, y] == null);
                        if ((VoidSpace(i + 1, j) && VoidSpace(i - 1, j)) ||
                            (VoidSpace(i, j + 1) && VoidSpace(i, j - 1))) isSpace = true;
                        if (!isSpace)
                            if ((InsideField(i + 1, j) && field[i + 1, j] == p) ||
                                (InsideField(i - 1, j) && field[i - 1, j] == p) ||
                                (InsideField(i, j + 1) && field[i, j + 1] == p) ||
                                (InsideField(i, j - 1) && field[i, j - 1] == p)) isPlayer = true;
                        if (!isSpace && isPlayer) set.Add(new Point(i, j));
                    }
            return set;
        }
        protected bool InsideField(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < Width && y < Height);
        }
        protected Direction Rotate(Direction d, bool right)
        {
            if (right)
                return (Direction)((int)(d + 1) % 4);
            else
                return (Direction)((int)(d + 3) % 4);
        }
        protected Point Neighboard(Point current, Direction d)
        {
            switch (d)
            {
                case Direction.Up:
                    return new Point(current.X, current.Y - 1);
                case Direction.Right:
                    return new Point(current.X + 1, current.Y);
                case Direction.Down:
                    return new Point(current.X, current.Y + 1);
                case Direction.Left:
                    return new Point(current.X - 1, current.Y);
            }
            return current;
        }
        protected void FillField(IPlayer p, Fill[,] fills)
        {
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                    if (fills[i, j] == Fill.Time) field[i, j] = p;
        }
        protected void FillTime(Fill[,] fills, Fill NEW)
        {
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                    if (fills[i, j] == Fill.Time) fills[i, j] = NEW;
        }
    }

    public class Field2 : Field
    {
        IPlayer wall = new Player("", Color.Gray);

        public Field2(int Width, int Height) : base(Width, Height)
        {
            Random r = new Random();
            for (int i = 0; i < Width * Height / 20; i++)
                field[r.Next(Width), r.Next(Height)] = wall;
        }
        public override void Update()
        {
            if (!FieldChanged) return;
            FieldChanged = false;
            Fill[,] points = new Fill[Width, Height];
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                    if (field[i, j] == null) points[i, j] = Fill.Void;
                    else points[i, j] = Fill.Player;
            HashSet<IPlayer> neighboard = new HashSet<IPlayer>();
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                    if (points[i, j] == Fill.Void)
                    {
                        Point coordinate = new Point(i, j);
                        Point startPoint = new Point(i, j);
                        Direction angle = Direction.Up;
                        int countRotate = 0;
                        while (true)
                        {
                            points[coordinate.X, coordinate.Y] = Fill.Time;
                            angle = Rotate(angle, false);
                            --countRotate;
                            Point n = Neighboard(coordinate, angle);
                            while (!InsideField(n.X, n.Y))
                            {
                                angle = Rotate(angle, true);
                                ++countRotate;
                                n = Neighboard(coordinate, angle);
                            }
                            if (points[n.X, n.Y] == Fill.Void || points[n.X, n.Y] == Fill.Time)
                                coordinate = n;
                            else if (points[n.X, n.Y] == Fill.Player)
                            {
                                neighboard.Add(field[n.X, n.Y]);
                                angle = Rotate(Rotate(angle, true), true);
                                countRotate += 2;
                                if (coordinate == startPoint && countRotate > 3)
                                {
                                    if (neighboard.Count == 1)
                                    {
                                        FillField(neighboard.ElementAt(0), points);
                                        FillTime(points, Fill.Player);
                                    }
                                    else
                                    {
                                        neighboard.Remove(wall);
                                        if (neighboard.Count == 1)
                                        {
                                            FillField(neighboard.ElementAt(0), points);
                                            FillTime(points, Fill.Player);
                                        }
                                        else if (neighboard.Count > 1)
                                            FillTime(points, Fill.None);
                                    }
                                    break;
                                }
                            }
                            else if (points[n.X, n.Y] == Fill.None)
                            {
                                FillTime(points, Fill.None);
                                break;
                            }
                        }
                        neighboard.Clear();
                    }
        }
    }
}
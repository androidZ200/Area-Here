using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AreaHere
{
    public class Rectangle
    {
        private int left = 0;
        private int up = 0;
        private int right = 0;
        private int down = 0;

        public int Left
        {
            get { return left; }
            set
            {
                left = value;
                Distribution();
            }
        }
        public int Up
        {
            get { return up; }
            set
            {
                up = value;
                Distribution();
            }
        }
        public int Right
        {
            get { return right; }
            set
            {
                right = value;
                Distribution();
            }
        }
        public int Down
        {
            get { return down; }
            set
            {
                down = value;
                Distribution();
            }
        }

        public Rectangle(int left, int up, int right, int down)
        {
            this.left = left;
            this.up = up;
            this.right = right;
            this.down = down;
            Distribution();
        }
        public Rectangle() { }

        public bool Inside(Rectangle big)
        {
            return Left >= big.Left && Right <= big.Right && Up >= big.Up && Down <= big.Down;
        }
        public bool Inside(Point point)
        {
            return point.X >= Left && point.X <= Right && point.Y >= Up && point.Y <= Down;
        }
        public bool Outside(Rectangle small)
        {
            return small.Inside(this);
        }
        public Rectangle Copy()
        {
            Rectangle t = new Rectangle(left, up, right, down);
            return t;
        }

        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return left.Right == right.Right && left.Up == right.Up && left.Left == right.Left && left.Down == right.Down;
        }
        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !(left == right);
        }

        private void Distribution()
        {
            if (left > right)
            {
                int t = left; left = right; right = t;
            }
            if (up > down)
            {
                int t = up; up = down; down = t;
            }
        }
    }
}

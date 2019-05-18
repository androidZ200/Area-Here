using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AreaHere
{
    public class Player : IPlayer
    {
        public event Func<Rectangle> GetMoveRectangle;

        public Player(string Name, Color color)
        {
            this.Name = Name;
            myColor = color;
        }
        public override Rectangle GetMove(Field field, int a, int b)
        {
            return GetMoveRectangle();
        }
    }
}

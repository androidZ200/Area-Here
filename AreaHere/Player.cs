using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AreaHere
{
    public class Player : IPlayer
    {
        public string Name { get; private set; }
        public event Func<Rectangle> GetMoveRectangle;

        public Player(string Name)
        {
            this.Name = Name;
        }

        public Rectangle GetMove(Field field, int a, int b)
        {
            return GetMoveRectangle();
        }
    }
}

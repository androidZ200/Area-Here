using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AreaHere
{
    class Bot : IPlayer
    {
        private Random rand = new Random();
        public string Name { get; private set; }

        public Bot(string Name)
        {
            this.Name = Name;
        }

        public Rectangle GetMove(Field field, int a, int b)
        {
            HashSet<Rectangle> rectangles = field.GetStartPoint(this, a, b);
            if(rectangles.Count == 0)
            {
                rectangles.Add(new Rectangle(0, 0, a - 1, b - 1));
                rectangles.Add(new Rectangle(0, 0, b - 1, a - 1));

                rectangles.Add(new Rectangle(field.Width - a, field.Height - b, field.Width - 1, field.Height - 1));
                rectangles.Add(new Rectangle(field.Width - b, field.Height - a, field.Width - 1, field.Height - 1));

                rectangles.Add(new Rectangle(field.Width - a, b - 1, field.Width - 1, 0));
                rectangles.Add(new Rectangle(field.Width - b, a - 1, field.Width - 1, 0));

                rectangles.Add(new Rectangle(0, field.Height - b, a - 1, field.Height - 1));
                rectangles.Add(new Rectangle(0, field.Height - a, b - 1, field.Height - 1));
            }
            return rectangles.ElementAt(rand.Next(rectangles.Count));
        }
    }
}

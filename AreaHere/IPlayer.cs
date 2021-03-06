﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AreaHere
{
    public abstract class IPlayer
    {
        public string Name { get; protected set; }
        public Color myColor { get; protected set; }
        public bool inGame { get; protected set; } = true;

        public abstract Rectangle GetMove(Field field, int a, int b);
        public virtual void UpdateField(Field newField)
        {

        }
        public virtual Image GetImage(int width)
        {
            Image m = new Bitmap(width, width);
            Graphics g = Graphics.FromImage(m);
            g.Clear(myColor);
            return m;
        }
        public virtual void EndGame(IPlayer winer) { }
        public virtual void UpdatePlayerMove(IPlayer player, int a, int b) { }
    }
}

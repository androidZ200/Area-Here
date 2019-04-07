using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AreaHere
{
    public interface IPlayer
    {
        Rectangle GetMove(Field field, int a, int b);
        string Name { get; }
    }
}

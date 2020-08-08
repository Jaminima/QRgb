using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qrgb
{
    public static class Conversions
    {
        public static bool[] ByteToBool(byte B, ushort TrimLeft = 0, ushort TrimRight=0)
        {
            bool[] bits = new bool[8];
            for (ushort i = 0; i < 8; i++) bits[i] = (B & (1 << i)) != 0;
            return bits.Skip(TrimRight).Reverse().Skip(TrimLeft).ToArray();
        }
    }
}

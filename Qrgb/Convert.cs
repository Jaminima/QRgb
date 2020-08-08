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

        public static bool[] ByteToBool(byte[] B, ushort TrimLeft = 0, ushort TrimRight = 0)
        {
            return B.SelectMany(x => ByteToBool(x)).ToArray();
        }

        public static byte BoolToByte(bool[] B)
        {
            byte b = 0;
            for (int i=0;i<8;i++) if (B[i]) b |= (byte)(1 << 7-i);
            return b;
        }

        public static byte[] BoolSetToByte(bool[] B)
        {
            byte[] Data = new byte[B.Length / 8];

            for (int i = 0; i < Data.Length; i++) Data[i] = BoolToByte(B.Skip(i * 8).Take(8).ToArray());

            return Data;
        }
    }
}

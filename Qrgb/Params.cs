using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qrgb
{
    public static class Params
    {
        public static ushort[] RGBmax = { 255,255,255 };

        public static ushort[] RGBGap = { 255,255,255};

        public static ushort[] BitsPerColour = { 2,2,2 };
        public static ushort TotalBits = 6;

        public static void Calculate()
        {
            for (uint i = 0; i < 3; i++)
            {
                RGBGap[i] = (ushort)Convert.ToByte(Params.RGBmax[i] / (ushort)(Math.Pow(2, Params.BitsPerColour[i])));
            }
        }
    }
}

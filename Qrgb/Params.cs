using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qrgb
{
    public static class Params
    {
        //Max Is 8 Bits
        public const ushort PerColourEncodingBits = 4;

        public static ushort[] RGBmax = { 255,255,255 };

        public static ushort[] RGBGap = { 255,255,255};

        //Each colour shouldnt have more than PerColourEncodingBits bits
        public static ushort[] BitsPerColour = { 6,6,4 };
        public const ushort TotalBits = 16;

        public static void Calculate()
        {
            for (uint i = 0; i < 3; i++)
            {
                RGBGap[i] = (ushort)Convert.ToByte(Params.RGBmax[i] / (ushort)(Math.Pow(2, Params.BitsPerColour[i])));
            }
        }
    }
}

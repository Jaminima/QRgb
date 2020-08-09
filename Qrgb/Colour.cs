using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qrgb
{
    public class Colour
    {
        public static Colour Red = new Colour(255, 0, 0), Green = new Colour(0, 255, 0), Blue = new Colour(0, 0, 255), White = new Colour(255, 255, 255);

        public ushort[] Values = { 0, 0, 0 };

        public ushort R { get { return Values[0]; } set { Values[0] = value; } }
        public ushort G { get { return Values[1]; } set { Values[1] = value; } }
        public ushort B { get { return Values[2]; } set { Values[2] = value; } }

        public Colour(ushort r, ushort g, ushort b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public Colour(bool[] Data)
        {
            for (int i = 0, j = 0; j<3; j++)
            {
                int T = 0;
                for (int k = 0; k < Params.BitsPerColour[j]; k++)
                {
                    if (Data[i + k]) T += (int)Math.Pow(2, Params.BitsPerColour[j]-1- k);
                }
                Values[j] = (ushort)Convert.ToByte((T+0.5) * Params.RGBGap[j]);
                i += Params.BitsPerColour[j];
            }
        }

        public bool[] ToDataBits()
        {
            bool[] Bits = new bool[Params.TotalBits];

            ConvertColour(R, 0, ref Bits);
            ConvertColour(G, 1, ref Bits);
            ConvertColour(B, 2, ref Bits);

            return Bits;
        }

        private void ConvertColour(ushort C, ushort i, ref bool[] Bits)
        {
            ushort StartsAt = 0;
            for (uint k = 0; k < i; k++) StartsAt += Params.BitsPerColour[k];

            int Dec = 0;
            while (C > Params.RGBGap[i])
            {
                Dec++;
                C -= Params.RGBGap[i];
            }

            for (int p = Params.BitsPerColour[i] - 1, j=0; p >= 0; p--,j++)
            {
                int Po = (int)Math.Pow(2, p);
                if (Dec >= Po)
                {
                    Bits[StartsAt + j] = true;
                    Dec -= Po;
                }
            }
        }
    }

}

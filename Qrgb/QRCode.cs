using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;

namespace Qrgb
{
    public class QRCode
    {
        public Colour[] Squares;
        public QRCode(string Data)
        {
            Squares = new Colour[Data.Length];

            char C;
            for (int i = 0; i < Squares.Length; i++) {
                C = Data[i];
                bool[] bits = Conversions.ByteToBool((byte)C,1);
                Squares[i] = new Colour(bits);
            }
        }

        public void SaveOut(string Path = "./image.png", int width = 400, int height = 400)
        {
            int Len = (int)Math.Ceiling(Math.Sqrt(Squares.Length));

            Bitmap image = new Bitmap(Len,Len,System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Colour C;
            for (int i = 0; i < Squares.Length; i++)
            {
                C = Squares[i];
                image.SetPixel(i%Len, i/Len, Color.FromArgb(C.R,C.G,C.B));
            }

            image.Save(Path);
        }
    }
}

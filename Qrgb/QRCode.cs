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
        public QRCode(string Data):this(Data.ToArray().Select(x=>(byte) x).ToArray()){}

        public QRCode(byte[] Data):this(Conversions.ByteToBool(Data)) { }

        public QRCode(bool[] Data)
        {
            Squares = new Colour[Data.Length / Params.TotalBits];

            for (int i = 0; i < Squares.Length; i++)
            {
                Squares[i] = new Colour(Data.Skip(i* Params.TotalBits).Take(Params.TotalBits).ToArray());
            }
        }

        public bool[] GetData()
        {
            bool[] Data = new bool[Squares.Length * Params.TotalBits];

            for (int i = 0; i < Squares.Length; i++)
            {
                Squares[i].ToDataBits().CopyTo(Data, i * Params.TotalBits);
            }

            return Data;
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

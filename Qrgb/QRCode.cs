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

        public override string ToString()
        {
            byte[] Data = Conversions.BoolSetToByte(ToData());
            char[] CharSet = Data.Select(x => (char)x).ToArray();

            return new string(CharSet);
        }

        public bool[] ToData()
        {
            bool[] Data = new bool[Squares.Length * Params.TotalBits];

            for (int i = 0; i < Squares.Length; i++)
            {
                Squares[i].ToDataBits().CopyTo(Data, i * Params.TotalBits);
            }

            return Data;
        }

        public void Save(string Path = "./image.png", int squareSize=1)
        {
            int Len = (int)Math.Ceiling(Math.Sqrt(Squares.Length));

            Bitmap image = new Bitmap(Len*squareSize,Len*squareSize,System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Colour C;
            int x = 0, y = 0;
            for (int i = 0; i < Squares.Length; i++)
            {
                x = (i % Len)*squareSize; y = (i / Len)*squareSize;
                C = Squares[i];

                for (int X = x, Y = y; Y < y + squareSize;)
                {
                    image.SetPixel(X, Y, Color.FromArgb(C.R, C.G, C.B));
                    X++;
                    if (X >= x + squareSize) { X = x;Y++; }
                }
            }

            image.Save(Path);
        }

        public void Load(string Path = "./image.png")
        {
            Bitmap image = new Bitmap(Path);
        }
    }
}

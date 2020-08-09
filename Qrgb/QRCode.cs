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
            int EncodingHop = Params.PerColourEncodingBits * 3;
            Squares = new Colour[(Data.Length / Params.TotalBits) + EncodingHop];

            for (int i = 0; i < Squares.Length - EncodingHop; i++)
            {
                Squares[i+ EncodingHop] = new Colour(Data.Skip(i* Params.TotalBits).Take(Params.TotalBits).ToArray());
            }

            for (int i = 0; i < 3; i++)
            {
                bool[] Bits = Conversions.ByteToBool((byte)Params.BitsPerColour[i]);
                for (int j = 0; j < Params.PerColourEncodingBits; j++)
                {
                    if (Bits[j + (8-Params.PerColourEncodingBits)]) Squares[i * Params.PerColourEncodingBits + j] = Colour.Green;
                    else Squares[i * Params.PerColourEncodingBits + j] = Colour.White;
                }
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
            int EncodingHop = Params.PerColourEncodingBits * 3;
            bool[] Data = new bool[(Squares.Length-EncodingHop) * Params.TotalBits];

            for (int i = 0; i < Squares.Length-EncodingHop; i++)
            {
                Squares[i+EncodingHop].ToDataBits().CopyTo(Data, i * Params.TotalBits);
            }

            return Data;
        }

        public void Save(string Path = "./image.png", int squareSize=1)
        {
            squareSize *= 2;

            int Len = (int)Math.Ceiling(Math.Sqrt(Squares.Length));

            int imageSize = ((Len+6) * squareSize);

            Bitmap image = new Bitmap(imageSize, imageSize, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            DrawAlignmentSquare(0, 0, Colour.Red, ref image, squareSize);
            DrawAlignmentSquare(Len*2+6, 0, Colour.Green, ref image, squareSize);
            DrawAlignmentSquare(0, Len * 2 + 6, Colour.Blue, ref image, squareSize);

            Colour C;
            for (int i = 0,x=0,y=0; i < Squares.Length; i++)
            {
                x = ((i % Len) + 3); y = ((i / Len) + 3);
                C = Squares[i];

                DrawSquare(x, y, C, ref image, squareSize);
            }

            image.Save(Path);
        }

        private void DrawAlignmentSquare(int x,int y, Colour coreColour, ref Bitmap image, int squareSize = 1)
        {
            Colour C = Colour.White;

            DrawSquare((x + 2)/2, (y + 2)/2, coreColour, ref image, squareSize);

            squareSize /= 2;

            for (int i = 0; i < 5; i++) { 
                DrawSquare(x + i, y, C, ref image, squareSize);
                DrawSquare(x + i, y+5, C, ref image, squareSize);

                DrawSquare(x, y+i, C, ref image, squareSize);
                DrawSquare(x + 5, y + i, C, ref image, squareSize);
            }
            DrawSquare(x + 5, y + 5, C, ref image, squareSize);
        }

        private void DrawSquare(int x,int y, Colour C, ref Bitmap image, int squareSize = 1)
        {
            x *= squareSize; y *= squareSize;
            for (int X = x, Y = y; Y < y + squareSize;)
            {
                image.SetPixel(X, Y, Color.FromArgb(C.R, C.G, C.B));
                X++;
                if (X >= x + squareSize) { X = x; Y++; }
            }
        }

        public void Load(string Path = "./image.png")
        {
            Bitmap image = new Bitmap(Path);
        }
    }
}

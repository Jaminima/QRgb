using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QRgb
{
    public class QR
    {
        #region Fields

        private Colour[,] colours;

        #endregion Fields

        #region Methods

        private static bool[] ByteArrToBoolArr(byte[] data)
        {
            IEnumerable<bool[]> bData = data.Select(x =>
            {
                BitArray arr = new BitArray(new byte[] { x });
                bool[] bitarr = new bool[arr.Length];
                arr.CopyTo(bitarr, 0);
                Array.Reverse(bitarr);
                return bitarr;
            });
            bool[] bitArray = bData.SelectMany(x => x).ToArray();
            return bitArray;
        }

        private static byte[] ConvertBoolArrayToByte(bool[] source)
        {
            byte[] result = new byte[(int)Math.Ceiling((double)source.Length/8)];
            
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i])
                    result[i / 8] |= (byte)(1 << (7-(i % 8))); 
            }

            return result;
        }

        private int GetByteSegment(int bitI, bool[] data)
        {
            int value = 0;

            for (int i = 0; i < bitsPerChannel; i++)
            {
                if (bitI + i < data.Length)
                    value += data[bitI + i] ? (int)Math.Pow(2, bitsPerChannel - 1 - i) : 0;
            }

            return value;
        }

        private void SetBitSegment(int val, int bitI, ref bool[] data)
        {
            for (int i = 0; i < bitsPerChannel; i++)
            {
                if (bitI + i < data.Length)
                {
                    int posVal = (int)Math.Pow(2, bitsPerChannel - 1 - i);
                    if (val >= posVal)
                    {
                        data[bitI + (i)] = true;
                        val -= posVal;
                    }
                    else
                    {
                        data[bitI + (i)] = false;
                    }
                }
            }
        }

        private Colour GetColour(int bitI, bool[] data)
        {
            int r = GetByteSegment(bitI, data);
            bitI += bitsPerChannel;
            int g = GetByteSegment(bitI, data);
            bitI += bitsPerChannel;
            int b = GetByteSegment(bitI, data);

            int maxWithBits = (int)Math.Pow(2, bitsPerChannel)-1;
            int stepMul = 255 / maxWithBits;

            Colour c = new Colour(r * stepMul, g * stepMul, b * stepMul);

            return c;
        }

        private void ConvertColourToBits(Colour c, int bitI,ref bool[] bits)
        {
            int maxWithBits = (int)Math.Pow(2, bitsPerChannel)-1;
            int stepMul = 255 / maxWithBits;

            int rr = (int)Math.Round((decimal)c.R / stepMul),
                rg = (int)Math.Round((decimal)c.G / stepMul),
                rb = (int)Math.Round((decimal)c.B / stepMul);

            SetBitSegment(rr, bitI, ref bits);
            bitI += bitsPerChannel;
            SetBitSegment(rg, bitI, ref bits);
            bitI += bitsPerChannel;
            SetBitSegment(rb, bitI, ref bits);
        }

        #endregion Methods

        public const ushort Channels = 3;
        public readonly ushort bitsPerChannel = 1;
        public readonly int squareCount = 0, wh = 0;

        public QR(Image<Rgb24> image, ushort bitsPerChannel = 1)
        {
            this.bitsPerChannel = bitsPerChannel;

            float[,] edges = Processing.DetectEdges(image);
            Processing.EdgesToPNG(edges);
            int sqSize = Processing.DetermineSize(edges);

            wh = image.Width / sqSize;
            squareCount = wh * wh;
            colours = new Colour[wh, wh];

            sqSize = image.Width / wh;

            for (int x = 0, y = 0; y < wh;)
            {
                colours[y, x] = Processing.GetSquare(image, edges, x, y, sqSize);
                x++;
                if (x == wh) { x = 0; y++; }
            }
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(ToBytes());
        }

        public byte[] ToBytes()
        {
            int bitStep = bitsPerChannel * Channels;
            bool[] bitData = new bool[squareCount*bitStep];

            for (int x = 0, y = 0, bitI = 0; y < wh; bitI += bitStep)
            {
                Colour c = colours[y, x];
                ConvertColourToBits(c,bitI,ref bitData);

                x++;
                if (x == wh) { x = 0;y++; }
            }

            byte[] bytes = ConvertBoolArrayToByte(bitData);

            return bytes;
        }

        public QR(string str, ushort bitsPerChannel = 1) : this(Encoding.UTF8.GetBytes(str), bitsPerChannel)
        {
        }

        public QR(byte[] data, ushort bitsPerChannel = 1)
        {
            bool[] bitArray = ByteArrToBoolArr(data);

            this.bitsPerChannel = bitsPerChannel;

            int bitStep = bitsPerChannel * Channels;

            int bitCount = (data.Length * 8);
            squareCount = (int)Math.Ceiling((decimal)bitCount / bitStep);
            wh = (int)Math.Ceiling(Math.Sqrt(squareCount));

            colours = new Colour[wh, wh];

            for (int bitI = 0, x = 0, y = 0; bitI < bitCount; bitI += bitStep)
            {
                Colour c = GetColour(bitI, bitArray);

                colours[x, y] = c;

                x++;
                if (x == wh) { x = 0; y++; }
            }
        }

        public static QR Load(string path = "./image.png", ushort bitsPerChannel=1)
        {
            return new QR(Image.Load<Rgb24>(path), bitsPerChannel);
        }

        public void Save(string path = "./image.png", int squareSize = 1, int blackBorder = 0)
        {
            int imgwh = (wh * squareSize) + (blackBorder / 2);
            Image<Rgb24> img = new Image<Rgb24>(imgwh, imgwh);

            Pen borderPen = blackBorder > 0 ? new Pen(Color.Black, blackBorder) : new Pen(Color.Black, 1);

            for (int x = 0, y = 0, i = 0; i < squareCount; i++)
            {
                Colour c = colours[x, y];

                Rectangle r = new Rectangle(x * squareSize, y * squareSize, squareSize, squareSize);

                img.Mutate(x =>
                {
                    x.Fill(new Rgb24((byte)c.R, (byte)c.G, (byte)c.B), r);
                    if (blackBorder > 0) x.Draw(borderPen, r);
                });

                x++;
                if (x == wh) { x = 0; y++; }
            }

            img.SaveAsPng(path);
        }
    }
}
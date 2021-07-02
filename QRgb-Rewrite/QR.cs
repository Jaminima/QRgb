using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace QRgb
{
    public class QR
    {
        public Colour[,] colours;
        public readonly ushort bitsPerChannel = 1;
        public const ushort Channels = 3;
        public readonly int squareCount = 0, wh = 0, bitCount=0;

        public QR(string str, ushort bitsPerChannel = 1) : this(Encoding.UTF8.GetBytes(str), bitsPerChannel)
        {
        }

        public QR(byte[] data, ushort bitsPerChannel = 1)
        {
            IEnumerable<bool[]> bData = data.Select(x => { 
                BitArray arr = new BitArray(new byte[] { x });
                bool[] bitarr = new bool[arr.Length];
                arr.CopyTo(bitarr, 0);
                Array.Reverse(bitarr);
                return bitarr;
            });
            bool[] bitArray = bData.SelectMany(x=>x).ToArray();

            this.bitsPerChannel = bitsPerChannel;

            int bitStep = bitsPerChannel * Channels;

            bitCount = (data.Length * 8);
            squareCount = (int)Math.Ceiling((decimal)bitCount / bitStep);
            wh = (int)Math.Ceiling(Math.Sqrt(squareCount));

            colours = new Colour[wh, wh];

            for (int bitI = 0, x=0, y=0; bitI<bitCount; bitI += bitStep)
            {
                Colour c = GetColour(bitI, bitArray);

                colours[x, y] = c;

                x++;
                if (x == wh) { x = 0; y++; }
            }
        }

        public Colour GetColour(int bitI, bool[] data)
        {
            int r = GetByteSegment(bitI, data);
            bitI += bitsPerChannel;
            int g = GetByteSegment(bitI, data);
            bitI += bitsPerChannel;
            int b = GetByteSegment(bitI, data);

            int maxWithBits = (int) Math.Pow(2, bitsPerChannel-1);
            int stepMul = 255 / maxWithBits;

            Colour c = new Colour(r * stepMul, g * stepMul, b * stepMul);

            return c;
        }

        public int GetByteSegment(int bitI, bool[] data)
        {
            int value = 0;

            for (int i = 0; i < bitsPerChannel; i++)
            {
                if (bitI+i<data.Length)
                    value += data[bitI + i] ? (int)Math.Pow(2,i) : 0;
            }

            return value;
        }

        public void Save(string path = "./image.png")
        {
            Image<Rgb24> img = new Image<Rgb24>(wh,wh);

            for (int x = 0, y = 0, i = 0; i < squareCount; i++)
            {
                Colour c = colours[x, y];
                img[x, y] = new Rgb24((byte)c.R, (byte)c.G, (byte)c.B);

                x++;
                if (x == wh) { x = 0;  y++; }
            }

            img.SaveAsPng(path);
        }
    }
}

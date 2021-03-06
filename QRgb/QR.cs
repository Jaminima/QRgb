﻿using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Text;

namespace QRgb
{
    public class QR
    {
        #region Fields

        private Colour[,] colours;

        #endregion Fields

        #region Methods

        private static int BiasedRound(float val, float bound = 0.2f)
        {
            if (val > Math.Floor(val) + bound) return (int)Math.Ceiling(val);
            else return (int)Math.Floor(val);
        }

        private void ConvertColourToBits(Colour c, int bitI, ref bool[] bits)
        {
            int maxWithBits = (int)Math.Pow(2, bitsPerChannel) - 1;
            float stepMul = 255.0f / maxWithBits;

            int rr = BiasedRound(c.R / stepMul),
                rg = BiasedRound(c.G / stepMul),
                rb = BiasedRound(c.B / stepMul);

            SetBitSegment(rr, bitI, ref bits);
            bitI += bitsPerChannel;
            SetBitSegment(rg, bitI, ref bits);
            bitI += bitsPerChannel;
            SetBitSegment(rb, bitI, ref bits);
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

        private Colour GetColour(int bitI, bool[] data)
        {
            int r = GetByteSegment(bitI, data);
            bitI += bitsPerChannel;
            int g = GetByteSegment(bitI, data);
            bitI += bitsPerChannel;
            int b = GetByteSegment(bitI, data);

            int maxWithBits = (int)Math.Pow(2, bitsPerChannel) - 1;
            float stepMul = 255.0f / maxWithBits;

            Colour c = new Colour(BiasedRound(r * stepMul), BiasedRound(g * stepMul), BiasedRound(b * stepMul));

            return c;
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

        public QR(string str, ushort bitsPerChannel = 1) : this(Encoding.UTF8.GetBytes(str), bitsPerChannel)
        {
        }

        public QR(byte[] data, ushort bitsPerChannel = 1)
        {
            bool[] bitArray = Conversions.ByteArrToBoolArr(data);

            this.bitsPerChannel = bitsPerChannel;

            int bitStep = bitsPerChannel * Channels;

            int bitCount = (data.Length * 8);
            squareCount = (int)Math.Ceiling((decimal)bitCount / bitStep);
            wh = (int)Math.Ceiling(Math.Sqrt(squareCount));

            colours = new Colour[wh, wh];

            for (int bitI = 0, x = 0, y = 0; bitI < bitCount; bitI += bitStep)
            {
                Colour c = GetColour(bitI, bitArray);

                colours[y, x] = c;

                x++;
                if (x == wh) { x = 0; y++; }
            }
        }

        public static QR Load(string path = "./image.png", ushort bitsPerChannel = 1)
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
                Colour c = colours[y, x];

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

        public byte[] ToBytes()
        {
            int bitStep = bitsPerChannel * Channels;
            bool[] bitData = new bool[squareCount * bitStep];

            for (int x = 0, y = 0, bitI = 0; y < wh; bitI += bitStep)
            {
                Colour c = colours[y, x];
                if (c != null)
                    ConvertColourToBits(c, bitI, ref bitData);

                x++;
                if (x == wh) { x = 0; y++; }
            }

            byte[] bytes = Conversions.ConvertBoolArrayToByte(bitData);

            return bytes;
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(ToBytes());
        }
    }
}
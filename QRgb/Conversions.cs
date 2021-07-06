using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QRgb
{
    public static class Conversions
    {
        #region Methods

        public static bool[] ByteArrToBoolArr(byte[] data)
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

        public static byte[] ConvertBoolArrayToByte(bool[] source)
        {
            byte[] result = new byte[(int)Math.Ceiling((double)source.Length / 8)];

            for (int i = 0; i < source.Length; i++)
            {
                if (source[i])
                    result[i / 8] |= (byte)(1 << (7 - (i % 8)));
            }

            return result;
        }

        #endregion Methods
    }
}
using QRgb;
using System;

namespace Demo
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            string text = "Colour QR Codes are not very practical in the real world.... But hey ho!";

            for (ushort i = 1; i < 9; i++)
            {
                QR qR = new QR(text, i);

                qR.Save($"./out-{i}.png", squareSize: 50, blackBorder: 3);

                QR loaded_QR = QR.Load($"./out-{i}.png", i);

                Console.WriteLine($"Bits Per Channel {i}\n{loaded_QR.ToString()}");
            }

            //QR loaded_qR = QR.Load("./out-3.png", 3);
            //string s = loaded_qR.ToString();
            //Console.WriteLine(s);
            //loaded_qR.Save("./out-3-noise-loaded.png", 50, 3);
        }

        #endregion Methods
    }
}
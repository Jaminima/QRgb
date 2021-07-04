using QRgb;

namespace Demo
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            string text = "Colour QR Codes are not very practical in the real world.... But hey ho!";

            for (ushort i = 1; i < 8; i++)
            {
                QR qR = new QR(text, i);
                qR.Save($"./out-{i}.png",squareSize: 50, blackBorder: 3);
            }
        }

        #endregion Methods
    }
}
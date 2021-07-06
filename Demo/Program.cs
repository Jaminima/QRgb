using QRgb;

namespace Demo
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            string text = "Colour QR Codes are not very practical in the real world.... But hey ho!";

            for (ushort i = 2; i < 3; i++)
            {
                QR qR = new QR(text, i);
                qR.Save($"./out-{i}.png", squareSize: 50, blackBorder: 3);
            }

            QR loaded_qR = QR.Load("./out-2.png",2);
            string s = loaded_qR.ToString();
            loaded_qR.Save("./out-2-noise-loaded.png", 50, 3);
        }

        #endregion Methods
    }
}
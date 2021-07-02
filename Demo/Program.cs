using QRgb;

namespace Demo
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            QR qR = new QR("I Like Broccoli, So Wery Very Much :)", 3);
            qR.Save();
        }

        #endregion Methods
    }
}
using System;
using QRgb;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            QR qR = new QR("I Like Broccoli, So Wery Very Much :)",1);
            qR.Save();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qrgb;

namespace Qrgb_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Params.Calculate();

            //QRCode C = new QRCode(@"This is a test! Hopefully it wont poo itself. But only time will tell ;)");

            //byte[] D = Conversions.BoolSetToByte(C.ToData());

            //C.Save(squareSize:2);

            QRCode L = QRCode.Load();

            string S = L.ToString();
        }
    }
}

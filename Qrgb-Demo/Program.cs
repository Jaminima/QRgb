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
            Params.Calculate();

            QRCode C = new QRCode("You are the most beautiful thing in the world");

            byte[] D = Conversions.BoolSetToByte(C.GetData());

            char[] DC = D.Select(x => (char)x).ToArray();

            C.SaveOut();
        }
    }
}

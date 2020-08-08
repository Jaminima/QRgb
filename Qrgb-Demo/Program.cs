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

            Colour C = new Colour(230, 126, 127);
            bool[] D = C.ToDataBits();

            Colour nC = new Colour(D);
            bool[] nD = nC.ToDataBits();
        }
    }
}

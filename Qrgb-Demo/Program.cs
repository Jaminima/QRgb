﻿using System;
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

            QRCode C = new QRCode("Sorry for everything, i do love you~djdsnhdsfhsd ifhsweikf whfiue hfieh fb  k IU GHDIJR HGIERH FIHB IHHGIHWIJHrsedhfusdhfuhsdhfusehfhsdhfhu");

            string S = C.ToString();

            C.Save(squareSize:50);
        }
    }
}

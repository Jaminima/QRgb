using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

namespace QRgb
{
    public static class Processing
    {
        const float MaxLinear = 255 * 3;

        public static float GetLinearFromColour(Rgb24 rgb)
        {
            return rgb.R + rgb.G + rgb.B;
        }

        public static float[,] DetectEdges(Image<Rgb24> image)
        {
            float[,] imageEdges = new float[image.Height, image.Width];

            for (int x = 0, y = 0; y < image.Height;)
            {
                imageEdges[y, x] = 1 - (GetLinearFromColour(image[x, y]) / MaxLinear);

                x++;
                if (x == image.Width)
                {
                    x = 0;
                    y++;
                }
            }

            return imageEdges;
        }

        public static void EdgesToPNG(float[,] edges, string path="./edges.png")
        {
            int h = edges.GetLength(1), w = edges.GetLength(0);
            Image<Rgb24> image = new Image<Rgb24>(h, w);

            for (int x = 0, y = 0; y < h;)
            {
                image[y, x] = edges[y, x] == 1 ? Color.White : Color.Black;
                x++;
                if (x == w) { x = 0; y++; }
            }

            image.SaveAsPng(path);
        }
    }
}

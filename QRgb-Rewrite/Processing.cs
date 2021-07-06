using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace QRgb
{
    public static class Processing
    {
        #region Fields

        private const float MaxLinear = 255 * 3;
        private const float MaxLinearSum = MaxLinear * 5;

        #endregion Fields

        #region Methods

        public static float[,] DetectEdges(Image<Rgb24> image)
        {
            float[,] imageEdges = new float[image.Height, image.Width];

            for (int x = 0, y = 0; y < image.Height;)
            {
                float f = 0;

                f += GetLinearFromPixel(image, x, y);

                f += GetLinearFromPixel(image, x - 1, y);
                f += GetLinearFromPixel(image, x + 1, y);

                f += GetLinearFromPixel(image, x, y + 1);
                f += GetLinearFromPixel(image, x, y - 1);

                f /= MaxLinearSum;

                f = 1 - f;

                imageEdges[y, x] = f;

                x++;
                if (x == image.Width)
                {
                    x = 0;
                    y++;
                }
            }

            return imageEdges;
        }

        public static void EdgesToPNG(float[,] edges, string path = "./edges.png")
        {
            int h = edges.GetLength(1), w = edges.GetLength(0);
            Image<Rgb24> image = new Image<Rgb24>(h, w);

            for (int x = 0, y = 0; y < h;)
            {
                image[y, x] = edges[y, x] > 0.8f ? Color.White : Color.Black;
                x++;
                if (x == w) { x = 0; y++; }
            }

            image.SaveAsPng(path);
        }

        public static float GetLinearFromColour(Rgb24 rgb)
        {
            return rgb.R + rgb.G + rgb.B;
        }

        public static float GetLinearFromPixel(Image<Rgb24> image, int x, int y)
        {
            if (x >= 0 && y >= 0 && x < image.Width && y < image.Height)
                return GetLinearFromColour(image[y, x]);
            else
                return 0;
        }

        #endregion Methods
    }
}
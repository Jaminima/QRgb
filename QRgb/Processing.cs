using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace QRgb
{
    public static class Processing
    {
        #region Fields

        private const float EdgeMin = 0.97f;
        private const float MaxLinear = 255 * 3;
        private const float MaxLinearSum = MaxLinear * 1;
        private const int MinGap = 20;

        #endregion Fields

        #region Methods

        public static float[,] DetectEdges(Image<Rgb24> image)
        {
            float[,] imageEdges = new float[image.Height, image.Width];

            for (int x = 0, y = 0; y < image.Height;)
            {
                float f = 0;

                f += GetLinearFromPixel(image, x, y);

                //f += GetLinearFromPixel(image, x - 1, y);
                //f += GetLinearFromPixel(image, x + 1, y);

                //f += GetLinearFromPixel(image, x, y + 1);
                //f += GetLinearFromPixel(image, x, y - 1);

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

        public static int DetermineSize(float[,] edges)
        {
            int lastXEdge = 0;
            int edgeGapSum = 0, edgeGapCount = 0;

            for (int x = 0, y = 0; y < edges.GetLength(1);)
            {
                if (edges[y, x] > EdgeMin)
                {
                    if (x - lastXEdge > MinGap)
                    {
                        edgeGapSum += x - lastXEdge;
                        edgeGapCount++;
                    }

                    lastXEdge = x;
                }

                x++;
                if (x == edges.GetLength(0)) { x = 0; y++; lastXEdge = 0; }
            }
            int s = edgeGapSum / edgeGapCount;
            return s;
        }

        public static void EdgesToPNG(float[,] edges, string path = "./edges.png")
        {
            int h = edges.GetLength(1), w = edges.GetLength(0);
            Image<Rgb24> image = new Image<Rgb24>(h, w);

            for (int x = 0, y = 0; y < h;)
            {
                image[x, y] = edges[y, x] > EdgeMin ? Color.White : Color.Black;
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
                return GetLinearFromColour(image[x, y]);
            else
                return 0;
        }

        public static Colour GetSquare(Image<Rgb24> image, float[,] edges, int x, int y, int sqSize)
        {
            Colour sumColour = new Colour();
            int sumCount = 0;

            x *= sqSize;
            y *= sqSize;
            for (int _x = x, _y = y; _y < y + sqSize;)
            {
                if (edges[_y, _x] < EdgeMin)
                {
                    Rgb24 c = image[_x, _y];
                    sumColour = new Colour(sumColour.R + c.R, sumColour.G + c.G, sumColour.B + c.B);
                    sumCount++;
                }
                _x++;
                if (_x == x + sqSize) { _x = x; _y++; }
            }

            if (sumCount > 0)
                return new Colour(sumColour.R / sumCount, sumColour.G / sumCount, sumColour.B / sumCount);
            else
                return new Colour(0, 0, 0);
        }

        #endregion Methods
    }
}
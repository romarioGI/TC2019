using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace BinaryCodeToImage
{
    public static class BitmapCoder
    {
        private static Bitmap CreateBitmap(int length)
        {
            var n = (length >> 5) + 1;
            var h = (int) Math.Sqrt(n);
            var w = (n + h - 1) / h;

            return new Bitmap(w, h);
        }

        private static readonly byte[] TwoDegrees = {1, 2, 4, 8, 16, 32, 64, 128};

        public static Bitmap GetBitmap(BitArray code)
        {
            var index = 0;
            var bmpIndex = 0;
            var pixel = new byte[4];
            var bitmap = CreateBitmap(code.Length);

            while (index < code.Length)
            {
                pixel[0] = pixel[1] = pixel[2] = pixel[3] = 0;
                for (var i = 0; i < 4; i++)
                for (var j = 0; j < 8; j++)
                {
                    if ((index < code.Length && code[index]) || index == code.Length)
                        pixel[i] += TwoDegrees[j];
                    index++;
                }

                var color = Color.FromArgb(pixel[0], pixel[1], pixel[2], pixel[3]);
                bitmap.SetPixel(bmpIndex % bitmap.Width, bmpIndex / bitmap.Width, color);
                bmpIndex++;
            }

            return bitmap;
        }

        public static BitArray GetCode(Bitmap bitmap)
        {
            var byteList = new List<byte>();

            for (var y = 0; y < bitmap.Height; y++)
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                byteList.Add(pixel.A);
                byteList.Add(pixel.R);
                byteList.Add(pixel.G);
                byteList.Add(pixel.B);
            }

            var res = new BitArray(byteList.ToArray());
            while (res[res.Count - 1] == false)
                res.Length--;
            res.Length--;

            return res;
        }
    }
}

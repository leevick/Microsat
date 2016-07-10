using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsat.SpecProc;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace TestSpecProc
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap bmp = new Bitmap(1024, 768, PixelFormat.Format48bppRgb);
            BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, 1024, 768), ImageLockMode.ReadWrite, PixelFormat.Format48bppRgb);
            byte[] buffer = new byte[1024*768*3*2];
            for (int j = 0; j < 768; j++)
            {
                for (int i = 0; i < 1024; i++)
                {
                    buffer[1024*2*3*j+3*2*i+1] = 255;
                    buffer[1024 * 2 * 3 * j + 3 * 2 * i + 3] = 255;
                    buffer[1024 * 2 * 3 * j + 3 * 2 * i + 5] = 255;
                }
            }
            Marshal.Copy(buffer, 0, bmpdata.Scan0, 1024 * 768 * 3 * 2);
            bmp.UnlockBits(bmpdata);

            bmp.Save("test.png",ImageFormat.Png);

        }
    }
}

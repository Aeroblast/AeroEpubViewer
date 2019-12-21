﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
namespace AeroEpubViewer
{
    class ImageHack
    {

        static ImageAttributes imageAttributes = new ImageAttributes();
        static bool prepared = false;
        static void PrepareWarmer()
        {
            int warmColor = 0xffe6a0;
            float[][] colorMatrixElements = {
   new float[] {1-(0xff-(warmColor>>16))/(float)0xff,  0,  0,  0, 0},
   new float[] {0, 1 - (0xff - ((warmColor&0xff00) >> 8)) / (float)0xff,  0,  0, 0},
   new float[] {0,  0, 1 - (0xff - (warmColor & 0xff) ) / (float)0xff,  0, 0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0, 0, 0, 0, 1F}};
            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            prepared = true;
        }
        public static byte[] Warmer(byte[] data)
        {
            if (!prepared) PrepareWarmer();
            Bitmap b = new Bitmap(new MemoryStream(data));
            Bitmap r = new Bitmap(b.Width, b.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(r))
            {
                g.DrawImage(b, new Rectangle(0, 0, b.Width, b.Height), 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, imageAttributes);
                var output = new MemoryStream();
                r.Save(output, ImageFormat.Bmp);
                return output.ToArray();
            }
        }
    }
}
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;//Add PresentationCore.dll  WindowsBase to references
namespace AeroEpubViewer
{
    class ImageHack
    {

        static ImageAttributes imageAttributes = new ImageAttributes();
        static bool prepared = false;
        public static int warmColor = 0xffe6a0;
        public static void SetWarmColor(string hex)
        {
            if (hex.Length == 7 && hex[0] == '#')
            {
                hex = hex.Substring(1);
                int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out warmColor);
            }
        }
        static void PrepareWarmer()
        {
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
            using (Bitmap b = new Bitmap(new MemoryStream(data)))
            using (Bitmap r = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppArgb))
            using (Graphics g = Graphics.FromImage(r))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(b, new Rectangle(0, 0, b.Width, b.Height), 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, imageAttributes);
                var output = new MemoryStream();
                r.Save(output, ImageFormat.Bmp);
                return output.ToArray();
            }
        }
       
        public static byte[] TryDecode(byte[] data)
        {//https://docs.microsoft.com/en-us/dotnet/framework/debug-trace-profile/contextswitchdeadlock-mda
            try
            {
                using (MemoryStream output = new MemoryStream()) 
                {
                    var t = BitmapDecoder.Create(new MemoryStream(data), BitmapCreateOptions.None, BitmapCacheOption.None);
                    var encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(t.Frames[0]);
                    encoder.Save(output);
                    return output.ToArray();
                }
            }
            catch (Exception)
            {
                Log.log("[Warn]Cannot decode image.");
                return null;
            }

        }
    }
}

using BCnEncoder.Decoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using War3Net.Drawing.Blp;

namespace BetterTriggers.Utility
{
    public static class Images
    {
        private static BcDecoder bcDecoder = new BcDecoder();

        /// <summary>
        /// Accepts DDS and BLP streams.
        /// </summary>
        public static Bitmap ReadImage(Stream stream)
        {
            byte[] bytes = new byte[4];
            var bytes_length = stream.Read(bytes, 0, bytes.Length);
            if (bytes_length != bytes.Length)
                throw new Exception("Couldn't read image");

            string format = System.Text.Encoding.UTF8.GetString(bytes);
            stream.Position = 0;
            Bitmap image = null;
            if (format == "BLP1")
                image = ReadBLP(stream);
            else if (format.StartsWith("DDS"))
                image = ReadDDS(stream);
            else
            {
                SixLabors.ImageSharp.Image<Rgba32> tga = SixLabors.ImageSharp.Image.Load<Rgba32>(stream);
                image = ToBitmap(tga);
            }

            return image;
        }

        private static Bitmap ReadBLP(Stream stream)
        {
            BlpFile blpFile = new BlpFile(stream);
            int width;
            int height;

            // The library does not determine what's BLP1 and BLP2 properly, so we manually set bool bgra in GetPixels depending on the checkbox.
            byte[] bytes = blpFile.GetPixels(0, out width, out height); // 0 indicates first mipmap layer. width and height are assigned width and height in GetPixels().
            stream.Close();

            Bitmap image = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            BitmapData bmpData = image.LockBits(
                                 new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                                 ImageLockMode.WriteOnly, image.PixelFormat);

            Marshal.Copy(bytes, 0, bmpData.Scan0, bytes.Length);
            image.UnlockBits(bmpData);
            blpFile.Dispose();

            return image;
        }

        private static Bitmap ReadDDS(Stream stream)
        {
            SixLabors.ImageSharp.Image<Rgba32> image = null;
            using (stream)
            {
                image = bcDecoder.Decode(stream);
                return ToBitmap(image);
            }
        }

        private static System.Drawing.Bitmap ToBitmap(SixLabors.ImageSharp.Image<Rgba32> image)
        {
            Stream stream = new System.IO.MemoryStream();
            SixLabors.ImageSharp.Formats.Bmp.BmpEncoder bmpEncoder = new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder(); // we need an encoder to preserve transparency.
            bmpEncoder.BitsPerPixel = SixLabors.ImageSharp.Formats.Bmp.BmpBitsPerPixel.Pixel32; // bitmap transparency needs 32 bits per pixel before we set transparency support.
            bmpEncoder.SupportTransparency = true;
            image.SaveAsBmp(stream, bmpEncoder);
            System.Drawing.Image img = System.Drawing.Image.FromStream(stream);

            return new System.Drawing.Bitmap(stream);
        }
    }
}

using BCnEncoder.Decoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Utility
{
    public static class Converters
    {
        private static BcDecoder bcDecoder = new BcDecoder();

        public static Bitmap ReadDDS(Stream stream)
        {
            SixLabors.ImageSharp.Image<Rgba32> image = null;
            using (stream)
            {
                image = bcDecoder.Decode(stream);
                return ToBitmap(image);
            }
        }

        public static System.Drawing.Bitmap ToBitmap(SixLabors.ImageSharp.Image<Rgba32> image)
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

using BCnEncoder.Decoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
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
using System.Windows.Media.Imaging;
using War3Net.Common.Extensions;
using War3Net.Drawing.Blp;

namespace BetterTriggers.Utility
{
    public static class Images
    {
        /// <summary>
        /// Accepts DDS, BLP and TGA streams.
        /// </summary>
        public static byte[] ReadImage(Stream stream)
        {
            byte[] bytes = new byte[4];
            var bytes_length = stream.Read(bytes, 0, bytes.Length);
            if (bytes_length != bytes.Length)
                throw new Exception("Couldn't read image");

            string format = System.Text.Encoding.UTF8.GetString(bytes);
            stream.Position = 0;
            if (format == "BLP1")
            {
                var blp = new BlpFile(stream);
                var bitmapSource = blp.GetBitmapSource();
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(ms);
                    return ms.ToArray();
                }
            }
            else if (format.StartsWith("DDS"))
            {
                byte[] image = new byte[stream.Length];
                stream.CopyTo(image, 0, (int)stream.Length);
                return image;
            }
            else
            {
                Image<Rgba32> tga = SixLabors.ImageSharp.Image.Load<Rgba32>(stream);
                using (var ms = new MemoryStream())
                {
                    PngEncoder encoder = new PngEncoder();
                    tga.SaveAsPng(ms, encoder);
                    return ms.ToArray();
                }
            }
        }
    }
}

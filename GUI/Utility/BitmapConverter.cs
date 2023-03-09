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

namespace GUI.Utility
{
    public static class BitmapConverter
    {
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        /// <summary>
        /// Get the system icon corresponding to the specified index
        /// </summary>
        public static Bitmap GetSystemIcon(int index)
        {
            LoadSystemIcon();
            return index < SystemIconList.Count ? SystemIconList[index].ToBitmap() : null;
        }

        public static List<Bitmap> GetSystemIconAll()
        {
            LoadSystemIcon();
            return SystemIconList.Select(x => x.ToBitmap()).ToList();

        }


        private static List<Icon> SystemIconList = new List<Icon>(); // Record system icon

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //private static extern bool MessageBeep(uint type);

        [DllImport("Shell32.dll")]
        public extern static int ExtractIconEx(string libName, int iconIndex, IntPtr[] largeIcon, IntPtr[] smallIcon, int nIcons);

        private static IntPtr[] largeIcon;
        private static IntPtr[] smallIcon;

        /// <summary>
        /// Get all system icon images
        /// </summary>
        private static void LoadSystemIcon()
        {
            if (SystemIconList.Count > 0) return;

            largeIcon = new IntPtr[1000];
            smallIcon = new IntPtr[1000];

            ExtractIconEx("imageres.dll", 0, largeIcon, smallIcon, 1000);

            SystemIconList.Clear();
            for (int i = 0; i < largeIcon.Length; i++)
            {
                try
                {
                    Icon ic = Icon.FromHandle(largeIcon[i]);
                    SystemIconList.Add(ic);
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }
    }
}

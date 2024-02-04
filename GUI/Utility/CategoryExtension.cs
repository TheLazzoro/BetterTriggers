using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GUI.Utility
{
    internal class CategoryExtension
    {
        private static Dictionary<string, ImageSource> categories = new Dictionary<string, ImageSource>();

        internal static ImageSource getImageByCategory(string category)
        {
            Category cat = Category.Get(category);
            if(categories.ContainsKey(category))
            {
                ImageSource image;
                categories.TryGetValue(category, out image);
                return image;
            }
            else
            {
                ImageSource image = BitmapConverter.ByteToImage(cat.Icon);
                categories.Add(category, image);
                return image;
            }
        }
    }
}

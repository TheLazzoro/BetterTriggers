using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GUI.Utility
{
    internal class CategoryExtension
    {
        private static Dictionary<string, BitmapImage> categories = new Dictionary<string, BitmapImage>();

        internal static BitmapImage getImageByCategory(string category)
        {
            Category cat = Category.Get(category);
            if(categories.ContainsKey(category))
            {
                BitmapImage image;
                categories.TryGetValue(category, out image);
                return image;
            }
            else
            {
                BitmapImage image = BitmapConverter.ToBitmapImage(cat.Icon);
                categories.Add(category, image);
                return image;
            }
        }
    }
}

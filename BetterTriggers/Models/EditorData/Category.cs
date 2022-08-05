
using System.Collections.Generic;
using System.IO;
using System.Drawing;

namespace BetterTriggers.Models.EditorData
{
    public class Category
    {
        private static Dictionary<string, Category> categories = new Dictionary<string, Category>();

        public Bitmap Icon { get; }
        public string Name { get; }
        public bool ShouldDisplay { get; }

        private Category(Bitmap icon, string name, bool shouldDisplay)
        {
            this.Name = name;
            this.Icon = icon;
            this.ShouldDisplay = shouldDisplay;
        }

        public static void Create(string key, Bitmap icon, string name, bool shouldDisplay)
        {
            Category category = new Category(icon, name, shouldDisplay);
            categories.Add(key, category);
        }

        public static Category Get(string key)
        {
            if (key == null)
                return new Category(null, "", false); // hack

            Category category;
            categories.TryGetValue(key, out category);
            return category;
        }
    }
}
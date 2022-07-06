
using System.Collections.Generic;
using System.IO;

namespace BetterTriggers.Models.EditorData
{
    public class Category
    {
        private static Dictionary<string, Category> categories = new Dictionary<string, Category>();

        public Stream Icon { get; }
        public string Name { get; }
        public bool ShouldDisplay { get; }

        private Category(string key, Stream icon, string name, bool shouldDisplay)
        {
            this.Name = name;
            this.Icon = icon;
            this.ShouldDisplay = shouldDisplay;
        }

        public static void Create(string key, Stream icon, string name, bool shouldDisplay)
        {
            Category category = new Category(key, icon, name, shouldDisplay);
            categories.Add(key, category);
        }

        public static Category Get(string key)
        {
            if (key == null)
                return new Category("", null, "", false); // hack

            Category category;
            categories.TryGetValue(key, out category);
            return category;
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Drawing;

namespace BetterTriggers.Models.EditorData
{
    public class Category
    {
        private static Dictionary<string, Category> categories = new Dictionary<string, Category>();

        /// <summary>
        /// Image icon file stored in bytes.
        /// </summary>
        public byte[] Icon { get; }
        public string Name { get; }
        public bool ShouldDisplay { get; }

        private Category(byte[] icon, string name, bool shouldDisplay)
        {
            this.Name = name;
            this.Icon = icon;
            this.ShouldDisplay = shouldDisplay;
        }

        public static void Create(string key, byte[] icon, string name, bool shouldDisplayInCategoryList)
        {
            Category category = new Category(icon, name, shouldDisplayInCategoryList);
            categories.Add(key, category);
        }

        public static Category Get(string key)
        {
            if (key == null)
                return new Category(null, "", false); // hack

            Category category;
            categories.TryGetValue(key, out category);
            if(category == null)
                return new Category(null, "", false); // hack

            return category;
        }

        internal static void Clear()
        {
            categories.Clear();
        }
    }
}
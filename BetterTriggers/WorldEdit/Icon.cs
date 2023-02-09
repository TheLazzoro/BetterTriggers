using BetterTriggers.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.WorldEdit
{
    public class Icon
    {
        public static Dictionary<string, Icon> icons = new();

        public string path { get; }
        public string displayName { get; }
        public string category { get; }

        public Icon(string path, string displayName, string category)
        {
            this.path = path;
            this.displayName = displayName;
            this.category = category;
            icons.TryAdd(path, this);
        }

        public static Bitmap Get(string path)
        {
            string filePath = Path.Combine(CustomMapData.mapPath, path);
            if (File.Exists(filePath))
            {
                using (Stream fs = new FileStream(filePath, FileMode.Open))
                {
                    return Images.ReadImage(fs);
                }
            }

            if (Casc.GetCasc().FileExists("War3.w3mod/" + Path.ChangeExtension(path, ".dds")))
                return Images.ReadImage(Casc.GetCasc().OpenFile("War3.w3mod/" + Path.ChangeExtension(path, ".dds")));

            return new Bitmap(4, 4);
        }

        public static List<Icon> GetAll()
        {
            List<Icon> list = new List<Icon>();
            var enumerator = icons.GetEnumerator();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current.Value);
            }

            return list;
        }
    }
}

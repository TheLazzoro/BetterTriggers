using BetterTriggers.Containers;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit.GameDataReader;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.IO.Mpq;

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

        /// <exception cref="IOException">When MPQ archive is in use by another process.</exception>
        public static byte[] Get(string path)
        {
            string fullMapPath = Project.CurrentProject.GetFullMapPath();
            string filePath = Path.Combine(fullMapPath, path);
            if (File.Exists(filePath))
            {
                using (Stream fs = new FileStream(filePath, FileMode.Open))
                {
                    return Images.ReadImage(fs);
                }
            }
            if (File.Exists(fullMapPath))
            {
                if (CustomMapData.MPQMap.ImportedFiles != null)
                {
                    var imports = CustomMapData.MPQMap.ImportedFiles.Files;
                    var pathFormatted = path.Replace('/', '\\');
                    for (int i = 0; i < imports.Count; i++)
                    {
                        if (imports[i].FullPath.ToLower() == pathFormatted.ToLower())
                        {
                            MpqArchive mpq = MpqArchive.Open(fullMapPath);
                            Stream stream = MpqFile.OpenRead(mpq, pathFormatted);
                            mpq.Dispose();
                            return Images.ReadImage(stream);
                        }
                    }
                }
            }

            return Images.ReadImage(WarcraftStorageReader.OpenFile(Path.ChangeExtension(path, WarcraftStorageReader.ImageExt)));
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

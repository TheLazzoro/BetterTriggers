using BetterTriggers.Models.War3Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using War3Net.Build.Environment;
using War3Net.Build.Extensions;
using War3Net.Build.Widget;

namespace BetterTriggers.WorldEdit
{
    internal class Destructibles
    {
        private static List<DoodadData> destructibles = new List<DoodadData>();

        internal static List<DoodadData> GetAll()
        {
            return destructibles;
        }

        /// <summary>
        /// Loads all placed destructibles on the map.
        /// </summary>
        /// <returns></returns>
        internal static void Load()
        {
            destructibles.Clear();
            var destructibleData = DestructibleTypes.GetAll();

            string filePath = "war3map.doo";
            if (!File.Exists(Path.Combine(CustomMapData.mapPath, filePath)))
                return;

            while (CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            using (Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, filePath), FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(s);
                var doodads = BinaryReaderExtensions.ReadMapDoodads(reader);

                // TODO: ugly loop
                for (int i = 0; i < doodads.Doodads.Count; i++)
                {
                    var doodad = doodads.Doodads[i];

                    for (int j = 0; j < destructibleData.Count; j++)
                    {
                        if (doodad.ToString() == destructibleData[j].DestCode)
                        {
                            destructibles.Add(doodad);
                            break;
                        }

                    }
                }
            }
        }
    }
}

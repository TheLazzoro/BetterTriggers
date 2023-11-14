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
    public class Destructibles
    {
        private static List<DoodadData> destructibles = new List<DoodadData>();

        public static List<DoodadData> GetAll()
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

            MapDoodads doodads;
            doodads = CustomMapData.MPQMap.Doodads;
            if (doodads == null)
                return;

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

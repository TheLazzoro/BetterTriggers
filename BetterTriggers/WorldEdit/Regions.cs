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

namespace BetterTriggers.WorldEdit
{
    internal static class Regions
    {
        private static List<Region> regions = new List<Region>();

        internal static List<Region> GetAll()
        {
            return regions;
        }

        internal static void Load()
        {
            regions.Clear();

            string filePath = "war3map.w3r";
            if (!File.Exists(Path.Combine(CustomMapData.mapPath, filePath)))
                return;

            while (CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            using (Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, filePath), FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(s);
                var mapRegions = BinaryReaderExtensions.ReadMapRegions(reader);

                mapRegions.Regions.ForEach(r => regions.Add(r));
            }
        }
    }
}

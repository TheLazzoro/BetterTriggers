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
            MapRegions mapRegions = CustomMapData.MPQMap.Regions;
            if (mapRegions == null)
                return;

            mapRegions.Regions.ForEach(r => regions.Add(r));
        }
    }
}

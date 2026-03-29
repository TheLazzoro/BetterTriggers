using BetterTriggers.Containers;
using System.Collections.Generic;
using War3Net.Build.Environment;

namespace BetterTriggers.WorldEdit
{
    public class Regions
    {
        private List<Region> regions = new List<Region>();

        public List<Region> GetAll()
        {
            return regions;
        }

        internal void Load(Project project)
        {
            regions.Clear();
            MapRegions mapRegions = project.MPQMap.Regions;
            if (mapRegions == null)
                return;

            mapRegions.Regions.ForEach(r => regions.Add(r));
        }
    }
}

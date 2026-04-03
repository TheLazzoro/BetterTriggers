using BetterTriggers.Containers;
using System.Collections.Generic;
using War3Net.Build.Widget;

namespace BetterTriggers.WorldEdit
{
    public class Destructibles
    {
        private List<DoodadData> destructibles = new List<DoodadData>();
        private List<DoodadData> allDoodads = new List<DoodadData>();

        public List<DoodadData> GetAll()
        {
            return destructibles;
        }

        public List<DoodadData> GetAllDoodads()
        {
            return allDoodads;
        }

        /// <summary>
        /// Loads all placed destructibles on the map.
        /// </summary>
        /// <returns></returns>
        internal void Load(Project project)
        {
            destructibles.Clear();
            allDoodads.Clear();
            var destructibleData = project.DestructibleTypes.GetAll();

            MapDoodads doodads;
            doodads = project.MPQMap.Doodads;
            if (doodads == null)
                return;

            // TODO: ugly loop
            for (int i = 0; i < doodads.Doodads.Count; i++)
            {
                var doodad = doodads.Doodads[i];
                allDoodads.Add(doodad);

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

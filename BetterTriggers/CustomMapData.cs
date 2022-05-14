using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.WorldEdit;
using Model.EditorData;
using Model.War3Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.Build.Environment;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Build.Widget;

namespace BetterTriggers
{
    public static class CustomMapData
    {
        public static string mapPath = @"D:\Test\TestMap.w3x";

        public static void Load()
        {
            UnitTypes.Load();
            ItemTypes.Load();
            DestructibleTypes.Load();
            DoodadTypes.Load();
            AbilityTypes.Load();
            BuffTypes.Load();
            UpgradeTypes.Load();

            Cameras.Load();
            Destructibles.Load();
            Regions.Load();
            Units.Load();

            ModelData.Load(); // requires above
        }

        /// <summary>
        /// Removes all used map data that no longer exists in the map.
        /// </summary>
        /// <returns>A list of modified triggers.</returns>
        public static List<ExplorerElementTrigger> RemoveInvalidReferences()
        {
            List<ExplorerElementTrigger> modified = new List<ExplorerElementTrigger>();
            ControllerTrigger controller = new ControllerTrigger();
            var triggers = ContainerTriggers.GetAll();
            for (int i = 0; i < triggers.Count; i++)
            {
                bool wasRemoved = controller.RemoveInvalidReferences(triggers[i]);
                if (wasRemoved)
                    modified.Add(triggers[i]);

                triggers[i].Notify();
            }

            return modified;
        }
    }
}

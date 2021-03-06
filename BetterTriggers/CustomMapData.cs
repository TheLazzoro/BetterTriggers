using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.WorldEdit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using War3Net.Build.Environment;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Build.Widget;

namespace BetterTriggers
{
    public class CustomMapData
    {
        public static string mapPath;
        //public static string mapPath = @"D:\Test\Another Test Project\map\TestMap.w3x"; // 1.32 map
        //public static string mapPath = @"...\Desktop\Direct Strike Reforged (JASS).w3x"; // 1.33 map
        private static FileSystemWatcher watcher;
        public static event FileSystemEventHandler OnSaving;

        private static void InvokeOnSaving(object sender, FileSystemEventArgs e)
        {
            if (OnSaving != null)
                OnSaving(sender, e);
        }

        public static void Init(string _mapPath)
        {
            mapPath = _mapPath;
            watcher = new System.IO.FileSystemWatcher();
            watcher.Path = Path.GetDirectoryName(mapPath);
            watcher.EnableRaisingEvents = true;
            watcher.Created += Watcher_Created;

            Load();
        }

        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            if(e.Name == Path.GetFileName(mapPath) + "Temp")
                InvokeOnSaving(sender, e);
        }

        public static bool IsMapSaving()
        {
            if (Directory.Exists(mapPath + "Temp"))
                return true;
            else if (Directory.Exists(mapPath + "Backup"))
                return true;
            else
                return false;
        }

        // TODO: Optimize to only reload custom map data
        // Right now this is re-run when new map data is detected.
        public static void Load()
        {
            while (IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            Info.Load();
            MapStrings.Load();
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
            Sounds.Load();
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
            var triggers = Triggers.GetAll();
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

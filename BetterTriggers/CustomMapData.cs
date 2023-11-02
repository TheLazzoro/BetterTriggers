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
using War3Net.Build;
using War3Net.Build.Environment;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Build.Widget;
using War3Net.IO.Mpq;

namespace BetterTriggers
{
    public class CustomMapData
    {
        internal static Map MPQMap;
        private static FileSystemWatcher watcher;
        public static event FileSystemEventHandler OnSaving;

        private static void InvokeOnSaving(object sender, FileSystemEventArgs e)
        {
            if (OnSaving != null)
                OnSaving(sender, e);
        }

        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            // this try-block is only here because of the TriggerConverter.
            try
            {
                var mapPath = Project.GetFullMapPath();
                if (e.Name == Path.GetFileName(mapPath) + "Temp")
                    InvokeOnSaving(sender, e);
            }
            catch (Exception)
            {

            }
        }

        public static bool IsMapSaving(string fullMapPath = null)
        {
            if (string.IsNullOrEmpty(fullMapPath))
            {
                fullMapPath = Project.GetFullMapPath();
            }

            if (Directory.Exists(fullMapPath + "Temp"))
                return true;
            else if (Directory.Exists(fullMapPath + "Backup"))
                return true;
            else
                return false;
        }


        public static void Load(string fullMapPath = null)
        {
            if (string.IsNullOrEmpty(fullMapPath))
            {
                fullMapPath = Project.GetFullMapPath();
            }

            while (IsMapSaving(fullMapPath))
            {
                Thread.Sleep(1000);
            }
            MPQMap = Map.Open(fullMapPath);

            Info.Load();
            MapStrings.Load();
            UnitTypes.Load(fullMapPath);
            ItemTypes.Load();
            DestructibleTypes.Load();
            DoodadTypes.Load(fullMapPath);
            AbilityTypes.Load();
            BuffTypes.Load();
            UpgradeTypes.Load();
            SkinFiles.Load();

            Cameras.Load();
            Destructibles.Load();
            Regions.Load();
            Sounds.Load();
            Units.Load();

            if (watcher != null)
                watcher.Created -= Watcher_Created;

            watcher = new System.IO.FileSystemWatcher();
            watcher.Path = Path.GetDirectoryName(fullMapPath);
            watcher.EnableRaisingEvents = true;
            watcher.Created += Watcher_Created;
        }

        /// <summary>
        /// Removes all used map data that no longer exists in the map.
        /// </summary>
        /// <returns>A list of modified triggers.</returns>
        public static List<IExplorerElement> RemoveInvalidReferences()
        {
            List<IExplorerElement> modified = new List<IExplorerElement>();
            var triggers = Triggers.GetAll();
            for (int i = 0; i < triggers.Count; i++)
            {
                bool wasRemoved = ControllerTrigger.RemoveInvalidReferences(triggers[i]);
                if (wasRemoved)
                    modified.Add(triggers[i]);

                triggers[i].Notify();
            }
            var variables = Variables.GetGlobals();
            for (int i = 0; i < variables.Count; i++)
            {
                bool wasRemoved = ControllerVariable.RemoveInvalidReference(variables[i]);
                if (wasRemoved)
                    modified.Add(variables[i]);
            }

            return modified;
        }

    }
}

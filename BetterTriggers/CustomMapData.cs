using BetterTriggers.Containers;
using BetterTriggers.Models;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.War3Data;
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
        public static event Action OnSaving;

        private static System.Timers.Timer ThresholdBeforeReloadingTimer;
        private const int THRESHOLD_BEFORE_SAVING_MS = 50;
        private static bool isVanillaWESaving;

        /// <summary>
        /// Method used for detecting the vanilla WE saving the map.
        /// </summary>
        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            // this try-block is only here because of the TriggerConverter.
            try
            {
                var mapPath = Project.CurrentProject.GetFullMapPath();
                if (e.Name == Path.GetFileName(mapPath) + "Temp")
                {
                    isVanillaWESaving = true;
                    OnSaving?.Invoke();
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Method used for detecting other tools changing the map.
        /// </summary>
        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            // this try-block is only here because of the TriggerConverter.
            try
            {
                if (!isVanillaWESaving)
                {
                    string mapPath = Project.CurrentProject.GetFullMapPath();
                    bool fileIsInMap = e.FullPath.StartsWith(mapPath);
                    if (fileIsInMap)
                    {
                        if (ThresholdBeforeReloadingTimer == null)
                        {
                            ThresholdBeforeReloadingTimer = new System.Timers.Timer();
                            ThresholdBeforeReloadingTimer.AutoReset = false;
                            ThresholdBeforeReloadingTimer.Elapsed += ThresholdBeforeReloadingTimer_Elapsed;
                        }
                        ThresholdBeforeReloadingTimer.Stop();
                        ThresholdBeforeReloadingTimer.Interval = THRESHOLD_BEFORE_SAVING_MS;
                        ThresholdBeforeReloadingTimer.Start();

                        isThresholdTimerRunning = true;
                        OnSaving?.Invoke();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private static bool isThresholdTimerRunning;
        private static void ThresholdBeforeReloadingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            isThresholdTimerRunning = false;
            ThresholdBeforeReloadingTimer.Stop();
        }

        public static bool IsMapSaving(string fullMapPath = null)
        {
            if (string.IsNullOrEmpty(fullMapPath))
            {
                fullMapPath = Project.CurrentProject.GetFullMapPath();
            }

            if (Directory.Exists(fullMapPath + "Temp"))
                return true;
            else if (Directory.Exists(fullMapPath + "Backup"))
                return true;
            else if (isThresholdTimerRunning)
                return true;
            else
                return false;
        }


        public static void Load(string fullMapPath = null)
        {
            if (string.IsNullOrEmpty(fullMapPath))
            {
                fullMapPath = Project.CurrentProject.GetFullMapPath();
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

            isVanillaWESaving = false;

            if (watcher != null)
            {
                watcher.Created -= Watcher_Created;
                watcher.Changed -= Watcher_Changed;
            }

            watcher = new System.IO.FileSystemWatcher();
            watcher.Path = Path.GetDirectoryName(fullMapPath);
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
            watcher.Created += Watcher_Created;
            watcher.Changed += Watcher_Changed;
        }


        /// <summary>
        /// Removes all used map data that no longer exists in the map.
        /// Also checks for ID collisions.
        /// </summary>
        /// <returns>A list of modified triggers.</returns>
        public static List<ExplorerElement> ReloadMapData()
        {
            // Check for ID collisions
            List<Tuple<ExplorerElement, ExplorerElement>> triggersWithIdCollision = new();
            List<Tuple<ExplorerElement, ExplorerElement>> variablesWithIdCollision = new();
            List<ExplorerElement> checkedTriggers = new List<ExplorerElement>();
            List<ExplorerElement> checkedVariables = new List<ExplorerElement>();

            var triggers = Project.CurrentProject.Triggers.GetAll();
            var variables = Project.CurrentProject.Variables.GetGlobals();
            triggers.ForEach(t =>
            {
                checkedTriggers.ForEach(check =>
                {
                    if (t.GetId() == check.GetId())
                        triggersWithIdCollision.Add(new Tuple<ExplorerElement, ExplorerElement>(t, check));
                });

                checkedTriggers.Add(t);
            });
            variables.ForEach(v =>
            {
                checkedVariables.ForEach(check =>
                {
                    if (v.GetId() == check.GetId())
                        variablesWithIdCollision.Add(new Tuple<ExplorerElement, ExplorerElement>(v, check));
                });

                checkedVariables.Add(v);
            });
            if (triggersWithIdCollision.Count > 0 || variablesWithIdCollision.Count > 0)
            {
                throw new IdCollisionException(triggersWithIdCollision, variablesWithIdCollision);
            }

            Project.CurrentProject.CommandManager.Reset();
            CustomMapData.Load();
            var changed = CustomMapData.RemoveInvalidReferences();
            changed.ForEach(trig => trig.AddToUnsaved());

            return changed;
        }

        private static List<ExplorerElement> RemoveInvalidReferences()
        {
            List<ExplorerElement> modified = new List<ExplorerElement>();
            var triggers = Project.CurrentProject.Triggers.GetAll();
            for (int i = 0; i < triggers.Count; i++)
            {
                int invalidCount = Project.CurrentProject.Triggers.RemoveInvalidReferences(triggers[i]);
                if (invalidCount > 0)
                    modified.Add(triggers[i]);

                triggers[i].Notify();
            }
            var variables = Project.CurrentProject.Variables.GetGlobals();
            for (int i = 0; i < variables.Count; i++)
            {
                bool wasRemoved = Project.CurrentProject.Variables.RemoveInvalidReference(variables[i]);
                if (wasRemoved)
                    modified.Add(variables[i]);
            }

            return modified;
        }

        /// <summary>
        /// TODO: This function is hella expensive.
        /// </summary>
        /// <param name="value">Reference to map data.</param>
        /// <returns></returns>
        internal static bool ReferencedDataExists(Value value, string returnType)
        {
            if (returnType == "unitcode")
            {
                List<UnitType> unitTypes = UnitTypes.GetAll();
                for (int i = 0; i < unitTypes.Count; i++)
                {
                    if (value.value == unitTypes[i].Id)
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "unit")
            {
                var units = Units.GetAll();
                for (int i = 0; i < units.Count; i++)
                {
                    if (value.value == $"{units[i].ToString()}_{units[i].CreationNumber.ToString("D4")}")
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "destructablecode")
            {
                List<DestructibleType> destTypes = DestructibleTypes.GetAll();
                for (int i = 0; i < destTypes.Count; i++)
                {
                    if (value.value == destTypes[i].DestCode)
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "destructable")
            {
                var dests = Destructibles.GetAll();
                for (int i = 0; i < dests.Count; i++)
                {
                    if (value.value == $"{dests[i].ToString()}_{dests[i].CreationNumber.ToString("D4")}")
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "itemcode")
            {
                List<ItemType> itemTypes = ItemTypes.GetAll();
                for (int i = 0; i < itemTypes.Count; i++)
                {
                    if (value.value == itemTypes[i].ItemCode)
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "item")
            {
                List<UnitData> itemTypes = Units.GetMapItemsAll();
                for (int i = 0; i < itemTypes.Count; i++)
                {
                    if (value.value == $"{itemTypes[i].ToString()}_{itemTypes[i].CreationNumber.ToString("D4")}")
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "doodadcode")
            {
                List<DoodadType> doodadTypes = DoodadTypes.GetAll();
                for (int i = 0; i < doodadTypes.Count; i++)
                {
                    if (value.value == doodadTypes[i].DoodCode)
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "abilcode")
            {
                var abilities = AbilityTypes.GetAll();
                for (int i = 0; i < abilities.Count; i++)
                {
                    if (value.value == abilities[i].AbilCode)
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "buffcode")
            {
                var buffs = BuffTypes.GetAll();
                for (int i = 0; i < buffs.Count; i++)
                {
                    if (value.value == buffs[i].BuffCode)
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "techcode")
            {
                var tech = UpgradeTypes.GetAll();
                for (int i = 0; i < tech.Count; i++)
                {
                    if (value.value == tech[i].UpgradeCode)
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "rect")
            {
                var regions = Regions.GetAll();
                for (int i = 0; i < regions.Count; i++)
                {
                    /* The string Replace exists because values converted with 'TriggerConverter' from a map
                     * have '_' in variable references, but War3Net values have spaces ' ' in them.
                     * Same goes for 'camerasetup' below.
                     */
                    if (value.value.Replace(" ", "_") == regions[i].ToString().Replace(" ", "_"))
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "camerasetup")
            {
                var cameras = Cameras.GetAll();
                for (int i = 0; i < cameras.Count; i++)
                {
                    if (value.value.Replace(" ", "_") == cameras[i].ToString().Replace(" ", "_"))
                    {
                        return true;
                    }
                }
            }
            else
                return true;

            return false;
        }
    }
}

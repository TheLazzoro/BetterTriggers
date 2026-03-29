using BetterTriggers.Containers;
using BetterTriggers.Models;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.War3Data;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using War3Net.Build;
using War3Net.Build.Widget;

namespace BetterTriggers
{
    public class CustomMapData
    {
        private static FileSystemWatcher watcher;
        public static event Action OnSaving;

        private static System.Timers.Timer ThresholdBeforeReloadingTimer;
        private const int THRESHOLD_BEFORE_SAVING_MS = 50;
        private static bool isVanillaWESaving;

        private static Project? _project;

        /// <summary>
        /// Method used for detecting the vanilla WE saving the map.
        /// </summary>
        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            // this try-block is only here because of the TriggerConverter.
            try
            {
                var mapPath = _project.GetFullMapPath();
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
                    string mapPath = _project.GetFullMapPath();
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

        public static bool IsMapSaving(Project project, string fullMapPath = null)
        {
            if (string.IsNullOrEmpty(fullMapPath))
            {
                fullMapPath = project.GetFullMapPath();
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


        public static void Load(Project? project, string fullMapPath = null, bool isFilesystemWatcherEnabled = true)
        {
            _project = project;
            if (string.IsNullOrEmpty(fullMapPath))
            {
                fullMapPath = project.GetFullMapPath();
            }

            while (IsMapSaving(project, fullMapPath))
            {
                Thread.Sleep(1000);
            }
            project.MPQMap = Map.Open(fullMapPath);

            project.Info.Load(project);
            project.MapStrings.Load(project);
            project.UnitTypes.Load(project, fullMapPath);
            project.ItemTypes.Load(project);
            project.DestructibleTypes.Load(project);
            project.DoodadTypes.Load(project, fullMapPath);
            project.AbilityTypes.Load(project);
            project.BuffTypes.Load(project);
            project.UpgradeTypes.Load(project);
            SkinFiles.Load(project);

            project.Cameras.Load(project);
            project.Destructibles.Load(project);
            project.Regions.Load(project);
            project.Sounds.Load(project);
            project.Units.Load(project);

            isVanillaWESaving = false;

            if (isFilesystemWatcherEnabled)
            {
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
        }


        /// <summary>
        /// Removes all used map data that no longer exists in the map.
        /// Also checks for ID collisions.
        /// </summary>
        /// <returns>A list of modified triggers.</returns>
        public static List<ExplorerElement> ReloadMapData(Project project)
        {
            // Check for ID collisions
            List<Tuple<ExplorerElement, ExplorerElement>> idCollision = new();
            List<ExplorerElement> checkedTriggers = new List<ExplorerElement>();
            List<ExplorerElement> checkedVariables = new List<ExplorerElement>();
            List<ExplorerElement> checkedActionDefs = new List<ExplorerElement>();
            List<ExplorerElement> checkedConditionDefs = new List<ExplorerElement>();
            List<ExplorerElement> checkedFunctionDefs = new List<ExplorerElement>();

            var triggers = project.Triggers.GetAll();
            var variables = project.Variables.GetGlobals();
            var actionsDefs = project.ActionDefinitions.GetAll();
            var conditionDefs = project.ConditionDefinitions.GetAll();
            var functionDefs = project.FunctionDefinitions.GetAll();
            triggers.ForEach(t =>
            {
                checkedTriggers.ForEach(check =>
                {
                    if (t.GetId() == check.GetId())
                        idCollision.Add(new Tuple<ExplorerElement, ExplorerElement>(t, check));
                });

                checkedTriggers.Add(t);
            });
            variables.ForEach(v =>
            {
                checkedVariables.ForEach(check =>
                {
                    if (v.GetId() == check.GetId())
                        idCollision.Add(new Tuple<ExplorerElement, ExplorerElement>(v, check));
                });

                checkedVariables.Add(v);
            });
            actionsDefs.ForEach(v =>
            {
                checkedActionDefs.ForEach(check =>
                {
                    if (v.GetId() == check.GetId())
                        idCollision.Add(new Tuple<ExplorerElement, ExplorerElement>(v, check));
                });

                checkedActionDefs.Add(v);
            });
            conditionDefs.ForEach(v =>
            {
                checkedConditionDefs.ForEach(check =>
                {
                    if (v.GetId() == check.GetId())
                        idCollision.Add(new Tuple<ExplorerElement, ExplorerElement>(v, check));
                });

                checkedConditionDefs.Add(v);
            });
            functionDefs.ForEach(v =>
            {
                checkedFunctionDefs.ForEach(check =>
                {
                    if (v.GetId() == check.GetId())
                        idCollision.Add(new Tuple<ExplorerElement, ExplorerElement>(v, check));
                });

                checkedFunctionDefs.Add(v);
            });

            if (idCollision.Count > 0)
            {
                throw new IdCollisionException(idCollision);
            }

            project.CommandManager.Reset();
            CustomMapData.Load(project);
            var changed = CustomMapData.RemoveInvalidReferences(project);
            changed.ForEach(trig => trig.AddToUnsaved());

            return changed;
        }

        private static List<ExplorerElement> RemoveInvalidReferences(Project project)
        {
            List<ExplorerElement> modified = new List<ExplorerElement>();
            var explorerElements = project.GetAllExplorerElements();
            for (int i = 0; i < explorerElements.Count; i++)
            {
                var explorerElement = explorerElements[i];
                TriggerValidator validator = new TriggerValidator(_project, explorerElement);
                int invalidCount = validator.RemoveInvalidReferences();
                if (invalidCount > 0)
                    modified.Add(explorerElement);

                if (explorerElement.ElementType != ExplorerElementEnum.Script)
                    explorerElement.Notify();
            }
            var variables = project.Variables.GetGlobals();
            for (int i = 0; i < variables.Count; i++)
            {
                bool wasRemoved = project.Variables.RemoveInvalidReference(variables[i]);
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
        internal static bool ReferencedDataExists(Project project, Value value, string returnType)
        {
            if (returnType == "unitcode")
            {
                List<UnitType> unitTypes = project.UnitTypes.GetAll();
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
                var units = project.Units.GetAll();
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
                List<DestructibleType> destTypes = project.DestructibleTypes.GetAll();
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
                var dests = project.Destructibles.GetAll();
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
                List<ItemType> itemTypes = project.ItemTypes.GetAll();
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
                List<UnitData> itemTypes = project.Units.GetMapItemsAll();
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
                List<DoodadType> doodadTypes = project.DoodadTypes.GetAll();
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
                var abilities = project.AbilityTypes.GetAll();
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
                var buffs = project.BuffTypes.GetAll();
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
                var tech = project.UpgradeTypes.GetAll();
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
                var regions = project.Regions.GetAll();
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
                var cameras = project.Cameras.GetAll();
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

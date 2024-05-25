using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BetterTriggers.Utility;
using Newtonsoft.Json;
using System.IO;
using BetterTriggers.Commands;
using BetterTriggers.WorldEdit;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace BetterTriggers.Containers
{
    public class Triggers
    {
        private HashSet<ExplorerElement> triggerElementContainer = new HashSet<ExplorerElement>();
        private ExplorerElement lastCreated;

        public void AddTrigger(ExplorerElement trigger)
        {
            triggerElementContainer.Add(trigger);
            lastCreated = trigger;
        }

        public int Count()
        {
            return triggerElementContainer.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public bool Contains(string name)
        {
            bool found = false;

            foreach (var item in triggerElementContainer)
            {
                if (item.GetName().ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
                {
                    found = true;
                }
            }

            return found;
        }

        public bool Contains(int id)
        {
            bool found = false;
            foreach (var item in triggerElementContainer)
            {
                if (item.trigger.Id == id)
                {
                    found = true;
                }
            }
            return found;
        }

        public string GetName(int triggerId)
        {
            var element = GetById(triggerId);
            return element.GetName();
        }

        public ExplorerElement GetById(int id)
        {
            ExplorerElement trigger = null;
            var enumerator = triggerElementContainer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.trigger.Id == id)
                {
                    trigger = enumerator.Current;
                    break;
                }
            }

            return trigger;
        }

        public ExplorerElement GetLastCreated()
        {
            return lastCreated;
        }

        public List<ExplorerElement> GetAll()
        {
            return triggerElementContainer.Select(x => x).ToList();
        }

        public void Remove(ExplorerElement explorerElement)
        {
            triggerElementContainer.Remove(explorerElement);
        }

        public ExplorerElement GetByReference(TriggerRef triggerRef)
        {
            return GetById(triggerRef.TriggerId);
        }

        /// <returns>Full file path.</returns>
        public string Create()
        {
            var project = Project.CurrentProject;
            string directory = project.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = GenerateTriggerName();

            Trigger_Saveable trigger = new Trigger_Saveable()
            {
                Id = project.GenerateId(),
            };
            string json = JsonConvert.SerializeObject(trigger);

            string fullPath = Path.Combine(directory, name);
            File.WriteAllText(fullPath, json);

            return fullPath;
        }

        internal string GenerateTriggerName(string name = "Untitled Trigger")
        {
            string generatedName = name;
            bool ok = false;
            int i = 0;
            while (!ok)
            {
                if (!Contains(generatedName))
                    ok = true;
                else
                {
                    generatedName = name + i;
                }

                i++;
            }

            return generatedName + ".trg";
        }

        /// <summary>
        /// Creates a list of saveable trigger refs
        /// </summary>
        public List<TriggerRef> GetTriggerRefs()
        {
            List<ExplorerElement> elements = GetAll();
            List<TriggerRef> list = new List<TriggerRef>();

            for (int i = 0; i < elements.Count; i++)
            {
                TriggerRef trigRef = new TriggerRef()
                {
                    TriggerId = elements[i].trigger.Id,
                };

                list.Add(trigRef);
            }

            return list;
        }

        public string GetValueName(string key, string returnType)
        {
            string text = key;
            switch (returnType)
            {
                case "unit":
                    text = $"{UnitTypes.GetName(key.Substring(0, 4))} {key.Substring(5, key.Length - 5)} <gen>";
                    break;
                case "item":
                    text = $"{ItemTypes.GetName(key.Substring(0, 4))} {key.Substring(5, key.Length - 5)} <gen>";
                    break;
                case "destructable":
                    text = $"{DestructibleTypes.GetName(key.Substring(0, 4))} {key.Substring(5, key.Length - 5)} <gen>";
                    break;
                case "camerasetup":
                    text = $"{key} <gen>";
                    break;
                case "rect":
                    text = $"{key} <gen>";
                    break;
                case "unitcode":
                    text = UnitTypes.GetName(key);
                    break;
                case "destructablecode":
                    text = DestructibleTypes.GetName(key);
                    break;
                case "abilcode":
                    text = AbilityTypes.GetName(key);
                    break;
                case "buffcode":
                    text = BuffTypes.GetName(key);
                    break;
                case "techcode":
                    text = UpgradeTypes.GetName(key);
                    break;
                case "itemcode":
                    text = ItemTypes.GetName(key);
                    break;
                case "doodadcode":
                    text = DoodadTypes.GetName(key);
                    break;
                case "string":
                case "StringExt":
                    if (key == string.Empty)
                        text = "<Empty String>";
                    break;
                case "frameevents":
                    text = "Events...";
                    break;
                default:
                    break;
            }

            if (text == null)
                text = string.Empty;

            return text;
        }

        public static string GetFourCCDisplay(string key, string returnType)
        {
            string text = string.Empty;
            if (returnType == "unitcode")
                text = $"[{key}] ";
            else if (returnType == "destructablecode")
                text = $"[{key}] ";
            else if (returnType == "abilcode")
                text = $"[{key}] ";
            else if (returnType == "buffcode")
                text = $"[{key}] ";
            else if (returnType == "techcode")
                text = $"[{key}] ";
            else if (returnType == "itemcode")
                text = $"[{key}] ";
            else if (returnType == "doodadcode")
                text = $"[{key}] ";

            return text;
        }
    }
}

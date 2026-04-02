using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.EditorData.TriggerEditor;
using BetterTriggers.Models.SaveableData;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BetterTriggers.Containers
{
    public class ActionDefinitions
    {
        internal HashSet<ExplorerElement> container = new();
        private ExplorerElement lastCreated;
        private Project _project;

        public ActionDefinitions(Project project)
        {
            _project = project;
        }

        private static object _lock = new object();
        public void Add(ExplorerElement actionDefinition)
        {
            lock (_lock)
            {
                container.Add(actionDefinition);
                lastCreated = actionDefinition;
            }
        }

        /// <returns>Full file path.</returns>
        public string Create()
        {
            string directory = _project.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = GenerateActionDefName();

            var actionDef = new ActionDefinition_Saveable()
            {
                Id = _project.GenerateId(),
            };
            string json = JsonConvert.SerializeObject(actionDef);

            string fullPath = Path.Combine(directory, name);
            File.WriteAllText(fullPath, json);

            return fullPath;
        }


        internal string GenerateActionDefName(string name = "Untitled Action Definition")
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

            return generatedName + ".act";
        }

        public bool Contains(int id)
        {
            bool found = false;
            foreach (var item in container)
            {
                if (item.actionDefinition.Id == id)
                {
                    found = true;
                }
            }
            return found;
        }

        public int Count()
        {
            return container.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public bool Contains(string name)
        {
            bool found = false;

            foreach (var item in container)
            {
                if (item.GetName().ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
                {
                    found = true;
                }
            }

            return found;
        }

        public ActionDefinition? GetByKey(string name)
        {
            ActionDefinition actionDefinition = null;
            foreach (var item in container)
            {
                if (item.GetName().ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
                {
                    actionDefinition = item.actionDefinition;
                    break;
                }
            }
            return actionDefinition;
        }

        public ExplorerElement? FindByRef(ActionDefinitionRef actionDefRef)
        {
            return FindById(actionDefRef.ActionDefinitionId);
        }

        public ExplorerElement? FindById(int id)
        {
            ExplorerElement actionDefinition = null;
            var enumerator = container.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.actionDefinition.Id == id)
                {
                    actionDefinition = enumerator.Current;
                    break;
                }
            }

            return actionDefinition;
        }

        public ExplorerElement GetLastCreated()
        {
            return lastCreated;
        }

        internal List<ExplorerElement> GetAll()
        {
            return container.Select(x => x).ToList();
        }

        public void Remove(ExplorerElement explorerElement)
        {
            lock (_lock)
            {
                container.Remove(explorerElement);
            }
        }

        internal ExplorerElement GetByReference(ActionDefinitionRef actionDefinitionRef)
        {
            return FindById(actionDefinitionRef.ActionDefinitionId);
        }

    }
}
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BetterTriggers.Utility;
using Newtonsoft.Json;
using System.IO;
using BetterTriggers.Models.EditorData.TriggerEditor;

namespace BetterTriggers.Containers
{
    public class ConditionDefinitions
    {
        internal Dictionary<string, ExplorerElement> container = new();
        private ExplorerElement lastCreated;

        public void Add(ExplorerElement conditionDefinition)
        {
            container.Add(conditionDefinition.GetName(), conditionDefinition);
            lastCreated = conditionDefinition;
        }

        /// <returns>Full file path.</returns>
        public string Create()
        {
            string directory = Project.CurrentProject.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = GenerateConditionDefName();

            var conditionDef = new ConditionDefinition_Saveable()
            {
                Id = GenerateId(),
            };
            string json = JsonConvert.SerializeObject(conditionDef);

            string fullPath = Path.Combine(directory, name);
            File.WriteAllText(fullPath, json);

            return fullPath;
        }

        internal string GenerateConditionDefName(string name = "Untitled Condition Definition")
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

            return generatedName + ".cond";
        }

        public int GenerateId()
        {
            int generatedId = 0;
            bool isIdValid = false;
            while (!isIdValid)
            {
                bool doesIdExist = false;
                var enumerator = container.GetEnumerator();
                while (!doesIdExist && enumerator.MoveNext())
                {
                    if (enumerator.Current.Value.conditionDefinition.Id == generatedId)
                        doesIdExist = true;
                }

                if (!doesIdExist)
                    isIdValid = true;
                else
                    generatedId = RandomUtil.GenerateInt();
            }

            return generatedId;
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
                if (item.Value.GetName().ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
                {
                    found = true;
                }
            }

            return found;
        }

        internal ConditionDefinition GetByKey(string name)
        {
            container.TryGetValue(name, out var result);
            return result.conditionDefinition;
        }

        internal string GetName(int id)
        {
            var element = FindById(id);
            return element.GetName();
        }

        public ExplorerElement? FindByRef(ConditionDefinitionRef conditionDefRef)
        {
            return FindById(conditionDefRef.ConditionDefinitionId);
        }

        public ExplorerElement? FindById(int id)
        {
            ExplorerElement conditionDefinition = null;
            var enumerator = container.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value.conditionDefinition.Id == id)
                {
                    conditionDefinition = enumerator.Current.Value;
                    break;
                }
            }

            return conditionDefinition;
        }

        public ExplorerElement GetLastCreated()
        {
            return lastCreated;
        }

        internal List<ExplorerElement> GetAll()
        {
            return container.Select(x => x.Value).ToList();
        }

        public void Remove(ExplorerElement explorerElement)
        {
            container.Remove(explorerElement.GetName());
        }

        internal ExplorerElement GetByReference(ConditionDefinition conditionDefinitionRef)
        {
            return FindById(conditionDefinitionRef.Id);
        }

        internal void Clear()
        {
            container.Clear();
        }

    }
}
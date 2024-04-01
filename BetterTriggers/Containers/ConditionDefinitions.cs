using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BetterTriggers.Utility;
using Newtonsoft.Json;
using System.IO;

namespace BetterTriggers.Containers
{
    public class ConditionDefinitions
    {
        private static HashSet<ExplorerElement> conditionDefinitionContainer = new();
        private static ExplorerElement lastCreated;

        public void Add(ExplorerElement conditionDefinition)
        {
            conditionDefinitionContainer.Add(conditionDefinition);
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
                var enumerator = conditionDefinitionContainer.GetEnumerator();
                while (!doesIdExist && enumerator.MoveNext())
                {
                    if (enumerator.Current.conditionDefinition.Id == generatedId)
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
            return conditionDefinitionContainer.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public bool Contains(string name)
        {
            bool found = false;

            foreach (var item in conditionDefinitionContainer)
            {
                if (item.GetName().ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
                {
                    found = true;
                }
            }

            return found;
        }

        internal string GetName(int id)
        {
            var element = FindById(id);
            return element.GetName();
        }

        public ExplorerElement FindById(int id)
        {
            ExplorerElement conditionDefinition = null;
            var enumerator = conditionDefinitionContainer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.actionDefinition.Id == id)
                {
                    conditionDefinition = enumerator.Current;
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
            return conditionDefinitionContainer.Select(x => x).ToList();
        }

        public void Remove(ExplorerElement explorerElement)
        {
            conditionDefinitionContainer.Remove(explorerElement);
        }

        internal ExplorerElement GetByReference(ConditionDefinition conditionDefinitionRef)
        {
            return FindById(conditionDefinitionRef.Id);
        }

        internal void Clear()
        {
            conditionDefinitionContainer.Clear();
        }
    }
}
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
    public class FunctionDefinitions
    {
        internal Dictionary<string, ExplorerElement> container = new();
        private ExplorerElement lastCreated;

        public void Add(ExplorerElement functionDefinition)
        {
            container.Add(functionDefinition.GetName(), functionDefinition);
            lastCreated = functionDefinition;
        }

        /// <returns>Full file path.</returns>
        public string Create()
        {
            string directory = Project.CurrentProject.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = GenerateFunctionDefName();
            var returnStatement = new ReturnStatement_Saveable();
            returnStatement.function.parameters.Add(new Value_Saveable()
            {
                value = "0",
            });
            var functionDef = new FunctionDefinition_Saveable()
            {
                Id = GenerateId(),
                Actions = new List<TriggerElement_Saveable>()
                {
                    returnStatement,
                }
            };
            string json = JsonConvert.SerializeObject(functionDef);

            string fullPath = Path.Combine(directory, name);
            File.WriteAllText(fullPath, json);

            return fullPath;
        }

        internal string GenerateFunctionDefName(string name = "Untitled Function Definition")
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

            return generatedName + ".func";
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
                    if (enumerator.Current.Value.functionDefinition.Id == generatedId)
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

        internal string GetName(int id)
        {
            var element = FindById(id);
            return element.GetName();
        }

        public ExplorerElement FindById(int id)
        {
            ExplorerElement functionDefinition = null;
            var enumerator = container.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value.functionDefinition.Id == id)
                {
                    functionDefinition = enumerator.Current.Value;
                    break;
                }
            }

            return functionDefinition;
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

        internal ExplorerElement GetByReference(FunctionDefinitionRef functionDefinitionRef)
        {
            return FindById(functionDefinitionRef.FunctionDefinitionId);
        }

        internal void Clear()
        {
            container.Clear();
        }
    }
}
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
        internal HashSet<ExplorerElement> container = new();
        private ExplorerElement lastCreated;

        public void Add(ExplorerElement functionDefinition)
        {
            container.Add(functionDefinition);
            lastCreated = functionDefinition;
        }

        /// <returns>Full file path.</returns>
        public string Create()
        {
            var project = Project.CurrentProject;
            string directory = project.currentSelectedElement;
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
                Id = project.GenerateId(),
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

        public int Count()
        {
            return container.Count;
        }

        public bool Contains(int id)
        {
            bool found = false;

            foreach (var item in container)
            {
                if (item.functionDefinition.Id == id)
                {
                    found = true;
                }
            }

            return found;
        }

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

        public ExplorerElement FindById(int id)
        {
            ExplorerElement functionDefinition = null;
            var enumerator = container.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.functionDefinition.Id == id)
                {
                    functionDefinition = enumerator.Current;
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
            return container.Select(x => x).ToList();
        }

        public void Remove(ExplorerElement explorerElement)
        {
            container.Remove(explorerElement);
        }

        internal ExplorerElement GetByReference(FunctionDefinitionRef functionDefinitionRef)
        {
            return FindById(functionDefinitionRef.FunctionDefinitionId);
        }

        internal void Clear()
        {
            container.Clear();
        }

        internal FunctionDefinition FindByName(string name)
        {
            FunctionDefinition functionDefinition = null;
            var enumerator = container.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.GetName() == name)
                {
                    functionDefinition = enumerator.Current.functionDefinition;
                    break;
                }
            }

            return functionDefinition;
        }
    }
}
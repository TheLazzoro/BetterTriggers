﻿using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BetterTriggers.Utility;
using Newtonsoft.Json;
using System.IO;
using BetterTriggers.Models.EditorData.TriggerEditor;
using ICSharpCode.Decompiler.CSharp.Syntax;

namespace BetterTriggers.Containers
{
    public class ConditionDefinitions
    {
        internal HashSet<ExplorerElement> container = new();
        private ExplorerElement lastCreated;

        public void Add(ExplorerElement conditionDefinition)
        {
            container.Add(conditionDefinition);
            lastCreated = conditionDefinition;
        }

        /// <returns>Full file path.</returns>
        public string Create()
        {
            var project = Project.CurrentProject;
            string directory = project.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = GenerateConditionDefName();
            var returnStatement = new ReturnStatement_Saveable();
            returnStatement.function.parameters.Add(new Value_Saveable()
            {
                value = "true"
            });
            var conditionDef = new ConditionDefinition_Saveable()
            {
                Id = project.GenerateId(),
                Actions = new List<TriggerElement_Saveable>()
                {
                    returnStatement,
                }
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

        public int Count()
        {
            return container.Count;
        }

        public bool Contains(int id)
        {
            bool found = false;

            foreach (var item in container)
            {
                if (item.conditionDefinition.Id == id)
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

        internal ConditionDefinition? GetByKey(string name)
        {
            ConditionDefinition conditionDefinition = null;
            foreach (var item in container)
            {
                if (item.GetName().ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
                {
                    conditionDefinition = item.conditionDefinition;
                    break;
                }
            }
            return conditionDefinition;
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
                if (enumerator.Current.conditionDefinition.Id == id)
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
            return container.Select(x => x).ToList();
        }

        public void Remove(ExplorerElement explorerElement)
        {
            container.Remove(explorerElement);
        }

        internal ExplorerElement GetByReference(ConditionDefinitionRef conditionDefinitionRef)
        {
            return FindById(conditionDefinitionRef.ConditionDefinitionId);
        }

    }
}
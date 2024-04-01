using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BetterTriggers.Utility;

namespace BetterTriggers.Containers
{
    public class FunctionDefinitions
    {
        private static HashSet<ExplorerElement> functionDefinitionContainer = new();
        private static ExplorerElement lastCreated;

        public void Add(ExplorerElement functionDefinition)
        {
            functionDefinitionContainer.Add(functionDefinition);
            lastCreated = functionDefinition;
        }

        public int GenerateId()
        {
            int generatedId = 0;
            bool isIdValid = false;
            while (!isIdValid)
            {
                bool doesIdExist = false;
                var enumerator = functionDefinitionContainer.GetEnumerator();
                while (!doesIdExist && enumerator.MoveNext())
                {
                    if (enumerator.Current.functionDefinition.Id == generatedId)
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
            return functionDefinitionContainer.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public bool Contains(string name)
        {
            bool found = false;

            foreach (var item in functionDefinitionContainer)
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
            ExplorerElement functionDefinition = null;
            var enumerator = functionDefinitionContainer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.actionDefinition.Id == id)
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
            return functionDefinitionContainer.Select(x => x).ToList();
        }

        public void Remove(ExplorerElement explorerElement)
        {
            functionDefinitionContainer.Remove(explorerElement);
        }

        internal ExplorerElement GetByReference(ActionDefinition functionDefinitionRef)
        {
            return FindById(functionDefinitionRef.Id);
        }

        internal void Clear()
        {
            functionDefinitionContainer.Clear();
        }
    }
}
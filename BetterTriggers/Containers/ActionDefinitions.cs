using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BetterTriggers.Utility;

namespace BetterTriggers.Containers
{
    public static class ActionDefinitions
    {
        private static HashSet<ExplorerElementActionDefinition> actionDefinitionContainer = new();
        private static ExplorerElementActionDefinition lastCreated;

        public static void Add(ExplorerElementActionDefinition actionDefinition)
        {
            actionDefinitionContainer.Add(actionDefinition);
            lastCreated = actionDefinition;
        }

        public static int GenerateId()
        {
            int generatedId = 0;
            bool isIdValid = false;
            while (!isIdValid)
            {
                bool doesIdExist = false;
                var enumerator = actionDefinitionContainer.GetEnumerator();
                while (!doesIdExist && enumerator.MoveNext())
                {
                    if (enumerator.Current.actionDefinition.Id == generatedId)
                        doesIdExist = true;
                }

                if (!doesIdExist)
                    isIdValid = true;
                else
                    generatedId = RandomUtil.GenerateInt();
            }

            return generatedId;
        }

        public static int Count()
        {
            return actionDefinitionContainer.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public static bool Contains(string name)
        {
            bool found = false;

            foreach (var item in actionDefinitionContainer)
            {
                if (item.GetName().ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
                {
                    found = true;
                }
            }

            return found;
        }

        internal static string GetName(int id)
        {
            var element = FindById(id);
            return element.GetName();
        }

        public static ExplorerElementActionDefinition FindById(int id)
        {
            ExplorerElementActionDefinition actionDefinition = null;
            var enumerator = actionDefinitionContainer.GetEnumerator();
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

        public static ExplorerElementActionDefinition GetLastCreated()
        {
            return lastCreated;
        }

        internal static List<ExplorerElementActionDefinition> GetAll()
        {
            return actionDefinitionContainer.Select(x => x).ToList();
        }

        public static void Remove(ExplorerElementActionDefinition explorerElement)
        {
            actionDefinitionContainer.Remove(explorerElement);
        }

        internal static ExplorerElementActionDefinition GetByReference(ActionDefinitionRef actionDefinitionRef)
        {
            return FindById(actionDefinitionRef.ActionDefinitionId);
        }

        internal static void Clear()
        {
            actionDefinitionContainer.Clear();
        }
    }
}

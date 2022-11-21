using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetterTriggers.Containers
{
    public static class Triggers
    {
        private static List<ExplorerElementTrigger> triggerElementContainer = new List<ExplorerElementTrigger>();

        public static void AddTrigger(ExplorerElementTrigger trigger)
        {
            triggerElementContainer.Add(trigger);
        }

        public static int GenerateId()
        {
            int generatedId = 0;
            bool isIdValid = false;
            while (!isIdValid)
            {
                int i = 0;
                bool doesIdExist = false;
                while (!doesIdExist && i < triggerElementContainer.Count)
                {
                    if (triggerElementContainer[i].trigger.Id == generatedId)
                        doesIdExist = true;
                    else
                        i++;
                }

                if (!doesIdExist)
                    isIdValid = true;
                else
                    generatedId++;
            }

            return generatedId;
        }

        public static int Count()
        {
            return triggerElementContainer.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public static bool Contains(string name)
        {
            bool found = false;

            foreach (var item in triggerElementContainer)
            {
                if(item.GetName().ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
                {
                    found = true;
                }
            }

            return found;
        }

        internal static string GetName(int triggerId)
        {
            var element = FindById(triggerId);
            return element.GetName();
        }

        public static ExplorerElementTrigger FindById(int id)
        {
            ExplorerElementTrigger trigger = null;
            for (int i = 0; i < triggerElementContainer.Count; i++)
            {
                if(triggerElementContainer[i].trigger.Id == id)
                {
                    trigger = triggerElementContainer[i];
                    break;
                }
            }

            return trigger;
        }

        public static ExplorerElementTrigger GetLastCreated()
        {
            return triggerElementContainer[triggerElementContainer.Count - 1];
        }

        internal static List<ExplorerElementTrigger> GetAll()
        {
            return triggerElementContainer;
        }

        public static void Remove(ExplorerElementTrigger explorerElement)
        {
            triggerElementContainer.Remove(explorerElement);
        }

        internal static ExplorerElementTrigger GetByReference(TriggerRef triggerRef)
        {
            return FindById(triggerRef.TriggerId);
        }

        internal static void Clear()
        {
            triggerElementContainer.Clear();
        }
    }
}

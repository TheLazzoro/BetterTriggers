using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BetterTriggers.Utility;

namespace BetterTriggers.Containers
{
    public static class Triggers
    {
        private static HashSet<ExplorerElementTrigger> triggerElementContainer = new HashSet<ExplorerElementTrigger>();
        private static ExplorerElementTrigger lastCreated;

        public static void AddTrigger(ExplorerElementTrigger trigger)
        {
            triggerElementContainer.Add(trigger);
            lastCreated = trigger;
        }

        public static int GenerateId()
        {
            int generatedId = 0;
            bool isIdValid = false;
            while (!isIdValid)
            {
                bool doesIdExist = false;
                var enumerator = triggerElementContainer.GetEnumerator();
                while (!doesIdExist && enumerator.MoveNext())
                {
                    if (enumerator.Current.trigger.Id == generatedId)
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
                if (item.GetName().ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
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

        public static ExplorerElementTrigger GetLastCreated()
        {
            return lastCreated;
        }

        internal static List<ExplorerElementTrigger> GetAll()
        {
            return triggerElementContainer.Select(x => x).ToList();
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

using Model.EditorData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Facades.Containers
{
    public static class ContainerTriggers
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
                if(item.GetName() == name)
                {
                    found = true;
                }
            }

            return found;
        }

        public static ExplorerElementTrigger Get(int index)
        {
            return triggerElementContainer[index];
        }

        public static void Remove(ExplorerElementTrigger explorerElement)
        {
            triggerElementContainer.Remove(explorerElement);
        }

        public static void RemoveByFilePath(string filePath)
        {
            for( int i = 0; i < triggerElementContainer.Count; i++)
            {
                var item = triggerElementContainer[i];
                if (item.GetPath() == filePath)
                {
                    triggerElementContainer.Remove(item);
                }
            }
        }
    }
}

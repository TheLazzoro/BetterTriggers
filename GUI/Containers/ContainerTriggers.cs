using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerTriggers
    {
        private static List<ExplorerTrigger> triggerElementContainer = new List<ExplorerTrigger>();


        public static void AddTriggerElement(ExplorerTrigger triggerElement)
        {
            triggerElementContainer.Add(triggerElement);
        }

        public static int Count()
        {
            return triggerElementContainer.Count;
        }

        public static ExplorerTrigger Get(int index)
        {
            return triggerElementContainer[index];
        }
    }
}

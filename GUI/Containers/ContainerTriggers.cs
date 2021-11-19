using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerTriggers
    {
        private static List<Trigger> triggerElementContainer = new List<Trigger>();


        public static void AddTriggerElement(Trigger triggerElement)
        {
            triggerElementContainer.Add(triggerElement);
        }

        public static int Count()
        {
            return triggerElementContainer.Count;
        }

        public static Trigger Get(int index)
        {
            return triggerElementContainer[index];
        }
    }
}

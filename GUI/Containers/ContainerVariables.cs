using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerVariables
    {
        private static List<ExplorerElement> triggerElementContainer = new List<ExplorerElement>();

        public static void AddTriggerElement(ExplorerElement variable)
        {
            triggerElementContainer.Add(variable);
        }

        public static int Count()
        {
            return triggerElementContainer.Count;
        }

        public static ExplorerElement Get(int index)
        {
            return triggerElementContainer[index];
        }
    }
}

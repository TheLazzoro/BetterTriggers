using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerScripts
    {
        private static List<ExplorerScript> scriptContainer = new List<ExplorerScript>();

        public static void AddTriggerElement(ExplorerScript triggerElement)
        {
            scriptContainer.Add(triggerElement);
        }

        public static int Count()
        {
            return scriptContainer.Count;
        }

        public static ExplorerScript Get(int index)
        {
            return scriptContainer[index];
        }
    }
}

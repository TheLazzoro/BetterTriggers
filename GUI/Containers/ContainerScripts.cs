using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerScripts
    {
        private static List<ExplorerElement> scriptContainer = new List<ExplorerElement>();

        public static void AddTriggerElement(ExplorerElement script)
        {
            scriptContainer.Add(script);
        }

        public static int Count()
        {
            return scriptContainer.Count;
        }

        public static ExplorerElement Get(int index)
        {
            return scriptContainer[index];
        }
    }
}

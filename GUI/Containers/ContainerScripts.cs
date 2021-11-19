using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerScripts
    {
        private static List<Script> scriptContainer = new List<Script>();

        public static void AddTriggerElement(Script triggerElement)
        {
            scriptContainer.Add(triggerElement);
        }

        public static int Count()
        {
            return scriptContainer.Count;
        }

        public static Script Get(int index)
        {
            return scriptContainer[index];
        }
    }
}

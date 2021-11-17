using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerScripts
    {
        private static List<Script> triggerElementContainer = new List<Script>();

        public static void AddTriggerElement(Script triggerElement)
        {
            triggerElementContainer.Add(triggerElement);
        }
    }
}

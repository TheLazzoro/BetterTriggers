using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerVariables
    {
        private static List<Variable> triggerElementContainer = new List<Variable>();

        public static void AddTriggerElement(Variable triggerElement)
        {
            triggerElementContainer.Add(triggerElement);
        }
    }
}

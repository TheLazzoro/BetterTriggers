using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerConditionDefinitions
    {
        private static List<ConditionDefinition> triggerElementContainer = new List<ConditionDefinition>();

        public static void AddTriggerElement(ConditionDefinition triggerElement)
        {
            triggerElementContainer.Add(triggerElement);
        }
    }
}

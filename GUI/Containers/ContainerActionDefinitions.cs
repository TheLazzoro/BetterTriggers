using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerActionDefinitions
    {
        private static List<ActionDefinition> actionDefinitionContainer = new List<ActionDefinition>();

        public static void AddTriggerElement(ActionDefinition triggerElement)
        {
            actionDefinitionContainer.Add(triggerElement);
        }

        public static int Count()
        {
            return actionDefinitionContainer.Count;
        }

        public static ActionDefinition Get(int index)
        {
            return actionDefinitionContainer[index];
        }
    }
}

using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerConditionDefinitions
    {
        private static List<ConditionDefinition> conditionDefinitionContainer = new List<ConditionDefinition>();

        public static void AddTriggerElement(ConditionDefinition triggerElement)
        {
            conditionDefinitionContainer.Add(triggerElement);
        }

        public static int Count()
        {
            return conditionDefinitionContainer.Count;
        }

        public static ConditionDefinition Get(int index)
        {
            return conditionDefinitionContainer[index];
        }
    }
}

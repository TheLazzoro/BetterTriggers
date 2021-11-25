using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerITriggerElements
    {
        private static List<ITriggerExplorerElement> ITriggerElements = new List<ITriggerExplorerElement>();

        public static void AddTriggerElement(ITriggerExplorerElement triggerElement)
        {
            ITriggerElements.Add(triggerElement);
        }

        public static int Count()
        {
            return ITriggerElements.Count;
        }

        public static string GenerateScript()
        {
            string finalScript = string.Empty;

            for(int i = 0; i < ITriggerElements.Count; i++)
            {
                var script = ITriggerElements[i].GetScript();
                finalScript += $"{script} \n\n";
            }

            return finalScript;
        }
    }
}

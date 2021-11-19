using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerITriggerElements
    {
        private static List<ITriggerElement> ITriggerElements = new List<ITriggerElement>();

        public static void AddTriggerElement(ITriggerElement triggerElement)
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

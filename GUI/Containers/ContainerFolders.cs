using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerFolders
    {
        private static List<TriggerFolder> folderContainer = new List<TriggerFolder>();

        public static void AddTriggerElement(TriggerFolder triggerElement)
        {
            folderContainer.Add(triggerElement);
        }

        public static int Count()
        {
            return folderContainer.Count;
        }

        public static TriggerFolder Get(int index)
        {
            return folderContainer[index];
        }
    }
}

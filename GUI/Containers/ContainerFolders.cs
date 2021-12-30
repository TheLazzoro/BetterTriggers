using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerFolders
    {
        private static List<ExplorerElement> folderContainer = new List<ExplorerElement>();

        public static void AddTriggerElement(ExplorerElement trigger)
        {
            folderContainer.Add(trigger);
        }

        public static int Count()
        {
            return folderContainer.Count;
        }

        public static ExplorerElement Get(int index)
        {
            return folderContainer[index];
        }
    }
}

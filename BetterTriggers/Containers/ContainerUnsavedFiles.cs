using Model.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Containers
{
    internal static class ContainerUnsavedFiles
    {
        private static List<IExplorerElement> UnsavedFiles = new List<IExplorerElement>();

        internal static void AddToUnsaved(IExplorerElement element)
        {
            if (UnsavedFiles.Contains(element))
                return;

            UnsavedFiles.Add(element);
        }

        internal static void RemoveFromUnsaved(IExplorerElement element)
        {
            UnsavedFiles.Remove(element);
        }

        internal static List<IExplorerElement> GetAllUnsaved()
        {
            return UnsavedFiles;
        }

        internal static int Count()
        {
            return UnsavedFiles.Count;
        }

        internal static void Clear()
        {
            UnsavedFiles.Clear();
        }
    }
}

using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Containers
{
    internal static class UnsavedFiles
    {
        private static List<IExplorerElement> unsavedFiles = new List<IExplorerElement>();

        internal static void AddToUnsaved(IExplorerElement element)
        {
            if (unsavedFiles.Contains(element))
                return;

            unsavedFiles.Add(element);
        }

        internal static void RemoveFromUnsaved(IExplorerElement element)
        {
            unsavedFiles.Remove(element);
        }

        internal static List<IExplorerElement> GetAllUnsaved()
        {
            return unsavedFiles;
        }

        internal static int Count()
        {
            return unsavedFiles.Count;
        }

        internal static void Clear()
        {
            unsavedFiles.Clear();
        }
    }
}

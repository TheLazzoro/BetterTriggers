using BetterTriggers.Models.EditorData;
using System.Collections.Generic;

namespace BetterTriggers.Containers
{
    public class UnsavedFiles
    {
        private HashSet<ExplorerElement> unsavedFiles = new();

        public void AddToUnsaved(ExplorerElement element)
        {
            unsavedFiles.Add(element);
        }

        public void RemoveFromUnsaved(ExplorerElement element)
        {
            unsavedFiles.Remove(element);
        }

        public void SaveAll()
        {
            foreach (ExplorerElement element in unsavedFiles)
            {
                element.Save();
            }
            unsavedFiles.Clear();
        }

        public bool Contains(ExplorerElement element)
        {
            return unsavedFiles.Contains(element);
        }

        internal int Count()
        {
            return unsavedFiles.Count;
        }
    }
}

using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal int Count()
        {
            return unsavedFiles.Count;
        }
    }
}

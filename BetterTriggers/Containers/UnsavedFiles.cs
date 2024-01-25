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
        private List<ExplorerElement> unsavedFiles = new List<ExplorerElement>();

        public void AddToUnsaved(ExplorerElement element)
        {
            if (unsavedFiles.Contains(element))
                return;

            unsavedFiles.Add(element);
            element.HasUnsavedChanges = true;
        }

        public void RemoveFromUnsaved(ExplorerElement element)
        {
            unsavedFiles.Remove(element);
            element.HasUnsavedChanges = false;
        }

        internal List<ExplorerElement> GetAllUnsaved()
        {
            return unsavedFiles;
        }

        internal int Count()
        {
            return unsavedFiles.Count;
        }

        public void Clear()
        {
            unsavedFiles.Clear();
        }
    }
}

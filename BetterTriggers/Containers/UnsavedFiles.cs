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
        private List<IExplorerElement> unsavedFiles = new List<IExplorerElement>();

        public void AddToUnsaved(IExplorerElement element)
        {
            if (unsavedFiles.Contains(element))
                return;

            unsavedFiles.Add(element);
        }

        public void RemoveFromUnsaved(IExplorerElement element)
        {
            unsavedFiles.Remove(element);
        }

        internal List<IExplorerElement> GetAllUnsaved()
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

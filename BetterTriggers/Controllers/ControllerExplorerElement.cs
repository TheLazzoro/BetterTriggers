
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Controllers
{
    public class ControllerExplorerElement
    {
        public void AddToUnsaved(IExplorerElement explorerElement)
        {
            UnsavedFiles.AddToUnsaved(explorerElement);
        }

        public void RemoveFromUnsaved(IExplorerElement explorerElement)
        {
            UnsavedFiles.RemoveFromUnsaved(explorerElement);
        }
    }
}
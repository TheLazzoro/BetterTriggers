
using BetterTriggers.Containers;
using Model.EditorData;

namespace BetterTriggers.Controllers
{
    public class ControllerExplorerElement
    {
        public void AddToUnsaved(IExplorerElement explorerElement)
        {
            ContainerUnsavedFiles.AddToUnsaved(explorerElement);
        }

        public void RemoveFromUnsaved(IExplorerElement explorerElement)
        {
            ContainerUnsavedFiles.RemoveFromUnsaved(explorerElement);
        }

        public void ClearUnsaved()
        {
            ContainerUnsavedFiles.Clear();
        }
    }
}
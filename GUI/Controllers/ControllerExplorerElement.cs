using GUI.Components.TriggerExplorer;
using GUI.Container;

namespace GUI.Controllers
{
    internal class ControllerExplorerElement
    {
        public void AddToUnsaved(TreeItemExplorerElement explorerElement)
        {
            ContainerUnsavedElements.UnsavedElements.Add(explorerElement);
        }

        public void RemoveFromUnsaved(TreeItemExplorerElement explorerElement)
        {
            ContainerUnsavedElements.UnsavedElements.Remove(explorerElement);
        }

        public void ClearUnsaved()
        {
            ContainerUnsavedElements.UnsavedElements.Clear();
        }
    }
}
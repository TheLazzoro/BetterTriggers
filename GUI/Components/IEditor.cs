using GUI.Components.TriggerExplorer;

namespace GUI.Components
{
    public interface IEditor
    {
        void Refresh();
        string GetSaveString();
        void OnRemoteChange();
        void Attach(TreeItemExplorerElement explorerElement);
        void Detach(TreeItemExplorerElement explorerElement);
        void OnStateChange();
    }
}
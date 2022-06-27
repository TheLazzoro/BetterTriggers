using GUI.Components.TriggerExplorer;

namespace GUI.Components
{
    public interface IEditor
    {
        void OnRemoteChange();
        void SetElementEnabled(bool isEnabled);
        void SetElementInitiallyOn(bool isInitiallyOn);
        void Attach(TreeItemExplorerElement explorerElement);
        void Detach(TreeItemExplorerElement explorerElement);
        void OnStateChange();
    }
}
namespace BetterTriggers.Models.EditorData
{
    public interface IExplorerElementUI
    {
        void UpdatePosition();
        void Reload();
        void OnCreated(int insertIndex);
        void OnSaved();
        void OnRemoteChange();
        void Delete();
        void RefreshHeader();
    }
}

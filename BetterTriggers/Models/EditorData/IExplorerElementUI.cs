namespace BetterTriggers.Models.EditorData
{
    public interface IExplorerElementUI
    {
        void UpdatePosition(string oldFullPath, string newFullPath);
        void Reload();
        void OnCreated(int insertIndex);
        void OnSaved();
        void OnRemoteChange();
        void Delete();
        void RefreshHeader();
    }
}

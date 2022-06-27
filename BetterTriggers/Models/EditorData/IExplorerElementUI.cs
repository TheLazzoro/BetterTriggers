namespace BetterTriggers.Models.EditorData
{
    public interface IExplorerElementUI
    {
        void UpdatePosition();
        void Reload(IExplorerElement subject);
        void OnCreated(int insertIndex);
        void OnSaved();
        void Delete();
    }
}

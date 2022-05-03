namespace Model.EditorData
{
    public interface IExplorerElementUI
    {
        void UpdatePosition();
        void Update(IExplorerElement subject);
        void OnCreated(int insertIndex);
        void Delete();
    }
}

namespace Model.EditorData
{
    public interface IExplorerElementObserver
    {
        // Receive update from subject
        void Update(IExplorerElement subject);
        void Delete();
    }
}

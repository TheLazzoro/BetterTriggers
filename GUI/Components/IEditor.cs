namespace GUI.Components
{
    public interface IEditor
    {
        void Refresh();
        string GetSaveString();
        void OnRemoteChange();
    }
}
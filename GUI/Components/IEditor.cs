namespace GUI.Components
{
    public interface IEditor
    {
        string GetSaveString();
        void OnRemoteChange();
    }
}
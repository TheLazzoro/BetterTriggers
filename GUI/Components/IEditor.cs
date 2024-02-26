namespace GUI.Components
{
    public interface IEditor
    {
        void SetElementEnabled(bool isEnabled);
        void SetElementInitiallyOn(bool isInitiallyOn);
        void OnStateChange();
    }
}
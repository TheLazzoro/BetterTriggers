namespace Model.SaveableData
{
    public interface ITriggerElementUI
    {
        void UpdatePosition();
        void UpdateParams();
        void UpdateEnabled();
        void OnCreated(int insertIndex);
        void OnDeleted();
    }
}

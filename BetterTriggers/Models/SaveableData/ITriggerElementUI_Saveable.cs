namespace BetterTriggers.Models.SaveableData
{
    public interface ITriggerElementUI_Saveable
    {
        void UpdatePosition();
        void UpdateParams();
        void UpdateEnabled();
        void OnCreated(int insertIndex);
        void OnDeleted();
    }
}

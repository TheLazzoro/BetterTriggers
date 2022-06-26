namespace BetterTriggers.Models.SaveableData
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

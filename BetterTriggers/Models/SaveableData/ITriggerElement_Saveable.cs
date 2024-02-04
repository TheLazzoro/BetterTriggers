using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public interface ITriggerElement_Saveable
    {
        void Attach(ITriggerElementUI_Saveable elementUI);
        void Detach(ITriggerElementUI_Saveable elementUI);
        void ChangedPosition();
        void ChangedParams();
        void ChangedEnabled();
        void Deleted();
        void Created(int insertIndex);
        void SetParent(List<ITriggerElement_Saveable> parent, int insertIndex);
        List<ITriggerElement_Saveable> GetParent();
        void RemoveFromParent();
        /// <summary>
        /// Only use this when the trigger loads the first time.
        /// Otherwise use SetParent(parent, insertIndex).
        /// </summary>
        void SetParent(List<ITriggerElement_Saveable> triggerElements);
    }
}

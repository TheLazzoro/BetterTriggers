using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public interface ITriggerElement
    {
        void Attach(ITriggerElementUI elementUI);
        void Detach(ITriggerElementUI elementUI);
        void ChangedPosition();
        void ChangedParams();
        void ChangedEnabled();
        void Deleted();
        void Created(int insertIndex);
        void SetParent(List<ITriggerElement> parent, int insertIndex);
        List<ITriggerElement> GetParent();
        void RemoveFromParent();
        /// <summary>
        /// Only use this when the trigger loads the first time.
        /// Otherwise use SetParent(parent, insertIndex).
        /// </summary>
        void SetParent(List<ITriggerElement> triggerElements);
    }
}

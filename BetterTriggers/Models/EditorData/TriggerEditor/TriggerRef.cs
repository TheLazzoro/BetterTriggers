using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class TriggerRef : Parameter
    {
        public int TriggerId;

        public override TriggerRef Clone()
        {
            TriggerRef cloned = new TriggerRef();
            string value = null;
            if (this.value != null)
                value = new string(this.value);

            cloned.value = value;
            cloned.TriggerId = TriggerId;

            return cloned;
        }
    }
}

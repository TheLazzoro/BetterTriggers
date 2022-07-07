using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class TriggerRef : Parameter
    {
        public readonly int ParamType = 4; // DO NOT CHANGE
        public int TriggerId;

        public override TriggerRef Clone()
        {
            TriggerRef cloned = new TriggerRef();
            string identifier = null;
            if (this.identifier != null)
                identifier = new string(this.identifier);

            cloned.identifier = identifier;
            cloned.TriggerId = TriggerId;

            return cloned;
        }
    }
}

using Model.JsonBaseConverter;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Model.SaveableData
{
    [JsonConverter(typeof(BaseConverter))]
    public class TriggerRef : Parameter
    {
        public readonly int ParamType = 17; // DO NOT CHANGE
        public int TriggerId;

        internal TriggerRef Clone()
        {
            TriggerRef cloned = new TriggerRef();
            string identifier = null;
            if (this.identifier != null)
                identifier = new string(this.identifier);

            cloned.identifier = identifier;
            cloned.returnType = returnType;
            cloned.TriggerId = TriggerId;

            return cloned;
        }
    }
}

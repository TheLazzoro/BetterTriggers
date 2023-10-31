using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ConditionDefinitionRef : Parameter
    {
        public readonly int ParamType = 7; // DO NOT CHANGE
        public int ActionDefinitionId;

        public override ConditionDefinitionRef Clone()
        {
            ConditionDefinitionRef cloned = new ConditionDefinitionRef();
            string value = null;
            if (this.value != null)
                value = new string(this.value);

            cloned.value = value;
            cloned.ActionDefinitionId = ActionDefinitionId;

            return cloned;
        }
    }
}

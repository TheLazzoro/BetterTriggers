using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ActionDefinitionRef : Parameter
    {
        public readonly int ParamType = 6; // DO NOT CHANGE
        public int ActionDefinitionId;

        public override ActionDefinitionRef Clone()
        {
            ActionDefinitionRef cloned = new ActionDefinitionRef();
            string value = null;
            if (this.value != null)
                value = new string(this.value);

            cloned.value = value;
            cloned.ActionDefinitionId = ActionDefinitionId;

            return cloned;
        }
    }
}

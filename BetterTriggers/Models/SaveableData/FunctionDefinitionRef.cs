using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class FunctionDefinitionRef : Parameter
    {
        public readonly int ParamType = 8; // DO NOT CHANGE
        public int ActionDefinitionId;

        public override FunctionDefinitionRef Clone()
        {
            FunctionDefinitionRef cloned = new FunctionDefinitionRef();
            string value = null;
            if (this.value != null)
                value = new string(this.value);

            cloned.value = value;
            cloned.ActionDefinitionId = ActionDefinitionId;

            return cloned;
        }
    }
}

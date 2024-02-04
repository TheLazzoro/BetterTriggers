using BetterTriggers.Models.SaveableData;
using System;

namespace BetterTriggers.Models.Templates
{
    public class ValueTemplate : ParameterTemplate
    {
        public string value;

        public override ValueTemplate Clone()
        {
            ValueTemplate clone = new ValueTemplate();
            clone.value = new string(value);
            clone.returnType = new string(returnType);

            return clone;
        }

        public override Value_Saveable ToParameter()
        {
            Value_Saveable value = new Value_Saveable();
            value.value = new string(this.value);
            return value;
        }
    }
}

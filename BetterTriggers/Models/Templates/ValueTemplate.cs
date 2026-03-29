using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
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

        public override Value ToParameter(Project project)
        {
            Value value = new Value();
            value.value = new string(this.value);
            return value;
        }
    }
}

using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class AndMultiple : ECA
    {
        public TriggerElementCollection And = new(TriggerElementType.Condition);

        public AndMultiple()
        {
            function.value = "AndMultiple";
            Elements = new();
            And.SetParent(this, 0);
        }

        public override AndMultiple Clone()
        {
            AndMultiple andMultiple = new AndMultiple();
            andMultiple.function = this.function.Clone();
            andMultiple.And = And.Clone();

            return andMultiple;
        }
    }
}

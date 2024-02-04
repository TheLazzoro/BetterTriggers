using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class OrMultiple : ECA
    {
        public TriggerElementCollection Or = new(TriggerElementType.Condition);

        public OrMultiple()
        {
            function.value = "OrMultiple";
            Elements = new();
            Elements.Add(Or);
        }

        public override OrMultiple Clone()
        {
            OrMultiple andMultiple = new OrMultiple();
            andMultiple.function = this.function.Clone();
            andMultiple.Or = Or.Clone();

            return andMultiple;
        }
    }
}

using System;
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
            Or.SetParent(this, 0);
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

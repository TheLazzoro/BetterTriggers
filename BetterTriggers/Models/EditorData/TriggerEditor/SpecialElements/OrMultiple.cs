using System;
using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class OrMultiple : ECA
    {
        public TriggerElementCollection Or
        {
            get => _or;
            set
            {
                if (_or != null)
                {
                    _or.RemoveFromParent();
                }
                _or = value;
                _or.SetParent(this, 0);
            }
        }

        private TriggerElementCollection _or;

        public OrMultiple()
        {
            function.value = "OrMultiple";
            Elements = new();
            Or = new(TriggerElementType.Condition);
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

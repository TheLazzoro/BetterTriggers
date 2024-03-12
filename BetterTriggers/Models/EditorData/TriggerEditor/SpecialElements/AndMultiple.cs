using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class AndMultiple : ECA
    {
        public TriggerElementCollection And
        {
            get => _and;
            set
            {
                if (_and != null)
                {
                    _and.RemoveFromParent();
                }
                _and = value;
                _and.SetParent(this, 0);
            }
        }

        private TriggerElementCollection _and;

        public AndMultiple()
        {
            function.value = "AndMultiple";
            Elements = new();
            And = new(TriggerElementType.Condition);
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

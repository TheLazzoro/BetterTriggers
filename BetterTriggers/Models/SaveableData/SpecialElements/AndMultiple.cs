using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class AndMultiple : TriggerElement
    {
        public readonly int ElementType = 2; // DO NOT CHANGE
        public List<TriggerElement> And = new List<TriggerElement>();

        public AndMultiple()
        {
            function.value = "AndMultiple";
        }

        public override AndMultiple Clone()
        {
            AndMultiple andMultiple = new AndMultiple();
            andMultiple.function = this.function.Clone();
            andMultiple.And = new List<TriggerElement>();
            And.ForEach(element => andMultiple.And.Add(element.Clone()));

            return andMultiple;
        }
    }
}

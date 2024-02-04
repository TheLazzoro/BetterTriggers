using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class AndMultiple_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 2; // DO NOT CHANGE
        public List<TriggerElement_Saveable> And = new List<TriggerElement_Saveable>();

        public AndMultiple_Saveable()
        {
            function.value = "AndMultiple";
        }

        public override AndMultiple_Saveable Clone()
        {
            AndMultiple_Saveable andMultiple = new AndMultiple_Saveable();
            andMultiple.function = this.function.Clone();
            andMultiple.And = new List<TriggerElement_Saveable>();
            And.ForEach(element => andMultiple.And.Add(element.Clone()));

            return andMultiple;
        }
    }
}

using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class OrMultiple_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 3; // DO NOT CHANGE
        public List<TriggerElement_Saveable> Or = new List<TriggerElement_Saveable>();

        public OrMultiple_Saveable()
        {
            function.value = "OrMultiple";
        }

        public override OrMultiple_Saveable Clone()
        {
            OrMultiple_Saveable andMultiple = new OrMultiple_Saveable();
            andMultiple.function = this.function.Clone();
            andMultiple.Or = new List<TriggerElement_Saveable>();
            Or.ForEach(element => andMultiple.Or.Add(element.Clone()));

            return andMultiple;
        }
    }
}

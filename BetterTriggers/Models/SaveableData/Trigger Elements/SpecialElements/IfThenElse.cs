using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class IfThenElse : ECA
    {
        public readonly int ElementType = 1; // DO NOT CHANGE
        public List<TriggerElement> If = new List<TriggerElement>();
        public List<TriggerElement> Then = new List<TriggerElement>();
        public List<TriggerElement> Else = new List<TriggerElement>();
        
        public IfThenElse()
        {
            function.value = "IfThenElseMultiple";
        }

        public override IfThenElse Clone()
        {
            IfThenElse ifThenElse = new IfThenElse();
            ifThenElse.function = this.function.Clone();
            ifThenElse.If = new List<TriggerElement>();
            ifThenElse.Then = new List<TriggerElement>();
            ifThenElse.Else = new List<TriggerElement>();
            If.ForEach(element => ifThenElse.If.Add(element.Clone()));
            Then.ForEach(element => ifThenElse.Then.Add(element.Clone()));
            Else.ForEach(element => ifThenElse.Else.Add(element.Clone()));

            return ifThenElse;
        }
    }
}

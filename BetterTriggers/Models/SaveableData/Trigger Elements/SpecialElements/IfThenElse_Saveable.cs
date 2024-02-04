using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class IfThenElse_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 1; // DO NOT CHANGE
        public List<TriggerElement_Saveable> If = new List<TriggerElement_Saveable>();
        public List<TriggerElement_Saveable> Then = new List<TriggerElement_Saveable>();
        public List<TriggerElement_Saveable> Else = new List<TriggerElement_Saveable>();
        
        public IfThenElse_Saveable()
        {
            function.value = "IfThenElseMultiple";
        }

        public override IfThenElse_Saveable Clone()
        {
            IfThenElse_Saveable ifThenElse = new IfThenElse_Saveable();
            ifThenElse.function = this.function.Clone();
            ifThenElse.If = new List<TriggerElement_Saveable>();
            ifThenElse.Then = new List<TriggerElement_Saveable>();
            ifThenElse.Else = new List<TriggerElement_Saveable>();
            If.ForEach(element => ifThenElse.If.Add(element.Clone()));
            Then.ForEach(element => ifThenElse.Then.Add(element.Clone()));
            Else.ForEach(element => ifThenElse.Else.Add(element.Clone()));

            return ifThenElse;
        }
    }
}

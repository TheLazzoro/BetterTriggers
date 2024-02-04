using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ForLoopVarMultiple_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 8; // DO NOT CHANGE
        public List<TriggerElement_Saveable> Actions = new List<TriggerElement_Saveable>();

        public ForLoopVarMultiple_Saveable()
        {
            function.value = "ForLoopVarMultiple";
        }

        public override ForLoopVarMultiple_Saveable Clone()
        {
            ForLoopVarMultiple_Saveable forLoop = new ForLoopVarMultiple_Saveable();
            forLoop.function = this.function.Clone();
            forLoop.Actions = new List<TriggerElement_Saveable>();
            Actions.ForEach(element => forLoop.Actions.Add(element.Clone()));

            return forLoop;
        }
    }
}

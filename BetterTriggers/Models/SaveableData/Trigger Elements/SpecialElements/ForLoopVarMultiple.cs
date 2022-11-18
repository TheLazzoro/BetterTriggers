using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ForLoopVarMultiple : ECA
    {
        public readonly int ElementType = 8; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();

        public ForLoopVarMultiple()
        {
            function.value = "ForLoopVarMultiple";
        }

        public override ForLoopVarMultiple Clone()
        {
            ForLoopVarMultiple forLoop = new ForLoopVarMultiple();
            forLoop.function = this.function.Clone();
            forLoop.Actions = new List<TriggerElement>();
            Actions.ForEach(element => forLoop.Actions.Add(element.Clone()));

            return forLoop;
        }
    }
}

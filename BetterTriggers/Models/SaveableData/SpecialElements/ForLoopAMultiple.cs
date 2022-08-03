using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ForLoopAMultiple : TriggerElement
    {
        public readonly int ElementType = 6; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();

        public ForLoopAMultiple()
        {
            function.value = "ForLoopAMultiple";
        }

        public override ForLoopAMultiple Clone()
        {
            ForLoopAMultiple forLoop = new ForLoopAMultiple();
            forLoop.function = this.function.Clone();
            forLoop.Actions = new List<TriggerElement>();
            Actions.ForEach(element => forLoop.Actions.Add(element.Clone()));

            return forLoop;
        }
    }
}

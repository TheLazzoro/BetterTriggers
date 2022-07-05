using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ForLoopBMultiple : TriggerElement
    {
        public readonly int ElementType = 7; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();

        public ForLoopBMultiple()
        {
            function.identifier = "ForLoopBMultiple";
        }

        public new ForLoopBMultiple Clone()
        {
            ForLoopBMultiple forLoop = new ForLoopBMultiple();
            forLoop.function = this.function.Clone();
            forLoop.Actions = new List<TriggerElement>();
            Actions.ForEach(element => forLoop.Actions.Add(element.Clone()));

            return forLoop;
        }
    }
}

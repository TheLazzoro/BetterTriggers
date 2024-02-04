using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ForLoopAMultiple_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 6; // DO NOT CHANGE
        public List<TriggerElement_Saveable> Actions = new List<TriggerElement_Saveable>();

        public ForLoopAMultiple_Saveable()
        {
            function.value = "ForLoopAMultiple";
        }

        public override ForLoopAMultiple_Saveable Clone()
        {
            ForLoopAMultiple_Saveable forLoop = new ForLoopAMultiple_Saveable();
            forLoop.function = this.function.Clone();
            forLoop.Actions = new List<TriggerElement_Saveable>();
            Actions.ForEach(element => forLoop.Actions.Add(element.Clone()));

            return forLoop;
        }
    }
}

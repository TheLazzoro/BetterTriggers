using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ForGroupMultiple_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 4; // DO NOT CHANGE
        public List<TriggerElement_Saveable> Actions = new List<TriggerElement_Saveable>();

        public ForGroupMultiple_Saveable()
        {
            function.value = "ForGroupMultiple";
        }

        public override ForGroupMultiple_Saveable Clone()
        {
            ForGroupMultiple_Saveable forGroup = new ForGroupMultiple_Saveable();
            forGroup.function = this.function.Clone();
            forGroup.Actions = new List<TriggerElement_Saveable>();
            Actions.ForEach(element => forGroup.Actions.Add(element.Clone()));

            return forGroup;
        }
    }
}

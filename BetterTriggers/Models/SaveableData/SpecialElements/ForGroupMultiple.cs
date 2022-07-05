using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ForGroupMultiple : TriggerElement
    {
        public readonly int ElementType = 4; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();

        public ForGroupMultiple()
        {
            function.identifier = "ForGroupMultiple";
        }

        public new ForGroupMultiple Clone()
        {
            ForGroupMultiple forGroup = new ForGroupMultiple();
            forGroup.function = this.function.Clone();
            forGroup.Actions = new List<TriggerElement>();
            Actions.ForEach(element => forGroup.Actions.Add(element.Clone()));

            return forGroup;
        }
    }
}

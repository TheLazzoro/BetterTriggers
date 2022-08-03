using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ForForceMultiple : TriggerElement
    {
        public readonly int ElementType = 5; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();

        public ForForceMultiple()
        {
            function.value = "ForForceMultiple";
        }

        public override ForForceMultiple Clone()
        {
            ForForceMultiple forForce = new ForForceMultiple();
            forForce.function = this.function.Clone();
            forForce.Actions = new List<TriggerElement>();
            Actions.ForEach(element => forForce.Actions.Add(element.Clone()));

            return forForce;
        }
    }
}

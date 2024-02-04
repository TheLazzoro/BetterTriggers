using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ForForceMultiple_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 5; // DO NOT CHANGE
        public List<TriggerElement_Saveable> Actions = new List<TriggerElement_Saveable>();

        public ForForceMultiple_Saveable()
        {
            function.value = "ForForceMultiple";
        }

        public override ForForceMultiple_Saveable Clone()
        {
            ForForceMultiple_Saveable forForce = new ForForceMultiple_Saveable();
            forForce.function = this.function.Clone();
            forForce.Actions = new List<TriggerElement_Saveable>();
            Actions.ForEach(element => forForce.Actions.Add(element.Clone()));

            return forForce;
        }
    }
}

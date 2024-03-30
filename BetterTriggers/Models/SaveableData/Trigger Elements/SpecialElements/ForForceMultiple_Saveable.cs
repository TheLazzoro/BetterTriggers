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
    }
}

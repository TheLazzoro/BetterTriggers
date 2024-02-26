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
    }
}

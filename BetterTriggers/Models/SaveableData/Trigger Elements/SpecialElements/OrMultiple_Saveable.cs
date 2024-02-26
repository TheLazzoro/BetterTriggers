using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class OrMultiple_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 3; // DO NOT CHANGE
        public List<TriggerElement_Saveable> Or = new List<TriggerElement_Saveable>();

        public OrMultiple_Saveable()
        {
            function.value = "OrMultiple";
        }
    }
}

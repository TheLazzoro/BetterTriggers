using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class EnumDestructiblesInCircleBJMultiple_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 11; // DO NOT CHANGE
        public List<TriggerElement_Saveable> Actions = new List<TriggerElement_Saveable>();

        public EnumDestructiblesInCircleBJMultiple_Saveable()
        {
            function.value = "EnumDestructablesInCircleBJMultiple";
        }
    }
}

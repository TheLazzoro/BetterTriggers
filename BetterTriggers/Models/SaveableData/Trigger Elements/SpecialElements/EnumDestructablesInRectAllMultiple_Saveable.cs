using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class EnumDestructablesInRectAllMultiple_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 10; // DO NOT CHANGE
        public List<TriggerElement_Saveable> Actions = new List<TriggerElement_Saveable>();

        public EnumDestructablesInRectAllMultiple_Saveable()
        {
            function.value = "EnumDestructablesInRectAllMultiple";
        }
    }
}

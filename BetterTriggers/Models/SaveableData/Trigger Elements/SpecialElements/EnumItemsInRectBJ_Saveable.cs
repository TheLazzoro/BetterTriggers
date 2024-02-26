using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class EnumItemsInRectBJ_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 12; // DO NOT CHANGE
        public List<TriggerElement_Saveable> Actions = new List<TriggerElement_Saveable>();

        public EnumItemsInRectBJ_Saveable()
        {
            function.value = "EnumItemsInRectBJMultiple";
        }
    }
}

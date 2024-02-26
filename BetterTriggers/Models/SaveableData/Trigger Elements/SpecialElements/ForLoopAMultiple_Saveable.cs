using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ForLoopAMultiple_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 6; // DO NOT CHANGE
        public List<TriggerElement_Saveable> Actions = new List<TriggerElement_Saveable>();

        public ForLoopAMultiple_Saveable()
        {
            function.value = "ForLoopAMultiple";
        }
    }
}

using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ForLoopBMultiple_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 7; // DO NOT CHANGE
        public List<TriggerElement_Saveable> Actions = new List<TriggerElement_Saveable>();

        public ForLoopBMultiple_Saveable()
        {
            function.value = "ForLoopBMultiple";
        }
    }
}

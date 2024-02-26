using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class IfThenElse_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 1; // DO NOT CHANGE
        public List<TriggerElement_Saveable> If = new List<TriggerElement_Saveable>();
        public List<TriggerElement_Saveable> Then = new List<TriggerElement_Saveable>();
        public List<TriggerElement_Saveable> Else = new List<TriggerElement_Saveable>();
        
        public IfThenElse_Saveable()
        {
            function.value = "IfThenElseMultiple";
        }
    }
}

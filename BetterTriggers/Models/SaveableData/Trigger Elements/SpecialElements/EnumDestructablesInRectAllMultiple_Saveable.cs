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

        public override EnumDestructablesInRectAllMultiple_Saveable Clone()
        {
            EnumDestructablesInRectAllMultiple_Saveable enumDest = new EnumDestructablesInRectAllMultiple_Saveable();
            enumDest.function = this.function.Clone();
            enumDest.Actions = new List<TriggerElement_Saveable>();
            Actions.ForEach(element => enumDest.Actions.Add(element.Clone()));

            return enumDest;
        }
    }
}

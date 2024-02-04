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

        public override EnumDestructiblesInCircleBJMultiple_Saveable Clone()
        {
            EnumDestructiblesInCircleBJMultiple_Saveable enumDest = new EnumDestructiblesInCircleBJMultiple_Saveable();
            enumDest.function = this.function.Clone();
            enumDest.Actions = new List<TriggerElement_Saveable>();
            Actions.ForEach(element => enumDest.Actions.Add(element.Clone()));

            return enumDest;
        }
    }
}

using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class EnumDestructablesInRectAllMultiple : TriggerElement
    {
        public readonly int ElementType = 10; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();

        public EnumDestructablesInRectAllMultiple()
        {
            function.value = "EnumDestructablesInRectAllMultiple";
        }

        public new EnumDestructablesInRectAllMultiple Clone()
        {
            EnumDestructablesInRectAllMultiple enumDest = new EnumDestructablesInRectAllMultiple();
            enumDest.function = this.function.Clone();
            enumDest.Actions = new List<TriggerElement>();
            Actions.ForEach(element => enumDest.Actions.Add(element.Clone()));

            return enumDest;
        }
    }
}

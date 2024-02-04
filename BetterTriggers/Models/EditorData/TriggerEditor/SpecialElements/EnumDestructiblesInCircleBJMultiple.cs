using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class EnumDestructiblesInCircleBJMultiple : ECA
    {
        public TriggerElementCollection Actions = new(TriggerElementType.Action);

        public EnumDestructiblesInCircleBJMultiple()
        {
            function.value = "EnumDestructablesInCircleBJMultiple";
            Elements = new();
            Elements.Add(Actions);
        }

        public override EnumDestructiblesInCircleBJMultiple Clone()
        {
            EnumDestructiblesInCircleBJMultiple enumDest = new EnumDestructiblesInCircleBJMultiple();
            enumDest.function = this.function.Clone();
            enumDest.Actions = Actions.Clone();

            return enumDest;
        }
    }
}

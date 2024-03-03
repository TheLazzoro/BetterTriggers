using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class EnumDestructablesInRectAllMultiple : ECA
    {
        public TriggerElementCollection Actions = new(TriggerElementType.Action);

        public EnumDestructablesInRectAllMultiple()
        {
            function.value = "EnumDestructablesInRectAllMultiple";
            Elements = new();
            Actions.SetParent(this, 0);
            Actions.DisplayText = "Loop - Actions";
        }

        public override EnumDestructablesInRectAllMultiple Clone()
        {
            EnumDestructablesInRectAllMultiple enumDest = new EnumDestructablesInRectAllMultiple();
            enumDest.function = this.function.Clone();
            enumDest.Actions = Actions.Clone();

            return enumDest;
        }
    }
}

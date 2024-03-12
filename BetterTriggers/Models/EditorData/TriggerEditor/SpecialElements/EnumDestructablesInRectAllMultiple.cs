using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class EnumDestructablesInRectAllMultiple : ECA
    {
        public TriggerElementCollection Actions
        {
            get => _actions;
            set
            {
                if (_actions != null)
                {
                    _actions.RemoveFromParent();
                }
                _actions = value;
                _actions.SetParent(this, 0);
            }
        }

        private TriggerElementCollection _actions;

        public EnumDestructablesInRectAllMultiple()
        {
            function.value = "EnumDestructablesInRectAllMultiple";
            Elements = new();
            Actions = new(TriggerElementType.Action);
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

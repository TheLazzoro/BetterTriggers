using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class EnumDestructiblesInCircleBJMultiple : ECA
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

        public EnumDestructiblesInCircleBJMultiple()
        {
            function.value = "EnumDestructablesInCircleBJMultiple";
            Elements = new();
            Actions = new(TriggerElementType.Action);
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

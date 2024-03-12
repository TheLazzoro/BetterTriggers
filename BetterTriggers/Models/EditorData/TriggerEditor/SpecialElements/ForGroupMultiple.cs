using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class ForGroupMultiple : ECA
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

        public ForGroupMultiple()
        {
            function.value = "ForGroupMultiple";
            Elements = new();
            Actions = new(TriggerElementType.Action);
        }

        public override ForGroupMultiple Clone()
        {
            ForGroupMultiple forGroup = new ForGroupMultiple();
            forGroup.function = this.function.Clone();
            forGroup.Actions = Actions.Clone();

            return forGroup;
        }
    }
}

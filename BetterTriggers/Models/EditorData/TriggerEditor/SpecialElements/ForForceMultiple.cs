using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class ForForceMultiple : ECA
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

        public ForForceMultiple()
        {
            function.value = "ForForceMultiple";
            Elements = new();
            Actions = new(TriggerElementType.Action);
        }

        public override ForForceMultiple Clone()
        {
            ForForceMultiple forForce = new ForForceMultiple();
            forForce.function = this.function.Clone();
            forForce.Actions = Actions.Clone();

            return forForce;
        }
    }
}

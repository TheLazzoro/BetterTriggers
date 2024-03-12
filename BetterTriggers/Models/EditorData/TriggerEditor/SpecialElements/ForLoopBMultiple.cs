using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class ForLoopBMultiple : ECA
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

        public ForLoopBMultiple()
        {
            function.value = "ForLoopBMultiple";
            Elements = new();
            Actions = new(TriggerElementType.Action);
        }

        public override ForLoopBMultiple Clone()
        {
            ForLoopBMultiple forLoop = new ForLoopBMultiple();
            forLoop.function = this.function.Clone();
            forLoop.Actions = Actions.Clone();

            return forLoop;
        }
    }
}

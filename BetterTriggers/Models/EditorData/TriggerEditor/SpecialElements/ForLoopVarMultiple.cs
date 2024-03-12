using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class ForLoopVarMultiple : ECA
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

        public ForLoopVarMultiple()
        {
            function.value = "ForLoopVarMultiple";
            Elements = new();
            Actions = new(TriggerElementType.Action);
        }

        public override ForLoopVarMultiple Clone()
        {
            ForLoopVarMultiple forLoop = new ForLoopVarMultiple();
            forLoop.function = this.function.Clone();
            forLoop.Actions = Actions.Clone();

            return forLoop;
        }
    }
}

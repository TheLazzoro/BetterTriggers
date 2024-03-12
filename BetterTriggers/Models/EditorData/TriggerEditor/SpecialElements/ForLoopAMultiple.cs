using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class ForLoopAMultiple : ECA
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

        public ForLoopAMultiple()
        {
            function.value = "ForLoopAMultiple";
            Elements = new();
            Actions = new(TriggerElementType.Action);
        }

        public override ForLoopAMultiple Clone()
        {
            ForLoopAMultiple forLoop = new ForLoopAMultiple();
            forLoop.function = this.function.Clone();
            forLoop.Actions = Actions.Clone();

            return forLoop;
        }
    }
}

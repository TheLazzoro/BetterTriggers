using System;
using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class IfThenElse : ECA
    {
        public TriggerElementCollection If
        {
            get => _if;
            set
            {
                if (_if != null)
                {
                    _if.RemoveFromParent();
                }
                _if = value;
                _if.SetParent(this, 0);
                _if.DisplayText = "If - Conditions";
            }
        }
        public TriggerElementCollection Then
        {
            get => _then;
            set
            {
                if (_then != null)
                {
                    _then.RemoveFromParent();
                }
                _then = value;
                _then.SetParent(this, 1);
                _then.DisplayText = "Then - Actions";
            }
        }
        public TriggerElementCollection Else
        {
            get => _else;
            set
            {
                if (_else != null)
                {
                    _else.RemoveFromParent();
                }
                _else = value;
                _else.SetParent(this, 2);
                _else.DisplayText = "Else - Actions";
            }
        }

        private TriggerElementCollection _if;
        private TriggerElementCollection _then;
        private TriggerElementCollection _else;

        public IfThenElse()
        {
            function.value = "IfThenElseMultiple";
            Elements = new();
            If = new(TriggerElementType.Condition);
            Then = new(TriggerElementType.Action);
            Else = new(TriggerElementType.Action);
        }

        public override IfThenElse Clone()
        {
            IfThenElse ifThenElse = new IfThenElse();
            ifThenElse.function = this.function.Clone();
            ifThenElse.If = If.Clone();
            ifThenElse.Then = Then.Clone();
            ifThenElse.Else = Else.Clone();

            return ifThenElse;
        }
    }
}

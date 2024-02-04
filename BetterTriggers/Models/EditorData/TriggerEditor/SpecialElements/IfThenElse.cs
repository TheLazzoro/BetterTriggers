using System;
using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class IfThenElse : ECA
    {
        public TriggerElementCollection If = new(TriggerElementType.Condition);
        public TriggerElementCollection Then = new(TriggerElementType.Action);
        public TriggerElementCollection Else = new(TriggerElementType.Action);

        public IfThenElse()
        {
            function.value = "IfThenElseMultiple";
            Elements = new();
            Elements.Add(If);
            Elements.Add(Then);
            Elements.Add(Else);
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

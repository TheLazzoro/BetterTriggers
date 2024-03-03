using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class ForLoopVarMultiple : ECA
    {
        public TriggerElementCollection Actions = new(TriggerElementType.Action);

        public ForLoopVarMultiple()
        {
            function.value = "ForLoopVarMultiple";
            Elements = new();
            Actions.SetParent(this, 0);
            Actions.DisplayText = "Loop - Actions";
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

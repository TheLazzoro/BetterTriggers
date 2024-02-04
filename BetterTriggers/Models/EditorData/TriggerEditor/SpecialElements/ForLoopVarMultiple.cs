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
            Elements.Add(Actions);
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

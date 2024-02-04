using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class ForLoopBMultiple : ECA
    {
        public TriggerElementCollection Actions = new(TriggerElementType.Action);

        public ForLoopBMultiple()
        {
            function.value = "ForLoopBMultiple";
            Elements = new();
            Elements.Add(Actions);
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

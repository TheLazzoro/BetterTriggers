using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class ForLoopAMultiple : ECA
    {
        public TriggerElementCollection Actions = new(TriggerElementType.Action);

        public ForLoopAMultiple()
        {
            function.value = "ForLoopAMultiple";
            Elements = new();
            Elements.Add(Actions);
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

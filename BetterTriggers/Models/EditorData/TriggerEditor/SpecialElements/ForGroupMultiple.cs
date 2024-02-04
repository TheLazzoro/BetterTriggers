using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class ForGroupMultiple : ECA
    {
        public TriggerElementCollection Actions = new(TriggerElementType.Action);

        public ForGroupMultiple()
        {
            function.value = "ForGroupMultiple";
            Elements = new();
            Elements.Add(Actions);
        }

        public override ForGroupMultiple Clone()
        {
            ForGroupMultiple forGroup = new ForGroupMultiple();
            forGroup.function = this.function.Clone();
            forGroup.Actions = Actions.Clone();

            return forGroup;
        }
    }
}

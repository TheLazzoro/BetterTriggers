using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class ForForceMultiple : ECA
    {
        public TriggerElementCollection Actions = new(TriggerElementType.Action);

        public ForForceMultiple()
        {
            function.value = "ForForceMultiple";
            Elements = new();
            Elements.Add(Actions);
        }

        public override ForForceMultiple Clone()
        {
            ForForceMultiple forForce = new ForForceMultiple();
            forForce.function = this.function.Clone();
            forForce.Actions = Actions.Clone();

            return forForce;
        }
    }
}

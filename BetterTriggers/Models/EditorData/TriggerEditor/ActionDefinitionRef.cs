using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace BetterTriggers.Models.EditorData.TriggerEditor
{
    public class ActionDefinitionRef : ECA, IReferable
    {
        public int ActionDefinitionId;
        public List<Parameter> Parameters = new();

        public override ActionDefinitionRef Clone()
        {
            var cloned = new ActionDefinitionRef();
            cloned.ActionDefinitionId = ActionDefinitionId;
            Parameters.ForEach(p => cloned.Parameters.Add(p.Clone()));

            return cloned;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace BetterTriggers.Models.EditorData.TriggerEditor
{
    public class ConditionDefinitionRef : ECA, IReferable
    {
        public int ConditionDefinitionId;
        public List<Parameter> Parameters = new();

        public override ConditionDefinitionRef Clone()
        {
            var cloned = new ConditionDefinitionRef();
            cloned.ConditionDefinitionId = ConditionDefinitionId;
            Parameters.ForEach(p => cloned.Parameters.Add(p.Clone()));

            return cloned;
        }
    }
}

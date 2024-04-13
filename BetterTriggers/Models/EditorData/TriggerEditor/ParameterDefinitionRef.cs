using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace BetterTriggers.Models.EditorData.TriggerEditor
{
    public class ParameterDefinitionRef : Parameter, IReferable
    {
        public int ParameterDefinitionId;

        public override ParameterDefinitionRef Clone()
        {
            var cloned = new ParameterDefinitionRef();
            cloned.ParameterDefinitionId = ParameterDefinitionId;

            return cloned;
        }
    }
}

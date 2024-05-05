using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace BetterTriggers.Models.EditorData.TriggerEditor
{
    public class FunctionDefinitionRef : Function
    {
        public int FunctionDefinitionId;

        public override FunctionDefinitionRef Clone()
        {
            var function = base.Clone();
            var cloned = new FunctionDefinitionRef();
            cloned.parameters = function.parameters;
            cloned.value = function.value;
            cloned.FunctionDefinitionId = FunctionDefinitionId;

            return cloned;
        }
    }
}

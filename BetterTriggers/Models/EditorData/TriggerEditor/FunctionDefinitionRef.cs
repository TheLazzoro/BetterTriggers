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
            var cloned = new FunctionDefinitionRef();
            cloned.FunctionDefinitionId = FunctionDefinitionId;

            return cloned;
        }
    }
}

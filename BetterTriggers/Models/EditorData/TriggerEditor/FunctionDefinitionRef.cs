using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace BetterTriggers.Models.EditorData.TriggerEditor
{
    public class FunctionDefinitionRef : Parameter
    {
        public int FunctionDefinitionId;
        public List<Parameter> Parameters = new();

        public override FunctionDefinitionRef Clone()
        {
            var cloned = new FunctionDefinitionRef();
            cloned.FunctionDefinitionId = FunctionDefinitionId;
            Parameters.ForEach(p => cloned.Parameters.Add(p.Clone()));

            return cloned;
        }
    }
}

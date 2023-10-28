using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models
{
    public class IdCollisionException : Exception
    {
        public List<Tuple<ExplorerElementTrigger, ExplorerElementTrigger>> triggers { get; }
        public List<Tuple<ExplorerElementVariable, ExplorerElementVariable>> variables { get; }
        public override string Message { get; }


        public IdCollisionException(List<Tuple<ExplorerElementTrigger, ExplorerElementTrigger>> triggers, List<Tuple<ExplorerElementVariable, ExplorerElementVariable>> variables)
        {
            this.triggers = triggers;
            this.variables = variables;
            Message = "Detected ID-collisions between triggers or variables!";
        }
    }
}

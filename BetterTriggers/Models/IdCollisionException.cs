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
        public List<Tuple<ExplorerElement, ExplorerElement>> triggers { get; }
        public List<Tuple<ExplorerElement, ExplorerElement>> variables { get; }
        public override string Message { get; }


        public IdCollisionException(List<Tuple<ExplorerElement, ExplorerElement>> triggers, List<Tuple<ExplorerElement, ExplorerElement>> variables)
        {
            this.triggers = triggers;
            this.variables = variables;
            Message = "Detected ID-collisions between triggers or variables!";
        }
    }
}

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
        public List<Tuple<ExplorerElement, ExplorerElement>> IdCollisions { get; }
        public List<Tuple<ExplorerElement, ExplorerElement>> variables { get; }
        public override string Message { get; }


        public IdCollisionException(List<Tuple<ExplorerElement, ExplorerElement>> idCollisions)
        {
            this.IdCollisions = idCollisions;
            Message = "Detected ID-collisions between triggers or variables!";
        }
    }
}

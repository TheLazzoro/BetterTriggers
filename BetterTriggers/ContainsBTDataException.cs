using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers
{
    public class ContainsBTDataException : Exception
    {
        public List<Tuple<ExplorerElement, string>> ExplorerElementsWithBTData { get; init; }
        
        public ContainsBTDataException(List<Tuple<ExplorerElement, string>> explorerElementsWithBTData, string message) : base(message)
        {
            ExplorerElementsWithBTData = explorerElementsWithBTData;
        }
    }
}

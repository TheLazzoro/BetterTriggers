using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Controllers
{
    public class ControllerReferences
    {
        public static List<ExplorerElementTrigger> GetReferrers(IReferable element) {
            return References.GetReferreres(element);
        }
    }
}
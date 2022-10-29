using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System.Collections.Generic;

namespace BetterTriggers.Containers
{
    internal static class CopiedElements
    {
        internal static List<ITriggerElement> CopiedTriggerElements = new List<ITriggerElement>();
        internal static List<ITriggerElement> CutTriggerElements = new List<ITriggerElement>();
        internal static ExplorerElementTrigger CopiedFromTrigger;

        internal static IExplorerElement CopiedExplorerElement;
        internal static IExplorerElement CutExplorerElement;
    }
}

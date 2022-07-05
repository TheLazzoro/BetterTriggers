using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System.Collections.Generic;

namespace BetterTriggers.Containers
{
    internal static class CopiedElements
    {
        internal static List<TriggerElement> CopiedTriggerElements = new List<TriggerElement>();
        internal static List<TriggerElement> CutTriggerElements = new List<TriggerElement>();
        internal static ExplorerElementTrigger CopiedFromTrigger;

        internal static IExplorerElement CopiedExplorerElement;
        internal static IExplorerElement CutExplorerElement;
    }
}

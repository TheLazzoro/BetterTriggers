using Model.EditorData;
using Model.SaveableData;
using System.Collections.Generic;

namespace BetterTriggers.Containers
{
    internal static class ContainerCopiedElements
    {
        internal static List<TriggerElement> CopiedTriggerElements = new List<TriggerElement>();
        internal static List<TriggerElement> CutTriggerElements = new List<TriggerElement>();

        internal static IExplorerElement CopiedExplorerElement;
        internal static IExplorerElement CutExplorerElement;
    }
}

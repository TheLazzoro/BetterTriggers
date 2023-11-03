using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System.Collections.Generic;

namespace BetterTriggers.Containers
{
    /// <summary>
    /// This class is static so you can copy-paste across projects.
    /// </summary>
    public static class CopiedElements
    {
        public static List<TriggerElement> CopiedTriggerElements = new();
        public static List<TriggerElement> CutTriggerElements = new();
        public static ExplorerElementTrigger CopiedFromTrigger;

        public static IExplorerElement CopiedExplorerElement;
        public static IExplorerElement CutExplorerElement;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model.EditorData;

namespace BetterTriggers.Containers
{
    public class ContainerReferences
    {
        private static Dictionary<ExplorerElementTrigger, HashSet<IExplorerElement>> fromTrigger = new Dictionary<ExplorerElementTrigger, HashSet<IExplorerElement>>();
        private static Dictionary<IExplorerElement, HashSet<ExplorerElementTrigger>> fromReference = new Dictionary<IExplorerElement, HashSet<ExplorerElementTrigger>>();

        internal static List<ExplorerElementTrigger> GetReferreres(IExplorerElement explorerElement)
        {
            HashSet<ExplorerElementTrigger> referrers = new HashSet<ExplorerElementTrigger>();
            fromReference.TryGetValue(explorerElement, out referrers);

            if (referrers == null)
                return new List<ExplorerElementTrigger>();

            return referrers.ToList();
        }

        internal static void AddReferrer(ExplorerElementTrigger source, IExplorerElement reference)
        {
            if (reference == null)
                return;

            HashSet<IExplorerElement> references = null;
            if (!fromTrigger.TryGetValue(source, out references)) {
                references = new HashSet<IExplorerElement>();
                fromTrigger.Add(source, references);
            }
            references.Add(reference);

            HashSet<ExplorerElementTrigger> triggers = null;
            fromReference.TryGetValue(reference, out triggers);
            if (triggers == null)
            {
                triggers = new HashSet<ExplorerElementTrigger>();
                fromReference.Add(reference, triggers);
            }

            triggers.Add(source);
        }

        /// <summary>
        /// Removes a referrer from all variables. Used when a trigger gets deleted.
        /// </summary>
        internal static void RemoveReferrer(ExplorerElementTrigger source)
        {
            HashSet<IExplorerElement> references = null;
            fromTrigger.TryGetValue(source, out references);
            if (references == null)
                return;

            foreach (var reference in references)
            {
                HashSet<ExplorerElementTrigger> triggers = null;
                fromReference.TryGetValue(reference, out triggers);
                triggers.Remove(source);
            }
        }

        internal static void ResetVariableReferences(ExplorerElementVariable variable)
        {
            HashSet<ExplorerElementTrigger> empty = new HashSet<ExplorerElementTrigger>();
            fromReference.Remove(variable);
            fromReference.Add(variable, empty);
        }
    }
}
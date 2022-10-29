using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Containers
{
    public class References
    {
        private static Dictionary<ExplorerElementTrigger, HashSet<IReferable>> fromTrigger = new Dictionary<ExplorerElementTrigger, HashSet<IReferable>>();
        private static Dictionary<IReferable, HashSet<ExplorerElementTrigger>> fromReference = new Dictionary<IReferable, HashSet<ExplorerElementTrigger>>();

        /// <summary>
        /// Returns a list of trigger which are referencing the given referable.
        /// </summary>
        internal static List<ExplorerElementTrigger> GetReferreres(IReferable referable)
        {
            HashSet<ExplorerElementTrigger> referrers = new HashSet<ExplorerElementTrigger>();
            fromReference.TryGetValue(referable, out referrers);

            if (referrers == null)
                return new List<ExplorerElementTrigger>();

            return referrers.ToList();
        }

        internal static void AddReferrer(ExplorerElementTrigger source, IReferable reference)
        {
            if (reference == null)
                return;

            HashSet<IReferable> references = null;
            if (!fromTrigger.TryGetValue(source, out references)) {
                references = new HashSet<IReferable>();
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
            HashSet<IReferable> references = null;
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

        internal static void ResetVariableReferences(Variable variable)
        {
            HashSet<ExplorerElementTrigger> empty = new HashSet<ExplorerElementTrigger>();
            fromReference.Remove(variable);
            fromReference.Add(variable, empty);
        }
    }
}
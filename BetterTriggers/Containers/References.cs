using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Containers
{
    /// <summary>
    /// 'Source' in this context is either a trigger, action-, condition-, or function-definition.
    /// They can all make references to variables and each other.
    /// </summary>
    public class References
    {
        private static Dictionary<IExplorerElement, HashSet<IReferable>> fromSource = new();
        private static Dictionary<IReferable, HashSet<IExplorerElement>> fromReference = new();

        /// <summary>
        /// Returns a list of sources which are referencing the given referable.
        /// </summary>
        internal static List<IExplorerElement> GetReferreres(IReferable referable)
        {
            HashSet<IExplorerElement> referrers;
            fromReference.TryGetValue(referable, out referrers);

            if (referrers == null)
                return new List<IExplorerElement>();

            return referrers.ToList();
        }

        internal static void AddReferrer(ExplorerElementTrigger source, IReferable reference)
        {
            if (reference == null)
                return;

            // Gets a list of references, given the source referencing them.
            HashSet<IReferable> references;
            if (!fromSource.TryGetValue(source, out references))
            {
                references = new HashSet<IReferable>();
                fromSource.Add(source, references);
            }
            references.Add(reference); // Adds the reference to the source's total references.

            // And vice versa; Gets a list of all sources referencing the referable.
            HashSet<IExplorerElement> sources;
            fromReference.TryGetValue(reference, out sources);
            if (sources == null)
            {
                sources = new HashSet<IExplorerElement>();
                fromReference.Add(reference, sources);
            }
            sources.Add(source); // Adds the source to the referable's total number of referencing sources.
        }

        /// <summary>
        /// Removes a source reference from all referrers. Used when a source gets deleted.
        /// </summary>
        internal static void RemoveReferrer(IExplorerElement source)
        {
            HashSet<IReferable> references;
            fromSource.TryGetValue(source, out references);
            if (references == null)
                return;

            foreach (var reference in references)
            {
                HashSet<IExplorerElement> sources;
                fromReference.TryGetValue(reference, out sources);
                sources.Remove(source);
            }
        }

        internal static void ResetVariableReferences(Variable variable)
        {
            HashSet<IExplorerElement> empty = new();
            fromReference.Remove(variable);
            fromReference.Add(variable, empty);
        }

        /// <summary>
        /// Updates a trigger with all variable and trigger refs.
        /// </summary>
        internal static void UpdateReferences(ExplorerElementTrigger t)
        {
            List<ExplorerElementVariable> explorerElementVariables = new List<ExplorerElementVariable>();

            RemoveReferrer(t);

            var parameters = ControllerTrigger.GetParametersFromTrigger(t);
            parameters.ForEach(p =>
            {
                if (p is VariableRef)
                {
                    VariableRef varRef = (VariableRef)p;
                    Variable element = Variables.GetVariableById_AllLocals(varRef.VariableId);
                    if (element != null)
                        AddReferrer(t, element);
                }
                else if (p is TriggerRef)
                {
                    TriggerRef tRef = (TriggerRef)p;
                    ExplorerElementTrigger element = Triggers.FindById(tRef.TriggerId);
                    if (element != null)
                        AddReferrer(t, element.trigger);
                }
            });
        }

        /// <summary>
        /// Refreshes references for a given variable.
        /// </summary>
        internal static void UpdateReferences(Variable variable)
        {
            References.ResetVariableReferences(variable);
            var triggers = ControllerTrigger.GetTriggersAll();
            triggers.ForEach(exTrig =>
            {
                var parameters = ControllerTrigger.GetParametersFromTrigger(exTrig);
                parameters.ForEach(p =>
                {
                    if (p is VariableRef varRef)
                    {
                        if (varRef.VariableId == variable.Id)
                            References.AddReferrer(exTrig, variable);
                    }
                });
            });

        }

        internal static void UpdateReferencesAll()
        {
            var triggers = ControllerTrigger.GetTriggersAll();
            triggers.ForEach(trigger => UpdateReferences(trigger));
        }
    }
}
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
        private Dictionary<ExplorerElement, HashSet<IReferable_Saveable>> fromTrigger = new Dictionary<ExplorerElement, HashSet<IReferable_Saveable>>();
        private Dictionary<IReferable_Saveable, HashSet<ExplorerElement>> fromReference = new Dictionary<IReferable_Saveable, HashSet<ExplorerElement>>();

        /// <summary>
        /// Returns a list of trigger which are referencing the given referable.
        /// </summary>
        public List<ExplorerElement> GetReferrers(IReferable_Saveable referable)
        {
            HashSet<ExplorerElement> referrers = new HashSet<ExplorerElement>();
            fromReference.TryGetValue(referable, out referrers);

            if (referrers == null)
                return new List<ExplorerElement>();

            return referrers.ToList();
        }

        internal void AddReferrer(ExplorerElement source, IReferable_Saveable reference)
        {
            if (reference == null)
                return;

            // Gets a list of references, given the trigger referencing them.
            HashSet<IReferable_Saveable> references = null;
            if (!fromTrigger.TryGetValue(source, out references)) {
                references = new HashSet<IReferable_Saveable>();
                fromTrigger.Add(source, references);
            }
            references.Add(reference); // Adds the reference to the trigger's total references.

            // And vice versa; Gets a list of all triggers referencing the referable.
            HashSet<ExplorerElement> triggers = null;
            fromReference.TryGetValue(reference, out triggers);
            if (triggers == null)
            {
                triggers = new HashSet<ExplorerElement>();
                fromReference.Add(reference, triggers);
            }
            triggers.Add(source); // Adds the trigger to the referable's total number of referencing triggers.
        }

        /// <summary>
        /// Removes a referrer from all variables. Used when a trigger gets deleted.
        /// </summary>
        internal void RemoveReferrer(ExplorerElement source)
        {
            HashSet<IReferable_Saveable> references = null;
            fromTrigger.TryGetValue(source, out references);
            if (references == null)
                return;

            foreach (var reference in references)
            {
                HashSet<ExplorerElement> triggers = null;
                fromReference.TryGetValue(reference, out triggers);
                triggers.Remove(source);
            }
        }

        internal void ResetVariableReferences(Variable variable)
        {
            HashSet<ExplorerElement> empty = new HashSet<ExplorerElement>();
            fromReference.Remove(variable);
            fromReference.Add(variable, empty);
        }

        /// <summary>
        /// Updates a trigger with all variable and trigger refs.
        /// </summary>
        internal void UpdateReferences(ExplorerElement t)
        {
            List<ExplorerElement> explorerElementVariables = new List<ExplorerElement>();

            RemoveReferrer(t);

            var parameters = Triggers.GetParametersFromTrigger(t);
            var variables = Project.CurrentProject.Variables;
            var triggers = Project.CurrentProject.Triggers;
            parameters.ForEach(p =>
            {
                if (p is VariableRef)
                {
                    VariableRef varRef = (VariableRef)p;
                    Variable element = variables.GetVariableById_AllLocals(varRef.VariableId);
                    if (element != null)
                        AddReferrer(t, element);
                }
                else if (p is TriggerRef)
                {
                    TriggerRef tRef = (TriggerRef)p;
                    ExplorerElement element = triggers.GetById(tRef.TriggerId);
                    if(element != null)
                        AddReferrer(t, element.trigger);
                }
            });
        }

        /// <summary>
        /// Refreshes references for a given variable.
        /// </summary>
        internal void UpdateReferences(Variable variable)
        {
            ResetVariableReferences(variable);
            var triggers = Project.CurrentProject.Triggers.GetAll();
            triggers.ForEach(exTrig =>
            {
                var parameters = Triggers.GetParametersFromTrigger(exTrig);
                parameters.ForEach(p =>
                {
                    if (p is VariableRef varRef)
                    {
                        if (varRef.VariableId == variable.Id)
                            AddReferrer(exTrig, variable);
                    }
                });
            });

        }

        internal void UpdateReferencesAll()
        {
            var triggers = Project.CurrentProject.Triggers.GetAll();
            triggers.ForEach(trigger => UpdateReferences(trigger));
        }
    }
}
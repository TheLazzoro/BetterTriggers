using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Containers
{
    public class References
    {
        private Dictionary<ExplorerElementTrigger, HashSet<IReferable>> fromTrigger = new Dictionary<ExplorerElementTrigger, HashSet<IReferable>>();
        private Dictionary<IReferable, HashSet<ExplorerElementTrigger>> fromReference = new Dictionary<IReferable, HashSet<ExplorerElementTrigger>>();

        /// <summary>
        /// Returns a list of trigger which are referencing the given referable.
        /// </summary>
        public List<ExplorerElementTrigger> GetReferrers(IReferable referable)
        {
            HashSet<ExplorerElementTrigger> referrers = new HashSet<ExplorerElementTrigger>();
            fromReference.TryGetValue(referable, out referrers);

            if (referrers == null)
                return new List<ExplorerElementTrigger>();

            return referrers.ToList();
        }

        internal void AddReferrer(ExplorerElementTrigger source, IReferable reference)
        {
            if (reference == null)
                return;

            // Gets a list of references, given the trigger referencing them.
            HashSet<IReferable> references = null;
            if (!fromTrigger.TryGetValue(source, out references)) {
                references = new HashSet<IReferable>();
                fromTrigger.Add(source, references);
            }
            references.Add(reference); // Adds the reference to the trigger's total references.

            // And vice versa; Gets a list of all triggers referencing the referable.
            HashSet<ExplorerElementTrigger> triggers = null;
            fromReference.TryGetValue(reference, out triggers);
            if (triggers == null)
            {
                triggers = new HashSet<ExplorerElementTrigger>();
                fromReference.Add(reference, triggers);
            }
            triggers.Add(source); // Adds the trigger to the referable's total number of referencing triggers.
        }

        /// <summary>
        /// Removes a referrer from all variables. Used when a trigger gets deleted.
        /// </summary>
        internal void RemoveReferrer(ExplorerElementTrigger source)
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

        internal void ResetVariableReferences(Variable variable)
        {
            HashSet<ExplorerElementTrigger> empty = new HashSet<ExplorerElementTrigger>();
            fromReference.Remove(variable);
            fromReference.Add(variable, empty);
        }

        /// <summary>
        /// Updates a trigger with all variable and trigger refs.
        /// </summary>
        internal void UpdateReferences(ExplorerElementTrigger t)
        {
            List<ExplorerElementVariable> explorerElementVariables = new List<ExplorerElementVariable>();

            RemoveReferrer(t);

            var parameters = Project.CurrentProject.Triggers.GetParametersFromTrigger(t);
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
                    ExplorerElementTrigger element = triggers.GetById(tRef.TriggerId);
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
                var parameters = Project.CurrentProject.Triggers.GetParametersFromTrigger(exTrig);
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
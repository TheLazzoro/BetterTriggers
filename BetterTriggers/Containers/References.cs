using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.EditorData.TriggerEditor;
using Cake.Common.Build.AppVeyor;

namespace BetterTriggers.Containers
{
    public class References
    {
        private Dictionary<ExplorerElement, HashSet<IReferable>> fromTrigger = new Dictionary<ExplorerElement, HashSet<IReferable>>();
        private Dictionary<IReferable, HashSet<ExplorerElement>> fromReference = new Dictionary<IReferable, HashSet<ExplorerElement>>();

        /// <summary>
        /// Returns a list of trigger which are referencing the given referable.
        /// </summary>
        public List<ExplorerElement> GetReferrers(IReferable referable)
        {
            HashSet<ExplorerElement> referrers = new HashSet<ExplorerElement>();
            fromReference.TryGetValue(referable, out referrers);

            if (referrers == null)
                return new List<ExplorerElement>();

            return referrers.ToList();
        }

        internal void AddReferrer(ExplorerElement source, IReferable reference)
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
            HashSet<IReferable> references = null;
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
        internal void UpdateReferences(ExplorerElement ex)
        {
            List<ExplorerElement> explorerElementVariables = new List<ExplorerElement>();

            RemoveReferrer(ex);

            var variables = Project.CurrentProject.Variables;
            var triggers = Project.CurrentProject.Triggers;
            var functionDefinitions = Project.CurrentProject.FunctionDefinitions;
            var actionDefinitions = Project.CurrentProject.ActionDefinitions;
            var conditionDefinitions = Project.CurrentProject.ConditionDefinitions;

            var parameters = Parameter.GetParametersFromExplorerElement(ex);
            parameters.ForEach(p =>
            {
                if (p is VariableRef varRef)
                {
                    Variable element = variables.GetVariableById_AllLocals(varRef.VariableId);
                    if (element != null)
                        AddReferrer(ex, element);
                }
                else if (p is TriggerRef tRef)
                {
                    ExplorerElement element = triggers.GetById(tRef.TriggerId);
                    if(element != null)
                        AddReferrer(ex, element.trigger);
                }
                else if(p is FunctionDefinitionRef functionDefRef)
                {
                    ExplorerElement element = functionDefinitions.FindById(functionDefRef.FunctionDefinitionId);
                    if (element != null)
                        AddReferrer(ex, element.functionDefinition);
                }
                else if (p is ParameterDefinitionRef paramDefRef)
                {
                    var paramCollection = ex.GetParameterCollection();
                    var element = paramCollection.GetByReference(paramDefRef);
                    if (element != null)
                        AddReferrer(ex, element);
                }
            });

            var triggerElements = Project.CurrentProject.GetTriggerElementsFromExplorerElement(ex);
            triggerElements.ForEach(t =>
            {
                if(t is ActionDefinitionRef actionDefRef)
                {
                    ExplorerElement element = actionDefinitions.FindById(actionDefRef.ActionDefinitionId);
                    if (element != null)
                        AddReferrer(ex, element.actionDefinition);
                }
                else if (t is ConditionDefinitionRef conditionDefRef)
                {
                    ExplorerElement element = conditionDefinitions.FindById(conditionDefRef.ConditionDefinitionId);
                    if (element != null)
                        AddReferrer(ex, element.conditionDefinition);
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
                var parameters = Parameter.GetParametersFromExplorerElement(exTrig);
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

        /// <summary>
        /// Refreshes references for a given <see cref="FunctionDefinition"/>.
        /// </summary>
        internal void UpdateReferences(FunctionDefinition functionDef)
        {
            HashSet<ExplorerElement> empty = new HashSet<ExplorerElement>();
            fromReference.Remove(functionDef);
            fromReference.Add(functionDef, empty);

            var elements = Project.CurrentProject.GetAllExplorerElements();
            elements.ForEach(ex =>
            {
                var parameters = Parameter.GetParametersFromExplorerElement(ex);
                parameters.ForEach(p =>
                {
                    if (p is FunctionDefinitionRef funcRef)
                    {
                        if (funcRef.FunctionDefinitionId == functionDef.Id)
                            AddReferrer(ex, functionDef);
                    }
                });
            });
        }

        /// <summary>
        /// Refreshes references for a given <see cref="ParameterDefinition"/>.
        /// </summary>
        internal void UpdateReferences(ParameterDefinition parameterDef)
        {
            HashSet<ExplorerElement> empty = new HashSet<ExplorerElement>();
            fromReference.Remove(parameterDef);
            fromReference.Add(parameterDef, empty);

            var elements = Project.CurrentProject.GetAllExplorerElements();
            elements.ForEach(ex =>
            {
                var parameters = Parameter.GetParametersFromExplorerElement(ex);
                parameters.ForEach(p =>
                {
                    if (p is ParameterDefinitionRef paramDefRef)
                    {
                        if (paramDefRef.ParameterDefinitionId == parameterDef.Id)
                            AddReferrer(ex, parameterDef);
                    }
                });
            });
        }

        internal void UpdateReferencesAll()
        {
            var explorerElements = Project.CurrentProject.GetAllExplorerElements();
            explorerElements.ForEach(ex => UpdateReferences(ex));
        }
    }
}
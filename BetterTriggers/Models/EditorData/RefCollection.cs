using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData.TriggerEditor;
using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    /// <summary>
    /// A collection of <see cref="VariableRef"/> or <see cref="TriggerRef"/> with the given Referable.
    /// Refs can be removed and re-added to the <see cref="Function"/> they were attached to.
    /// 
    /// With the addition of <see cref="ActionDefinition"/> and <see cref="ConditionDefinition"/>
    /// refs to those can also be removed and re-added to the <see cref="TriggerElementCollection"/>.
    /// <see cref="FunctionDefinition"/> was also added and can be removed from the parameters of a <see cref="Function"/>.
    /// </summary>
    internal class RefCollection
    {
        public List<ExplorerElement> TriggersToUpdate { get; private set; } = new();
        List<RefParent> refParents = new List<RefParent>();

        internal RefCollection(Variable variable)
        {
            CreateVarRefs(variable);
        }

        internal RefCollection(Variable variable, War3Type newType)
        {
            CreateVarRefs(variable, newType);
        }

        internal RefCollection(Trigger trigger)
        {
            CreateTrigRefs(trigger);
        }

        internal RefCollection(FunctionDefinition functionDefinition)
        {
            CreateFunctionDefRefs(functionDefinition);
        }

        internal RefCollection(ConditionDefinition conditionDefinition)
        {
            CreateConditionDefRefs(conditionDefinition);
        }

        internal RefCollection(ParameterDefinition parameterDefinition)
        {
            CreateParameterDefRefs(parameterDefinition);
        }

        internal RefCollection(ExplorerElement explorerElement)
        {
            if (explorerElement.ElementType == ExplorerElementEnum.GlobalVariable)
                CreateVarRefs(explorerElement.variable);
            else if (explorerElement.ElementType == ExplorerElementEnum.Trigger)
                CreateTrigRefs(explorerElement.trigger);
        }

        private void CreateVarRefs(Variable variable, War3Type newType = null)
        {
            this.TriggersToUpdate = Project.CurrentProject.References.GetReferrers(variable);
            var functions = Project.CurrentProject.GetFunctionsAll();
            functions.ForEach(f =>
            {
                f.parameters.ForEach(p =>
                {
                    if (p is VariableRef varRef)
                    {
                        if (varRef.VariableId == variable.Id)
                        {
                            var refParent = new RefParent(varRef, f, newType);
                            refParents.Add(refParent);
                        }
                    }
                });
            });
        }

        private void CreateTrigRefs(Trigger trigger)
        {
            this.TriggersToUpdate = Project.CurrentProject.References.GetReferrers(trigger);
            var functions = Project.CurrentProject.GetFunctionsAll();
            functions.ForEach(f =>
            {
                f.parameters.ForEach(p =>
                {
                    if (p is TriggerRef trigRef)
                    {
                        if (trigRef.TriggerId == trigger.Id)
                        {
                            var refParent = new RefParent(trigRef, f);
                            refParents.Add(refParent);
                        }
                    }
                });
            });
        }

        private void CreateFunctionDefRefs(FunctionDefinition functionDef)
        {
            this.TriggersToUpdate = Project.CurrentProject.References.GetReferrers(functionDef);
            var functions = Project.CurrentProject.GetFunctionsAll();
            functions.ForEach(f =>
            {
                f.parameters.ForEach(p =>
                {
                    if (p is FunctionDefinitionRef funcDefRef)
                    {
                        if (funcDefRef.FunctionDefinitionId == functionDef.Id)
                        {
                            var refParent = new RefParent(funcDefRef, f);
                            refParents.Add(refParent);
                        }
                    }
                });
            });
        }

        private void CreateConditionDefRefs(ConditionDefinition conditionDef)
        {
            this.TriggersToUpdate = Project.CurrentProject.References.GetReferrers(conditionDef);
            var functions = Project.CurrentProject.GetFunctionsAll();
            var triggerElements = Project.CurrentProject.GetAllTriggerElements();

            triggerElements.ForEach(t =>
            {
                if (t is ConditionDefinitionRef condDefRef)
                {
                    if (condDefRef.ConditionDefinitionId == conditionDef.Id)
                    {
                        var refParent = new RefParent(condDefRef);
                        refParents.Add(refParent);
                    }
                }
            });
        }

        private void CreateParameterDefRefs(ParameterDefinition parameterDef)
        {
            this.TriggersToUpdate = Project.CurrentProject.References.GetReferrers(parameterDef);
            var functions = Project.CurrentProject.GetFunctionsAll();

            functions.ForEach(f =>
            {
                f.parameters.ForEach(p =>
                {
                    if (p is ParameterDefinitionRef condDefRef)
                    {
                        if (condDefRef.ParameterDefinitionId == parameterDef.Id)
                        {
                            var refParent = new RefParent(condDefRef, f);
                            refParents.Add(refParent);
                        }
                    }
                });
            });
        }

        internal void RemoveRefsFromParent()
        {
            refParents.ForEach(r => r.RemoveFromParent());
            TriggersToUpdate.ForEach(t => t.Notify());
        }

        internal void AddRefsToParent()
        {
            refParents.ForEach(r => r.AddToParent());
            TriggersToUpdate.ForEach(t => t.Notify());
        }

        internal void Notify()
        {
            TriggersToUpdate.ForEach(t => t.Notify());
        }
    }

    internal class RefParent
    {
        Parameter parameter;
        Parameter setvarOldValue; // hack for 'SetVariable' value undo/redo
        Function parent;
        int index;
        internal RefParent(Parameter parameter, Function parent, War3Type newType = null)
        {
            this.parameter = parameter;
            this.parent = parent;
            this.index = parent.parameters.IndexOf(parameter);
            if (newType != null && parent.value == "SetVariable" && parameter == parent.parameters[0])
            {
                var varRef = (VariableRef)parameter;
                var variable = Project.CurrentProject.Variables.GetByReference(varRef);
                if (variable.War3Type.Type != newType.Type)
                    setvarOldValue = parent.parameters[1];
            }
        }

        TriggerElement triggerElement;
        TriggerElement parentTrigElement;
        internal RefParent(TriggerElement triggerElement)
        {
            this.triggerElement = triggerElement;
            parentTrigElement = triggerElement.GetParent();
            index = parentTrigElement.Elements.IndexOf(triggerElement);
        }

        internal void RemoveFromParent()
        {
            if (parameter != null)
            {
                parent.parameters.Remove(parameter);
                parent.parameters.Insert(index, new Parameter());
                if (setvarOldValue != null)
                    parent.parameters[1] = new Parameter();
            }
            else if (triggerElement != null)
            {
                parentTrigElement.Elements.Remove(triggerElement);
                parentTrigElement.Elements.Insert(index, new InvalidECA());
            }
        }

        internal void AddToParent()
        {
            if (parameter != null)
            {
                parent.parameters.RemoveAt(index);
                parent.parameters.Insert(index, parameter);
                if (setvarOldValue != null)
                    parent.parameters[1] = setvarOldValue;
            }
            else if (triggerElement != null)
            {
                parentTrigElement.Elements.RemoveAt(index);
                parentTrigElement.Elements.Insert(index, triggerElement);
            }
        }
    }
}

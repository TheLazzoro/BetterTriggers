using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData.TriggerEditor;
using ICSharpCode.Decompiler.IL;
using System;
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
        private Project _project;

        internal RefCollection(Project project, ExplorerElement explorerElement, Variable variable)
        {
            _project = project;
            CreateVarRefs(variable, explorerElement);
        }

        internal RefCollection(Project project, Variable variable, War3Type newType, ExplorerElement explorerElement)
        {
            _project = project;
            CreateVarRefs(variable, explorerElement, newType);
        }

        internal RefCollection(Project project, ExplorerElement explorerElement, ParameterDefinition parameterDefinition)
        {
            _project = project;
            CreateParameterDefRefs(explorerElement, parameterDefinition);
        }

        internal RefCollection(Project project, ExplorerElement explorerElement)
        {
            _project = project;
            if (explorerElement.ElementType == ExplorerElementEnum.GlobalVariable)
                CreateVarRefs(explorerElement.variable, explorerElement);
            else if (explorerElement.ElementType == ExplorerElementEnum.Trigger)
                CreateTrigRefs(explorerElement, explorerElement.trigger);
            else if (explorerElement.ElementType == ExplorerElementEnum.FunctionDefinition)
                CreateFunctionDefRefs(explorerElement, explorerElement.functionDefinition);
            else if (explorerElement.ElementType == ExplorerElementEnum.ActionDefinition)
                CreateActionDefRefs(explorerElement.actionDefinition);
            else if (explorerElement.ElementType == ExplorerElementEnum.ConditionDefinition)
                CreateConditionDefRefs(explorerElement.conditionDefinition);
        }

        private void CreateVarRefs(Variable variable, ExplorerElement explorerElement, War3Type newType = null)
        {
            this.TriggersToUpdate = _project.References.GetReferrers(variable);
            var functions = _project.GetFunctionsAll();
            functions.ForEach(f =>
            {
                f.parameters.ForEach(p =>
                {
                    if (p is VariableRef varRef)
                    {
                        if (varRef.VariableId == variable.Id)
                        {
                            var refParent = new RefParent(_project, varRef, f, explorerElement, newType);
                            refParents.Add(refParent);
                        }
                    }
                });
            });
        }

        private void CreateTrigRefs(ExplorerElement explorerElement, Trigger trigger)
        {
            this.TriggersToUpdate = _project.References.GetReferrers(trigger);
            var functions = _project.GetFunctionsAll();
            functions.ForEach(f =>
            {
                f.parameters.ForEach(p =>
                {
                    if (p is TriggerRef trigRef)
                    {
                        if (trigRef.TriggerId == trigger.Id)
                        {
                            var refParent = new RefParent(_project, trigRef, f, explorerElement);
                            refParents.Add(refParent);
                        }
                    }
                });
            });
        }

        private void CreateFunctionDefRefs(ExplorerElement explorerElement, FunctionDefinition functionDef)
        {
            this.TriggersToUpdate = _project.References.GetReferrers(functionDef);
            var functions = _project.GetFunctionsAll();
            functions.ForEach(f =>
            {
                f.parameters.ForEach(p =>
                {
                    if (p is FunctionDefinitionRef funcDefRef)
                    {
                        if (funcDefRef.FunctionDefinitionId == functionDef.Id)
                        {
                            var refParent = new RefParent(_project, funcDefRef, f, explorerElement);
                            refParents.Add(refParent);
                        }
                    }
                });
            });
        }

        private void CreateActionDefRefs(ActionDefinition actionDef)
        {
            this.TriggersToUpdate = _project.References.GetReferrers(actionDef);
            var functions = _project.GetFunctionsAll();
            var triggerElements = _project.GetAllTriggerElements();

            triggerElements.ForEach(t =>
            {
                if (t is ActionDefinitionRef actionDefRef)
                {
                    if (actionDefRef.ActionDefinitionId == actionDef.Id)
                    {
                        var refParent = new RefParent(actionDef.explorerElement, actionDefRef);
                        refParents.Add(refParent);
                    }
                }
            });
        }

        private void CreateConditionDefRefs(ConditionDefinition conditionDef)
        {
            this.TriggersToUpdate = _project.References.GetReferrers(conditionDef);
            var functions = _project.GetFunctionsAll();
            var triggerElements = _project.GetAllTriggerElements();

            triggerElements.ForEach(t =>
            {
                if (t is ConditionDefinitionRef condDefRef)
                {
                    if (condDefRef.ConditionDefinitionId == conditionDef.Id)
                    {
                        var refParent = new RefParent(conditionDef.explorerElement, condDefRef);
                        refParents.Add(refParent);
                    }
                }
            });
        }

        private void CreateParameterDefRefs(ExplorerElement explorerElement, ParameterDefinition parameterDef)
        {
            this.TriggersToUpdate = _project.References.GetReferrers(parameterDef);
            var functions = _project.GetFunctionsAll();

            functions.ForEach(f =>
            {
                f.parameters.ForEach(p =>
                {
                    if (p is ParameterDefinitionRef condDefRef)
                    {
                        if (condDefRef.ParameterDefinitionId == parameterDef.Id)
                        {
                            var refParent = new RefParent(_project, condDefRef, f, explorerElement);
                            refParents.Add(refParent);
                        }
                    }
                });
            });
        }

        internal void RemoveRefsFromParent()
        {
            refParents.ForEach(r => r.RemoveFromParent());
            TriggersToUpdate.ForEach(t => t.InvokeChange());
        }

        internal void AddRefsToParent()
        {
            refParents.ForEach(r => r.AddToParent());
            TriggersToUpdate.ForEach(t => t.InvokeChange());
        }

        internal void Notify()
        {
            TriggersToUpdate.ForEach(t => t.InvokeChange());
        }


        /// <summary>
        /// Should only be used when refreshing a reference to an
        /// <see cref="ActionDefinition"/>, <see cref="ConditionDefinition"/> or <see cref="FunctionDefinition"/>.
        /// </summary>
        internal void ResetParameters()
        {
            refParents.ForEach(t => t.ResetParameters());
            TriggersToUpdate.ForEach(t => t.InvokeChange());
        }

        /// <summary>
        /// Should only be used when refreshing a reference to an
        /// <see cref="ActionDefinition"/>, <see cref="ConditionDefinition"/> or <see cref="FunctionDefinition"/>.
        /// </summary>
        internal void RevertToOldParameters()
        {
            refParents.ForEach(t => t.RevertToOldParameters());
            TriggersToUpdate.ForEach(t => t.InvokeChange());
        }
    }

    internal class RefParent
    {
        ExplorerElement explorerElement;
        Project _project;
        Parameter parameter;
        Parameter setvarOldValue; // hack for 'SetVariable' value undo/redo
        Function parent;
        int index;
        internal RefParent(Project project, Parameter parameter, Function parent, ExplorerElement explorerElement, War3Type newType = null)
        {
            _project = project;
            this.explorerElement = explorerElement;
            this.parameter = parameter;
            this.parent = parent;
            this.index = parent.parameters.IndexOf(parameter);
            if (newType != null && parent.value == "SetVariable" && parameter == parent.parameters[0])
            {
                var varRef = (VariableRef)parameter;
                var variable = _project.Variables.GetByReference(varRef, explorerElement);
                if (variable.War3Type.Type != newType.Type)
                    setvarOldValue = parent.parameters[1];
            }
        }

        TriggerElement triggerElement;
        TriggerElement parentTrigElement;
        internal RefParent(ExplorerElement explorerElement, TriggerElement triggerElement)
        {
            this.explorerElement = explorerElement;
            this.triggerElement = triggerElement;
            parentTrigElement = triggerElement.GetParent();
            index = parentTrigElement.Elements.IndexOf(triggerElement);
        }

        internal void RemoveFromParent()
        {
            if (parameter != null && parent.parameters.Contains(parameter))
            {
                parent.parameters.Remove(parameter);
                parent.parameters.Insert(index, new Parameter());
                if (setvarOldValue != null)
                    parent.parameters[1] = new Parameter();
            }
            else if (triggerElement != null)
            {
                triggerElement.RemoveFromParent();
                var invalid = new InvalidECA();
                invalid.SetParent(parentTrigElement, index);
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
                triggerElement.SetParent(parentTrigElement, index);
            }
        }

        List<Parameter> oldParameters;
        List<Parameter> resetParameters;
        internal void ResetParameters()
        {
            Function function = null;
            oldParameters = new List<Parameter>();
            resetParameters = new List<Parameter>();
            if(parameter is ParameterDefinitionRef paramDefRef || parameter is VariableRef || parameter is TriggerRef)
            {
                function = parent;
                for (int i = 0; i < parent.parameters.Count; i++)
                {
                    var _parameter = parent.parameters[i];
                    oldParameters.Add(_parameter);
                    if (_parameter == parameter)
                    {
                        resetParameters.Add(new Parameter());
                    }
                    else
                    {
                        resetParameters.Add(_parameter);
                    }
                }
            }
            else if (parameter != null)
            {
                function = (Function)parameter;
                oldParameters = function.parameters;
                var functionDefRef = (FunctionDefinitionRef)function;
                var functionDef = _project.FunctionDefinitions.GetByReference(functionDefRef);
                var parameters = functionDef.GetParameterCollection();
                parameters.Elements.ForEach(p => resetParameters.Add(new Parameter()));
            }
            else if (triggerElement != null)
            {
                ECA eca = (ECA)triggerElement;
                function = eca.function;
                switch (triggerElement)
                {
                    case ActionDefinitionRef actionDefRef:
                        oldParameters = actionDefRef.function.parameters;
                        var actionDef = _project.ActionDefinitions.GetByReference(actionDefRef);
                        var parameters = actionDef.GetParameterCollection();
                        parameters.Elements.ForEach(p => resetParameters.Add(new Parameter()));
                        break;
                    case ConditionDefinitionRef conditionDefRef:
                        oldParameters = conditionDefRef.function.parameters;
                        var conditionDef = _project.ConditionDefinitions.GetByReference(conditionDefRef);
                        var parameters1 = conditionDef.GetParameterCollection();
                        parameters1.Elements.ForEach(p => resetParameters.Add(new Parameter()));
                        break;
                    default:
                        break;
                }
            }

            function.parameters = resetParameters;
        }

        internal void RevertToOldParameters()
        {
            if (parameter is ParameterDefinitionRef || parameter is VariableRef || parameter is TriggerRef)
            {
                parent.parameters = oldParameters;
            }
            else if (parameter != null)
            {
                var function = (Function)parameter;
                function.parameters = oldParameters;
            }
            else if (triggerElement != null)
            {
                ECA eca = (ECA)triggerElement;
                eca.function.parameters = oldParameters;
            }
        }
    }
}

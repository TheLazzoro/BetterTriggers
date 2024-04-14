using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.EditorData.TriggerEditor;
using BetterTriggers.WorldEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Utility
{
    /// <summary>
    /// Validates a trigger's parameters and removes invalid ones.
    /// </summary>
    public class TriggerValidator
    {
        private bool _includeUnsetParameters;
        private ExplorerElement _explorerElement;
        private Trigger _trigger;
        private ActionDefinition _actionDefinition;
        private ConditionDefinition _conditionDefinition;
        private FunctionDefinition _functionDefinition;

        public TriggerValidator(ExplorerElement explorerElement, bool validateUnsetParameters = false)
        {
            _includeUnsetParameters = validateUnsetParameters;
            _explorerElement = explorerElement;
            _trigger = explorerElement.trigger;
            _actionDefinition = explorerElement.actionDefinition;
            _conditionDefinition = explorerElement.conditionDefinition;
            _functionDefinition = explorerElement.functionDefinition;
        }

        /// <returns>Whether the trigger had invalid references removed.</returns>
        public int RemoveInvalidReferences()
        {
            int removeCount = 0;
            switch (_explorerElement.ElementType)
            {
                case ExplorerElementEnum.Trigger:
                    removeCount += RemoveInvalidReferences(_trigger.Events);
                    removeCount += RemoveInvalidReferences(_trigger.Conditions);
                    removeCount += RemoveInvalidReferences(_trigger.LocalVariables);
                    removeCount += RemoveInvalidReferences(_trigger.Actions);
                    break;
                case ExplorerElementEnum.ActionDefinition:
                    removeCount += RemoveInvalidReferences(_actionDefinition.LocalVariables);
                    removeCount += RemoveInvalidReferences(_actionDefinition.Actions);
                    break;
                case ExplorerElementEnum.ConditionDefinition:
                    removeCount += RemoveInvalidReferences(_conditionDefinition.LocalVariables);
                    removeCount += RemoveInvalidReferences(_conditionDefinition.Actions);
                    break;
                case ExplorerElementEnum.FunctionDefinition:
                    removeCount += RemoveInvalidReferences(_functionDefinition.LocalVariables);
                    removeCount += RemoveInvalidReferences(_functionDefinition.Actions);
                    break;
                default:
                    break;
            }

            return removeCount;
        }


        public int RemoveInvalidReferences(TriggerElement triggerElement)
        {
            int removeCount = 0;

            for (int i = 0; i < triggerElement.Elements.Count; i++)
            {
                if (triggerElement.Elements[i] is LocalVariable localVar)
                {
                    if (localVar.variable.InitialValue is Value value)
                    {
                        bool dataExists = CustomMapData.ReferencedDataExists(value, localVar.variable.Type);
                        if (!dataExists)
                        {
                            localVar.variable.InitialValue = new Parameter();
                            removeCount += 1;
                        }
                    }
                    continue;
                }
                else if(triggerElement.Elements[i] is ParameterDefinition)
                {
                    continue;
                }

                var eca = (ECA)triggerElement.Elements[i];
                bool ecaExists = TriggerData.FunctionExists(eca.function);
                if (!ecaExists)
                {
                    triggerElement.Elements[i] = new InvalidECA();
                    removeCount += 1;
                }
                List<string> returnTypes = TriggerData.GetParameterReturnTypes(eca.function);
                int invalidCount = VerifyParametersAndRemove(eca.function.parameters, returnTypes);
                eca.HasErrors = invalidCount > 0;
                removeCount += invalidCount;


                if (eca is IfThenElse)
                {
                    var special = (IfThenElse)eca;
                    removeCount += RemoveInvalidReferences(special.If);
                    removeCount += RemoveInvalidReferences(special.Then);
                    removeCount += RemoveInvalidReferences(special.Else);
                }
                else if (eca is AndMultiple)
                {
                    var special = (AndMultiple)eca;
                    removeCount += RemoveInvalidReferences(special.And);
                }
                else if (eca is ForForceMultiple)
                {
                    var special = (ForForceMultiple)eca;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (eca is ForGroupMultiple)
                {
                    var special = (ForGroupMultiple)eca;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (eca is ForLoopAMultiple)
                {
                    var special = (ForLoopAMultiple)eca;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (eca is ForLoopBMultiple)
                {
                    var special = (ForLoopBMultiple)eca;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (eca is ForLoopVarMultiple)
                {
                    var special = (ForLoopVarMultiple)eca;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (eca is OrMultiple)
                {
                    var special = (OrMultiple)eca;
                    removeCount += RemoveInvalidReferences(special.Or);
                }
                else if (eca is EnumDestructablesInRectAllMultiple)
                {
                    var special = (EnumDestructablesInRectAllMultiple)eca;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (eca is EnumDestructiblesInCircleBJMultiple)
                {
                    var special = (EnumDestructiblesInCircleBJMultiple)eca;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (eca is EnumItemsInRectBJ)
                {
                    var special = (EnumItemsInRectBJ)eca;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
            }

            return removeCount;
        }


        /// <param name="parameters"></param>
        /// <param name="returnTypes"></param>
        /// <returns></returns>
        private int VerifyParametersAndRemove(List<Parameter> parameters, List<string> returnTypes)
        {
            int removeCount = 0;

            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];


                if (parameter is VariableRef varRef)
                {
                    Variable variable = Project.CurrentProject.Variables.GetById(varRef.VariableId, _explorerElement);
                    if (variable == null)
                    {
                        removeCount++;
                        parameters[i] = new Parameter();
                    }
                    else
                    {
                        List<Parameter> arrays = new();
                        List<string> returnTypes_Array = new();
                        if (variable.IsArray)
                        {
                            arrays.Add(varRef.arrayIndexValues[0]);
                            returnTypes_Array.Add("integer");
                        }
                        if (variable.IsArray && variable.IsTwoDimensions)
                        {
                            arrays.Add(varRef.arrayIndexValues[1]);
                            returnTypes_Array.Add("integer");
                        }

                        removeCount += VerifyParametersAndRemove(arrays, returnTypes_Array);
                    }
                }
                else if (parameter is TriggerRef)
                {
                    var trigger = Project.CurrentProject.Triggers.GetByReference(parameter as TriggerRef);
                    if (trigger == null || trigger.trigger == null)
                    {
                        removeCount++;
                        parameters[i] = new Parameter();
                    }
                }
                else if (parameter is ParameterDefinitionRef paramDefRef)
                {
                    var paramDefCollection = _explorerElement.GetParameterCollection();
                    if (paramDefCollection == null)
                    {
                        removeCount++;
                        parameters[i] = new Parameter();
                    }
                    else
                    {
                        var reference = paramDefCollection.GetByReference(paramDefRef);
                        if(reference == null)
                        {
                            removeCount++;
                            parameters[i] = new Parameter();
                        }
                    }
                }
                else if (parameter is Value value)
                {
                    bool refExists = CustomMapData.ReferencedDataExists(value, returnTypes[i]);
                    if (!refExists)
                    {
                        removeCount++;
                        parameters[i] = new Parameter();
                    }

                }
                else if (parameter is Function function)
                {
                    bool functionExists = TriggerData.FunctionExists(function);
                    if (!functionExists)
                    {
                        parameters[i] = new Parameter();
                        removeCount++;
                    }

                    List<string> _returnTypes = TriggerData.GetParameterReturnTypes(function);
                    removeCount += VerifyParametersAndRemove(function.parameters, _returnTypes);
                }
                else if (parameter is Preset preset)
                {
                    bool constantExists = TriggerData.ConstantExists(preset.value);
                    if (!constantExists)
                    {
                        parameters[i] = new Parameter();
                        removeCount++;
                    }
                }
                else if (_includeUnsetParameters)
                {
                    removeCount++;
                }
            }

            return removeCount;
        }
    }
}

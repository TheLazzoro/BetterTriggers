using BetterTriggers.Models.EditorData.TriggerEditor;
using BetterTriggers.Models.SaveableData;
using Cake.Incubator.AssertExtensions;
using ICSharpCode.Decompiler.DebugInfo;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace BetterTriggers.Models.EditorData
{
    public static class TriggerSerializer
    {
        #region Serializers

        /// <summary>
        /// Transforms a trigger into a saveable trigger.
        /// </summary>
        public static string SerializeTrigger(Trigger trigger)
        {
            SaveableData.Trigger_Saveable saveableTrig = new Trigger_Saveable();
            saveableTrig.Id = trigger.Id;
            saveableTrig.Script = trigger.Script;
            saveableTrig.IsScript = trigger.IsScript;
            saveableTrig.RunOnMapInit = trigger.RunOnMapInit;
            saveableTrig.Comment = trigger.Comment;

            saveableTrig.Events = ConvertTriggerElements(trigger.Events.Elements);
            saveableTrig.Conditions = ConvertTriggerElements(trigger.Conditions.Elements);
            saveableTrig.LocalVariables = ConvertTriggerElements(trigger.LocalVariables.Elements);
            saveableTrig.Actions = ConvertTriggerElements(trigger.Actions.Elements);

            return JsonConvert.SerializeObject(saveableTrig, Formatting.Indented);
        }

        internal static string SerializeVariable(Variable variable)
        {
            Variable_Saveable converted = new Variable_Saveable();
            converted.Id = variable.Id;
            converted.Name = variable.Name;
            converted.Type = variable.Type;
            converted.IsArray = variable.IsArray;
            converted.IsTwoDimensions = variable.IsTwoDimensions;
            converted.ArraySize = variable.ArraySize;
            converted.InitialValue = ConvertParameter(variable.InitialValue);

            return JsonConvert.SerializeObject(converted, Formatting.Indented);
        }

        internal static string SerializeActionDefinition(ActionDefinition actionDefinition)
        {
            var converted = new ActionDefinition_Saveable();
            converted.Id = actionDefinition.Id;
            converted.Comment = actionDefinition.Comment;
            converted.Parameters = ConvertParameterDefinitions(actionDefinition.Parameters);
            converted.Actions = ConvertTriggerElements(actionDefinition.Actions.Elements);
            converted.LocalVariables = ConvertTriggerElements(actionDefinition.LocalVariables.Elements);

            return JsonConvert.SerializeObject(converted, Formatting.Indented);
        }

        internal static string SerializeConditionDefinition(ConditionDefinition conditionDefinition)
        {
            var converted = new ConditionDefinition_Saveable();
            converted.Id = conditionDefinition.Id;
            converted.Comment = conditionDefinition.Comment;
            converted.Parameters = ConvertParameterDefinitions(conditionDefinition.Parameters);
            converted.Actions = ConvertTriggerElements(conditionDefinition.Actions.Elements);
            converted.LocalVariables = ConvertTriggerElements(conditionDefinition.LocalVariables.Elements);

            return JsonConvert.SerializeObject(converted, Formatting.Indented);
        }

        internal static string SerializeFunctionDefinition(FunctionDefinition functionDefinition)
        {
            var converted = new FunctionDefinition_Saveable();
            converted.Id = functionDefinition.Id;
            converted.Comment = functionDefinition.Comment;
            converted.ReturnType = functionDefinition.ReturnType.War3Type.Type;
            converted.Category = functionDefinition.Category;
            converted.Parameters = ConvertParameterDefinitions(functionDefinition.Parameters);
            converted.Actions = ConvertTriggerElements(functionDefinition.Actions.Elements);
            converted.LocalVariables = ConvertTriggerElements(functionDefinition.LocalVariables.Elements);

            return JsonConvert.SerializeObject(converted, Formatting.Indented);
        }

        private static List<TriggerElement_Saveable> ConvertTriggerElements(ObservableCollection<TriggerElement> elements)
        {
            List<TriggerElement_Saveable> list = new();
            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                TriggerElement_Saveable converted = null;

                if (element is ECA eca)
                {
                    ECA_Saveable ECA_Saveable = new();
                    switch (eca)
                    {
                        case AndMultiple thing:
                            AndMultiple_Saveable andMultiple_Saveable = new();
                            andMultiple_Saveable.And = ConvertTriggerElements(thing.And.Elements);
                            ECA_Saveable = andMultiple_Saveable;
                            break;
                        case EnumDestructablesInRectAllMultiple thing:
                            EnumDestructablesInRectAllMultiple_Saveable EnumDestRect_Saveable = new();
                            EnumDestRect_Saveable.Actions = ConvertTriggerElements(thing.Actions.Elements);
                            ECA_Saveable = EnumDestRect_Saveable;
                            break;
                        case EnumDestructiblesInCircleBJMultiple thing:
                            EnumDestructiblesInCircleBJMultiple_Saveable EnumDestCircle_Saveable = new();
                            EnumDestCircle_Saveable.Actions = ConvertTriggerElements(thing.Actions.Elements);
                            ECA_Saveable = EnumDestCircle_Saveable;
                            break;
                        case EnumItemsInRectBJ thing:
                            EnumItemsInRectBJ_Saveable EnumItemsInRectBJ_Saveable = new();
                            EnumItemsInRectBJ_Saveable.Actions = ConvertTriggerElements(thing.Actions.Elements);
                            ECA_Saveable = EnumItemsInRectBJ_Saveable;
                            break;
                        case ForForceMultiple thing:
                            ForForceMultiple_Saveable ForForceMultiple_Saveable = new();
                            ForForceMultiple_Saveable.Actions = ConvertTriggerElements(thing.Actions.Elements);
                            ECA_Saveable = ForForceMultiple_Saveable;
                            break;
                        case ForGroupMultiple thing:
                            ForGroupMultiple_Saveable ForGroupMultiple_Saveable = new();
                            ForGroupMultiple_Saveable.Actions = ConvertTriggerElements(thing.Actions.Elements);
                            ECA_Saveable = ForGroupMultiple_Saveable;
                            break;
                        case ForLoopAMultiple thing:
                            ForLoopAMultiple_Saveable ForLoopAMultiple_Saveable = new();
                            ForLoopAMultiple_Saveable.Actions = ConvertTriggerElements(thing.Actions.Elements);
                            ECA_Saveable = ForLoopAMultiple_Saveable;
                            break;
                        case ForLoopBMultiple thing:
                            ForLoopBMultiple_Saveable ForLoopBMultiple_Saveable = new();
                            ForLoopBMultiple_Saveable.Actions = ConvertTriggerElements(thing.Actions.Elements);
                            ECA_Saveable = ForLoopBMultiple_Saveable;
                            break;
                        case ForLoopVarMultiple thing:
                            ForLoopVarMultiple_Saveable ForLoopVarMultiple_Saveable = new();
                            ForLoopVarMultiple_Saveable.Actions = ConvertTriggerElements(thing.Actions.Elements);
                            ECA_Saveable = ForLoopVarMultiple_Saveable;
                            break;
                        case IfThenElse thing:
                            IfThenElse_Saveable IfThenElse_Saveable = new();
                            IfThenElse_Saveable.If = ConvertTriggerElements(thing.If.Elements);
                            IfThenElse_Saveable.Then = ConvertTriggerElements(thing.Then.Elements);
                            IfThenElse_Saveable.Else = ConvertTriggerElements(thing.Else.Elements);
                            ECA_Saveable = IfThenElse_Saveable;
                            break;
                        case OrMultiple thing:
                            OrMultiple_Saveable OrMultiple_Saveable = new();
                            OrMultiple_Saveable.Or = ConvertTriggerElements(thing.Or.Elements);
                            ECA_Saveable = OrMultiple_Saveable;
                            break;
                        case SetVariable:
                            SetVariable_Saveable SetVariable_Saveable = new();
                            ECA_Saveable = SetVariable_Saveable;
                            break;
                        case ReturnStatement:
                            ReturnStatement_Saveable ReturnStatement_Saveable = new();
                            ECA_Saveable = ReturnStatement_Saveable;
                            break;
                        case ActionDefinitionRef thing:
                            ActionDefinitionRef_Saveable ActionDefinitionRef_Saveable = new();
                            ActionDefinitionRef_Saveable.ActionDefinitionId = thing.ActionDefinitionId;
                            ECA_Saveable = ActionDefinitionRef_Saveable;
                            break;
                        case ConditionDefinitionRef thing:
                            ConditionDefinitionRef_Saveable ConditionDefinitionRef_Saveable = new();
                            ConditionDefinitionRef_Saveable.ConditionDefinitionId = thing.ConditionDefinitionId;
                            ECA_Saveable = ConditionDefinitionRef_Saveable;
                            break;
                        default:
                            break;
                    }

                    ECA_Saveable.function = ConvertFunction(eca.function);
                    ECA_Saveable.isEnabled = eca.IsEnabled;
                    converted = ECA_Saveable;
                }
                else if (element is LocalVariable localVar)
                {
                    converted = new LocalVariable_Saveable
                    {
                        variable = new Variable_Saveable
                        {
                            Id = localVar.variable.Id,
                            Name = localVar.variable.Name,
                            ArraySize = localVar.variable.ArraySize,
                            IsTwoDimensions = localVar.variable.IsTwoDimensions,
                            IsArray = localVar.variable.IsArray,
                            Type = localVar.variable.Type,
                            InitialValue = ConvertParameter(localVar.variable.InitialValue)
                        }
                    };
                }

                if (converted != null)
                    list.Add(converted);

            }

            return list;
        }

        private static List<ParameterDefinition_Saveable> ConvertParameterDefinitions(ParameterDefinitionCollection paramCollection)
        {
            var list = new List<ParameterDefinition_Saveable>();
            for (int i = 0; i < paramCollection.Count(); i++)
            {
                var paramDefinition = (ParameterDefinition)paramCollection.Elements[i];
                var saveable = new ParameterDefinition_Saveable
                {
                    Id = paramDefinition.Id,
                    Name = paramDefinition.Name,
                    ReturnType = paramDefinition.ReturnType.Type
                };
                list.Add(saveable);
            }
            return list;
        }

        private static Function_Saveable ConvertFunction(Function function)
        {
            Function_Saveable converted = new Function_Saveable();
            converted.value = function.value;
            for (int i = 0; i < function.parameters.Count; i++)
            {
                var converted_param = ConvertParameter(function.parameters[i]);
                converted.parameters.Add(converted_param);
            }

            return converted;
        }

        private static Parameter_Saveable ConvertParameter(Parameter parameter)
        {
            var converted_param = new Parameter_Saveable();
            switch (parameter)
            {
                case Function func:
                    converted_param = ConvertFunction(func);
                    break;
                case Preset:
                    Constant_Saveable constant = new();
                    converted_param = constant;
                    break;
                case VariableRef variableRef:
                    VariableRef_Saveable variableRef_Saveable = new();
                    variableRef_Saveable.VariableId = variableRef.VariableId;
                    variableRef.arrayIndexValues.ForEach(varRef =>
                    {
                        var _param = ConvertParameter(varRef);
                        variableRef_Saveable.arrayIndexValues.Add(_param);
                    });
                    converted_param = variableRef_Saveable;
                    break;
                case TriggerRef triggerRef:
                    TriggerRef_Saveable triggerRef_Saveable = new();
                    triggerRef_Saveable.TriggerId = triggerRef.TriggerId;
                    converted_param = triggerRef_Saveable;
                    break;
                case ParameterDefinitionRef paramRef:
                    ParameterDefinitionRef_Saveable paramRef_Saveable = new();
                    paramRef_Saveable.ParameterDefinitionId = paramRef.ParameterDefinitionId;
                    converted_param = paramRef_Saveable;
                    break;
                case Value:
                    Value_Saveable value = new();
                    converted_param = value;
                    break;
                default:
                    break;
            }

            converted_param.value = parameter.value;
            return converted_param;
        }


        #endregion

        #region Deserializers

        public static Trigger Deserialize(Trigger_Saveable saveableTrig)
        {
            Trigger trigger = new Trigger();
            trigger.Id = saveableTrig.Id;
            trigger.Script = saveableTrig.Script;
            trigger.IsScript = saveableTrig.IsScript;
            trigger.RunOnMapInit = saveableTrig.RunOnMapInit;
            trigger.Comment = saveableTrig.Comment;

            trigger.Events = ConvertTriggerElements_Deserialize(saveableTrig.Events, TriggerElementType.Event);
            trigger.Conditions = ConvertTriggerElements_Deserialize(saveableTrig.Conditions, TriggerElementType.Condition);
            trigger.LocalVariables = ConvertTriggerElements_Deserialize(saveableTrig.LocalVariables, TriggerElementType.LocalVariable);
            trigger.Actions = ConvertTriggerElements_Deserialize(saveableTrig.Actions, TriggerElementType.Action);

            return trigger;
        }

        public static Variable DeserializeVariable(Variable_Saveable saveableVariable)
        {
            Variable variable = new Variable();
            variable.Id = saveableVariable.Id;
            variable.Name = saveableVariable.Name;
            variable.Type = saveableVariable.Type;
            variable.IsArray = saveableVariable.IsArray;
            variable.IsTwoDimensions = saveableVariable.IsTwoDimensions;
            variable.ArraySize = saveableVariable.ArraySize;
            variable.InitialValue = ConvertParameter_Deserialize(saveableVariable.InitialValue);

            return variable;
        }

        internal static ActionDefinition DeserializeActionDefinition(ExplorerElement explorerElement, ActionDefinition_Saveable saveableActionDef)
        {
            var converted = new ActionDefinition(explorerElement);
            converted.explorerElement = explorerElement;
            converted.Id = saveableActionDef.Id;
            converted.Comment = saveableActionDef.Comment;
            converted.Parameters = ConvertParameterDefinitions_Deserialize(saveableActionDef.Parameters);
            converted.Actions = ConvertTriggerElements_Deserialize(saveableActionDef.Actions, TriggerElementType.Action);
            converted.LocalVariables = ConvertTriggerElements_Deserialize(saveableActionDef.LocalVariables, TriggerElementType.LocalVariable);

            return converted;
        }

        internal static ConditionDefinition DeserializeConditionDefinition(ExplorerElement explorerElement, ConditionDefinition_Saveable saveableConditionDef)
        {
            var converted = new ConditionDefinition(explorerElement);
            converted.Id = saveableConditionDef.Id;
            converted.Comment = saveableConditionDef.Comment;
            converted.Actions = ConvertTriggerElements_Deserialize(saveableConditionDef.Actions, TriggerElementType.Action);
            converted.LocalVariables = ConvertTriggerElements_Deserialize(saveableConditionDef.LocalVariables, TriggerElementType.LocalVariable);

            return converted;
        }

        internal static FunctionDefinition DeserializeFunctionDefinition(FunctionDefinition_Saveable saveableFunctionDef)
        {
            var converted = new FunctionDefinition();
            converted.Id = saveableFunctionDef.Id;
            converted.Comment = saveableFunctionDef.Comment;
            converted.Category = saveableFunctionDef.Category;
            converted.ReturnType = new ReturnType(saveableFunctionDef.ReturnType);
            converted.Actions = ConvertTriggerElements_Deserialize(saveableFunctionDef.Actions, TriggerElementType.Action);
            converted.LocalVariables = ConvertTriggerElements_Deserialize(saveableFunctionDef.LocalVariables, TriggerElementType.LocalVariable);

            return converted;
        }

        private static TriggerElementCollection ConvertTriggerElements_Deserialize(List<TriggerElement_Saveable> elements, TriggerElementType type)
        {
            TriggerElementCollection collection = new(type);

            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                TriggerElement converted = null;
                if (element is ECA_Saveable ECA_Saveable)
                {
                    ECA eca = new();
                    switch (ECA_Saveable)
                    {
                        case AndMultiple_Saveable thing:
                            AndMultiple andMultiple = new();
                            andMultiple.And = ConvertTriggerElements_Deserialize(thing.And, TriggerElementType.Condition);
                            eca = andMultiple;
                            break;
                        case EnumDestructablesInRectAllMultiple_Saveable thing:
                            EnumDestructablesInRectAllMultiple EnumDestRect = new();
                            EnumDestRect.Actions = ConvertTriggerElements_Deserialize(thing.Actions, TriggerElementType.Action);
                            eca = EnumDestRect;
                            break;
                        case EnumDestructiblesInCircleBJMultiple_Saveable thing:
                            EnumDestructiblesInCircleBJMultiple EnumDestCircle = new();
                            EnumDestCircle.Actions = ConvertTriggerElements_Deserialize(thing.Actions, TriggerElementType.Action);
                            eca = EnumDestCircle;
                            break;
                        case EnumItemsInRectBJ_Saveable thing:
                            EnumItemsInRectBJ EnumItemsInRectBJ = new();
                            EnumItemsInRectBJ.Actions = ConvertTriggerElements_Deserialize(thing.Actions, TriggerElementType.Action);
                            eca = EnumItemsInRectBJ;
                            break;
                        case ForForceMultiple_Saveable thing:
                            ForForceMultiple ForForceMultiple = new();
                            ForForceMultiple.Actions = ConvertTriggerElements_Deserialize(thing.Actions, TriggerElementType.Action);
                            eca = ForForceMultiple;
                            break;
                        case ForGroupMultiple_Saveable thing:
                            ForGroupMultiple ForGroupMultiple = new();
                            ForGroupMultiple.Actions = ConvertTriggerElements_Deserialize(thing.Actions, TriggerElementType.Action);
                            eca = ForGroupMultiple;
                            break;
                        case ForLoopAMultiple_Saveable thing:
                            ForLoopAMultiple ForLoopAMultiple = new();
                            ForLoopAMultiple.Actions = ConvertTriggerElements_Deserialize(thing.Actions, TriggerElementType.Action);
                            eca = ForLoopAMultiple;
                            break;
                        case ForLoopBMultiple_Saveable thing:
                            ForLoopBMultiple ForLoopBMultiple = new();
                            ForLoopBMultiple.Actions = ConvertTriggerElements_Deserialize(thing.Actions, TriggerElementType.Action);
                            eca = ForLoopBMultiple;
                            break;
                        case ForLoopVarMultiple_Saveable thing:
                            ForLoopVarMultiple ForLoopVarMultiple = new();
                            ForLoopVarMultiple.Actions = ConvertTriggerElements_Deserialize(thing.Actions, TriggerElementType.Action);
                            eca = ForLoopVarMultiple;
                            break;
                        case IfThenElse_Saveable thing:
                            IfThenElse IfThenElse_Saveable = new();
                            IfThenElse_Saveable.If = ConvertTriggerElements_Deserialize(thing.If, TriggerElementType.Condition);
                            IfThenElse_Saveable.Then = ConvertTriggerElements_Deserialize(thing.Then, TriggerElementType.Action);
                            IfThenElse_Saveable.Else = ConvertTriggerElements_Deserialize(thing.Else, TriggerElementType.Action);
                            eca = IfThenElse_Saveable;
                            break;
                        case OrMultiple_Saveable thing:
                            OrMultiple OrMultiple_Saveable = new();
                            OrMultiple_Saveable.Or = ConvertTriggerElements_Deserialize(thing.Or, TriggerElementType.Condition);
                            eca = OrMultiple_Saveable;
                            break;
                        case SetVariable_Saveable:
                            SetVariable SetVariable = new();
                            eca = SetVariable;
                            break;
                        case ReturnStatement_Saveable:
                            ReturnStatement ReturnStatement = new();
                            eca = ReturnStatement;
                            break;
                        case ActionDefinitionRef_Saveable thing:
                            ActionDefinitionRef ActionDefinitionRef = new();
                            ActionDefinitionRef.ActionDefinitionId = thing.ActionDefinitionId;
                            eca = ActionDefinitionRef;
                            break;
                        case ConditionDefinitionRef_Saveable thing:
                            ConditionDefinitionRef ConditionDefinitionRef = new();
                            ConditionDefinitionRef.ConditionDefinitionId = thing.ConditionDefinitionId;
                            eca = ConditionDefinitionRef;
                            break;
                        default:
                            break;
                    }

                    eca.function = ConvertFunction_Deserialize(ECA_Saveable.function);
                    eca.IsEnabled = ECA_Saveable.isEnabled;
                    eca.ElementType = type;
                    converted = eca;
                }
                else if (element is LocalVariable_Saveable localVar)
                {
                    var variable = new Variable
                    {
                        _isLocal = true,
                        Id = localVar.variable.Id,
                        Name = localVar.variable.Name,
                        ArraySize = localVar.variable.ArraySize,
                        IsTwoDimensions = localVar.variable.IsTwoDimensions,
                        IsArray = localVar.variable.IsArray,
                        Type = localVar.variable.Type,
                        InitialValue = ConvertParameter_Deserialize(localVar.variable.InitialValue)
                    };
                    converted = new LocalVariable(variable)
                    {
                        DisplayText = localVar.variable.Name,
                        ElementType = TriggerElementType.LocalVariable,
                    };
                }

                if (converted != null)
                    converted.SetParent(collection, i);

            }

            return collection;
        }

        private static ParameterDefinitionCollection ConvertParameterDefinitions_Deserialize(List<ParameterDefinition_Saveable> parameter_Saveables)
        {
            var paramCollection = new ParameterDefinitionCollection(TriggerElementType.ParameterDef);
            for (int i = 0; i < parameter_Saveables.Count; i++)
            {
                var saved = parameter_Saveables[i];
                var paramDef = new ParameterDefinition
                {
                    Id = saved.Id,
                    ReturnType = War3Type.Get(saved.ReturnType),
                    ElementType = TriggerElementType.ParameterDef,
                    Name = saved.Name,
                };
                paramDef.SetParent(paramCollection, i);
            }
            return paramCollection;
        }

        private static Function ConvertFunction_Deserialize(Function_Saveable function_Saveable)
        {
            Function converted = new Function();
            converted.value = function_Saveable.value;
            for (int i = 0; i < function_Saveable.parameters.Count; i++)
            {
                var converted_param = ConvertParameter_Deserialize(function_Saveable.parameters[i]);
                converted.parameters.Add(converted_param);
            }

            return converted;
        }

        private static Parameter ConvertParameter_Deserialize(Parameter_Saveable parameter_Saveable)
        {
            var converted_param = new Parameter();
            switch (parameter_Saveable)
            {
                case Function_Saveable func:
                    converted_param = ConvertFunction_Deserialize(func);
                    break;
                case Constant_Saveable:
                    Preset preset = new();
                    converted_param = preset;
                    break;
                case VariableRef_Saveable variableRef_Saveable:
                    VariableRef variableRef = new();
                    variableRef.VariableId = variableRef_Saveable.VariableId;
                    variableRef_Saveable.arrayIndexValues.ForEach(varRef =>
                    {
                        var _param = ConvertParameter_Deserialize(varRef);
                        variableRef.arrayIndexValues.Add(_param);
                    });
                    converted_param = variableRef;
                    break;
                case TriggerRef_Saveable triggerRef_Saveable:
                    TriggerRef triggerRef = new();
                    triggerRef.TriggerId = triggerRef_Saveable.TriggerId;
                    converted_param = triggerRef;
                    break;
                case ParameterDefinitionRef_Saveable paramDefRef_Saveable:
                    ParameterDefinitionRef paramRef = new();
                    paramRef.ParameterDefinitionId = paramDefRef_Saveable.ParameterDefinitionId;
                    converted_param = paramRef;
                    break;
                case Value_Saveable:
                    Value value = new();
                    converted_param = value;
                    break;
                default:
                    break;
            }

            converted_param.value = parameter_Saveable.value;
            return converted_param;
        }

        #endregion
    }
}

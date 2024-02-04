using BetterTriggers.Models.SaveableData;
using ICSharpCode.Decompiler.IL;
using Microsoft.CodeAnalysis.Operations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public static class TriggerSerializer
    {
        #region Serializer

        /// <summary>
        /// Transforms a trigger into a saveable trigger.
        /// </summary>
        public static string Serialize(EditorData.Trigger trigger)
        {
            SaveableData.Trigger_Saveable saveableTrig = new SaveableData.Trigger_Saveable();
            saveableTrig.Id = trigger.Id;
            saveableTrig.Script = trigger.Script;
            saveableTrig.IsScript = trigger.IsScript;

            saveableTrig.Events = ConvertTriggerElements(trigger.Events.Elements);
            saveableTrig.Conditions = ConvertTriggerElements(trigger.Conditions.Elements);
            saveableTrig.LocalVariables = ConvertTriggerElements(trigger.LocalVariables.Elements);
            saveableTrig.Actions = ConvertTriggerElements(trigger.Actions.Elements);

            return JsonConvert.SerializeObject(saveableTrig, Formatting.Indented);
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
                        default:
                            break;
                    }

                    ECA_Saveable.function = ConvertFunction(eca.function);
                    ECA_Saveable.isEnabled = eca.isEnabled;
                    converted = ECA_Saveable;
                }
                else if (element is LocalVariable localVar)
                {
                    converted = new LocalVariable_Saveable
                    {
                        variable = new Variable_Saveable
                        {
                            Id = localVar.variable.Id,
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


                if (element.Elements != null)
                {
                    ConvertTriggerElements(element.Elements);
                }
            }

            return list;
        }

        private static Function_Saveable ConvertFunction(Function function)
        {
            Function_Saveable converted = new Function_Saveable();
            for (int i = 0; i < function.Parameters.Count; i++)
            {
                var converted_param = ConvertParameter(function.Parameters[i]);
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

        #region Deserializer

        public static Trigger Deserialize(SaveableData.Trigger_Saveable trigger)
        {

        }

        #endregion
    }
}

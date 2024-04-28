using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class Parameter
    {
        public string value { get; set; }

        public virtual Parameter Clone()
        {
            string value = null;
            if (this.value != null)
                value = new string(this.value);

            return new Parameter()
            {
                value = value,
            };
        }

        /// <summary>
        /// </summary>
        /// <returns>A list of every parameter in the given trigger.</returns>
        public static List<Parameter> GetParametersFromExplorerElement(ExplorerElement explorerElement)
        {
            List<Parameter> list = new List<Parameter>();
            switch (explorerElement.ElementType)
            {
                case ExplorerElementEnum.Trigger:
                    list.AddRange(GatherTriggerParameters(explorerElement.trigger.Events));
                    list.AddRange(GatherTriggerParameters(explorerElement.trigger.Conditions));
                    list.AddRange(GatherTriggerParameters(explorerElement.trigger.LocalVariables));
                    list.AddRange(GatherTriggerParameters(explorerElement.trigger.Actions));
                    break;
                case ExplorerElementEnum.ActionDefinition:
                    list.AddRange(GatherTriggerParameters(explorerElement.actionDefinition.Actions));
                    break;
                case ExplorerElementEnum.ConditionDefinition:
                    list.AddRange(GatherTriggerParameters(explorerElement.conditionDefinition.Actions));
                    break;
                case ExplorerElementEnum.FunctionDefinition:
                    list.AddRange(GatherTriggerParameters(explorerElement.functionDefinition.Actions));
                    break;
                default:
                    break;
            }

            return list;
        }

        private static List<Parameter> GatherTriggerParameters(TriggerElementCollection triggerElements)
        {
            List<Parameter> parameters = new List<Parameter>();

            for (int i = 0; i < triggerElements.Elements.Count; i++)
            {
                var triggerElement = triggerElements.Elements[i];
                if (triggerElement is LocalVariable localVar)
                {
                    parameters.Add(localVar.variable.InitialValue);
                    continue;
                }

                parameters.AddRange(GetElementParametersAll(triggerElement));

                if (triggerElement is IfThenElse)
                {
                    var special = (IfThenElse)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.If));
                    parameters.AddRange(GatherTriggerParameters(special.Then));
                    parameters.AddRange(GatherTriggerParameters(special.Else));
                }
                else if (triggerElement is AndMultiple)
                {
                    var special = (AndMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.And));
                }
                else if (triggerElement is ForForceMultiple)
                {
                    var special = (ForForceMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement is ForGroupMultiple)
                {
                    var special = (ForGroupMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement is ForLoopAMultiple)
                {
                    var special = (ForLoopAMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement is ForLoopBMultiple)
                {
                    var special = (ForLoopBMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement is ForLoopVarMultiple)
                {
                    var special = (ForLoopVarMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement is OrMultiple)
                {
                    var special = (OrMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Or));
                }
                else if (triggerElement is EnumDestructablesInRectAllMultiple)
                {
                    var special = (EnumDestructablesInRectAllMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement is EnumDestructiblesInCircleBJMultiple)
                {
                    var special = (EnumDestructiblesInCircleBJMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement is EnumItemsInRectBJ)
                {
                    var special = (EnumItemsInRectBJ)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
            }

            return parameters;
        }


        /// <returns>A list of all parameters given to a TriggerElement</returns>
        private static List<Parameter> GetElementParametersAll(TriggerElement te)
        {
            ECA eca = (ECA)te;
            var list = GetElementParametersAll(eca.function.parameters);
            return list;
        }

        private static List<Parameter> GetElementParametersAll(List<Parameter> parameters)
        {
            List<Parameter> list = new List<Parameter>();

            for (int i = 0; i < parameters.Count; i++)
            {
                list.Add(parameters[i]);
                if (parameters[i] is Function)
                {
                    Function f = (Function)parameters[i];
                    list.AddRange(GetElementParametersAll(f.parameters));
                }
            }

            return list;
        }
    }
}

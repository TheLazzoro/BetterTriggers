using BetterTriggers.Containers;
using ICSharpCode.Decompiler.Solution;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace BetterTriggers.Models.EditorData
{
    /// <summary>
    /// Things like 'CreateNUnitsAtLoc' or 'TriggerRegisterDeathEvent'
    /// </summary>
    public class Function : Parameter
    {
        public List<Parameter> parameters { get; set; } = new();

        public override Function Clone()
        {
            string value = null;
            if (this.value != null)
                value = new string(this.value);

            Function f = new Function();
            f.value = value;
            List<Parameter> parameters = new();

            for (int i = 0; i < this.parameters.Count; i++)
            {
                Parameter param = this.parameters[i];
                Parameter cloned;
                if (param is Function func)
                {
                    cloned = func.Clone();
                }
                else if (param is Preset preset)
                {
                    cloned = preset.Clone();
                }
                else if (param is VariableRef varRef)
                {
                    cloned = varRef.Clone();
                }
                else if (param is TriggerRef triggerRef)
                {
                    cloned = triggerRef.Clone();
                }
                else if (param is Value val)
                {
                    cloned = val.Clone();
                }
                else
                    cloned = param.Clone();

                parameters.Add(cloned);
            }
            f.parameters = parameters;

            return f;
        }

        public static List<Function> GetFunctionsFromTrigger(Project project, ExplorerElement explorerElement)
        {
            List<Function> list = new List<Function>();
            if (explorerElement.trigger != null)
            {
                list.AddRange(GatherFunctions(project, explorerElement.trigger.Events));
                list.AddRange(GatherFunctions(project, explorerElement.trigger.Conditions));
                list.AddRange(GatherFunctions(project, explorerElement.trigger.Actions));
            }
            else if (explorerElement.actionDefinition != null)
            {
                list.AddRange(GatherFunctions(project, explorerElement.actionDefinition.Actions));
            }
            else if (explorerElement.conditionDefinition != null)
            {
                list.AddRange(GatherFunctions(project, explorerElement.conditionDefinition.Actions));
            }
            else if (explorerElement.functionDefinition != null)
            {
                list.AddRange(GatherFunctions(project, explorerElement.functionDefinition.Actions));
            }

            return list;
        }

        private static List<Function> GatherFunctions(Project project, TriggerElementCollection triggerElements)
        {
            List<Function> list = new List<Function>();
            triggerElements.Elements.ForEach(t =>
            {
                var eca = (ECA)t;
                list.AddRange(GetFunctionsFromParameters(project, eca.function));

                if (t is IfThenElse)
                {
                    var special = (IfThenElse)t;
                    list.AddRange(GatherFunctions(project, special.If));
                    list.AddRange(GatherFunctions(project, special.Then));
                    list.AddRange(GatherFunctions(project, special.Else));
                }
                else if (t is AndMultiple)
                {
                    var special = (AndMultiple)t;
                    list.AddRange(GatherFunctions(project, special.And));
                }
                else if (t is ForForceMultiple)
                {
                    var special = (ForForceMultiple)t;
                    list.AddRange(GatherFunctions(project, special.Actions));
                }
                else if (t is ForGroupMultiple)
                {
                    var special = (ForGroupMultiple)t;
                    list.AddRange(GatherFunctions(project, special.Actions));
                }
                else if (t is ForLoopAMultiple)
                {
                    var special = (ForLoopAMultiple)t;
                    list.AddRange(GatherFunctions(project, special.Actions));
                }
                else if (t is ForLoopBMultiple)
                {
                    var special = (ForLoopBMultiple)t;
                    list.AddRange(GatherFunctions(project, special.Actions));
                }
                else if (t is ForLoopVarMultiple)
                {
                    var special = (ForLoopVarMultiple)t;
                    list.AddRange(GatherFunctions(project, special.Actions));
                }
                else if (t is OrMultiple)
                {
                    var special = (OrMultiple)t;
                    list.AddRange(GatherFunctions(project, special.Or));
                }
                else if (t is EnumDestructablesInRectAllMultiple)
                {
                    var special = (EnumDestructablesInRectAllMultiple)t;
                    list.AddRange(GatherFunctions(project, special.Actions));
                }
                else if (t is EnumDestructiblesInCircleBJMultiple)
                {
                    var special = (EnumDestructiblesInCircleBJMultiple)t;
                    list.AddRange(GatherFunctions(project, special.Actions));
                }
                else if (t is EnumItemsInRectBJ)
                {
                    var special = (EnumItemsInRectBJ)t;
                    list.AddRange(GatherFunctions(project, special.Actions));
                }
            });

            return list;
        }

        private static List<Function> GetFunctionsFromParameters(Project project, Function function)
        {
            List<Function> list = new List<Function>();
            list.Add(function);
            function.parameters.ForEach(p =>
            {
                if (p is VariableRef)
                {
                    VariableRef variableRef = p as VariableRef;
                    Variable variable = project.Variables.GetById(variableRef.VariableId);
                    if (variable == null)
                        return;

                    if (variable.IsArray)
                    {
                        if (variableRef.arrayIndexValues[0] is Function function)
                            list.AddRange(GetFunctionsFromParameters(project, function));
                    }
                    if (variable.IsTwoDimensions)
                    {
                        if (variableRef.arrayIndexValues[1] is Function function)
                            list.AddRange(GetFunctionsFromParameters(project, function));
                    }
                }

                if (p is Function f)
                    list.AddRange(GetFunctionsFromParameters(project, f));
            });

            return list;
        }

    }
}

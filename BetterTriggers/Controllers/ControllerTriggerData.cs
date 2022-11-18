using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.Templates;
using BetterTriggers.WorldEdit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BetterTriggers.Controllers
{
    public class ControllerTriggerData
    {
        public List<Types> LoadAllVariableTypes()
        {
            return Types.GetGlobalTypes();
        }


        /// <summary>
        /// TODO: We need to work more on initial variable values.
        /// This is a hardcoded quick fix.
        /// </summary>
        public string GetTypeInitialValue(string returnType)
        {
            string value = "null";

            if (returnType == "boolean")
                value = "false";
            else if (returnType == "integer" || returnType == "real")
                value = "0";
            else if (returnType == "group")
                value = "CreateGroup()";
            else if (returnType == "force")
                value = "CreateForce()";
            else if (returnType == "timer")
                value = "CreateTimer()";
            else if (returnType == "dialog")
                value = "DialogCreate()";

            return value;
        }


        public List<FunctionTemplate> LoadAllEvents()
        {
            List<FunctionTemplate> list = new List<FunctionTemplate>();
            var enumerator = TriggerData.EventTemplates.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var template = enumerator.Current.Value;
                list.Add(template.Clone());
            }
            return list;
        }

        public List<FunctionTemplate> LoadAllCalls(string returnType)
        {
            List<FunctionTemplate> list = new List<FunctionTemplate>();

            // Special case for for GUI "Matching" parameter
            bool wasBoolCall = false;
            if (returnType == "boolcall")
            {
                wasBoolCall = true;
                returnType = "boolexpr";
            }

            // Special case for GUI "Action" parameter
            else if (returnType == "code")
            {
                var enumerator = TriggerData.ActionTemplates.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var template = enumerator.Current.Value;
                    if (!template.value.Contains("Multiple"))
                        list.Add(template.Clone());
                }
                list.ForEach(call => call.returnType = "code");

                return list;
            }

            // Special case for GUI 'eventcall' parameter
            else if (returnType == "eventcall")
            {
                var enumerator = TriggerData.EventTemplates.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var template = enumerator.Current.Value;
                    list.Add(template.Clone());
                }
                list.ForEach(call => call.returnType = "eventcall");

                return list;
            }

            var enumCalls = TriggerData.CallTemplates.GetEnumerator();
            while (enumCalls.MoveNext())
            {
                var template = enumCalls.Current.Value;
                if (returnType == template.returnType)
                    list.Add(template.Clone());
            }
            var enumConditions = TriggerData.ConditionTemplates.GetEnumerator();
            while (enumConditions.MoveNext())
            {
                var template = enumConditions.Current.Value;
                if (returnType == template.returnType || (returnType == "boolexpr" && !template.value.EndsWith("Multiple")))
                    list.Add(template.Clone());
            }
            if (wasBoolCall)
            {
                list.ForEach(call => call.returnType = "boolcall");
            }

            return list;
        }

        public List<ConstantTemplate> LoadAllConstants()
        {
            List<ConstantTemplate> list = new List<ConstantTemplate>();
            var enumerator = TriggerData.ConstantTemplates.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var template = enumerator.Current.Value;
                list.Add(template.Clone());
            }
            return list;
        }

        public List<FunctionTemplate> LoadAllConditions()
        {
            List<FunctionTemplate> list = new List<FunctionTemplate>();
            var enumerator = TriggerData.ConditionTemplates.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var template = enumerator.Current.Value;
                list.Add(template.Clone());
            }
            return list;
        }

        public List<FunctionTemplate> LoadAllActions()
        {
            List<FunctionTemplate> list = new List<FunctionTemplate>();
            var enumerator = TriggerData.ActionTemplates.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var template = enumerator.Current.Value;
                list.Add(template.Clone());
            }
            return list;
        }

        public string GetParamDisplayName(Parameter parameter)
        {
            if (parameter is Value)
                return parameter.value;

            string displayName;
            TriggerData.ParamDisplayNames.TryGetValue(parameter.value, out displayName);
            return displayName;
        }

        public string GetParamText(TriggerElement triggerElement)
        {
            string paramText = string.Empty;
            if (triggerElement is ECA)
            {
                var element = (ECA)triggerElement;
                var function = element.function;
                paramText = GetParamText(function);
            }
            else if (triggerElement is LocalVariable)
            {
                var element = (LocalVariable)triggerElement;
                paramText = element.variable.Name;
            }

            return paramText;
        }

        public string GetParamText(Function function)
        {
            string paramText = string.Empty;
            TriggerData.ParamCodeText.TryGetValue(function.value, out paramText);
            if (paramText == null)
            {
                List<string> returnTypes = TriggerData.GetParameterReturnTypes(function);
                paramText = function.value + "(";
                for (int i = 0; i < function.parameters.Count; i++)
                {
                    var p = function.parameters[i];
                    paramText += ",~" + returnTypes[i] + ",";
                    if (i != function.parameters.Count - 1)
                        paramText += ", ";
                }
                paramText += ")";
            }

            return paramText;
        }

        public string GetCategoryTriggerElement(TriggerElement triggerElement)
        {
            string category = string.Empty;
            if (triggerElement is ECA)
            {
                var element = (ECA)triggerElement;
                TriggerData.FunctionCategories.TryGetValue(element.function.value, out category);
            }
            else if (triggerElement is LocalVariable)
                category = TriggerCategory.TC_LOCAL_VARIABLE;

            if (category == null || category == string.Empty)
                throw new Exception("Category was null");

            return category;
        }
    }
}

using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.EditorData.Enums;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BetterTriggers.Controllers
{
    public class ControllerTriggerData
    {
        public List<VariableType> LoadAllVariableTypes()
        {
            return TriggerData.VariableTypes;
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

        // TODO: Needs to accept an identifier instead of enum.
        // Some icons are used for multiple categories.
        public string GetNativeCategory(Category category)
        {
            string name = string.Empty;
            switch (category)
            {
                case Category.LocalVariable:
                    name = "??";
                    break;
                case Category.Action:
                    name = "Action";
                    break;
                case Category.Ability:
                    name = "Ability";
                    break;
                case Category.AI:
                    name = "AI";
                    break;
                case Category.Animation:
                    name = "Animation";
                    break;
                case Category.Camera:
                    name = "Camera";
                    break;
                case Category.Comment:
                    name = "Comment";
                    break;
                case Category.Destructible:
                    name = "Destructible";
                    break;
                case Category.Dialog:
                    name = "Dialog";
                    break;
                case Category.Environment:
                    name = "Environment";
                    break;
                case Category.Frame:
                    name = "Frame";
                    break;
                case Category.Game:
                    name = "Game";
                    break;
                case Category.Goldmine:
                    name = "Neutral Building";
                    break;
                case Category.Hero:
                    name = "Hero";
                    break;
                case Category.Item:
                    name = "Item";
                    break;
                case Category.Logical:
                    name = "";
                    break;
                case Category.Melee:
                    name = "Melee Game";
                    break;
                case Category.Nothing:
                    name = "";
                    break;
                case Category.Player:
                    name = "Player";
                    break;
                case Category.PlayerGroup:
                    name = "Player Group";
                    break;
                case Category.Quest:
                    name = "Quest";
                    break;
                case Category.Region:
                    name = "Region";
                    break;
                case Category.SetVariable:
                    name = "";
                    break;
                case Category.Sound:
                    name = "Sound";
                    break;
                case Category.Timer:
                    name = "Countdown Timer";
                    break;
                case Category.Unit:
                    name = "Unit";
                    break;
                case Category.UnitGroup:
                    name = "Unit Group";
                    break;
                case Category.UnitSelection:
                    name = "Unit Selection";
                    break;
                case Category.Visibility:
                    name = "Unit Visibility";
                    break;
                case Category.Wait:
                    name = "";
                    break;
                default:
                    break;
            }


            return name;
        }

        public List<FunctionTemplate> LoadAllEvents()
        {
            List<FunctionTemplate> list = new List<FunctionTemplate>();
            for (int i = 0; i < TriggerData.EventTemplates.Count; i++)
            {
                var template = TriggerData.EventTemplates[i];
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
                for (int i = 0; i < TriggerData.ActionTemplates.Count; i++)
                {
                    var template = TriggerData.ActionTemplates[i];
                    if (!template.identifier.Contains("Multiple"))
                        list.Add(template.Clone());
                }
                list.ForEach(call => call.returnType = "code");

                return list;
            }

            // Special case for GUI 'eventcall' parameter
            else if (returnType == "eventcall")
            {
                for (int i = 0; i < TriggerData.EventTemplates.Count; i++)
                {
                    var template = TriggerData.EventTemplates[i];
                    list.Add(template.Clone());
                }
                list.ForEach(call => call.returnType = "eventcall");

                return list;
            }

            for (int i = 0; i < TriggerData.CallTemplates.Count; i++)
            {
                var template = TriggerData.CallTemplates[i];
                if (returnType == template.returnType)
                    list.Add(template.Clone());
            }
            for (int i = 0; i < TriggerData.ConditionTemplates.Count; i++)
            {
                var template = TriggerData.ConditionTemplates[i];
                if (returnType == template.returnType)
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
            for (int i = 0; i < TriggerData.ConstantTemplates.Count; i++)
            {
                var template = TriggerData.ConstantTemplates[i];
                list.Add(template.Clone());
            }
            return list;
        }

        public List<FunctionTemplate> LoadAllConditions()
        {
            List<FunctionTemplate> list = new List<FunctionTemplate>();
            for (int i = 0; i < TriggerData.ConditionTemplates.Count; i++)
            {
                var template = TriggerData.ConditionTemplates[i];
                list.Add(template.Clone());
            }
            return list;
        }

        public List<FunctionTemplate> LoadAllActions()
        {
            List<FunctionTemplate> list = new List<FunctionTemplate>();
            for (int i = 0; i < TriggerData.ActionTemplates.Count; i++)
            {
                var template = TriggerData.ActionTemplates[i];
                list.Add(template.Clone());
            }
            return list;
        }

        public string GetParamDisplayName(Parameter parameter)
        {
            if (parameter is Value)
                return parameter.identifier;

            string displayName = string.Empty;
            bool found = false;
            int i = 0;

            if (parameter is Constant)
            {
                while (!found && i < TriggerData.ConstantTemplates.Count)
                {
                    if (TriggerData.ConstantTemplates[i].identifier == parameter.identifier)
                    {
                        found = true;
                        displayName = Locale.Translate(TriggerData.ConstantTemplates[i].name);
                    }

                    i++;
                }
            }

            else if (parameter is Function)
            {
                i = 0;
                while (!found && i < TriggerData.EventTemplates.Count)
                {
                    if (TriggerData.EventTemplates[i].identifier == parameter.identifier)
                    {
                        found = true;
                        displayName = TriggerData.EventTemplates[i].name;
                    }

                    i++;
                }

                i = 0;
                while (!found && i < TriggerData.ConditionTemplates.Count)
                {
                    if (TriggerData.ConditionTemplates[i].identifier == parameter.identifier)
                    {
                        found = true;
                        displayName = TriggerData.ConditionTemplates[i].name;
                    }

                    i++;
                }

                i = 0;
                while (!found && i < TriggerData.ActionTemplates.Count)
                {
                    if (TriggerData.ActionTemplates[i].identifier == parameter.identifier)
                    {
                        found = true;
                        displayName = TriggerData.ActionTemplates[i].name;
                    }

                    i++;
                }

                i = 0;
                while (!found && i < TriggerData.CallTemplates.Count)
                {
                    if (TriggerData.CallTemplates[i].identifier == parameter.identifier)
                    {
                        found = true;
                        displayName = TriggerData.CallTemplates[i].name;
                    }

                    i++;
                }

            }

            return displayName;
        }

        public string GetParamText(Function function)
        {
            string paramText = string.Empty;
            bool found = false;
            int i = 0;

            i = 0;
            while (!found && i < TriggerData.EventTemplates.Count)
            {
                if (TriggerData.EventTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    paramText = TriggerData.EventTemplates[i].paramText;
                }

                i++;
            }

            i = 0;
            while (!found && i < TriggerData.ConditionTemplates.Count)
            {
                if (TriggerData.ConditionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    paramText = TriggerData.ConditionTemplates[i].paramText;
                }

                i++;
            }

            i = 0;
            while (!found && i < TriggerData.ActionTemplates.Count)
            {
                if (TriggerData.ActionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    paramText = TriggerData.ActionTemplates[i].paramText;
                }

                i++;
            }

            i = 0;
            while (!found && i < TriggerData.CallTemplates.Count)
            {
                if (TriggerData.CallTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    paramText = TriggerData.CallTemplates[i].paramText;
                }

                i++;
            }

            if(paramText == null)
            {
                paramText = $"{function.identifier}(";
                function.parameters.ForEach(p => paramText += $",~{TriggerData.GetReturnType(p.identifier)}, ");
                paramText += ")";
            }

            return paramText;
        }

        public string GetDescription(Function function)
        {
            string description = string.Empty;
            bool found = false;
            int i = 0;

            i = 0;
            while (!found && i < TriggerData.EventTemplates.Count)
            {
                if (TriggerData.EventTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    description = TriggerData.EventTemplates[i].description;
                }

                i++;
            }

            i = 0;
            while (!found && i < TriggerData.ConditionTemplates.Count)
            {
                if (TriggerData.ConditionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    description = TriggerData.ConditionTemplates[i].description;
                }

                i++;
            }

            i = 0;
            while (!found && i < TriggerData.ActionTemplates.Count)
            {
                if (TriggerData.ActionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    description = TriggerData.ActionTemplates[i].description;
                }

                i++;
            }

            i = 0;
            while (!found && i < TriggerData.CallTemplates.Count)
            {
                if (TriggerData.CallTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    description = TriggerData.CallTemplates[i].description;
                }

                i++;
            }

            return description;
        }

        public Category GetCategoryTriggerElement(Function function)
        {
            Category category = Category.AI;
            bool found = false;
            int i = 0;

            i = 0;
            while (!found && i < TriggerData.EventTemplates.Count)
            {
                if (TriggerData.EventTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    category = TriggerData.EventTemplates[i].category;
                }

                i++;
            }

            i = 0;
            while (!found && i < TriggerData.ConditionTemplates.Count)
            {
                if (TriggerData.ConditionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    category = TriggerData.ConditionTemplates[i].category;
                }

                i++;
            }

            i = 0;
            while (!found && i < TriggerData.ActionTemplates.Count)
            {
                if (TriggerData.ActionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    category = TriggerData.ActionTemplates[i].category;
                }

                i++;
            }

            i = 0;
            while (!found && i < TriggerData.CallTemplates.Count)
            {
                if (TriggerData.CallTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    category = TriggerData.CallTemplates[i].category;
                }

                i++;
            }

            return category;
        }
    }
}

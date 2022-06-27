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
            return ContainerTriggerData.VariableTypes;
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
            for (int i = 0; i < ContainerTriggerData.EventTemplates.Count; i++)
            {
                var template = ContainerTriggerData.EventTemplates[i];
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
                for (int i = 0; i < ContainerTriggerData.ActionTemplates.Count; i++)
                {
                    var template = ContainerTriggerData.ActionTemplates[i];
                    if (!template.identifier.Contains("Multiple"))
                        list.Add(template.Clone());
                }
                list.ForEach(call => call.returnType = "code");

                return list;
            }

            // Special case for GUI 'eventcall' parameter
            else if (returnType == "eventcall")
            {
                for (int i = 0; i < ContainerTriggerData.EventTemplates.Count; i++)
                {
                    var template = ContainerTriggerData.EventTemplates[i];
                    list.Add(template.Clone());
                }
                list.ForEach(call => call.returnType = "eventcall");

                return list;
            }

            for (int i = 0; i < ContainerTriggerData.CallTemplates.Count; i++)
            {
                var template = ContainerTriggerData.CallTemplates[i];
                if (returnType == template.returnType)
                    list.Add(template.Clone());
            }
            for (int i = 0; i < ContainerTriggerData.ConditionTemplates.Count; i++)
            {
                var template = ContainerTriggerData.ConditionTemplates[i];
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
            for (int i = 0; i < ContainerTriggerData.ConstantTemplates.Count; i++)
            {
                var template = ContainerTriggerData.ConstantTemplates[i];
                list.Add(template.Clone());
            }
            return list;
        }

        public List<FunctionTemplate> LoadAllConditions()
        {
            List<FunctionTemplate> list = new List<FunctionTemplate>();
            for (int i = 0; i < ContainerTriggerData.ConditionTemplates.Count; i++)
            {
                var template = ContainerTriggerData.ConditionTemplates[i];
                list.Add(template.Clone());
            }
            return list;
        }

        public List<FunctionTemplate> LoadAllActions()
        {
            List<FunctionTemplate> list = new List<FunctionTemplate>();
            for (int i = 0; i < ContainerTriggerData.ActionTemplates.Count; i++)
            {
                var template = ContainerTriggerData.ActionTemplates[i];
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
                while (!found && i < ContainerTriggerData.ConstantTemplates.Count)
                {
                    if (ContainerTriggerData.ConstantTemplates[i].identifier == parameter.identifier)
                    {
                        found = true;
                        displayName = Locale.Translate(ContainerTriggerData.ConstantTemplates[i].name);
                    }

                    i++;
                }
            }

            else if (parameter is Function)
            {
                i = 0;
                while (!found && i < ContainerTriggerData.EventTemplates.Count)
                {
                    if (ContainerTriggerData.EventTemplates[i].identifier == parameter.identifier)
                    {
                        found = true;
                        displayName = ContainerTriggerData.EventTemplates[i].name;
                    }

                    i++;
                }

                i = 0;
                while (!found && i < ContainerTriggerData.ConditionTemplates.Count)
                {
                    if (ContainerTriggerData.ConditionTemplates[i].identifier == parameter.identifier)
                    {
                        found = true;
                        displayName = ContainerTriggerData.ConditionTemplates[i].name;
                    }

                    i++;
                }

                i = 0;
                while (!found && i < ContainerTriggerData.ActionTemplates.Count)
                {
                    if (ContainerTriggerData.ActionTemplates[i].identifier == parameter.identifier)
                    {
                        found = true;
                        displayName = ContainerTriggerData.ActionTemplates[i].name;
                    }

                    i++;
                }

                i = 0;
                while (!found && i < ContainerTriggerData.CallTemplates.Count)
                {
                    if (ContainerTriggerData.CallTemplates[i].identifier == parameter.identifier)
                    {
                        found = true;
                        displayName = ContainerTriggerData.CallTemplates[i].name;
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
            while (!found && i < ContainerTriggerData.EventTemplates.Count)
            {
                if (ContainerTriggerData.EventTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    paramText = ContainerTriggerData.EventTemplates[i].paramText;
                }

                i++;
            }

            i = 0;
            while (!found && i < ContainerTriggerData.ConditionTemplates.Count)
            {
                if (ContainerTriggerData.ConditionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    paramText = ContainerTriggerData.ConditionTemplates[i].paramText;
                }

                i++;
            }

            i = 0;
            while (!found && i < ContainerTriggerData.ActionTemplates.Count)
            {
                if (ContainerTriggerData.ActionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    paramText = ContainerTriggerData.ActionTemplates[i].paramText;
                }

                i++;
            }

            i = 0;
            while (!found && i < ContainerTriggerData.CallTemplates.Count)
            {
                if (ContainerTriggerData.CallTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    paramText = ContainerTriggerData.CallTemplates[i].paramText;
                }

                i++;
            }

            if(paramText == null)
            {
                paramText = $"{function.identifier}(";
                function.parameters.ForEach(p => paramText += $",~{p.returnType}, ");
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
            while (!found && i < ContainerTriggerData.EventTemplates.Count)
            {
                if (ContainerTriggerData.EventTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    description = ContainerTriggerData.EventTemplates[i].description;
                }

                i++;
            }

            i = 0;
            while (!found && i < ContainerTriggerData.ConditionTemplates.Count)
            {
                if (ContainerTriggerData.ConditionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    description = ContainerTriggerData.ConditionTemplates[i].description;
                }

                i++;
            }

            i = 0;
            while (!found && i < ContainerTriggerData.ActionTemplates.Count)
            {
                if (ContainerTriggerData.ActionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    description = ContainerTriggerData.ActionTemplates[i].description;
                }

                i++;
            }

            i = 0;
            while (!found && i < ContainerTriggerData.CallTemplates.Count)
            {
                if (ContainerTriggerData.CallTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    description = ContainerTriggerData.CallTemplates[i].description;
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
            while (!found && i < ContainerTriggerData.EventTemplates.Count)
            {
                if (ContainerTriggerData.EventTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    category = ContainerTriggerData.EventTemplates[i].category;
                }

                i++;
            }

            i = 0;
            while (!found && i < ContainerTriggerData.ConditionTemplates.Count)
            {
                if (ContainerTriggerData.ConditionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    category = ContainerTriggerData.ConditionTemplates[i].category;
                }

                i++;
            }

            i = 0;
            while (!found && i < ContainerTriggerData.ActionTemplates.Count)
            {
                if (ContainerTriggerData.ActionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    category = ContainerTriggerData.ActionTemplates[i].category;
                }

                i++;
            }

            i = 0;
            while (!found && i < ContainerTriggerData.CallTemplates.Count)
            {
                if (ContainerTriggerData.CallTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    category = ContainerTriggerData.CallTemplates[i].category;
                }

                i++;
            }

            return category;
        }
    }
}

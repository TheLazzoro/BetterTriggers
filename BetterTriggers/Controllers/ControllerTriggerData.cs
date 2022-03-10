using BetterTriggers.Containers;
using Model.Data;
using Model.EditorData;
using Model.EditorData.Enums;
using Model.SaveableData;
using Model.Templates;
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

        public List<FunctionTemplate> LoadAllEvents()
        {
            return ContainerTriggerData.EventTemplates;
        }

        public List<FunctionTemplate> LoadAllCalls()
        {
            return ContainerTriggerData.CallTemplates;
        }

        public List<ConstantTemplate> LoadAllConstants()
        {
            return ContainerTriggerData.ConstantTemplates;
        }

        public List<FunctionTemplate> LoadAllConditions()
        {
            return ContainerTriggerData.ConditionTemplates;
        }

        public List<FunctionTemplate> LoadAllActions()
        {
            return ContainerTriggerData.ActionTemplates;
        }

        public string GetParamDisplayName(Parameter parameter)
        {
            string displayName = string.Empty;
            bool found = false;
            int i = 0;

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

            i = 0;
            while (!found && i < ContainerTriggerData.ConstantTemplates.Count)
            {
                if (ContainerTriggerData.ConstantTemplates[i].identifier == parameter.identifier)
                {
                    found = true;
                    displayName = ContainerTriggerData.ConstantTemplates[i].name;
                }

                i++;
            }

            return displayName;
        }

        public  string GetParamText(Function function)
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

            return paramText;
        }

        public   string GetDescription(Function function)
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

        public   Category GetCategory(Function function)
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

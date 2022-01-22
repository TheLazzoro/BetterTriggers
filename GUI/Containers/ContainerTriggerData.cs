using Model.Enums;
using Model.SavableTriggerData;
using Model.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GUI.Containers
{
    public class ContainerTriggerData
    {
        public static List<ConstantTemplate> ConstantTemplates = JsonConvert.DeserializeObject<List<ConstantTemplate>>(File.ReadAllText(@"C:\Users\Lasse Dam\Desktop\JSON\constants.json"));
        public static List<FunctionTemplate> EventTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(@"C:\Users\Lasse Dam\Desktop\JSON\events.json"));
        public static List<FunctionTemplate> ConditionTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(@"C:\Users\Lasse Dam\Desktop\JSON\conditions.json"));
        public static List<FunctionTemplate> ActionTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(@"C:\Users\Lasse Dam\Desktop\JSON\actions.json"));
        public static List<FunctionTemplate> CallTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(@"C:\Users\Lasse Dam\Desktop\JSON\calls.json"));

        public static string GetParamDisplayName(Parameter parameter)
        {
            string displayName = string.Empty;
            bool found = false;
            int i = 0;

            i = 0;
            while (!found && i < EventTemplates.Count)
            {
                if (EventTemplates[i].identifier == parameter.identifier)
                {
                    found = true;
                    displayName = EventTemplates[i].name;
                }

                i++;
            }

            i = 0;
            while (!found && i < ConditionTemplates.Count)
            {
                if (ConditionTemplates[i].identifier == parameter.identifier)
                {
                    found = true;
                    displayName = ConditionTemplates[i].name;
                }

                i++;
            }

            i = 0;
            while (!found && i < ActionTemplates.Count)
            {
                if (ActionTemplates[i].identifier == parameter.identifier)
                {
                    found = true;
                    displayName = ActionTemplates[i].name;
                }

                i++;
            }

            i = 0;
            while (!found && i < CallTemplates.Count)
            {
                if (CallTemplates[i].identifier == parameter.identifier)
                {
                    found = true;
                    displayName = CallTemplates[i].name;
                }

                i++;
            }

            i = 0;
            while (!found && i < ConstantTemplates.Count)
            {
                if (ConstantTemplates[i].identifier == parameter.identifier)
                {
                    found = true;
                    displayName = ConstantTemplates[i].name;
                }

                i++;
            }

            return displayName;
        }
        
        public static string GetParamText(Function function)
        {
            string paramText = string.Empty;
            bool found = false;
            int i = 0;

            i = 0;
            while (!found && i < EventTemplates.Count)
            {
                if (EventTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    paramText = EventTemplates[i].paramText;
                }

                i++;
            }

            i = 0;
            while (!found && i < ConditionTemplates.Count)
            {
                if (ConditionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    paramText = ConditionTemplates[i].paramText;
                }

                i++;
            }

            i = 0;
            while (!found && i < ActionTemplates.Count)
            {
                if (ActionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    paramText = ActionTemplates[i].paramText;
                }

                i++;
            }

            i = 0;
            while (!found && i < CallTemplates.Count)
            {
                if (CallTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    paramText = CallTemplates[i].paramText;
                }

                i++;
            }

            return paramText;
        }

        public static string GetDescription(Function function)
        {
            string description = string.Empty;
            bool found = false;
            int i = 0;

            i = 0;
            while (!found && i < EventTemplates.Count)
            {
                if (EventTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    description = EventTemplates[i].description;
                }

                i++;
            }

            i = 0;
            while (!found && i < ConditionTemplates.Count)
            {
                if (ConditionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    description = ConditionTemplates[i].description;
                }

                i++;
            }

            i = 0;
            while (!found && i < ActionTemplates.Count)
            {
                if (ActionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    description = ActionTemplates[i].description;
                }

                i++;
            }

            i = 0;
            while (!found && i < CallTemplates.Count)
            {
                if (CallTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    description = CallTemplates[i].description;
                }

                i++;
            }

            return description;
        }

        public static Category GetCategory(Function function)
        {
            Category category = Category.AI;
            bool found = false;
            int i = 0;

            i = 0;
            while (!found && i < EventTemplates.Count)
            {
                if (EventTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    category = EventTemplates[i].category;
                }

                i++;
            }

            i = 0;
            while (!found && i < ConditionTemplates.Count)
            {
                if (ConditionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    category = ConditionTemplates[i].category;
                }

                i++;
            }

            i = 0;
            while (!found && i < ActionTemplates.Count)
            {
                if (ActionTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    category = ActionTemplates[i].category;
                }

                i++;
            }

            i = 0;
            while (!found && i < CallTemplates.Count)
            {
                if (CallTemplates[i].identifier == function.identifier)
                {
                    found = true;
                    category = CallTemplates[i].category;
                }

                i++;
            }

            return category;
        }
    }
}

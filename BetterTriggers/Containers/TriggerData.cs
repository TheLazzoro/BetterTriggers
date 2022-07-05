using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using War3Net.Build.Info;

namespace BetterTriggers.Containers
{
    public class TriggerData
    {
        internal static List<ConstantTemplate> ConstantTemplates = JsonConvert.DeserializeObject<List<ConstantTemplate>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\constants.json"));
        internal static List<FunctionTemplate> EventTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\events.json"));
        internal static List<FunctionTemplate> ConditionTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\conditions.json"));
        internal static List<FunctionTemplate> ActionTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\actions.json"));
        internal static List<FunctionTemplate> CallTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\calls.json"));
        internal static List<VariableType> VariableTypes = JsonConvert.DeserializeObject<List<VariableType>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\types.json"));

        private static Dictionary<string, string> ReturnTypes;
        private static Dictionary<string, string> ConstantCodeText;
        private static Dictionary<string, ConstantTemplate> Constants;
        private static Dictionary<string, FunctionTemplate> Functions;

        public static string GetReturnType(string identifier)
        {
            if (identifier == null)
                return null;

            string returnType;
            GetReturnTypeInstance().TryGetValue(identifier, out returnType);
            return returnType;
        }

        public static List<string> GetParameterReturnTypes(Function f)
        {
            List<string> list = new List<string>();
            FunctionTemplate function = GetFunction(f.identifier);
            function.parameters.ForEach(p => list.Add(p.returnType));
            return list;
        }

        private static Dictionary<string, string> GetReturnTypeInstance()
        {
            if (ReturnTypes == null)
            {
                ReturnTypes = new Dictionary<string, string>();


                ConstantTemplates.ForEach(el => ReturnTypes.Add(el.identifier, el.returnType));
                EventTemplates.ForEach(el => {
                    ReturnTypes.Add(el.identifier, el.returnType);
                    el.parameters.ForEach(p => { if (p.identifier != null) ReturnTypes.Add(p.identifier, p.identifier); });
                });
                ConditionTemplates.ForEach(el => {
                    ReturnTypes.Add(el.identifier, el.returnType);
                    el.parameters.ForEach(p => { if (p.identifier != null) ReturnTypes.Add(p.identifier, p.identifier); });
                });
                ActionTemplates.ForEach(el => {
                    ReturnTypes.Add(el.identifier, el.returnType);
                    el.parameters.ForEach(p => { if (p.identifier != null) ReturnTypes.Add(p.identifier, p.identifier); });
                });
                CallTemplates.ForEach(el => {
                    ReturnTypes.Add(el.identifier, el.returnType);
                    el.parameters.ForEach(p => { if (p.identifier != null) ReturnTypes.Add(p.identifier, p.identifier); });
                });
            }

            return ReturnTypes;
        }

        internal static string GetConstantCodeText(string identifier, ScriptLanguage language)
        {
            string codeText;
            GetCodeTextDictionaryInstance().TryGetValue(identifier, out codeText);
            if(language == ScriptLanguage.Lua)
            {
                if (codeText == "!=")
                    codeText = "~=";
                else if (codeText == "null")
                    codeText = "nil";
            }

            return codeText;
        }

        internal static ConstantTemplate GetContant(string identifier)
        {
            ConstantTemplate constantTemplate;
            GetConstantDictionaryInstance().TryGetValue(identifier, out constantTemplate);
            return constantTemplate;
        }

        internal static FunctionTemplate GetFunction(string identifier)
        {
            FunctionTemplate functionTemplate;
            GetFunctionDictionaryInstance().TryGetValue(identifier, out functionTemplate);
            return functionTemplate;
        }

        private static Dictionary<string, string> GetCodeTextDictionaryInstance()
        {
            if (ConstantCodeText == null)
            {
                ConstantCodeText = new Dictionary<string, string>();
                for (int i = 0; i < ConstantTemplates.Count; i++)
                {
                    var constant = ConstantTemplates[i];
                    ConstantCodeText.Add(constant.identifier, constant.codeText);
                }
            }

            return ConstantCodeText;
        }

        private static Dictionary<string, ConstantTemplate> GetConstantDictionaryInstance()
        {
            if (Constants == null)
            {
                Constants = new Dictionary<string, ConstantTemplate>();
                for (int i = 0; i < ConstantTemplates.Count; i++)
                {
                    var constant = ConstantTemplates[i];
                    Constants.Add(constant.identifier, constant);
                }
            }

            return Constants;
        }

        private static Dictionary<string, FunctionTemplate> GetFunctionDictionaryInstance()
        {
            if (Functions == null)
            {
                Functions = new Dictionary<string, FunctionTemplate>();
                for (int i = 0; i < EventTemplates.Count; i++)
                {
                    var function = EventTemplates[i];
                    Functions.Add(function.identifier, function);
                }
                for (int i = 0; i < ConditionTemplates.Count; i++)
                {
                    var function = ConditionTemplates[i];
                    Functions.Add(function.identifier, function);
                }
                for (int i = 0; i < ActionTemplates.Count; i++)
                {
                    var function = ActionTemplates[i];
                    Functions.Add(function.identifier, function);
                }
                for (int i = 0; i < CallTemplates.Count; i++)
                {
                    var function = CallTemplates[i];
                    Functions.Add(function.identifier, function);
                }
            }

            return Functions;
        }
    }
}

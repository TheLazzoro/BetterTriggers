using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BetterTriggers.Containers
{
    public class ContainerTriggerData
    {
        internal static List<ConstantTemplate> ConstantTemplates = JsonConvert.DeserializeObject<List<ConstantTemplate>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\constants.json"));
        internal static List<FunctionTemplate> EventTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\events.json"));
        internal static List<FunctionTemplate> ConditionTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\conditions.json"));
        internal static List<FunctionTemplate> ActionTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\actions.json"));
        internal static List<FunctionTemplate> CallTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\calls.json"));
        internal static List<VariableType> VariableTypes = JsonConvert.DeserializeObject<List<VariableType>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\types.json"));

        private static Dictionary<string, string> ConstantCodeText;

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

        internal static string GetConstantCodeText(string identifier)
        {
            string codeText;
            GetCodeTextDictionaryInstance().TryGetValue(identifier, out codeText);
            return codeText;
        }
    }
}

using Model.EditorData;
using Model.EditorData.Enums;
using Model.SaveableData;
using Model.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BetterTriggers.Containers
{
    public class ContainerTriggerData
    {
        public static List<ConstantTemplate> ConstantTemplates = JsonConvert.DeserializeObject<List<ConstantTemplate>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\constants.json"));
        public static List<FunctionTemplate> EventTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\events.json"));
        public static List<FunctionTemplate> ConditionTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\conditions.json"));
        public static List<FunctionTemplate> ActionTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\actions.json"));
        public static List<FunctionTemplate> CallTemplates = JsonConvert.DeserializeObject<List<FunctionTemplate>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\calls.json"));
        public static List<VariableType> VariableTypes = JsonConvert.DeserializeObject<List<VariableType>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\TriggerData\types.json"));
    }
}

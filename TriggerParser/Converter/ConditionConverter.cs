using Model.Containers;
using Model.SaveableData;
using Model.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggerParser.Conditions;
using TriggerParser.TriggerElements;

namespace TriggerParser.Converter
{
    public static class ConditionConverter
    {
        public static void ConvertConditions(List<TriggerCondition> triggerElements)
        {
            foreach (var item in triggerElements)
            {
                var arguments = item.arguments;
                List<Parameter> parameters = new List<Parameter>();

                foreach (var arg in arguments)
                {
                    var parameter = new Parameter()
                    {
                        returnType = arg.key
                    };

                    parameters.Add(parameter);
                }

                FunctionTemplate condition = new FunctionTemplate()
                {
                    identifier = item.key,
                    name = item.displayName,
                    parameters = parameters,
                    paramText = item.paramText,
                    category = CategoryConverter.ConvertBlizzardCategory(item.category)
                };

                ContainerConditions.AddCondition(condition);
            }

            string json = JsonConvert.SerializeObject(ContainerConditions.GetAllTypes());
            File.WriteAllText(@"C:\Users\Lasse Dam\Desktop\ParseTest\conditions.json", json);
        }
    }
}

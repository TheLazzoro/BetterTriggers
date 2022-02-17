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
using TriggerParser.Calls;

namespace TriggerParser.Converter
{
    public static class CallConverter
    {
        public static void ConvertCalls(List<TriggerCall> triggerElements)
        {
            foreach (var item in triggerElements)
            {
                List<Parameter> parameters = new List<Parameter>();
                for (int i = 0; i < item.arguments.Count; i++)
                {
                    var param = new Parameter()
                    {
                        identifier = item.arguments[i].key,
                        returnType = item.arguments[i].key,
                        //name = item.arguments[i].displayName // makes no sense?
                    };
                    parameters.Add(param);
                }

                FunctionTemplate call = new FunctionTemplate()
                {
                    identifier = item.key,
                    name = item.displayName,
                    parameters = parameters,
                    paramText = item.paramText,
                    returnType = item.returnType,
                    description = "",
                    category = CategoryConverter.ConvertBlizzardCategory(item.category)
                };

                ContainerFunctions.AddParameter(call);
            }

            string json = JsonConvert.SerializeObject(ContainerFunctions.GetAllTypes());
            File.WriteAllText(@"C:\Users\Lasse Dam\Desktop\ParseTest\calls.json", json);
        }
    }
}

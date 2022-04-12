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
using TriggerParser.TriggerElements;

namespace TriggerParser.Converter
{
    public static class EventConverter
    {
        public static void ConvertEvents(List<TriggerParser.TriggerElements.TriggerElement> triggerElements)
        {
            foreach (var item in triggerElements)
            {
                List<Parameter> parameters = new List<Parameter>();
                for(int i = 0; i < item.arguments.Count; i++)
                {
                    var param = new Parameter()
                    {
                        identifier = item.arguments[i].key,
                        returnType = item.arguments[i].key,
                        //name = item.arguments[i].displayName // makes no sense?
                    };
                    parameters.Add(param);
                }

                FunctionTemplate _event = new FunctionTemplate()
                {
                    identifier = item.key,
                    name = item.displayName,
                    parameters = parameters,
                    paramText = item.paramText,
                    returnType = "nothing", // temporary
                    description = "",
                    category = CategoryConverter.ConvertBlizzardCategory(item.category)
                };

                ContainerEvents.AddEvent(_event);
            }

            string json = JsonConvert.SerializeObject(ContainerEvents.GetAllTypes());
            File.WriteAllText(@"C:\Users\Lasse Dam\Desktop\ParseTest\events.json", json);
        }
    }
}

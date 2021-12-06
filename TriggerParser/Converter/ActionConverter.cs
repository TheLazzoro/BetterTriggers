using Model.Containers;
using Model.Natives;
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
    public static class ActionConverter
    {
        public static void ConvertActions(List<TriggerElement> triggerElements)
        {
            foreach (var item in triggerElements)
            {
                List<Parameter> parameters = new List<Parameter>();
                for (int i = 0; i < item.arguments.Count; i++)
                {
                    var param = new Parameter()
                    {
                        name = item.arguments[i].key,
                        returnType = item.arguments[i].key,
                        //name = item.arguments[i].displayName // makes no sense?
                    };
                    parameters.Add(param);
                }

                Function action = new Function()
                {
                    identifier = item.key,
                    name = item.displayName,
                    parameters = parameters,
                    paramText = item.paramText,
                    returnType = "nothing", // temporary
                    description = "",
                    category = CategoryConverter.ConvertBlizzardCategory(item.category)
                };

                ContainerFunctions.AddParameter(action);
            }

            string json = JsonConvert.SerializeObject(ContainerFunctions.GetAllTypes());
            File.WriteAllText(@"C:\Users\Lasse Dam\Desktop\ParseTest\actions.json", json);
        }
    }
}

using DataAccess.Containers;
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
    public static class ConditionConverter
    {
        public static void ConvertConditions(List<TriggerElement> triggerElements)
        {
            foreach (var item in triggerElements)
            {
                DataAccess.Natives.Function condition = new DataAccess.Natives.Function()
                {
                    identifier = item.key,
                    name = item.displayName,
                    parameters = new List<DataAccess.Natives.Parameter>(), // temporary
                    paramText = item.paramText,
                    returnType = new DataAccess.Natives.Type("nothing", "Nothing"), // temporary
                    description = "",
                    category = CategoryConverter.ConvertBlizzardCategory(item.category)
                };

                ContainerEvents.AddEvent(condition); // nocheckin
            }

            string json = JsonConvert.SerializeObject(ContainerEvents.GetAllTypes()); // nocheckin
            File.WriteAllText(@"C:\Users\Lasse Dam\Desktop\ParseTest\conditions.json", json);
        }
    }
}

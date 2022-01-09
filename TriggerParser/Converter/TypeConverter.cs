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
using TriggerParser.Types;
using Model.Data;

namespace TriggerParser.Converter
{
    public static class TypeConverter
    {
        public static void ConvertTypes(List<TriggerParser.Types.TriggerType> triggerTypes)
        {
            List<Model.Data.TriggerType> list = new List<Model.Data.TriggerType>();
            for (int i = 0; i < triggerTypes.Count; i++)
            {
                Model.Data.TriggerType type = new Model.Data.TriggerType()
                {
                    key = triggerTypes[i].key,
                    displayname = triggerTypes[i].displayName,
                };

                list.Add(type);
            }

            string json = JsonConvert.SerializeObject(list);
            File.WriteAllText(@"C:\Users\Lasse Dam\Desktop\ParseTest\types.json", json);
        }
    }
}

using DataAccess.Natives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.JsonBaseConverter
{
    public class BaseConverter : JsonConverter
    {
        static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new ParameterConverter() };

        public override bool CanConvert(System.Type objectType)
        {
            return (objectType == typeof(Parameter));
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            if (jo.ContainsKey("ParamType"))
            {

                switch (jo["ParamType"].Value<int>())
                {
                    case 1:
                        return JsonConvert.DeserializeObject<Function>(jo.ToString(), SpecifiedSubclassConversion);
                    case 2:
                        return JsonConvert.DeserializeObject<Constant>(jo.ToString(), SpecifiedSubclassConversion);
                    default:
                        throw new Exception();
                }
            } else
                return JsonConvert.DeserializeObject<Parameter>(jo.ToString(), SpecifiedSubclassConversion);

            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // won't be called because CanWrite returns false
        }
    }
}

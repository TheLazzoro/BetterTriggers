using BetterTriggers.Models.SaveableData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.JsonBaseConverter
{
    public class BaseConverterTriggerElement : JsonConverter
    {
        static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new JsonConverter_BT() };

        public override bool CanConvert(System.Type objectType)
        {
            return (objectType == typeof(ECA));
        }

        public object Create(Type objectType, JObject jObject)
        {
            if (jObject.ContainsKey("LocalVar"))
            {
                var n = (int)jObject.Property("LocalVar");
                if (n == 1) return new LocalVariable();
                throw new ApplicationException(String.Format("LocalVar type {0} not supported!", n));
            }

            if (jObject.ContainsKey("ElementType"))
            {
                var type = (int)jObject.Property("ElementType");
                switch (type)
                {
                    case 1:
                        return new IfThenElse();
                    case 2:
                        return new AndMultiple();
                    case 3:
                        return new OrMultiple();
                    case 4:
                        return new ForGroupMultiple();
                    case 5:
                        return new ForForceMultiple();
                    case 6:
                        return new ForLoopAMultiple();
                    case 7:
                        return new ForLoopBMultiple();
                    case 8:
                        return new ForLoopVarMultiple();
                    case 9:
                        return new SetVariable();
                    case 10:
                        return new EnumDestructablesInRectAllMultiple();
                    case 11:
                        return new EnumDestructiblesInCircleBJMultiple();
                    case 12:
                        return new EnumItemsInRectBJ();
                    default:
                        return new ECA();
                }

                throw new ApplicationException(String.Format("The given trigger element type {0} is not supported!", type));
            }
            else
                return new ECA();

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            reader.MaxDepth = 128;

            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            var target = Create(objectType, jObject);

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
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

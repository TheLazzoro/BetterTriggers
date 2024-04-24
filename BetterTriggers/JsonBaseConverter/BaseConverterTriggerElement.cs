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
            return (objectType == typeof(ECA_Saveable));
        }

        public object Create(Type objectType, JObject jObject)
        {
            if (jObject.ContainsKey("LocalVar"))
            {
                var n = (int)jObject.Property("LocalVar");
                if (n == 1) return new LocalVariable_Saveable();
                throw new ApplicationException(String.Format("LocalVar type {0} not supported!", n));
            }

            if (jObject.ContainsKey("ElementType"))
            {
                var type = (int)jObject.Property("ElementType");
                switch (type)
                {
                    case 1:
                        return new IfThenElse_Saveable();
                    case 2:
                        return new AndMultiple_Saveable();
                    case 3:
                        return new OrMultiple_Saveable();
                    case 4:
                        return new ForGroupMultiple_Saveable();
                    case 5:
                        return new ForForceMultiple_Saveable();
                    case 6:
                        return new ForLoopAMultiple_Saveable();
                    case 7:
                        return new ForLoopBMultiple_Saveable();
                    case 8:
                        return new ForLoopVarMultiple_Saveable();
                    case 9:
                        return new SetVariable_Saveable();
                    case 10:
                        return new EnumDestructablesInRectAllMultiple_Saveable();
                    case 11:
                        return new EnumDestructiblesInCircleBJMultiple_Saveable();
                    case 12:
                        return new EnumItemsInRectBJ_Saveable();
                    case 13:
                        return new ActionDefinitionRef_Saveable();
                    case 14:
                        return new ConditionDefinitionRef_Saveable();
                    case 15:
                        return new FunctionDefinitionRef_Saveable();
                    case 16:
                        return new ReturnStatement_Saveable();
                    default:
                        return new ECA_Saveable();
                }

                throw new ApplicationException(String.Format("The given trigger element type {0} is not supported!", type));
            }
            else
                return new ECA_Saveable();

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

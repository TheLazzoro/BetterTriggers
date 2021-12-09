using Model.JsonBaseConverter;
using Newtonsoft.Json;
using System;

namespace Model.Natives
{
    [JsonConverter(typeof(BaseConverter))]
    public class Parameter
    {
        public string identifier;
        public string name;
        public string returnType;
    }
}

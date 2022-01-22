using Model.JsonBaseConverter;
using Newtonsoft.Json;
using System;

namespace Model.Templates
{
    //[JsonConverter(typeof(BaseConverter))]
    public class ParameterTemplate
    {
        public string identifier;
        public string name;
        public string returnType;
    }
}

using Model.JsonBaseConverter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Model.SaveableData
{
    [JsonConverter(typeof(BaseConverter))]
    public class Parameter
    {
        public string identifier; // For convenience, values also use this field.
        public string returnType;

        public Parameter Clone()
        {
            string identifier = null;
            if (this.identifier != null)
                identifier = new string(this.identifier);

            return new Parameter()
            {
                identifier = identifier,
                returnType = new string(returnType),
            };
        }
    }
}

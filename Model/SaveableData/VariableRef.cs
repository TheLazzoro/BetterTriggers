using Model.JsonBaseConverter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Model.SaveableData
{
    [JsonConverter(typeof(BaseConverter))]
    public class VariableRef : Parameter
    {
        public readonly int ParamType = 3; // DO NOT CHANGE
        public int VariableId;
        public List<Parameter> arrayIndexValues = new List<Parameter>();


        public new VariableRef Clone()
        {
            List<Parameter> newArrayIndexValues = new List<Parameter>();
            newArrayIndexValues.Add(this.arrayIndexValues[0].Clone());
            newArrayIndexValues.Add(this.arrayIndexValues[1].Clone());

            string identifier = null;
            if (this.identifier != null)
                identifier = new string(this.identifier);
            return new VariableRef()
            {
                identifier = identifier,
                returnType = new string(returnType),
                VariableId = VariableId,
                arrayIndexValues = newArrayIndexValues,
            };
        }
    }
}

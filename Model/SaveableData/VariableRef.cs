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


        public VariableRef Clone()
        {
            List<Parameter> newArrayIndexValues = new List<Parameter>();
            throw new Exception("Below will create a 'Parameter' instance and not 'Function', 'Value' or whatever it might be.");
            Parameter array0 = this.arrayIndexValues[0].Clone();
            Parameter array1 = this.arrayIndexValues[1].Clone();
            newArrayIndexValues.Add(array0);
            newArrayIndexValues.Add(array1);
            return new VariableRef()
            {
                identifier = new string(identifier),
                returnType = new string(returnType),
                VariableId = VariableId,
                arrayIndexValues = newArrayIndexValues,
            };
        }
    }
}

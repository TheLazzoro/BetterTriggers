using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class VariableRef : Parameter
    {
        public readonly int ParamType = 3; // DO NOT CHANGE
        public long VariableId;
        public List<Parameter> arrayIndexValues = new List<Parameter>();


        public override VariableRef Clone()
        {
            List<Parameter> newArrayIndexValues = new List<Parameter>();
            newArrayIndexValues.Add(arrayIndexValues[0].Clone());
            newArrayIndexValues.Add(arrayIndexValues[1].Clone());

            string value = null;
            if (this.value != null)
                value = new string(this.value);
            return new VariableRef()
            {
                value = value,
                VariableId = VariableId,
                arrayIndexValues = newArrayIndexValues,
            };
        }
    }
}

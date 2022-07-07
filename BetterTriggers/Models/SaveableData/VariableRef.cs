using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class VariableRef : Parameter
    {
        public readonly int ParamType = 3; // DO NOT CHANGE
        public int VariableId;
        public List<Parameter> arrayIndexValues = new List<Parameter>();


        public override VariableRef Clone()
        {
            List<Parameter> newArrayIndexValues = new List<Parameter>();
            newArrayIndexValues.Add(arrayIndexValues[0].Clone());
            newArrayIndexValues.Add(arrayIndexValues[1].Clone());

            string identifier = null;
            if (this.identifier != null)
                identifier = new string(this.identifier);
            return new VariableRef()
            {
                identifier = identifier,
                VariableId = VariableId,
                arrayIndexValues = newArrayIndexValues,
            };
        }
    }
}

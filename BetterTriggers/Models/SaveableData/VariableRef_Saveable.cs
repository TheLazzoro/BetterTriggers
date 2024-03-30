using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class VariableRef_Saveable : Parameter_Saveable
    {
        public readonly int ParamType = 3; // DO NOT CHANGE
        public int VariableId;
        public List<Parameter_Saveable> arrayIndexValues = new List<Parameter_Saveable>();


        public override VariableRef_Saveable Clone()
        {
            List<Parameter_Saveable> newArrayIndexValues = new List<Parameter_Saveable>();
            newArrayIndexValues.Add(arrayIndexValues[0].Clone());
            newArrayIndexValues.Add(arrayIndexValues[1].Clone());

            string value = null;
            if (this.value != null)
                value = new string(this.value);
            return new VariableRef_Saveable()
            {
                value = value,
                VariableId = VariableId,
                arrayIndexValues = newArrayIndexValues,
            };
        }
    }
}

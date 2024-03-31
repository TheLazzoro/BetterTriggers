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
    }
}

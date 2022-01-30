using Model.JsonBaseConverter;
using Newtonsoft.Json;
using System;

namespace Model.SavableTriggerData
{
    [JsonConverter(typeof(BaseConverter))]
    public class Variable : Parameter, ICloneable
    {
        public readonly int ParamType = 3; // DO NOT CHANGE
        public int VariableId; // TODO: Id reference to variable

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}

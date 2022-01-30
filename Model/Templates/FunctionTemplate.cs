using Model.Enums;
using Model.SavableTriggerData;
using System;
using System.Collections.Generic;

namespace Model.Templates
{
    public class FunctionTemplate : ParameterTemplate, ICloneable
    {
        public readonly int ParamType = 1; // DO NOT CHANGE

        public List<Parameter> parameters = new List<Parameter>();
        public string paramText;
        public string description;
        public Category category;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}

using Model.EditorData.Enums;
using Model.SaveableData;
using System;
using System.Collections.Generic;

namespace Model.Templates
{
    public class FunctionTemplate : ParameterTemplate
    {
        public readonly int ParamType = 1; // DO NOT CHANGE

        public List<Parameter> parameters = new List<Parameter>();
        public string paramText;
        public string description;
        public Category category;

        public FunctionTemplate Clone()
        {
            FunctionTemplate clone = (FunctionTemplate)this.MemberwiseClone();
            clone.parameters = new List<Parameter>(parameters);
            clone.paramText = string.Copy(paramText);
            clone.description = string.Copy(description);
            clone.identifier = string.Copy(identifier);
            clone.name = string.Copy(name);
            clone.returnType = string.Copy(returnType);

            return clone;
        }
    }
}

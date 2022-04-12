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
            if (paramText != null) // some are null
                clone.paramText = string.Copy(paramText);
            if (description != null) // some are null
                clone.description = string.Copy(description);
            clone.identifier = string.Copy(identifier);
            clone.name = string.Copy(name);
            if (returnType != null) // some are null
                clone.returnType = string.Copy(returnType);

            return clone;
        }

        public TriggerElement ToTriggerElement()
        {
            TriggerElement f = new TriggerElement();
            f.function.identifier = identifier;
            f.function.parameters = parameters;
            f.function.returnType = returnType;

            switch (identifier)
            {
                case "IfThenElseMultiple":
                    f = new IfThenElse();
                    f.function.identifier = identifier;
                    f.function.parameters = parameters;
                    f.function.returnType = returnType;
                    break;
                default:
                    break;
            }

            return f;
        }
    }
}

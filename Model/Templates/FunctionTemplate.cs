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
            TriggerElement te = new TriggerElement();
            te.function = new Function();

            switch (identifier)
            {
                case "IfThenElseMultiple":
                    te.function = new IfThenElse();
                    break;
                case "AndMultiple":
                    te.function = new AndMultiple();
                    break;
                case "OrMultiple":
                    te.function = new OrMultiple();
                    break;
                case "ForGroup":
                    te.function = new ForGroupMultiple();
                    break;
                case "ForForce":
                    te.function = new ForForceMultiple();
                    break;
                case "ForLoopAMultiple":
                    te.function = new ForLoopAMultiple();
                    break;
                case "ForLoopBMultiple":
                    te.function = new ForLoopBMultiple();
                    break;
                case "ForLoopVarMultiple":
                    te.function = new ForLoopVarMultiple();
                    break;
                default:
                    break;
            }

            te.function.identifier = identifier;
            te.function.parameters = parameters;
            te.function.returnType = returnType;

            return te;
        }
    }
}

using BetterTriggers.Models.SaveableData;
using BetterTriggers.Utility;
using System;
using System.Collections.Generic;

namespace BetterTriggers.Models.Templates
{
    public class FunctionTemplate : ParameterTemplate
    {
        public List<ParameterTemplate> parameters = new List<ParameterTemplate>();
        public string identifier;
        public string name;
        public string description;
        public string paramText;
        public string category;

        public FunctionTemplate Clone()
        {
            FunctionTemplate clone = (FunctionTemplate)this.MemberwiseClone();
            clone.parameters = new List<ParameterTemplate>(parameters);
            if (paramText != null) // some are null
                clone.paramText = new string(paramText);
            if (description != null) // some are null
                clone.description = new string(description);
            clone.identifier = new string(identifier);
            clone.name = new string(name);
            if (returnType != null) // some are null
                clone.returnType = new string(returnType);

            return clone;
        }

        public TriggerElement ToTriggerElement()
        {
            TriggerElement te = TriggerElementFactory.Create(identifier);
            List<Parameter> parameters = new List<Parameter>();
            this.parameters.ForEach(p => parameters.Add(new Parameter()));

            te.function.identifier = identifier;
            te.function.parameters = parameters;

            return te;
        }

        public List<Parameter> ConvertParameters()
        {
            List<Parameter> parameters = new List<Parameter>();
            this.parameters.ForEach(p => parameters.Add(new Parameter()));
            return parameters;
        }
    }
}

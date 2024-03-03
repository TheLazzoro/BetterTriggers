using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;
using System;
using System.Collections.Generic;

namespace BetterTriggers.Models.Templates
{
    public class FunctionTemplate : ParameterTemplate
    {
        public List<ParameterTemplate> parameters = new List<ParameterTemplate>();
        public string value;
        public string name;
        public string scriptName;
        public string description;
        public string paramText;
        public string category;
        public TriggerElementType ElementType;

        public FunctionTemplate(TriggerElementType elementType)
        {
            ElementType = elementType;
        }

        public override FunctionTemplate Clone()
        {
            FunctionTemplate clone = (FunctionTemplate)this.MemberwiseClone();
            clone.ElementType = this.ElementType;
            clone.parameters = new List<ParameterTemplate>(parameters);
            if (paramText != null) // some are null
                clone.paramText = new string(paramText);
            if (description != null) // some are null
                clone.description = new string(description);
            clone.value = new string(value);
            clone.name = new string(name);
            if (returnType != null) // some are null
                clone.returnType = new string(returnType);

            return clone;
        }

        public override Function ToParameter()
        {
            Function function = new Function();
            List<Parameter> parameters = new List<Parameter>();
            this.parameters.ForEach(p => parameters.Add(p.ToParameter()));
            function.value = new string(this.value);
            function.parameters = parameters;
            return function;
        }

        public ECA ToECA()
        {
            ECA eca = TriggerElementFactory.Create(value);
            eca.function = ToParameter();
            eca.ElementType = ElementType;
            return eca;
        }

        public List<Parameter> ConvertParameters()
        {
            List<Parameter> parameters = new List<Parameter>();
            this.parameters.ForEach(p => parameters.Add(p.ToParameter()));
            return parameters;
        }
    }
}

using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.EditorData.TriggerEditor;
using BetterTriggers.Utility;
using Cake.Incubator.EnumerableExtensions;
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

            if (Project.CurrentProject.FunctionDefinitions.Contains(name))
            {
                var definition = Project.CurrentProject.FunctionDefinitions.FindByName(name);
                var reference = new FunctionDefinitionRef();
                reference.FunctionDefinitionId = definition.Id;
                definition.Parameters.Elements.ForEach(el =>
                {
                    var paramDef = (ParameterDefinition)el;
                    this.parameters.Add(new ParameterTemplate
                    {
                        returnType = paramDef.ReturnType.Type
                    });
                });

                function = reference;
            }

            List<Parameter> parameters = new List<Parameter>();
            this.parameters.ForEach(p => parameters.Add(p.ToParameter()));
            function.value = new string(this.value);
            function.parameters = parameters;
            return function;
        }

        public ECA ToECA()
        {
            var project = Project.CurrentProject;

            ECA eca;
            if (project.ActionDefinitions.Contains(name))
            {
                var definition = project.ActionDefinitions.GetByKey(name);
                ActionDefinitionRef reference = new();
                reference.ActionDefinitionId = definition.Id;
                definition.Parameters.Elements.ForEach(el =>
                {
                    var paramDef = (ParameterDefinition)el;
                    this.parameters.Add(new ParameterTemplate
                    {
                        returnType = paramDef.ReturnType.Type
                    });
                });

                eca = reference;
            }
            else if (project.ConditionDefinitions.Contains(name))
            {
                var definition = project.ConditionDefinitions.GetByKey(name);
                ConditionDefinitionRef reference = new();
                reference.ConditionDefinitionId = definition.Id;
                definition.Parameters.Elements.ForEach(el =>
                {
                    var paramDef = (ParameterDefinition)el;
                    this.parameters.Add(new ParameterTemplate
                    {
                        returnType = paramDef.ReturnType.Type
                    });
                });

                eca = reference;
            }
            else
            {
                eca = TriggerElementFactory.Create(value);
            }

            eca.function = ToParameter();
            eca.ElementType = ElementType;
            return eca;
        }
    }
}

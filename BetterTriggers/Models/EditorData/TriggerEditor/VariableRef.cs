﻿using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class VariableRef : Parameter
    {
        public int VariableId;
        public List<Parameter> arrayIndexValues = new List<Parameter>();


        public override VariableRef Clone()
        {
            List<Parameter> newArrayIndexValues = new List<Parameter>();
            newArrayIndexValues.Add(arrayIndexValues[0].Clone());
            newArrayIndexValues.Add(arrayIndexValues[1].Clone());

            string value = null;
            if (this.value != null)
                value = new string(this.value);
            return new VariableRef()
            {
                value = value,
                VariableId = VariableId,
                arrayIndexValues = newArrayIndexValues,
            };
        }

        public static List<VariableRef> GetVariableRefsFromTrigger(ExplorerElement explorerElement)
        {
            List<Parameter> _params = GetParametersFromExplorerElement(explorerElement);
            List<VariableRef> variableRefs = new List<VariableRef>();
            _params.ForEach(p =>
            {
                var varRef = p as VariableRef;
                if (varRef != null)
                    variableRefs.Add(varRef);
            });

            return variableRefs;
        }
    }
}

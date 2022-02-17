using Model.SaveableData;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.JsonBaseConverter
{
    public class ParameterConverter : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(System.Type objectType)
        {
            if (typeof(Parameter).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }
}

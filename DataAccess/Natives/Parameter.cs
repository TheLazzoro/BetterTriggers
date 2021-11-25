using DataAccess.Containers;
using DataAccess.JsonBaseConverter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Natives
{
    [JsonConverter(typeof(BaseConverter))]
    public class Parameter
    {
        public string name;
        public Type returnType;

        public Parameter(string identifier, Type returnType, string name)
        {
            this.returnType = returnType;
            this.name = name;
        }
    }
}

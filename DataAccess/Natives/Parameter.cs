using Model.Containers;
using Model.JsonBaseConverter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Natives
{
    [JsonConverter(typeof(BaseConverter))]
    public class Parameter
    {
        public string identifier;
        public string name;
        public string returnType;

        public Parameter()
        {
        }
    }
}

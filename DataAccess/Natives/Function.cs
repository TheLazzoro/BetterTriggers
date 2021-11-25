using DataAccess.Containers;
using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Natives
{
    public class Function : Parameter
    {
        public int ParamType = 1; // DO NOT CHANGE

        public string identifier;
        public List<Parameter> parameters = new List<Parameter>();
        public string funcText;
        public string description;
        public EnumCategory category;

        public Function(string identifier, List<Parameter> parameters, Type returnType, string name, string funcText, string description, EnumCategory category) : base(identifier, returnType, name)
        {
            this.identifier = identifier;
            this.parameters = parameters;
            this.funcText = funcText;
            this.description = description;
            this.category = category;

            ContainerFunctions.AddParameter(this);
        }
    }
}

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

        public List<Parameter> parameters = new List<Parameter>();
        public string funcText;
        public string description;

        public Function(string identifier, List<Parameter> parameters, Type returnType, string name, string funcText, string description) : base(identifier, returnType, name)
        {
            this.parameters = parameters;
            this.funcText = funcText;
            this.description = description;
        }
    }
}

using Model.Containers;
using Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Natives
{
    public class Function : Parameter
    {
        public int ParamType = 1; // DO NOT CHANGE

        public List<Parameter> parameters = new List<Parameter>();
        public string paramText;
        public string description;
        public EnumCategory category;

        public Function()
        {
        }
    }
}

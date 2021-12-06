using Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Natives
{
    public class Condition
    {
        public string identifier;
        public string displayName;
        public string paramText;
        public List<Parameter> parameters;
        public EnumCategory category;
    }
}

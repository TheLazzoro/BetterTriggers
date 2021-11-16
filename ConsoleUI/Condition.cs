using System;
using System.Collections.Generic;

namespace BetterTriggers.Data
{
    public class Condition
    {
        public string name;
        public List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();
        public string returnType;

        public Condition(string name, List<Tuple<string, string>> parameters, string returnType)
        {
            this.name = name;
            this.parameters = parameters;
            this.returnType = returnType;
        }
    }
}

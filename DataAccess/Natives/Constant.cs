
using DataAccess.Containers;

namespace DataAccess.Natives
{
    public class Constant : Parameter
    {
        public int ParamType = 2; // DO NOT CHANGE

        public Constant(string identifier, Type returnType, string name) : base(identifier, returnType, name)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.SaveableData
{
    public class Variable
    {
        public int Id;
        public string Name;
        public string Type;
        public bool IsArray;
        public bool IsTwoDimensions;
        public int[] ArraySize = new int[2];
        public string InitialValue;

        public List<int> TriggersUsing = new List<int>(); // trigger Id's using this variable

        internal Variable Clone()
        {
            Variable cloned = new Variable();
            cloned.Name = new string(Name);
            cloned.Type = new string(Type);
            cloned.IsArray = IsArray;
            cloned.IsTwoDimensions = IsTwoDimensions;
            cloned.ArraySize = new int[2] { ArraySize[0], ArraySize[1] };
            cloned.InitialValue = new string(InitialValue);

            return cloned;
        }
    }
}
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
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

        public Variable Clone()
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
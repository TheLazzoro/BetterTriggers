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
        public Parameter InitialValue;

        public Variable Clone()
        {
            Variable cloned = new Variable();
            cloned.Name = new string(Name);
            cloned.Type = new string(Type);
            cloned.IsArray = IsArray;
            cloned.IsTwoDimensions = IsTwoDimensions;
            cloned.ArraySize = new int[2] { ArraySize[0], ArraySize[1] };
            cloned.InitialValue = this.InitialValue.Clone();

            return cloned;
        }

        public string GetGlobalName()
        {
            string name = "udg_" + Name.Replace(" ", "_");
            if(name.EndsWith("_"))
                name = name + "v";

            return name;
        }
    }
}
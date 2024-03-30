using BetterTriggers.Models.EditorData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class Variable_Saveable
    {
        public int Id;
        public int[] ArraySize = new int[2];
        public string Name;
        public string Type;
        public bool IsArray;
        public bool IsTwoDimensions;
        public Parameter_Saveable InitialValue;
    }
}
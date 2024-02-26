using BetterTriggers.Models.EditorData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class Variable_Saveable
    {
        public int Id;
        public string Name;
        public string Type;
        public bool IsArray;
        public bool IsTwoDimensions;
        public int[] ArraySize = new int[2]; // TODO: how to detect set values in array indexes?
        public Parameter_Saveable InitialValue;
    }
}
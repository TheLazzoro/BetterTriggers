using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.SaveableData
{
    public class FunctionDefinition_Saveable : ECA
    {
        public readonly int ElementType = 15; // DO NOT CHANGE
        public int Id;
        public string Comment;
        public string ReturnType;
        public List<Parameter> Parameters = new();
        public List<TriggerElement> Actions = new();
        public List<TriggerElement> LocalVariables = new();
    }
}
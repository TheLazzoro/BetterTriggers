using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class Trigger_Saveable
    {
        public int Id;
        public string Comment;
        public bool IsScript;
        public bool RunOnMapInit;
        public string Script;
        public List<TriggerElement_Saveable> Events = new();
        public List<TriggerElement_Saveable> LocalVariables = new();
        public List<TriggerElement_Saveable> Conditions = new();
        public List<TriggerElement_Saveable> Actions = new();
    }
}

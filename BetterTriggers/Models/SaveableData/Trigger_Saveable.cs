using BetterTriggers.Containers;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class Trigger_Saveable : IReferable_Saveable
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

        internal Trigger_Saveable Clone()
        {
            Trigger_Saveable cloned = new Trigger_Saveable();
            cloned.Comment = new string(Comment);
            this.Events.ForEach(element => cloned.Events.Add(element.Clone()));
            this.Conditions.ForEach(element => cloned.Conditions.Add(element.Clone()));
            this.LocalVariables.ForEach(element => cloned.LocalVariables.Add(element.Clone()));
            this.Actions.ForEach(element => cloned.Actions.Add(element.Clone()));

            return cloned;
        }
    }
}

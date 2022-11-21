using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class Trigger : IReferable
    {
        public int Id;
        public string Comment;
        public bool IsScript;
        public bool RunOnMapInit;
        public string Script;
        public List<TriggerElement> Events = new ();
        public List<TriggerElement> Conditions = new ();
        public List<TriggerElement> LocalVariables = new ();
        public List<TriggerElement> Actions = new ();

        internal Trigger Clone()
        {
            Trigger cloned = new Trigger();
            cloned.Comment = new string(Comment);
            this.Events.ForEach(element => cloned.Events.Add(element.Clone()));
            this.Conditions.ForEach(element => cloned.Conditions.Add(element.Clone()));
            this.LocalVariables.ForEach(element => cloned.LocalVariables.Add(element.Clone()));
            this.Actions.ForEach(element => cloned.Actions.Add(element.Clone()));

            return cloned;
        }
    }
}

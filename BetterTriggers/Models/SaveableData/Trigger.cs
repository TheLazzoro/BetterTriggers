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
        public List<TriggerElement> Events = new List<TriggerElement>();
        public List<TriggerElement> Conditions = new List<TriggerElement>();
        public List<LocalVariable> LocalVariables = new List<LocalVariable>();
        public List<TriggerElement> Actions = new List<TriggerElement>();

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

        internal void CreateLocalVariable()
        {
            string name = "udl_UntitledVariable";
            int i = 0;
            string suffix = "";
            bool validName = false;
            while (!validName)
            {
                foreach (var localVar in LocalVariables)
                {
                    name = localVar.variable.Name;
                    
                }
            }
        }
    }
}

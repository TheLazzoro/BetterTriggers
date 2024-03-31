using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class ActionDefinition : ECA, IReferable
    {
        public int Id;
        public string Comment;
        public List<Parameter> Parameters = new();
        public List<TriggerElement> Actions = new();
        public List<TriggerElement> LocalVariables = new();

        public override ActionDefinition Clone()
        {
            ActionDefinition cloned = new ActionDefinition();
            cloned.Comment = new string(Comment);
            this.Actions.ForEach(element => cloned.Actions.Add(element.Clone()));
            this.LocalVariables.ForEach(element => cloned.LocalVariables.Add(element.Clone()));

            return cloned;
        }
    }
}
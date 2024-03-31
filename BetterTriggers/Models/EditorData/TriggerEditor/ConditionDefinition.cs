using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class ConditionDefinition : ECA
    {
        public int Id;
        public string Comment;
        public string ReturnType;
        public List<Parameter> Parameters = new();
        public List<TriggerElement> Actions = new();
        public List<TriggerElement> LocalVariables = new();

        public override ConditionDefinition Clone()
        {
            ConditionDefinition cloned = new ConditionDefinition();
            cloned.Comment = new string(Comment);
            this.Actions.ForEach(element => cloned.Actions.Add(element.Clone()));
            this.LocalVariables.ForEach(element => cloned.LocalVariables.Add(element.Clone()));

            return cloned;
        }
    }
}
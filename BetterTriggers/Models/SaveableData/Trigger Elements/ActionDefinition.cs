using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.SaveableData
{
    public class ActionDefinition : ECA, IReferable
    {
        public readonly int ElementType = 13; // DO NOT CHANGE
        public int Id;
        public string Comment;
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

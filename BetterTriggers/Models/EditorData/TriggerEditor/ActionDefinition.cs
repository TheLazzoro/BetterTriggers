using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class ActionDefinition : IReferable
    {
        public int Id;
        public string Comment;
        public string Category;
        public List<Parameter> Parameters = new();
        public TriggerElementCollection LocalVariables = new(TriggerElementType.LocalVariable);
        public TriggerElementCollection Actions = new(TriggerElementType.Action);

        public ActionDefinition Clone()
        {
            ActionDefinition cloned = new ActionDefinition();
            cloned.Comment = new string(Comment);
            cloned.Category = new string(Category);
            cloned.LocalVariables = LocalVariables.Clone();
            cloned.Actions = Actions.Clone();

            return cloned;
        }
    }
}
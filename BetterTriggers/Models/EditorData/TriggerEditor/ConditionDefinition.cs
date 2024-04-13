using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class ConditionDefinition : IReferable
    {
        public int Id;
        public string Comment;
        public string Category;
        public string ParamText;
        public string ReturnType;
        public ParameterDefinitionCollection Parameters = new(TriggerElementType.ParameterDef);
        public TriggerElementCollection LocalVariables = new(TriggerElementType.LocalVariable);
        public TriggerElementCollection Actions = new(TriggerElementType.Action);

        public ConditionDefinition Clone()
        {
            ConditionDefinition cloned = new ConditionDefinition();
            cloned.Comment = new string(Comment);
            cloned.Category = new string(Category);
            cloned.ParamText = new string(ParamText);
            cloned.ReturnType = new string(ReturnType);
            cloned.LocalVariables = LocalVariables.Clone();
            cloned.Actions = Actions.Clone();

            return cloned;
        }
    }
}
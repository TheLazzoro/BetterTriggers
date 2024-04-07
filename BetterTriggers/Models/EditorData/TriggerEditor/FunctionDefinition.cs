using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.EditorData.TriggerEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class FunctionDefinition : IReferable
    {
        public int Id;
        public string Comment;
        public string Category;
        public string ParamText;
        public ReturnType ReturnType;
        public TriggerElementCollection Parameters = new(TriggerElementType.ParameterDef);
        public TriggerElementCollection LocalVariables = new(TriggerElementType.LocalVariable);
        public TriggerElementCollection Actions = new(TriggerElementType.Action);

        public FunctionDefinition Clone()
        {
            FunctionDefinition cloned = new FunctionDefinition();
            cloned.Comment = new string(Comment);
            cloned.Category = new string(Category);
            cloned.ParamText = new string(ParamText);
            cloned.ReturnType = ReturnType.Clone();
            cloned.LocalVariables = LocalVariables.Clone();
            cloned.Actions = Actions.Clone();

            return cloned;
        }
    }
}
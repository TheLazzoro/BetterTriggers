using BetterTriggers.Containers;

namespace BetterTriggers.Models.EditorData
{
    public class FunctionDefinition : IReferable
    {
        public ExplorerElement explorerElement;
        public int Id;
        public string Comment;
        public string Category;
        public string ParamText;
        public ReturnType ReturnType;
        public ParameterDefinitionCollection Parameters;
        public TriggerElementCollection LocalVariables;
        public TriggerElementCollection Actions;

        private Project _project;

        public FunctionDefinition(Project project, ExplorerElement explorerElement)
        {
            _project = project;
            this.explorerElement = explorerElement;
            Parameters = new(project, TriggerElementType.ParameterDef);
            LocalVariables = new(project, TriggerElementType.LocalVariable);
            Actions = new(project, TriggerElementType.Action);
        }

        public FunctionDefinition Clone()
        {
            FunctionDefinition cloned = new FunctionDefinition(_project, explorerElement);
            cloned.Comment = new string(Comment);
            cloned.Category = new string(Category);
            cloned.ParamText = new string(ParamText);
            cloned.ReturnType = ReturnType.Clone();
            cloned.Parameters = Parameters.Clone();
            cloned.LocalVariables = LocalVariables.Clone();
            cloned.Actions = Actions.Clone();

            return cloned;
        }
    }
}
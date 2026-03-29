using BetterTriggers.Containers;
using System.Text;

namespace BetterTriggers.Models.EditorData
{
    public class ConditionDefinition : IReferable
    {
        public ExplorerElement explorerElement;
        public int Id;
        public string Comment;
        //public string Category; // TODO: maybe add support for this later?
        public string ParamText
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(explorerElement.GetName());
                sb.Append("(");
                for (int i = 0; i < Parameters.Elements.Count; i++)
                {
                    var parameter = (ParameterDefinition)Parameters.Elements[i];
                    sb.Append($",~{parameter.Name},");
                    if (i < Parameters.Elements.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
                sb.Append(")");
                return sb.ToString();
            }
        }
        public ParameterDefinitionCollection Parameters;
        public TriggerElementCollection LocalVariables;
        public TriggerElementCollection Actions;

        private Project _project;

        public ConditionDefinition(Project project, ExplorerElement explorerElement)
        {
            _project = project;
            this.explorerElement = explorerElement;
            Parameters = new(project, TriggerElementType.ParameterDef);
            LocalVariables = new(project, TriggerElementType.LocalVariable);
            Actions = new(project, TriggerElementType.Action);
        }

        public ConditionDefinition Clone()
        {
            ConditionDefinition cloned = new ConditionDefinition(_project, explorerElement);
            cloned.Comment = new string(Comment);
            cloned.Parameters = Parameters.Clone();
            cloned.LocalVariables = LocalVariables.Clone();
            cloned.Actions = Actions.Clone();

            return cloned;
        }
    }
}
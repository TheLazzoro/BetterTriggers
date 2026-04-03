using BetterTriggers.Containers;

namespace BetterTriggers.Models.EditorData.TriggerEditor
{
    public class ActionDefinitionRef : ECA, IReferable
    {
        public int ActionDefinitionId;

        public ActionDefinitionRef(Project project) : base(project) { }

        public override ActionDefinitionRef Clone()
        {
            var cloned = new ActionDefinitionRef(_project);
            cloned.function = this.function.Clone();
            cloned.ActionDefinitionId = ActionDefinitionId;
            cloned.ElementType = ElementType;
            cloned.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(cloned.IconImage, 0);

            return cloned;
        }
    }
}

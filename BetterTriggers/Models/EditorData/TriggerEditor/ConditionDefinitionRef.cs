using BetterTriggers.Containers;

namespace BetterTriggers.Models.EditorData.TriggerEditor
{
    public class ConditionDefinitionRef : ECA, IReferable
    {
        public int ConditionDefinitionId;

        public ConditionDefinitionRef(Project project) : base(project) { }

        public override ConditionDefinitionRef Clone()
        {
            var cloned = new ConditionDefinitionRef(_project);
            cloned.function = this.function.Clone();
            cloned.ConditionDefinitionId = ConditionDefinitionId;
            cloned.ElementType = ElementType;
            cloned.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(cloned.IconImage, 0);

            return cloned;
        }
    }
}

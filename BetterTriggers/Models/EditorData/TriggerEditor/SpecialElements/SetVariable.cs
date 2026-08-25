using BetterTriggers.Containers;

namespace BetterTriggers.Models.EditorData
{
    public class SetVariable : ECA
    {
        public SetVariable(Project project) : base(project)
        {
            function.value = "SetVariable";
        }

        public override SetVariable Clone()
        {
            SetVariable clone = new SetVariable(Project);
            clone.DisplayText = new string(DisplayText);
            clone.function = this.function.Clone();
            clone.ElementType = ElementType;
            clone.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(clone.IconImage, 0);

            return clone;
        }
    }
}

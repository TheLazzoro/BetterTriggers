using BetterTriggers.Containers;

namespace BetterTriggers.Models.EditorData
{
    public class AndMultiple : ECA
    {
        public TriggerElementCollection And
        {
            get => _and;
            set
            {
                if (_and != null)
                {
                    _and.RemoveFromParent();
                }
                _and = value;
                _and.SetParent(this, 0);
            }
        }

        private TriggerElementCollection _and;

        public AndMultiple(Project project) : base(project)
        {
            function.value = "AndMultiple";
            Elements = new();
            And = new(project, TriggerElementType.Condition);
            IsExpandedTreeItem = true;
        }

        public override AndMultiple Clone()
        {
            AndMultiple clone = new AndMultiple(Project);
            clone.DisplayText = new string(DisplayText);
            clone.function = this.function.Clone();
            clone.And = And.Clone();
            clone.ElementType = ElementType;
            clone.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(clone.IconImage, 0);

            return clone;
        }
    }
}

using BetterTriggers.Containers;

namespace BetterTriggers.Models.EditorData
{
    public class OrMultiple : ECA
    {
        public TriggerElementCollection Or
        {
            get => _or;
            set
            {
                if (_or != null)
                {
                    _or.RemoveFromParent();
                }
                _or = value;
                _or.SetParent(this, 0);
            }
        }

        private TriggerElementCollection _or;

        public OrMultiple(Project project) : base(project)
        {
            function.value = "OrMultiple";
            Elements = new();
            Or = new(project, TriggerElementType.Condition);
            IsExpandedTreeItem = true;
        }

        public override OrMultiple Clone()
        {
            OrMultiple clone = new OrMultiple(_project);
            clone.DisplayText = new string(DisplayText);
            clone.function = this.function.Clone();
            clone.Or = Or.Clone();
            clone.ElementType = ElementType;
            clone.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(clone.IconImage, 0);

            return clone;
        }
    }
}

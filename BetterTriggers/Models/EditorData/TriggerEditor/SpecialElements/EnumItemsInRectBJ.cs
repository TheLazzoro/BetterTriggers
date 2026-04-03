using BetterTriggers.Containers;

namespace BetterTriggers.Models.EditorData
{
    public class EnumItemsInRectBJ : ECA
    {
        public TriggerElementCollection Actions
        {
            get => _actions;
            set
            {
                if (_actions != null)
                {
                    _actions.RemoveFromParent();
                }
                _actions = value;
                _actions.SetParent(this, 0);
            }
        }

        private TriggerElementCollection _actions;

        public EnumItemsInRectBJ(Project project) : base(project)
        {
            function.value = "EnumItemsInRectBJMultiple";
            Elements = new();
            Actions = new(project, TriggerElementType.Action);
            IsExpandedTreeItem = true;
        }

        public override EnumItemsInRectBJ Clone()
        {
            EnumItemsInRectBJ clone = new EnumItemsInRectBJ(_project);
            clone.DisplayText = new string(DisplayText);
            clone.function = this.function.Clone();
            clone.Actions = Actions.Clone();
            clone.ElementType = ElementType;
            clone.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(clone.IconImage, 0);

            return clone;
        }
    }
}

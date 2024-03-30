using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class ForLoopAMultiple : ECA
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

        public ForLoopAMultiple()
        {
            function.value = "ForLoopAMultiple";
            Elements = new();
            Actions = new(TriggerElementType.Action);
            IsExpandedTreeItem = true;
        }

        public override ForLoopAMultiple Clone()
        {
            ForLoopAMultiple clone = new ForLoopAMultiple();
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

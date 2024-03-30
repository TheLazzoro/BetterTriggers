using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class ForGroupMultiple : ECA
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

        public ForGroupMultiple()
        {
            function.value = "ForGroupMultiple";
            Elements = new();
            Actions = new(TriggerElementType.Action);
            IsExpandedTreeItem = true;
        }

        public override ForGroupMultiple Clone()
        {
            ForGroupMultiple clone = new ForGroupMultiple();
            clone.DisplayText = new string(DisplayText);
            clone.function = this.function.Clone();
            clone.ElementType = ElementType;
            clone.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(clone.IconImage, 0);
            clone.Actions = Actions.Clone();

            return clone;
        }
    }
}

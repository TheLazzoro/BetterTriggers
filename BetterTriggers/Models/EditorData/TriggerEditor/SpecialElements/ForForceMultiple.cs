using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class ForForceMultiple : ECA
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

        public ForForceMultiple()
        {
            function.value = "ForForceMultiple";
            Elements = new();
            Actions = new(TriggerElementType.Action);
        }

        public override ForForceMultiple Clone()
        {
            ForForceMultiple clone = new ForForceMultiple();
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

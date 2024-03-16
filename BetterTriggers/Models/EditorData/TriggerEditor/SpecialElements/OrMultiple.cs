using System;
using System.Collections.Generic;

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

        public OrMultiple()
        {
            function.value = "OrMultiple";
            Elements = new();
            Or = new(TriggerElementType.Condition);
        }

        public override OrMultiple Clone()
        {
            OrMultiple clone = new OrMultiple();
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

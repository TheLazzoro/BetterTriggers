using System;

namespace BetterTriggers.Models.EditorData
{
    /// <summary>
    /// 'Event', 'Condition' and 'Action' nodes.
    /// </summary>
    public class TriggerElementCollection : TriggerElement
    {
        public TriggerElementCollection(TriggerElementType Type)
        {
            IsExpandedTreeItem = true;
            ElementType = Type;
            Elements = new();
            Category category;
            switch (Type)
            {
                case TriggerElementType.Event:
                    DisplayText = "Events";
                    category = Category.Get(TriggerCategory.TC_EVENT);
                    break;
                case TriggerElementType.Condition:
                    DisplayText = "Conditions";
                    category = Category.Get(TriggerCategory.TC_CONDITION_NEW);
                    break;
                case TriggerElementType.LocalVariable:
                    DisplayText = "Local Variables";
                    category = Category.Get(TriggerCategory.TC_LOCAL_VARIABLE);
                    break;
                case TriggerElementType.Action:
                    DisplayText = "Actions";
                    category = Category.Get(TriggerCategory.TC_ACTION);
                    break;
                default:
                    category = Category.Get(TriggerCategory.TC_EVENT);
                    break;
            }

            IconImage = category.Icon;
        }

        public override TriggerElementCollection Clone()
        {
            TriggerElementCollection clone = new TriggerElementCollection(ElementType);
            this.Elements.ForEach(element => clone.Elements.Add(element.Clone()));
            return clone;
        }

    }
}

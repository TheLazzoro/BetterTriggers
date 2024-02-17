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
            ElementType = Type;
            Elements = new();
        }

        public override TriggerElementCollection Clone()
        {
            TriggerElementCollection clone = new TriggerElementCollection(ElementType);
            this.Elements.ForEach(element => clone.Elements.Add(element.Clone()));
            return clone;
        }

        public int Count()
        {
            return Elements.Count;
        }

        public void Insert(TriggerElement triggerElement, int insertIndex)
        {
            Elements.Insert(insertIndex, triggerElement);
        }

        public int IndexOf(TriggerElement element)
        {
            return Elements.IndexOf(element);
        }
    }
}

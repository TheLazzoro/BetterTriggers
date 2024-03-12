using System.Collections.Generic;

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

        public EnumItemsInRectBJ()
        {
            function.value = "EnumItemsInRectBJMultiple";
            Elements = new();
            Actions = new(TriggerElementType.Action);
        }

        public override EnumItemsInRectBJ Clone()
        {
            EnumItemsInRectBJ enumItems = new EnumItemsInRectBJ();
            enumItems.function = this.function.Clone();
            enumItems.Actions = Actions.Clone();

            return enumItems;
        }
    }
}

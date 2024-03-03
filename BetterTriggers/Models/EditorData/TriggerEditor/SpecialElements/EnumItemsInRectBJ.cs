using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    public class EnumItemsInRectBJ : ECA
    {
        public TriggerElementCollection Actions = new(TriggerElementType.Action);

        public EnumItemsInRectBJ()
        {
            function.value = "EnumItemsInRectBJMultiple";
            Elements = new();
            Actions.SetParent(this, 0);
            Actions.DisplayText = "Loop - Actions";
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

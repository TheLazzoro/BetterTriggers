using System.Collections.Generic;

namespace Model.SaveableData
{
    public class EnumItemsInRectBJ : Function
    {
        public readonly int ParamType = 16; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();
        
        public EnumItemsInRectBJ Clone()
        {
            EnumItemsInRectBJ enumItems = new EnumItemsInRectBJ();
            enumItems.identifier = new string(identifier);
            enumItems.returnType = new string(returnType);
            enumItems.Actions = new List<TriggerElement>();
            Actions.ForEach(element => enumItems.Actions.Add(element.Clone()));

            Function f = base.Clone();
            enumItems.parameters = f.parameters;

            return enumItems;
        }
    }
}

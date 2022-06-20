using System.Collections.Generic;

namespace Model.SaveableData
{
    public class EnumDestructablesInRectAllMultiple : Function
    {
        public readonly int ParamType = 14; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();
        
        public new EnumDestructablesInRectAllMultiple Clone()
        {
            EnumDestructablesInRectAllMultiple enumDest = new EnumDestructablesInRectAllMultiple();
            enumDest.identifier = new string(identifier);
            enumDest.returnType = new string(returnType);
            enumDest.Actions = new List<TriggerElement>();
            Actions.ForEach(element => enumDest.Actions.Add(element.Clone()));

            Function f = base.Clone();
            enumDest.parameters = f.parameters;

            return enumDest;
        }
    }
}

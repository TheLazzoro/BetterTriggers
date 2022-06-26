using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class EnumDestructiblesInCircleBJMultiple : Function
    {
        public readonly int ParamType = 15; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();
        
        public new EnumDestructiblesInCircleBJMultiple Clone()
        {
            EnumDestructiblesInCircleBJMultiple enumDest = new EnumDestructiblesInCircleBJMultiple();
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

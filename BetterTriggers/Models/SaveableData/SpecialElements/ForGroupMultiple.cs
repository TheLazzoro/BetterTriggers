using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ForGroupMultiple : Function
    {
        public readonly int ParamType = 8; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();
        
        public new ForGroupMultiple Clone()
        {
            ForGroupMultiple forGroupMultiple = new ForGroupMultiple();
            forGroupMultiple.identifier = new string(identifier);
            forGroupMultiple.returnType = new string(returnType);
            forGroupMultiple.Actions = new List<TriggerElement>();
            Actions.ForEach(element => forGroupMultiple.Actions.Add(element.Clone()));

            Function f = base.Clone();
            forGroupMultiple.parameters = f.parameters;

            return forGroupMultiple;
        }
    }
}

using System.Collections.Generic;

namespace Model.SaveableData
{
    public class ForForceMultiple : Function
    {
        public readonly int ParamType = 9; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();
        
        public ForForceMultiple Clone()
        {
            ForForceMultiple forForceMultiple = new ForForceMultiple();
            forForceMultiple.identifier = new string(identifier);
            forForceMultiple.returnType = new string(returnType);
            forForceMultiple.Actions = new List<TriggerElement>();
            Actions.ForEach(element => forForceMultiple.Actions.Add(element.Clone()));

            Function f = base.Clone();
            forForceMultiple.parameters = f.parameters;

            return forForceMultiple;
        }
    }
}

using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ForLoopBMultiple : Function
    {
        public readonly int ParamType = 11; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();
        
        public new ForLoopBMultiple Clone()
        {
            ForLoopBMultiple forLoopBMultiple = new ForLoopBMultiple();
            forLoopBMultiple.identifier = new string(identifier);
            forLoopBMultiple.returnType = new string(returnType);
            forLoopBMultiple.Actions = new List<TriggerElement>();
            Actions.ForEach(element => forLoopBMultiple.Actions.Add(element.Clone()));

            Function f = base.Clone();
            forLoopBMultiple.parameters = f.parameters;

            return forLoopBMultiple;
        }
    }
}

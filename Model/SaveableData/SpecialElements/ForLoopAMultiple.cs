using System.Collections.Generic;

namespace Model.SaveableData
{
    public class ForLoopAMultiple : Function
    {
        public readonly int ParamType = 10; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();
        
        public ForLoopAMultiple Clone()
        {
            ForLoopAMultiple forLoopAMultiple = new ForLoopAMultiple();
            forLoopAMultiple.identifier = new string(identifier);
            forLoopAMultiple.returnType = new string(returnType);
            forLoopAMultiple.Actions = new List<TriggerElement>();
            Actions.ForEach(element => forLoopAMultiple.Actions.Add(element.Clone()));

            Function f = base.Clone();
            forLoopAMultiple.parameters = f.parameters;

            return forLoopAMultiple;
        }
    }
}

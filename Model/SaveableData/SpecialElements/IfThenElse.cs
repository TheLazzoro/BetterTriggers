using System.Collections.Generic;

namespace Model.SaveableData
{
    public class IfThenElse : Function
    {
        public readonly int ParamType = 5; // DO NOT CHANGE
        public List<TriggerElement> If = new List<TriggerElement>();
        public List<TriggerElement> Then = new List<TriggerElement>();
        public List<TriggerElement> Else = new List<TriggerElement>();
        
        public IfThenElse Clone()
        {
            IfThenElse ifThenElse = new IfThenElse();
            ifThenElse.identifier = new string(identifier);
            ifThenElse.returnType = new string(returnType);
            ifThenElse.If = new List<TriggerElement>();
            ifThenElse.Then = new List<TriggerElement>();
            ifThenElse.Else = new List<TriggerElement>();
            If.ForEach(element => ifThenElse.If.Add(element.Clone()));
            Then.ForEach(element => ifThenElse.Then.Add(element.Clone()));
            Else.ForEach(element => ifThenElse.Else.Add(element.Clone()));

            return ifThenElse;
        }
    }
}

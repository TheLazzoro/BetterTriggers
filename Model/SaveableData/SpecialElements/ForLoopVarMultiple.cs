using Model.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.SaveableData
{
    public class ForLoopVarMultiple : Function
    {
        public readonly int ParamType = 12; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();
        
        public ForLoopVarMultiple Clone()
        {
            ForLoopVarMultiple forLoopVarMultiple = new ForLoopVarMultiple();
            forLoopVarMultiple.identifier = new string(identifier);
            forLoopVarMultiple.returnType = new string(returnType);
            forLoopVarMultiple.Actions = new List<TriggerElement>();
            Actions.ForEach(element => forLoopVarMultiple.Actions.Add(element.Clone()));

            return forLoopVarMultiple;
        }
    }
}

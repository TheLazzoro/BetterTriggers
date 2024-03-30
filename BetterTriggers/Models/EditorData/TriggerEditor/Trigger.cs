using BetterTriggers.Models.SaveableData;
using ICSharpCode.Decompiler.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class Trigger : IReferable
    {
        public int Id;
        public string Comment;
        public string Script;
        public bool RunOnMapInit;
        public bool IsScript;
        public TriggerElementCollection Events = new(TriggerElementType.Event);
        public TriggerElementCollection Conditions = new(TriggerElementType.Condition);
        public TriggerElementCollection LocalVariables = new(TriggerElementType.LocalVariable);
        public TriggerElementCollection Actions = new(TriggerElementType.Action);

        public Trigger Clone()
        {
            // NOTE: we don't clone the script since it would cause script errors. Maybe we should?
            Trigger cloned = new Trigger();
            cloned.Comment = new string(Comment);
            cloned.Events = Events.Clone();
            cloned.Conditions = Conditions.Clone();
            cloned.LocalVariables = LocalVariables.Clone();
            cloned.Actions = Actions.Clone();

            return cloned;
        }
    }
}

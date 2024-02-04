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
    public class Trigger
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

        public Trigger(Trigger_Saveable trigger)
        {
            Id = trigger.Id;
            Comment = trigger.Comment;
            Script = trigger.Script;
            RunOnMapInit = trigger.RunOnMapInit;
            IsScript = trigger.IsScript;
            trigger.Events.ForEach(e => Events.Elements.Add(new ));
        }

        public Trigger Clone()
        {
            Trigger cloned = new Trigger();
            cloned.Comment = new string(Comment);
            cloned.Events = Events.Clone();
            cloned.Conditions = Conditions.Clone();
            cloned.LocalVariables = LocalVariables.Clone();
            cloned.Actions = Actions.Clone();

            return cloned;
        }

        public SaveableData.Trigger_Saveable Serialize()
        {
            SaveableData.Trigger_Saveable trig = new SaveableData.Trigger_Saveable();
            trig.Id = Id;
            trig.Comment = Comment;
            trig.Script = Script;
            trig.RunOnMapInit = RunOnMapInit;
            trig.IsScript = IsScript;
            Events.Elements.ForEach(e => trig.Events.Add(e.));
        }
    }
}

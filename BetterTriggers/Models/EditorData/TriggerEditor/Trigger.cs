using BetterTriggers.Containers;

namespace BetterTriggers.Models.EditorData
{
    public class Trigger : IReferable
    {
        public int Id;
        public string Comment;
        public string Script;
        public bool RunOnMapInit;
        public bool IsScript;
        public TriggerElementCollection Events;
        public TriggerElementCollection Conditions;
        public TriggerElementCollection LocalVariables;
        public TriggerElementCollection Actions;

        private Project _project;

        public Trigger(Project project)
        {
            _project = project;
            Events = new(project, TriggerElementType.Event);
            Conditions = new(project, TriggerElementType.Condition);
            LocalVariables = new(project, TriggerElementType.LocalVariable);
            Actions = new(project, TriggerElementType.Action);
        }

        public Trigger Clone()
        {
            // NOTE: we don't clone the script since it would cause script errors. Maybe we should?
            var cloned = new Trigger(_project);
            cloned.Comment = new string(Comment);
            cloned.Events = Events.Clone();
            cloned.Conditions = Conditions.Clone();
            cloned.LocalVariables = LocalVariables.Clone();
            cloned.Actions = Actions.Clone();

            return cloned;
        }
    }
}

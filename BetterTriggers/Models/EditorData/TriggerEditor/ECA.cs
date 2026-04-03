using BetterTriggers.Containers;

namespace BetterTriggers.Models.EditorData
{
    /// <summary>
    /// Event, Condition, Action.
    /// </summary>
    public class ECA : TriggerElement
    {
        public Function function = new Function();

        public ECA(Project project) : base(project) { }

        public ECA(Project project, string value) : base(project)
        {
            function.value = value;
        }

        public override ECA Clone()
        {
            ECA clone = new ECA(_project);
            clone.DisplayText = new string(DisplayText);
            clone.IsEnabled = IsEnabled;
            clone.function = function.Clone();
            clone.ElementType = ElementType;
            clone.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(clone.IconImage, 0);

            return clone;
        }
    }
}

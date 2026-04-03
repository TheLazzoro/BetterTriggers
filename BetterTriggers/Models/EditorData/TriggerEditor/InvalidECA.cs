using BetterTriggers.Containers;

namespace BetterTriggers.Models.EditorData
{
    public class InvalidECA : ECA
    {
        public InvalidECA(Project project) : base(project)
        {
            DisplayText = "Invalid trigger element";
            function.value = "InvalidECA";
            IsEnabled = false;
        }
    }
}

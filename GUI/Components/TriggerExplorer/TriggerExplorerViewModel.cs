using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using System.Collections.ObjectModel;

namespace GUI.Components
{
    public class TriggerExplorerViewModel
    {
        public ObservableCollection<ExplorerElement> ProjectFiles { get => _project.projectFiles; }
        public ObservableCollection<ExplorerElement> SearchedFiles { get; set; } = new();

        private Project _project;

        public TriggerExplorerViewModel(Project project)
        {
            _project = project;
        }
    }
}

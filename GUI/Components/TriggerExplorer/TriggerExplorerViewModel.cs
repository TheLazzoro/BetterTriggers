using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GUI.Components
{
    public class TriggerExplorerViewModel
    {
        public ObservableCollection<ExplorerElement> ProjectFiles { get => Project.CurrentProject.projectFiles; }
        public ObservableCollection<ExplorerElement> SearchedFiles { get; set; } = new();

    }
}

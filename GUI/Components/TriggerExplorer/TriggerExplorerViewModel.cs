using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using GUI.Commands;
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

        private RelayCommand<ExplorerElement> _renameBox_KeyDown;

        public ICommand RenameBox_KeyDown
        {
            get
            {
                if(_renameBox_KeyDown == null)
                {
                    _renameBox_KeyDown = new RelayCommand<ExplorerElement>(e =>
                    {
                        e.Rename();
                    });
                }
                return _renameBox_KeyDown;
            }
        }
    }
}

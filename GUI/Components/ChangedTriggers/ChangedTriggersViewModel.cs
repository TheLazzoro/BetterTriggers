using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.ChangedTriggers
{
    public class ChangedTriggersViewModel
    {
        public ObservableCollection<ExplorerElement> ChangedElements { get; set; } = new();

    }
}

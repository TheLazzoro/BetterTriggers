using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.ImportTriggers
{
    public class ImportTriggersViewModel
    {
        public ObservableCollection<ImportTriggerItem> ExplorerElements { get; set; } = new();
    }
}

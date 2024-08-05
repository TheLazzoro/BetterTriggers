using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.BuildMap
{
    public class BT2WEError
    {
        public string Name { get; set; }
        public string ErrorDescription { get; set; }
        public ExplorerElement ExplorerElement { get; set; }

        public BT2WEError(string Name, string ErrorDescription, ExplorerElement ExplorerElement)
        {
            this.Name = Name;
            this.ErrorDescription = ErrorDescription;
            this.ExplorerElement = ExplorerElement;
        }
    }

    public class ErrorBTOnlyDataWindowViewModel
    {
        public ObservableCollection<BT2WEError> Errors { get; set; } = new();
    }
}

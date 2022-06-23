using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components
{
    public class TabViewModel
    {
        public ObservableCollection<TabItemBT> Tabs { get; set; }

        public TabViewModel()
        {
            Tabs = new ObservableCollection<TabItemBT>();
        }
    }
}

using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.Tabs
{
    public class TabViewModel
    {
        public ObservableCollection<TabItemBT> Tabs { get; set; }

        public TabViewModel()
        {
            Tabs = new ObservableCollection<TabItemBT>();
        }

        public bool Contains(ExplorerElement explorerElement)
        {
            return Tabs.Where(el => el.explorerElement == explorerElement).ToList().Count > 0;
        }

        public int IndexOf(ExplorerElement explorerElement)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                if (Tabs[i].explorerElement == explorerElement)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}

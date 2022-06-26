using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for ChangedTriggersWindow.xaml
    /// </summary>
    public partial class ChangedTriggersWindow : Window
    {
        public ChangedTriggersWindow(List<ExplorerElementTrigger> explorerElements)
        {
            InitializeComponent();

            for (int i = 0; i < explorerElements.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Content = explorerElements[i].GetName();
                listView.Items.Add(item);
            }
        }
    }
}

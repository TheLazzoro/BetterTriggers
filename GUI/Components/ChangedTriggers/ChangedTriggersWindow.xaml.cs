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

namespace GUI.Components.ChangedTriggers
{
    /// <summary>
    /// Interaction logic for ChangedTriggersWindow.xaml
    /// </summary>
    public partial class ChangedTriggersWindow : Window
    {
        public ChangedTriggersWindow(List<IExplorerElement> explorerElements)
        {
            InitializeComponent();

            for (int i = 0; i < explorerElements.Count; i++)
            {
                var element = explorerElements[i];

                ListViewItem item = new ListViewItem();
                item.Content = element.GetName();
                listView.Items.Add(item);

                item.Selected += Item_Selected;
            }
        }

        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            var item = e.Source as ListViewItem;
            string name = item.Content as string;
            var triggerExplorer = TriggerExplorer.Current;

            triggerExplorer.Search(name);
        }
    }
}

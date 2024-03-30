using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Xml.Linq;

namespace GUI.Components.ChangedTriggers
{
    /// <summary>
    /// Interaction logic for ChangedTriggersWindow.xaml
    /// </summary>
    public partial class ChangedTriggersWindow : Window
    {
        public ChangedTriggersWindow(List<ExplorerElement> explorerElements)
        {
            InitializeComponent();

            var viewModel = new ChangedTriggersViewModel();
            DataContext = viewModel;

            for (int i = 0; i < explorerElements.Count; i++)
            {
                var element = explorerElements[i];
                viewModel.ChangedElements.Add(element);
            }

        }

        private void ListViewItem_MouseDown(object sender, RoutedEventArgs e)
        {
            var item = e.Source as ListViewItem;
            if (item == null)
                return;

            ExplorerElement element = item.DataContext as ExplorerElement;
            var triggerExplorer = TriggerExplorer.Current;
            triggerExplorer.NavigateToExplorerElement(element);
        }
    }
}

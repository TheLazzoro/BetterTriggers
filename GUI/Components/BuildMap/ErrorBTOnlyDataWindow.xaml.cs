using BetterTriggers.Models.EditorData;
using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.BuildMap
{
    public partial class ErrorBTOnlyDataWindow : Window
    {
        private ErrorBTOnlyDataWindowViewModel _viewModel;

        public ErrorBTOnlyDataWindow(List<Tuple<ExplorerElement, string>> explorerElementErrors)
        {
            InitializeComponent();

            Owner = MainWindow.GetMainWindow();

            _viewModel = new ErrorBTOnlyDataWindowViewModel();
            DataContext = _viewModel;


            for (int i = 0; i < explorerElementErrors.Count; i++)
            {
                var e = explorerElementErrors[i];
                var explorerElement = e.Item1;
                var errorDescription = e.Item2;
                var error = new BT2WEError(explorerElement.GetName(), errorDescription, explorerElement);
                _viewModel.Errors.Add(error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ListViewItem_MouseDown(object sender, RoutedEventArgs e)
        {
            var item = e.Source as ListViewItem;
            if (item == null)
                return;

            var error = item.DataContext as BT2WEError;
            var element = error.ExplorerElement;
            var triggerExplorer = TriggerExplorer.Current;
            triggerExplorer.NavigateToExplorerElement(element);
        }
    }
}

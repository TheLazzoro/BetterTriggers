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
using War3Net.Build.Script;
using War3Net.Build;
using GUI.Components.ImportTriggers;
using Xceed.Wpf.AvalonDock.Controls;
using GUI.Components.OpenMap;
using System.Threading;
using System.Windows.Threading;
using BetterTriggers.WorldEdit;

namespace GUI
{
    public partial class ImportTriggersWindow : Window
    {
        private ImportTriggerItem root;
        private string mapPath;

        public ImportTriggersWindow()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenWar3MapWindowHotfix window = new();
            window.ShowDialog();
            if (!string.IsNullOrEmpty(window.SelectedPath))
            {
                btnImport.IsEnabled = true;
                mapPath = window.SelectedPath;
                Thread newWindowThread = new Thread(LoadTriggersThread);
                newWindowThread.SetApartmentState(ApartmentState.STA);
                newWindowThread.IsBackground = true;
                newWindowThread.Start();
            }
        }

        private void LoadTriggersThread()
        {
            this.Dispatcher.BeginInvoke(LoadTriggers, DispatcherPriority.Background);
        }

        private void LoadTriggers()
        {
            treeView.Items.Clear();
            var map = Map.Open(mapPath);
            var triggerItems = map.Triggers.TriggerItems;

            Dictionary<int, ImportTriggerItem> items = new Dictionary<int, ImportTriggerItem>();

            // First create UI items and filter those we don't need.
            for (int i = 0; i < triggerItems.Count; i++)
            {
                var triggerItem = triggerItems[i];
                var treeItem = new ImportTriggerItem(triggerItem);
                if (triggerItem.Type == TriggerItemType.RootCategory)
                {
                    root = treeItem;
                }
                if (!treeItem.IsValid)
                {
                    continue;
                }

                items.TryAdd(triggerItem.Id, treeItem);
            }

            // Then we attach them to their corresponding parent
            foreach (var item in items)
            {
                var treeItem = item.Value;
                if (treeItem.triggerItem.Type == TriggerItemType.RootCategory)
                {
                    treeView.Items.Add(treeItem);
                }
                else
                {
                    ImportTriggerItem parent;
                    items.TryGetValue(treeItem.triggerItem.ParentId, out parent);
                    if (parent == null)
                        root.Items.Add(treeItem);
                    else
                        parent.Items.Add(treeItem);
                }
            }

            root.ExpandSubtree();
        }

        List<TriggerItem> triggerItems;
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                triggerItems = new List<TriggerItem>();
                GatherAllCheckedTriggerItems(root);

                TriggerConverter triggerConverter = new TriggerConverter();
                triggerConverter.ImportIntoCurrentProject(mapPath, triggerItems);
            }
            catch (Exception ex)
            {
                Components.Dialogs.MessageBox messageBox = new Components.Dialogs.MessageBox("Error", ex.Message);
                messageBox.ShowDialog();
            }
        }

        private void GatherAllCheckedTriggerItems(ImportTriggerItem parent)
        {
            if (parent.treeItemHeader.checkbox.IsChecked == true)
            {
                if (parent.triggerItem.Type != TriggerItemType.RootCategory)
                    triggerItems.Add(parent.triggerItem);
            }
            if (parent.Items.Count > 0)
            {
                foreach (ImportTriggerItem item in parent.Items)
                {
                    GatherAllCheckedTriggerItems(item);
                }
            }
        }
    }
}

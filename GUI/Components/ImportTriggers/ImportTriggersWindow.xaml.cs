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

namespace GUI
{
    public partial class ImportTriggersWindow : Window
    {
        private ImportTriggerItem root;

        public ImportTriggersWindow()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenWar3MapWindow window = new();
            window.ShowDialog();
            if (!string.IsNullOrEmpty(window.SelectedPath))
            {
                LoadTriggers(window.SelectedPath);
            }
        }

        private void LoadTriggers(string mapPath)
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
    }
}

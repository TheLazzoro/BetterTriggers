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
using System.ComponentModel;
using GUI.Components;

namespace GUI
{
    public partial class ImportTriggersWindow : Window
    {
        private ImportTriggerItem root;
        private string mapPath;
        private List<TriggerItem> triggerItems;
        private bool hasError;
        private string errorMsg;
        private BackgroundWorker worker;
        private List<string> itemsImported;

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
            txtTotalTriggerItems.Text = "Total items: " + triggerItems.Count;

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

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            btnImport.IsEnabled = false;
            triggerItems = new List<TriggerItem>();
            itemsImported = new List<string>();
            GatherAllCheckedTriggerItems(root);

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                TriggerConverter triggerConverter = new TriggerConverter();
                triggerConverter.OnExplorerElementImported += TriggerConverter_OnExplorerElementImported;
                triggerConverter.ImportIntoCurrentProject(mapPath, triggerItems);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                worker.ReportProgress(-1);
            }
        }

        private void TriggerConverter_OnExplorerElementImported(string fullPath)
        {
            itemsImported.Add(fullPath);
            float percent = (float)itemsImported.Count / triggerItems.Count * 100;
            worker.ReportProgress((int)percent);
        }


        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
            {
                Components.Dialogs.MessageBox messageBox = new Components.Dialogs.MessageBox("Error", errorMsg);
                messageBox.ShowDialog();
            }
            else if (e.ProgressPercentage == 100)
            {
                foreach (string fullPath in itemsImported)
                {
                    TriggerExplorer.Current.OnCreateElement(fullPath);
                }
            }
            else
            {
                txtProgressPercent.Text = $"{e.ProgressPercentage}%";
                progressBar.Value = e.ProgressPercentage;
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                Components.Dialogs.MessageBox messageBox = new Components.Dialogs.MessageBox("Error", e.Error.Message);
                messageBox.ShowDialog();
            }
            
            this.Close();
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

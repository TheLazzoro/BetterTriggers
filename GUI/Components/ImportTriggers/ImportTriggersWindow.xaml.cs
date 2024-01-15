using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.WorldEdit;
using GUI.Components;
using GUI.Components.ImportTriggers;
using GUI.Components.OpenMap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using War3Net.Build;

namespace GUI
{
    public partial class ImportTriggersWindow : Window
    {
        private ImportTriggerItem rootTreeItem;
        private string mapPath;
        private bool hasError;
        private string errorMsg;
        private BackgroundWorker worker;
        private List<string> itemsImported;
        Dictionary<string, ImportTriggerItem> treeItemExplorerElements;
        private List<IExplorerElement> elementsToImport;

        private UserControl control;

        public ImportTriggersWindow()
        {
            this.Owner = MainWindow.GetMainWindow();
            InitializeComponent();
            EditorSettings settings = EditorSettings.Load();
            this.Width = settings.triggerWindowWidth;
            this.Height = settings.triggerWindowHeight;
            this.Left = settings.triggerWindowX;
            this.Top = settings.triggerWindowY;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenWar3MapWindowHotfix window = new();
            window.ShowDialog();
            if (!string.IsNullOrEmpty(window.SelectedPath))
            {
                btnImport.IsEnabled = true;
                mapPath = window.SelectedPath;
                Thread thread = new Thread(LoadTriggersThread);
                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        private void LoadTriggersThread()
        {
            this.Dispatcher.BeginInvoke(LoadTriggers, DispatcherPriority.Background);
        }

        private void LoadTriggers()
        {
            try
            {

                treeView.Items.Clear();
                var map = Map.Open(mapPath);
                var triggerItems = map.Triggers.TriggerItems;
                var triggerConverter = new TriggerConverter(mapPath);
                var explorerElements = triggerConverter.ConvertAll_NoWrite();
                txtTotalTriggerItems.Text = "Total items: " + triggerItems.Count;

                this.treeItemExplorerElements = new Dictionary<string, ImportTriggerItem>();

                // First create UI items and filter those we don't need.
                var explorerRoot = new ExplorerElementFolder
                {
                    path = mapPath
                };
                rootTreeItem = new ImportTriggerItem(explorerRoot);
                this.treeItemExplorerElements.TryAdd(explorerRoot.GetPath(), rootTreeItem);
                rootTreeItem.Selected += ExplorerItem_Selected;
                for (int i = 0; i < explorerElements.Count; i++)
                {
                    var element = explorerElements[i];
                    var treeItem = new ImportTriggerItem(element);

                    this.treeItemExplorerElements.TryAdd(element.GetPath(), treeItem);
                    treeItem.Selected += ExplorerItem_Selected;
                }

                // Then we attach them to their corresponding parent
                foreach (var item in this.treeItemExplorerElements)
                {
                    var treeItem = item.Value;
                    if (treeItem == rootTreeItem)
                    {
                        treeView.Items.Add(treeItem);
                    }
                    else
                    {
                        ImportTriggerItem parent;
                        string parentPath = System.IO.Path.GetDirectoryName(treeItem.explorerElement.GetPath());
                        this.treeItemExplorerElements.TryGetValue(parentPath, out parent);
                        if (parent == null)
                            rootTreeItem.Items.Add(treeItem);
                        else
                            parent.Items.Add(treeItem);
                    }
                }

                rootTreeItem.ExpandSubtree();
            }
            catch (Exception ex)
            {
                Components.Dialogs.MessageBox messageBox = new Components.Dialogs.MessageBox("Error", ex.Message);
                messageBox.ShowDialog();
            }
        }

        private void ExplorerItem_Selected(object sender, RoutedEventArgs e)
        {
            txtTriggerNote.Visibility = Visibility.Hidden;
            var selected = (ImportTriggerItem)treeView.SelectedItem;
            var explorerElement = selected.explorerElement;
            if (explorerElement != null)
            {
                if (control != null)
                {
                    grid.Children.Remove(control);
                }
                if (explorerElement is ExplorerElementTrigger explorerTrigger)
                {
                    control = new TriggerControl(explorerTrigger);
                    var triggerControl = (TriggerControl)control;
                    triggerControl.checkBoxIsCustomScript.IsEnabled = false;
                    triggerControl.checkBoxIsEnabled.IsEnabled = false;
                    triggerControl.checkBoxIsInitiallyOn.IsEnabled = false;
                    triggerControl.checkBoxList.IsEnabled = false;
                    triggerControl.checkBoxRunOnMapInit.IsEnabled = false;
                    triggerControl.textBoxComment.IsReadOnly = true;
                    triggerControl.categoryAction.IsEnabled = false;
                    triggerControl.categoryCondition.IsEnabled = false;
                    triggerControl.categoryEvent.IsEnabled = false;
                    triggerControl.categoryLocalVariable.IsEnabled = false;
                    triggerControl.bottomControl.IsEnabled = false;

                    grid.Children.Add(control);
                    Grid.SetColumn(control, 2);
                    Grid.SetColumnSpan(control, 2);
                    Grid.SetRow(control, 3);
                    txtTriggerNote.Visibility = Visibility.Visible;
                }
                else if (explorerElement is ExplorerElementScript explorerScript)
                {
                    control = new ScriptControl(explorerScript);
                    var scriptControl = (ScriptControl)control;
                    scriptControl.textEditor.avalonEditor.IsReadOnly = true;
                    scriptControl.checkBoxIsEnabled.IsEnabled = false;

                    grid.Children.Add(control);
                    Grid.SetColumn(control, 2);
                    Grid.SetColumnSpan(control, 2);
                    Grid.SetRow(control, 2);
                    Grid.SetRowSpan(control, 2);
                }
                else if (explorerElement is ExplorerElementVariable explorerVariable)
                {
                    control = new VariableControl(explorerVariable.variable);
                    control.IsEnabled = false;

                    grid.Children.Add(control);
                    Grid.SetColumn(control, 2);
                    Grid.SetColumnSpan(control, 2);
                    Grid.SetRow(control, 2);
                    Grid.SetRowSpan(control, 2);
                }
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            btnImport.IsEnabled = false;
            elementsToImport = new List<IExplorerElement>();
            itemsImported = new List<string>();
            elementsToImport = treeItemExplorerElements
                .Where(item => (bool)item.Value.treeItemHeader.checkbox.IsChecked)
                .Where(item => item.Value != rootTreeItem)
                .Select(item => item.Value.explorerElement)
                .ToList();

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //try
            //{
                TriggerConverter triggerConverter = new TriggerConverter(mapPath, Project.CurrentProject.GetFullMapPath());
                triggerConverter.OnExplorerElementImported += TriggerConverter_OnExplorerElementImported;
                triggerConverter.WriteConvertedTriggers(elementsToImport);
                worker.ReportProgress(100);
                triggerConverter.OnExplorerElementImported -= TriggerConverter_OnExplorerElementImported;
            //}
            //catch (Exception ex)
            //{
            //    errorMsg = ex.Message;
            //    worker.ReportProgress(-1);
            //    TriggerConverter.OnExplorerElementImported -= TriggerConverter_OnExplorerElementImported;
            //}
        }

        private void TriggerConverter_OnExplorerElementImported(string fullPath)
        {
            itemsImported.Add(fullPath);
            float percent = (float)itemsImported.Count / elementsToImport.Count * 100; 
            worker.ReportProgress((int)percent);
        }

        bool didComplete = false;
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
            {
                Components.Dialogs.MessageBox messageBox = new Components.Dialogs.MessageBox("Error", errorMsg);
                messageBox.ShowDialog();
            }
            else if (e.ProgressPercentage == 100 && !didComplete)
            {
                didComplete = true; // hack. In some cases it reports 100% twice. Don't question it :)
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
            if (e.Error != null)
            {
                Components.Dialogs.MessageBox messageBox = new Components.Dialogs.MessageBox("Error", e.Error.Message);
                messageBox.ShowDialog();
            }

            this.Close();
        }

    }
}

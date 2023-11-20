using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BetterTriggers.Containers;
using GUI.Components.Shared;
using GUI.Utility;
using BetterTriggers.Models.EditorData;
using System.ComponentModel;
using System.Threading;
using BetterTriggers.Utility;
using GUI.Components.Dialogs;
using GUI.Components.Tabs;

namespace GUI.Components
{
    public partial class TriggerExplorer : UserControl, IDisposable
    {
        internal static TriggerExplorer Current;

        public TreeItemExplorerElement map;
        public TreeItemExplorerElement currentElement;
        public event Action<TreeItemExplorerElement> OnOpenExplorerElement;

        internal Dictionary<string, TreeItemExplorerElement> AllExplorerElements;

        // Drag and drop fields
        Point _startPoint;
        TreeItemExplorerElement dragItem;
        bool _IsDragging = false;
        TreeViewItem parentDropTarget;
        int insertIndex = 0; // used when a file is moved from one location to the other.
                             // We can use it when the user wants to drop a file at a specific index.

        // Visual indicators for TreeViewItem
        AdornerLayer adorner;
        TreeItemAdornerLine lineIndicator;
        TreeItemAdornerSquare squareIndicator;

        BackgroundWorker searchWorker;

        public TriggerExplorer()
        {
            InitializeComponent();

            AllExplorerElements = new ();
            Project.CurrentProject.OnCreated += ContainerProject_OnElementCreated;
            searchWorker = new BackgroundWorker();
            searchWorker.WorkerReportsProgress = true;
            searchWorker.WorkerSupportsCancellation = true;
            searchWorker.DoWork += SearchWorker_DoWork;
            searchWorker.ProgressChanged += SearchWorker_ProgressChanged;
            searchWorker.RunWorkerCompleted += SearchWorker_RunWorkerCompleted;
        }


        public void Dispose()
        {
            Project.CurrentProject.OnCreated -= ContainerProject_OnElementCreated;
        }

        public void Populate()
        {
            var root = Project.CurrentProject.GetRoot();
            AllExplorerElements.Add(root.GetPath(), map);
            for (int i = 0; i < root.explorerElements.Count; i++)
            {
                RecursePopulate(map, root.explorerElements[i]);
            }
        }

        private void RecursePopulate(TreeItemExplorerElement parent, IExplorerElement element)
        {
            var treeItem = new TreeItemExplorerElement(element);
            element.Attach(treeItem); // attach treeItem to element so it can respond to events happening to the element.
            parent.Items.Add(treeItem);
            AllExplorerElements.Add(element.GetPath(), treeItem);

            if (element is ExplorerElementFolder)
            {
                var folder = element as ExplorerElementFolder;
                if (folder.isExpanded)
                    treeItem.ExpandSubtree();

                for (int i = 0; i < folder.explorerElements.Count; i++)
                {
                    var child = folder.explorerElements[i];
                    RecursePopulate(treeItem, child);
                }
            }
        }

        public void OnSelectTab(TreeItemExplorerElement selectedItem, TabViewModel tabViewModel, TabControl tabControl)
        {
            if (selectedItem.Ielement is ExplorerElementTrigger exTrig)
                Project.CurrentProject.Triggers.SelectedTrigger = exTrig.trigger;

            if (selectedItem.editor == null || selectedItem.tabItem == null)
            {
                if (selectedItem.Ielement is ExplorerElementRoot)
                {
                    var rootControl = new RootControl((ExplorerElementRoot)selectedItem.Ielement);
                    rootControl.Attach(selectedItem);
                    selectedItem.editor = rootControl;
                }
                else if (selectedItem.Ielement is ExplorerElementTrigger)
                {
                    var triggerControl = new TriggerControl((ExplorerElementTrigger)selectedItem.Ielement);
                    triggerControl.Attach(selectedItem);
                    selectedItem.editor = triggerControl;
                }
                else if (selectedItem.Ielement is ExplorerElementScript)
                {
                    var scriptControl = new ScriptControl((ExplorerElementScript)selectedItem.Ielement);
                    scriptControl.Attach(selectedItem);
                    selectedItem.editor = scriptControl;
                }
                else if (selectedItem.Ielement is ExplorerElementVariable)
                {
                    var element = (ExplorerElementVariable)selectedItem.Ielement;
                    var variableControl = new VariableControl(element.variable);
                    variableControl.Attach(selectedItem);
                    selectedItem.editor = variableControl;
                }

                // select already open tab
                for (int i = 0; i < tabViewModel.Tabs.Count; i++)
                {
                    var tab = tabViewModel.Tabs[i];
                    if (tab.explorerElement.Ielement.GetPath() == selectedItem.Ielement.GetPath())
                    {
                        selectedItem.tabItem = tab;
                        tabViewModel.Tabs.IndexOf(selectedItem.tabItem);
                        return;
                    }
                }

                if (selectedItem.editor == null)
                    return;

                TabItemBT tabItem = new TabItemBT(selectedItem, tabViewModel);
                tabViewModel.Tabs.Add(tabItem);
                selectedItem.tabItem = tabItem;
            }

            if (selectedItem.tabItem != null)
            {
                if (!tabViewModel.Tabs.Contains(selectedItem.tabItem))
                    tabViewModel.Tabs.Add(selectedItem.tabItem);

                tabControl.SelectedIndex = tabViewModel.Tabs.IndexOf(selectedItem.tabItem);
            }
        }


        public void OnCreateElement(string fullPath)
        {
            if (!FileSystemUtil.IsExtensionValid(System.IO.Path.GetExtension(fullPath)))
                return;

            var project = Project.CurrentProject;
            var explorerElement = project.GetExplorerElement(fullPath);
            int insertIndex = explorerElement.GetParent().GetExplorerElements().IndexOf(explorerElement);

            TreeItemExplorerElement treeItemExplorerElement = new TreeItemExplorerElement(explorerElement);
            explorerElement.Attach(treeItemExplorerElement);
            treeItemExplorerElement.OnCreated(insertIndex);
        }

        internal void OnMoveElement(TriggerExplorer te, string oldFullPath, string newFullPath, int insertIndex)
        {
            TreeItemExplorerElement elementToMove = GetTreeNodeElement(oldFullPath);
            TreeItemExplorerElement oldParent = elementToMove.Parent as TreeItemExplorerElement;
            TreeItemExplorerElement newParent = GetTreeNodeElement(System.IO.Path.GetDirectoryName(newFullPath));

            Application.Current.Dispatcher.Invoke(delegate
            {
                if (newParent == null) // hack. idk why it fires twice
                    return;

                oldParent.Items.Remove(elementToMove);
                newParent.Items.Insert(insertIndex, elementToMove);
                AllExplorerElements.Remove(oldFullPath);
                AllExplorerElements.Add(newFullPath, elementToMove);
                elementToMove.IsSelected = true;
            });
        }

        internal TreeItemExplorerElement GetTreeNodeElement(string fullPath)
        {
            TreeItemExplorerElement element;
            AllExplorerElements.TryGetValue(fullPath, out element);
            return element;
        }

        internal TreeItemExplorerElement GetTreeNodeDirectory(string fullPath)
        {
            TreeItemExplorerElement element;
            AllExplorerElements.TryGetValue(fullPath, out element);
            if (element == null)
                return map;

            return element;
        }

        internal TreeItemExplorerElement FindTreeNodeElement(TreeItemExplorerElement parent, string path)
        {
            TreeItemExplorerElement node = null;

            for (int i = 0; i < parent.Items.Count; i++)
            {
                TreeItemExplorerElement element = parent.Items[i] as TreeItemExplorerElement;
                if (element.Ielement.GetPath() == path)
                {
                    node = element;
                    break;
                }
                if (Directory.Exists(element.Ielement.GetPath()) && node == null)
                {
                    node = FindTreeNodeElement(element, path);
                }
            }

            return node;
        }

        internal TreeItemExplorerElement FindTreeNodeDirectory(string directory)
        {
            var triggerExplorer = TriggerExplorer.Current;
            if (directory == triggerExplorer.map.Ielement.GetPath())
                return triggerExplorer.map;

            return FindTreeNodeDirectory(triggerExplorer.map, directory);
        }

        private TreeItemExplorerElement FindTreeNodeDirectory(TreeItemExplorerElement parent, string directory)
        {
            TreeItemExplorerElement node = null;

            for (int i = 0; i < parent.Items.Count; i++)
            {
                TreeItemExplorerElement element = parent.Items[i] as TreeItemExplorerElement;
                if (Directory.Exists(element.Ielement.GetPath()) && node == null)
                {
                    if (element.Ielement.GetPath() == directory)
                    {
                        node = element;
                        break;
                    }
                    else
                    {
                        node = FindTreeNodeDirectory(element, directory);
                    }
                }
            }

            return node;
        }


        // This function is invoked by a method in the container when a new file is created.
        internal void ContainerProject_OnElementCreated(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                OnCreateElement(Project.CurrentProject.createdPath); // hack
            });
        }

        /// <summary>
        /// // It is necessary to traverse the item's parents since drag & drop picks up
        /// things like 'TextBlock' and 'Border' on the drop target when dropping the 
        /// dragged element.
        /// </summary>
        /// <returns></returns>
        private TreeItemExplorerElement GetTraversedTargetDropItem(FrameworkElement dropTarget)
        {
            if (dropTarget == null || dropTarget is TreeView)
                return null;

            TreeItemExplorerElement traversedTarget = null;
            while (traversedTarget == null && dropTarget != null)
            {
                dropTarget = dropTarget.Parent as FrameworkElement;
                if (dropTarget is TreeViewItem)
                {
                    traversedTarget = (TreeItemExplorerElement)dropTarget;
                }
            }

            return traversedTarget;
        }

        private void treeViewItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !_IsDragging && !contextMenu.IsVisible)
            {
                Point position = e.GetPosition(null);
                if (Math.Abs(position.X - _startPoint.X) >
                        SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) >
                        SystemParameters.MinimumVerticalDragDistance)
                {
                    StartDrag(e);
                }
            }
        }


        private void StartDrag(MouseEventArgs e)
        {
            _IsDragging = true;
            dragItem = this.treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement;

            if (dragItem == null || dragItem.treeItemHeader.isRenaming)
            {
                _IsDragging = false;
                return;
            }

            DataObject data = new DataObject("inadt", dragItem);
            if (data != null)
            {
                DragDropEffects dde = DragDropEffects.Move;
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    dde = DragDropEffects.All;
                }
                DragDropEffects de = DragDrop.DoDragDrop(this.treeViewTriggerExplorer, data, dde);
            }
            _IsDragging = false;
        }

        public void CreateRootItem()
        {
            this.map = new TreeItemExplorerElement(Project.CurrentProject.projectFiles[0]);
            treeViewTriggerExplorer.Items.Add(this.map);
            this.map.IsExpanded = true;
            this.map.IsSelected = true;
        }

        private void treeViewItem_DragOver(object sender, DragEventArgs e)
        {
            if (dragItem == null)
                return;

            if (!dragItem.IsKeyboardFocused)
                return;

            TreeViewItem currentParent = dragItem.Parent as TreeViewItem;
            if (currentParent == null)
                return;

            TreeItemExplorerElement dropTarget = GetTraversedTargetDropItem(e.Source as FrameworkElement);
            int currentIndex = currentParent.Items.IndexOf(dragItem);
            if (dropTarget == null)
                return;

            dropTarget.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            if (lineIndicator != null)
                adorner.Remove(lineIndicator);
            if (squareIndicator != null)
                adorner.Remove(squareIndicator);

            if (dropTarget == dragItem)
            {
                parentDropTarget = null;
                return;
            }

            if (UIUtility.IsCircularParent(dragItem, dropTarget))
                return;

            if (dropTarget is TreeItemExplorerElement && !(dropTarget.Ielement is ExplorerElementRoot))
            {
                var relativePos = e.GetPosition(dropTarget);
                TreeItemLocation location = UIUtility.TreeItemGetMouseLocation(dropTarget, relativePos);

                if (dropTarget.Ielement is ExplorerElementFolder)
                    DragOverLogic(dropTarget, location, currentIndex);
                else
                {
                    bool InFirstHalf = UIUtility.IsMouseInFirstHalf(dropTarget, relativePos, Orientation.Vertical);
                    if (InFirstHalf)
                        DragOverLogic(dropTarget, TreeItemLocation.Top, currentIndex);
                    else
                        DragOverLogic(dropTarget, TreeItemLocation.Bottom, currentIndex);
                }

            }
            else
                parentDropTarget = null;
        }

        private void DragOverLogic(TreeItemExplorerElement dropTarget, TreeItemLocation location, int currentIndex)
        {
            if (location == TreeItemLocation.Top)
            {
                adorner = AdornerLayer.GetAdornerLayer(dropTarget);
                lineIndicator = new TreeItemAdornerLine(dropTarget, true);
                adorner.Add(lineIndicator);

                parentDropTarget = (TreeViewItem)dropTarget.Parent;
                insertIndex = parentDropTarget.Items.IndexOf(dropTarget);

                // We detach the item before inserting, so the index goes one down.
                if (dropTarget.Parent == dragItem.Parent && insertIndex > currentIndex)
                    insertIndex--;
            }
            else if (location == TreeItemLocation.Middle && dropTarget.Ielement is ExplorerElementFolder)
            {
                adorner = AdornerLayer.GetAdornerLayer(dropTarget);
                dropTarget.Background = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
                //squareIndicator = new TreeItemAdornerSquare(dropTarget);
                //adorner.Add(squareIndicator);

                parentDropTarget = dropTarget;
                insertIndex = 0;
            }
            else if (location == TreeItemLocation.Bottom)
            {
                adorner = AdornerLayer.GetAdornerLayer(dropTarget);
                lineIndicator = new TreeItemAdornerLine(dropTarget, false);
                adorner.Add(lineIndicator);

                parentDropTarget = (TreeViewItem)dropTarget.Parent;
                insertIndex = parentDropTarget.Items.IndexOf(dropTarget) + 1;

                // We detach the item before inserting, so the index goes one down.
                if (dropTarget.Parent == dragItem.Parent && insertIndex > currentIndex)
                    insertIndex--;
            }
        }

        private void treeViewTriggerExplorer_Drop(object sender, DragEventArgs e)
        {
            if (!_IsDragging || dragItem == null)
                return;

            if (adorner != null)
            {
                if (lineIndicator != null)
                    adorner.Remove(lineIndicator);
                if (squareIndicator != null)
                    adorner.Remove(squareIndicator);
            }

            if (parentDropTarget == null)
                return;
            if (parentDropTarget == dragItem) // cannot drag into self
                return;
            if (!dragItem.IsKeyboardFocused)
                return;

            parentDropTarget.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            var dragItemParent = (TreeItemExplorerElement)dragItem.Parent;
            if (dragItemParent == parentDropTarget)
            {
                Project.CurrentProject.RearrangeElement(dragItem.Ielement, insertIndex);
                return;
            }

            var dropTarget = (TreeItemExplorerElement)parentDropTarget;
            try
            {
                FileSystemUtil.Move(dragItem.Ielement.GetPath(), dropTarget.Ielement.GetPath(), this.insertIndex);
            }
            catch (Exception ex)
            {
                Dialogs.MessageBox dialogBox = new Dialogs.MessageBox("Error", ex.Message);
                dialogBox.ShowDialog();
            }

            // focus select item again
            dragItem.IsSelected = true;
        }

        private void treeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            var treeItem = (TreeItemExplorerElement)e.Source;
            var explorerElement = treeItem.Ielement as ExplorerElementFolder;
            if (explorerElement == null)
                return;

            explorerElement.isExpanded = true;
        }

        private void treeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            var treeItem = (TreeItemExplorerElement)e.Source;
            var explorerElement = treeItem.Ielement as ExplorerElementFolder;
            if (explorerElement == null)
                return;

            explorerElement.isExpanded = false;
        }

        private void treeViewTriggerExplorer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var selected = treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement;
                if (selected != null)
                    OnOpenExplorerElement?.Invoke(treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement);
            }

            else if (e.Key == Key.Delete)
            {
                TreeItemExplorerElement selectedElement = treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement;
                if (selectedElement == null || selectedElement == map)
                    return;

                List<ExplorerElementTrigger> refs = selectedElement.Ielement.GetReferrers();
                if (refs.Count > 0)
                {
                    DialogBoxReferences dialogBox = new DialogBoxReferences(refs, ExplorerAction.Delete);
                    dialogBox.ShowDialog();
                    if (!dialogBox.OK)
                        return;
                }

                FileSystemUtil.Delete(selectedElement.Ielement.GetPath());
            }
            else if (e.Key == Key.C && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                TreeItemExplorerElement selectedElement = treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement;
                Project.CurrentProject.CopyExplorerElement(selectedElement.Ielement);
            }
            else if (e.Key == Key.X && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                TreeItemExplorerElement selectedElement = treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement;
                Project.CurrentProject.CopyExplorerElement(selectedElement.Ielement, true);
            }
            else if (e.Key == Key.V && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                TreeItemExplorerElement selectedElement = treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement;
                IExplorerElement pasted = Project.CurrentProject.PasteExplorerElement(selectedElement.Ielement);

                var parent = GetTreeNodeDirectory(pasted.GetParent().GetPath());
                RecursePopulate(parent, pasted);
            }
            else if (e.Key == Key.F && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                //OpenSearchField();
            }
        }

        private void treeViewTriggerExplorer_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeItemExplorerElement rightClickedElement = GetTraversedTargetDropItem(e.Source as FrameworkElement);

            if (rightClickedElement == null)
                return;

            currentElement = rightClickedElement;
            rightClickedElement.IsSelected = true;
            rightClickedElement.ContextMenu = contextMenu;

            menuPaste.IsEnabled = CopiedElements.CopiedExplorerElement != null;
            menuElementEnabled.IsChecked = rightClickedElement.Ielement.GetEnabled();
            menuElementInitiallyOn.IsChecked = rightClickedElement.Ielement.GetInitiallyOn();
            menuElementEnabled.IsEnabled = rightClickedElement.Ielement is ExplorerElementTrigger || rightClickedElement.Ielement is ExplorerElementScript;
            menuElementInitiallyOn.IsEnabled = rightClickedElement.Ielement is ExplorerElementTrigger;
            menuRename.IsEnabled = rightClickedElement.Ielement is not ExplorerElementRoot;
            menuDelete.IsEnabled = rightClickedElement.Ielement is not ExplorerElementRoot;
            menuCut.IsEnabled = rightClickedElement.Ielement is not ExplorerElementRoot;
            menuCopy.IsEnabled = rightClickedElement.Ielement is not ExplorerElementRoot;
        }

        private void menuCut_Click(object sender, RoutedEventArgs e)
        {
            Project project = Project.CurrentProject;
            project.CopyExplorerElement(currentElement.Ielement, true);
        }

        private void menuCopy_Click(object sender, RoutedEventArgs e)
        {
            Project project = Project.CurrentProject;
            project.CopyExplorerElement(currentElement.Ielement);
        }

        private void menuPaste_Click(object sender, RoutedEventArgs e)
        {
            Project project = Project.CurrentProject;
            IExplorerElement pasted = project.PasteExplorerElement(currentElement.Ielement);

            var parent = GetTreeNodeDirectory(pasted.GetParent().GetPath());
            RecursePopulate(parent, pasted);
        }

        private void menuRename_Click(object sender, RoutedEventArgs e)
        {
            currentElement.ShowRenameBox();
        }

        private void menuDelete_Click(object sender, RoutedEventArgs e)
        {
            FileSystemUtil.Delete(currentElement.Ielement.GetPath());
        }

        private void menuNewCategory_Click(object sender, RoutedEventArgs e)
        {
            Project.CurrentProject.Folders.Create();
        }

        private void menuNewTrigger_Click(object sender, RoutedEventArgs e)
        {
            Project.CurrentProject.Triggers.Create();
        }

        private void menuNewScript_Click(object sender, RoutedEventArgs e)
        {
            Project.CurrentProject.Scripts.Create();
        }

        private void menuNewVariable_Click(object sender, RoutedEventArgs e)
        {
            Project.CurrentProject.Variables.Create();
        }

        private void menuElementEnabled_Click(object sender, RoutedEventArgs e)
        {
            // TODO: This will not update the checkmark in the editor view.
            currentElement.Ielement.SetEnabled(!currentElement.Ielement.GetEnabled());
            if (currentElement.editor != null)
                currentElement.editor.OnStateChange();
        }

        private void menuElementInitiallyOn_Click(object sender, RoutedEventArgs e)
        {
            currentElement.Ielement.SetInitiallyOn(!currentElement.Ielement.GetInitiallyOn());
            if (currentElement.editor != null)
                currentElement.editor.OnStateChange();
        }

        private void menuOpenInExplorer_Click(object sender, RoutedEventArgs e)
        {
            FileSystemUtil.OpenInExplorer(currentElement.Ielement.GetPath());
        }

        private void OpenSearchField()
        {
            searchField.Visibility = searchField.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            if (searchField.Visibility == Visibility.Visible)
                searchTextBox.Focus();
        }

        private void CloseSearchField()
        {
            searchField.Visibility = Visibility.Hidden;
            var treeItem = treeViewTriggerExplorer.SelectedItem as TreeViewItem;
            if (treeItem != null)
                treeItem.Focus();
        }

        private void btnCloseSearchField_Click(object sender, RoutedEventArgs e)
        {
            CloseSearchField();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Search(searchTextBox.Text);
        }

        private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search(searchTextBox.Text);
            else if (e.Key == Key.Escape)
                CloseSearchField();
        }

        /// <summary>
        /// Searches for a trigger element with the specified text, and brings the first matching result into view in the trigger explorer.
        /// </summary>
        internal void Search(string searchText)
        {
            TreeItemBT treeItem = SearchForElement(searchText, treeViewTriggerExplorer.Items[0] as TreeItemBT);
            if (treeItem != null)
            {
                TreeItemBT parent = treeItem.Parent as TreeItemBT;
                while (parent != null)
                {
                    parent.IsExpanded = true;
                    parent = parent.Parent as TreeItemBT;
                }
                treeItem.IsSelected = true;
                treeItem.BringIntoView();
                //treeItem.Focus();
            }
        }

        private TreeItemBT SearchForElement(string searchText, TreeItemBT parent)
        {
            if (parent.GetHeaderText() == searchText)
                return parent;

            TreeItemBT treeItem = null;
            if (parent.Items.Count > 0)
            {
                foreach (var item in parent.Items)
                {
                    treeItem = SearchForElement(searchText, item as TreeItemBT);
                    if (treeItem != null)
                        break;
                }
            }
            return treeItem;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                if (searchMenu.Visibility == Visibility.Hidden)
                {
                    treeViewSearch.Items.Clear();
                    searchMenu.Visibility = Visibility.Visible;
                }

                searchBox.Focus();
            }
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchWorker.IsBusy)
            {
                searchWorker.CancelAsync();
                return;
            }

            DoSearch();
        }

        private void SearchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                DoSearch();
        }

        private void DoSearch()
        {
            treeViewSearch.Items.Clear();
            if (string.IsNullOrEmpty(searchBox.Text))
                return;

            treeItems = GetSubTreeItems(map);
            searchWord = searchBox.Text.ToLower();
            searchWorker.RunWorkerAsync();
        }

        List<TreeItemExplorerElement> treeItems = new();
        string searchWord;
        private void SearchWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 100)
            {
                TreeItemExplorerElement newItem = new TreeItemExplorerElement(e.UserState as IExplorerElement);
                treeViewSearch.Items.Add(newItem);
            }
        }

        private void SearchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < treeItems.Count; i++)
            {
                var item = treeItems[i];
                if (searchWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                if (item.Ielement.GetName().ToLower().Contains(searchWord))
                {
                    searchWorker.ReportProgress(0, item.Ielement);
                    Thread.Sleep(5);
                }
            }

            searchWorker.ReportProgress(100);
        }

        private List<TreeItemExplorerElement> GetSubTreeItems(TreeItemExplorerElement source)
        {
            List<TreeItemExplorerElement> list = new();
            var items = source.Items.SourceCollection.GetEnumerator();
            while (items.MoveNext())
            {
                var item = (TreeItemExplorerElement)items.Current;
                list.Add(item);
                if (item.Items.Count > 0)
                    list.AddRange(GetSubTreeItems(item)); // recurse
            }

            return list;
        }

        private void treeViewTriggerExplorer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeItemExplorerElement selected = treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement;
            if (selected != null)
            {
                OnOpenExplorerElement?.Invoke(selected);
                e.Handled = true; // prevents event from firing up the parent items
            }
        }

        private void treeViewSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeItemExplorerElement selected = treeViewSearch.SelectedItem as TreeItemExplorerElement;
            if (selected != null)
            {
                OnOpenExplorerElement?.Invoke(selected);
                e.Handled = true; // prevents event from firing up the parent items
            }
        }

        private void treeViewSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            TreeItemExplorerElement selected = treeViewSearch.SelectedItem as TreeItemExplorerElement;
            if (selected != null)
                OnOpenExplorerElement?.Invoke(selected);
        }

        private void btnCloseSearchMenu_Click(object sender, RoutedEventArgs e)
        {
            searchMenu.Visibility = Visibility.Hidden;
        }

        private void searchMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                searchMenu.Visibility = Visibility.Hidden;
        }

    }
}

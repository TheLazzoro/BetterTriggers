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
using System.Windows.Media.Animation;

namespace GUI.Components
{
    public partial class TriggerExplorer : UserControl
    {
        internal static TriggerExplorer Current;

        public TreeViewItem map;
        public TreeViewItem currentElement;
        public event Action<ExplorerElement> OnOpenExplorerElement;

        // Drag and drop fields
        Point _startPoint;
        TreeViewItem dragItem;
        bool _IsDragging = false;
        TreeViewItem parentDropTarget;
        int insertIndex = 0; // used when a file is moved from one location to the other.
                             // We can use it when the user wants to drop a file at a specific index.

        // Visual indicators for TreeViewItem
        AdornerLayer adorner;
        TreeItemAdornerLine lineIndicator;
        TreeItemAdornerSquare squareIndicator;

        BackgroundWorker searchWorker;

        private TriggerExplorerViewModel viewModel;

        public TriggerExplorer()
        {
            InitializeComponent();

            searchWorker = new BackgroundWorker();
            searchWorker.WorkerReportsProgress = true;
            searchWorker.WorkerSupportsCancellation = true;
            searchWorker.DoWork += SearchWorker_DoWork;
            searchWorker.ProgressChanged += SearchWorker_ProgressChanged;
            searchWorker.RunWorkerCompleted += SearchWorker_RunWorkerCompleted;

            viewModel = new TriggerExplorerViewModel();
            DataContext = viewModel;

            KeyDown += TriggerExplorer_KeyDown;
        }

        /// <summary>
        /// Returns the underlying ExplorerElement from a given 'TreeViewItem'
        /// </summary>
        public ExplorerElement? GetExplorerElementFromItem(TreeViewItem item)
        {
            var explorerElement = treeViewTriggerExplorer.ItemContainerGenerator.ItemFromContainer(item) as ExplorerElement;
            return explorerElement;
        }

        public ExplorerElement? GetTreeItemFromExplorerElement(ExplorerElement explorerElement)
        {
            var treeItem = treeViewTriggerExplorer.ItemContainerGenerator.ContainerFromItem(explorerElement) as TreeViewItem;
            return explorerElement;
        }

        public ExplorerElement? GetSelectedExplorerElement()
        {
            ExplorerElement item = GetExplorerElementFromItem(treeViewTriggerExplorer.SelectedItem as TreeViewItem);
            return item;
        }

        public TextBox GetCurrentRenameBox()
        {
            var selected = treeViewTriggerExplorer.SelectedItem as TreeViewItem;
            var renameBox = selected.FindName("renameBox") as TextBox;
            return renameBox;
        }

        private void TriggerExplorer_KeyDown(object sender, KeyEventArgs e)
        {
            var selected = GetSelectedExplorerElement();

            if (e.Key == Key.F2)
                SetSelectedRenameBoxVisible(true);
            else if (e.Key == Key.Escape)
                SetSelectedRenameBoxVisible(false);
            if (e.Key == Key.Enter && selected.RenameBoxVisibility == Visibility.Visible)
                RenameExplorerElement();
            else if (e.Key == Key.Delete)
            {

            }
        }

        private void SetSelectedRenameBoxVisible(bool isVisible)
        {
            var selected = GetSelectedExplorerElement();
            if (isVisible)
                selected.RenameBoxVisibility = Visibility.Visible;
            else
                selected.RenameBoxVisibility = Visibility.Hidden;
        }

        private void RenameExplorerElement()
        {
            var selected = GetSelectedExplorerElement();
            selected.Rename();
        }

        /// <summary>
        /// // It is necessary to traverse the item's parents since drag & drop picks up
        /// things like 'TextBlock' and 'Border' on the drop target when dropping the 
        /// dragged element.
        /// </summary>
        /// <returns></returns>
        private TreeViewItem GetTraversedTargetDropItem(FrameworkElement dropTarget)
        {
            if (dropTarget == null || dropTarget is TreeView)
                return null;

            TreeViewItem traversedTarget = null;
            while (traversedTarget == null && dropTarget != null)
            {
                dropTarget = dropTarget.Parent as FrameworkElement;
                if (dropTarget is TreeViewItem)
                {
                    traversedTarget = (TreeViewItem)dropTarget;
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
            dragItem = this.treeViewTriggerExplorer.SelectedItem as TreeViewItem;
            var explorerElement = GetExplorerElementFromItem(dragItem);

            if (dragItem == null || explorerElement.IsRenaming)
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

            TreeViewItem dropTarget = GetTraversedTargetDropItem(e.Source as FrameworkElement);
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

            var explorerElement = GetExplorerElementFromItem(dropTarget);
            if (dropTarget is TreeViewItem && !(explorerElement.ElementType == ExplorerElementEnum.Root))
            {
                var relativePos = e.GetPosition(dropTarget);
                TreeItemLocation location = UIUtility.TreeItemGetMouseLocation(dropTarget, relativePos);

                if (explorerElement.ElementType == ExplorerElementEnum.Folder)
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

        private void DragOverLogic(TreeViewItem dropTarget, TreeItemLocation location, int currentIndex)
        {
            var explorerElement = GetExplorerElementFromItem(dropTarget);
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
            else if (location == TreeItemLocation.Middle && explorerElement.ElementType == ExplorerElementEnum.Folder)
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
            var dragItemParent = (TreeViewItem)dragItem.Parent;
            var explorerElementDragItem = GetExplorerElementFromItem(dragItem);
            if (dragItemParent == parentDropTarget)
            {
                Project.CurrentProject.RearrangeElement(explorerElementDragItem, insertIndex);
                return;
            }

            var dropTarget = parentDropTarget;
            var explorerElementDropTarget = GetExplorerElementFromItem(dropTarget);
            try
            {
                FileSystemUtil.Move(explorerElementDragItem.GetPath(), explorerElementDropTarget.GetPath(), this.insertIndex);
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
            var treeItem = (TreeViewItem)e.Source;
            var explorerElement = GetExplorerElementFromItem(treeItem);
            if (explorerElement.ElementType == ExplorerElementEnum.Folder)
            {
                explorerElement.isExpanded = true;
            }
        }

        private void treeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            var treeItem = (TreeViewItem)e.Source;
            var explorerElement = GetExplorerElementFromItem(treeItem);
            if (explorerElement.ElementType == ExplorerElementEnum.Folder)
            {
                explorerElement.isExpanded = false;
            }
        }

        private void treeViewTriggerExplorer_KeyDown(object sender, KeyEventArgs e)
        {
            var selected = treeViewTriggerExplorer.SelectedItem as TreeViewItem;
            if (selected == null)
            {
                return;
            }

            var explorerElement = GetExplorerElementFromItem(selected);
            if (e.Key == Key.Enter)
            {
                OnOpenExplorerElement?.Invoke(explorerElement);
            }

            else if (e.Key == Key.Delete)
            {
                TreeViewItem selectedElement = treeViewTriggerExplorer.SelectedItem as TreeViewItem;
                if (selectedElement == null || selectedElement == map)
                    return;

                List<ExplorerElement> refs = explorerElement.GetReferrers();
                if (refs.Count > 0)
                {
                    DialogBoxReferences dialogBox = new DialogBoxReferences(refs, ExplorerAction.Delete);
                    dialogBox.ShowDialog();
                    if (!dialogBox.OK)
                        return;
                }

                explorerElement.Delete();
            }
            else if (e.Key == Key.C && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                Project.CurrentProject.CopyExplorerElement(explorerElement);
            }
            else if (e.Key == Key.X && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                Project.CurrentProject.CopyExplorerElement(explorerElement, true);
            }
            else if (e.Key == Key.V && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                Project.CurrentProject.PasteExplorerElement(explorerElement);
            }
            else if (e.Key == Key.F && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                //OpenSearchField();
            }
        }

        private void treeViewTriggerExplorer_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem rightClickedElement = GetTraversedTargetDropItem(e.Source as FrameworkElement);
            var explorerElement = GetExplorerElementFromItem(rightClickedElement);

            if (rightClickedElement == null)
                return;

            currentElement = rightClickedElement;
            rightClickedElement.IsSelected = true;
            rightClickedElement.ContextMenu = contextMenu;

            menuPaste.IsEnabled = CopiedElements.CopiedExplorerElement != null;
            menuElementEnabled.IsChecked = explorerElement.GetEnabled();
            menuElementInitiallyOn.IsChecked = explorerElement.GetInitiallyOn();
            menuElementEnabled.IsEnabled = explorerElement.ElementType == ExplorerElementEnum.Trigger || explorerElement.ElementType == ExplorerElementEnum.Script;
            menuElementInitiallyOn.IsEnabled = explorerElement.ElementType == ExplorerElementEnum.Trigger;
            menuRename.IsEnabled = explorerElement.ElementType is not ExplorerElementEnum.Root;
            menuDelete.IsEnabled = explorerElement.ElementType is not ExplorerElementEnum.Root;
            menuCut.IsEnabled = explorerElement.ElementType is not ExplorerElementEnum.Root;
            menuCopy.IsEnabled = explorerElement.ElementType is not ExplorerElementEnum.Root;
        }

        private void menuCut_Click(object sender, RoutedEventArgs e)
        {
            Project project = Project.CurrentProject;
            var explorerElement = GetExplorerElementFromItem(currentElement);
            project.CopyExplorerElement(explorerElement, true);
        }

        private void menuCopy_Click(object sender, RoutedEventArgs e)
        {
            Project project = Project.CurrentProject;
            var explorerElement = GetExplorerElementFromItem(currentElement);
            project.CopyExplorerElement(explorerElement);
        }

        private void menuPaste_Click(object sender, RoutedEventArgs e)
        {
            Project project = Project.CurrentProject;
            var explorerElement = GetExplorerElementFromItem(currentElement);
            project.PasteExplorerElement(explorerElement);
        }

        private void menuRename_Click(object sender, RoutedEventArgs e)
        {
            var explorerElement = GetExplorerElementFromItem(currentElement);
            explorerElement.IsRenaming = true;
        }

        private void menuDelete_Click(object sender, RoutedEventArgs e)
        {
            var explorerElement = GetExplorerElementFromItem(currentElement);
            explorerElement.Delete();
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
            var explorerElement = GetExplorerElementFromItem(currentElement);
            explorerElement.SetEnabled(!explorerElement.GetEnabled());
        }

        private void menuElementInitiallyOn_Click(object sender, RoutedEventArgs e)
        {
            var explorerElement = GetExplorerElementFromItem(currentElement);
            explorerElement.SetInitiallyOn(!explorerElement.GetInitiallyOn());
        }

        private void menuOpenInExplorer_Click(object sender, RoutedEventArgs e)
        {
            var explorerElement = GetExplorerElementFromItem(currentElement);
            FileSystemUtil.OpenInExplorer(explorerElement.GetPath());
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
            TreeViewItem treeItem = SearchForElement(searchText, treeViewTriggerExplorer.Items[0] as TreeViewItem);
            if (treeItem != null)
            {
                TreeViewItem parent = treeItem.Parent as TreeViewItem;
                while (parent != null)
                {
                    parent.IsExpanded = true;
                    parent = parent.Parent as TreeViewItem;
                }
                treeItem.IsSelected = true;
                treeItem.BringIntoView();
                //treeItem.Focus();
            }
        }

        private TreeViewItem SearchForElement(string searchText, TreeViewItem parent)
        {
            var explorerElement = GetExplorerElementFromItem(parent);
            if (explorerElement.GetName() == searchText)
                return parent;

            TreeViewItem treeItem = null;
            if (parent.Items.Count > 0)
            {
                foreach (var item in parent.Items)
                {
                    treeItem = SearchForElement(searchText, item as TreeViewItem);
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

        List<TreeViewItem> treeItems = new();
        string searchWord;
        private void SearchWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 100)
            {
                TreeViewItem newItem = new TreeViewItem(e.UserState as IExplorerElement);
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

        private List<TreeViewItem> GetSubTreeItems(TreeViewItem source)
        {
            List<TreeViewItem> list = new();
            var items = source.Items.SourceCollection.GetEnumerator();
            while (items.MoveNext())
            {
                var item = (TreeViewItem)items.Current;
                list.Add(item);
                if (item.Items.Count > 0)
                    list.AddRange(GetSubTreeItems(item)); // recurse
            }

            return list;
        }

        private void treeViewTriggerExplorer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem selected = treeViewTriggerExplorer.SelectedItem as TreeViewItem;
            if (selected != null)
            {
                OnOpenExplorerElement?.Invoke(selected);
                e.Handled = true; // prevents event from firing up the parent items
            }
        }

        private void treeViewSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem selected = treeViewSearch.SelectedItem as TreeViewItem;
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

            TreeViewItem selected = treeViewSearch.SelectedItem as TreeViewItem;
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
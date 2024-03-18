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
            var explorerElement = item.DataContext as ExplorerElement;
            return explorerElement;
        }

        public TreeViewItem? GetTreeItemFromExplorerElement(ExplorerElement explorerElement)
        {
            List<ExplorerElement> list = new();
            TreeViewItem? treeViewItem = null;

            // walks up the element hierarchy until a parent attached to the TreeView is found.
            while (treeViewItem == null)
            {
                treeViewItem = treeViewTriggerExplorer.ItemContainerGenerator.ContainerFromItem(explorerElement) as TreeViewItem;
                if (treeViewItem == null)
                {
                    list.Add(explorerElement);
                    explorerElement = explorerElement.GetParent();
                }
            }

            // walks down the tree until the TreeViewItem has been pulled out.
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var element = list[i];
                treeViewItem = treeViewItem.ItemContainerGenerator.ContainerFromItem(element) as TreeViewItem;
            }

            return treeViewItem;
        }

        public ExplorerElement? GetSelectedExplorerElement()
        {
            ExplorerElement item = treeViewTriggerExplorer.SelectedItem as ExplorerElement;
            return item;
        }

        public void SetSelectedElement(ExplorerElement element)
        {
            currentElement = GetTreeItemFromExplorerElement(element);
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
        }

        private void SetSelectedRenameBoxVisible(bool isVisible)
        {
            var selected = GetSelectedExplorerElement();
            if (isVisible)
            {
                selected.RenameBoxVisibility = Visibility.Visible;
            }
            else
                selected.RenameBoxVisibility = Visibility.Hidden;
        }

        private void RenameExplorerElement()
        {
            try
            {
                var selected = GetSelectedExplorerElement();
                selected.Rename();
            }
            catch (Exception e)
            {
                Dialogs.MessageBox dialog = new Dialogs.MessageBox("Error renaming", e.Message);
                dialog.ShowDialog();
            }
        }

        private void treeViewItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (
                e.LeftButton == MouseButtonState.Pressed
                && !_IsDragging
                && !contextMenu.IsVisible
                )
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
            var explorerElement = this.treeViewTriggerExplorer.SelectedItem as ExplorerElement;
            dragItem = GetTreeItemFromExplorerElement(explorerElement);
            if (dragItem == null)
                return;

            if (explorerElement.ElementType == ExplorerElementEnum.Root)
                return;

            _IsDragging = true;

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

        private void treeViewItem_DragOver(object sender, DragEventArgs e)
        {
            if (dragItem == null)
                return;

            if (!dragItem.IsKeyboardFocused)
                return;

            var dragItemExplorerElement = GetExplorerElementFromItem(dragItem);
            if (dragItemExplorerElement.ElementType == ExplorerElementEnum.Root)
                return;

            var currentParent = TreeViewItemHelper.GetTreeItemParent(dragItem);
            TreeViewItem dropTarget = TreeViewItemHelper.GetTraversedTargetDropItem(e.Source as DependencyObject);
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

            var explorerElementDropTarget = GetExplorerElementFromItem(dropTarget);
            if (explorerElementDropTarget.ElementType != ExplorerElementEnum.Root)
            {
                var relativePos = e.GetPosition(dropTarget);
                TreeItemLocation location = UIUtility.TreeItemGetMouseLocation(dropTarget, relativePos);

                if (explorerElementDropTarget.ElementType == ExplorerElementEnum.Folder)
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
            //else
            //parentDropTarget = null;

            e.Handled = true;
        }

        private void DragOverLogic(TreeViewItem dropTarget, TreeItemLocation location, int currentIndex)
        {
            var explorerElementDropTarget = GetExplorerElementFromItem(dropTarget);
            if (location == TreeItemLocation.Top)
            {
                adorner = AdornerLayer.GetAdornerLayer(dropTarget);
                lineIndicator = new TreeItemAdornerLine(dropTarget, true);
                adorner.Add(lineIndicator);

                parentDropTarget = TreeViewItemHelper.GetTreeItemParent(dropTarget) as TreeViewItem;
                var explorerParentDropTarget = explorerElementDropTarget.GetParent();
                insertIndex = explorerParentDropTarget.ExplorerElements.IndexOf(explorerElementDropTarget);
            }
            else if (location == TreeItemLocation.Middle && explorerElementDropTarget.ElementType == ExplorerElementEnum.Folder)
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

                parentDropTarget = TreeViewItemHelper.GetTreeItemParent(dropTarget) as TreeViewItem;
                var explorerParentDropTarget = explorerElementDropTarget.GetParent();
                insertIndex = explorerParentDropTarget.ExplorerElements.IndexOf(explorerElementDropTarget) + 1;

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
            var dragItemParent = TreeViewItemHelper.GetTreeItemParent(dragItem);
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

        private void treeViewTriggerExplorer_KeyDown(object sender, KeyEventArgs e)
        {
            var selected = treeViewTriggerExplorer.SelectedItem as ExplorerElement;
            if (selected == null)
            {
                return;
            }

            if (e.Key == Key.Enter)
            {
                OnOpenExplorerElement?.Invoke(selected);
            }

            else if (e.Key == Key.Delete)
            {
                if (selected.ElementType == ExplorerElementEnum.Root)
                    return;

                List<ExplorerElement> refs = selected.GetReferrers();
                if (refs.Count > 0)
                {
                    DialogBoxReferences dialogBox = new DialogBoxReferences(refs, ExplorerAction.Delete);
                    dialogBox.ShowDialog();
                    if (!dialogBox.OK)
                        return;
                }

                DialogBox dialog = new DialogBox("Delete Element", $"Are you sure you want to delete '{selected.GetName()}' ?");
                dialog.ShowDialog();
                if(dialog.OK)
                {
                    selected.Delete();
                }
            }
            else if (e.Key == Key.C && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                Project.CurrentProject.CopyExplorerElement(selected);
            }
            else if (e.Key == Key.X && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                Project.CurrentProject.CopyExplorerElement(selected, true);
            }
            else if (e.Key == Key.V && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                Project.CurrentProject.PasteExplorerElement(selected);
            }
            else if (e.Key == Key.F && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                //OpenSearchField();
            }
        }

        private void treeViewItem_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var rightClickedElement = sender as TreeViewItem;
            if (rightClickedElement == null)
                return;

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

            contextMenu.IsOpen = true;

            e.Handled = true;
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
            explorerElement.RenameBoxVisibility = Visibility.Visible;
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

        public void NavigateToExplorerElement(ExplorerElement explorerElement)
        {
            List<ExplorerElement> itemsToExpand = new();
            while (true)
            {
                itemsToExpand.Add(explorerElement);
                explorerElement = explorerElement.GetParent();
                if (explorerElement.ElementType == ExplorerElementEnum.Root)
                    break;
            }

            for (int i = itemsToExpand.Count - 1; i >= 0; i--)
            {
                explorerElement = itemsToExpand[i];
                explorerElement.IsExpanded = true;
            }

            explorerElement.IsSelected = true;
        }

        private void treeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            // Only react to the Selected event raised by the TreeViewItem
            // whose IsSelected property was modified. Ignore all ancestors
            // who are merely reporting that a descendant's Selected fired.
            if (!Object.ReferenceEquals(sender, e.OriginalSource))
                return;

            TreeViewItem item = e.OriginalSource as TreeViewItem;
            if (item != null)
                item.BringIntoView();
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
                viewModel.SearchedFiles.Add(e.UserState as ExplorerElement);
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

                var explorerElement = GetExplorerElementFromItem(item);
                if (explorerElement == null)
                    continue;

                if (explorerElement.GetName().ToLower().Contains(searchWord))
                {
                    searchWorker.ReportProgress(0, explorerElement);
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

        private void treeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selected = treeViewTriggerExplorer.SelectedItem as ExplorerElement;
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
            {
                var explorerElement = GetExplorerElementFromItem(selected);
                OnOpenExplorerElement?.Invoke(explorerElement);
            }
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
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
using GUI.Components.TriggerExplorer;
using GUI.Controllers;
using BetterTriggers.Controllers;
using BetterTriggers.Containers;
using GUI.Components;
using GUI.Components.Shared;
using GUI.Utility;
using BetterTriggers.Models.EditorData;

namespace GUI
{
    public partial class TriggerExplorer : UserControl, IDisposable
    {
        internal static TriggerExplorer Current;

        public TreeItemExplorerElement map;
        public TreeItemExplorerElement currentElement;

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

        public TriggerExplorer()
        {
            InitializeComponent();

            ContainerProject.OnCreated += ContainerProject_OnElementCreated;
        }

        public void Dispose()
        {
            ContainerProject.OnCreated -= ContainerProject_OnElementCreated;
        }

        // This function is invoked by a method in the container when a new file is created.
        internal void ContainerProject_OnElementCreated(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                ControllerTriggerExplorer controller = new ControllerTriggerExplorer();
                controller.OnCreateElement(this, ContainerProject.createdPath); // hack
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
            if (e.LeftButton == MouseButtonState.Pressed && !_IsDragging)
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
            this.map = new TreeItemExplorerElement(ContainerProject.projectFiles[0]);
            treeViewTriggerExplorer.Items.Add(this.map);
            this.map.IsExpanded = true;
            this.map.IsSelected = true;
        }

        private void treeViewItem_DragOver(object sender, DragEventArgs e)
        {
            if (dragItem == null)
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

            if (!dragItem.IsKeyboardFocused)
                return;

            parentDropTarget.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            var dragItemParent = (TreeItemExplorerElement)dragItem.Parent;
            dragItemParent.Items.Remove(dragItem); // hack ?

            // We also insert the item here, in case the file only moved in the dir and didn't get moved to another location
            parentDropTarget.Items.Insert(this.insertIndex, dragItem);
            if (dragItemParent == parentDropTarget)
            {
                ControllerProject controllerProject = new ControllerProject();
                controllerProject.RearrangeElement(dragItem.Ielement, insertIndex);
                return;
            }

            var dropTarget = (TreeItemExplorerElement)parentDropTarget;
            ControllerFileSystem.Move(dragItem.Ielement.GetPath(), dropTarget.Ielement.GetPath(), this.insertIndex);

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
            if (e.Key == Key.Delete)
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

                ControllerFileSystem.Delete(selectedElement.Ielement.GetPath());
            }
            else if (e.Key == Key.C && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                TreeItemExplorerElement selectedElement = treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement;
                ControllerProject controllerProject = new ControllerProject();
                controllerProject.CopyExplorerElement(selectedElement.Ielement);
            }
            else if (e.Key == Key.X && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                TreeItemExplorerElement selectedElement = treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement;
                ControllerProject controllerProject = new ControllerProject();
                controllerProject.CopyExplorerElement(selectedElement.Ielement, true);
            }
            else if (e.Key == Key.V && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                TreeItemExplorerElement selectedElement = treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement;
                ControllerProject controllerProject = new ControllerProject();
                IExplorerElement pasted = controllerProject.PasteExplorerElement(selectedElement.Ielement);

                ControllerTriggerExplorer controllerTriggerExplorer = new ControllerTriggerExplorer();
                var parent = controllerTriggerExplorer.FindTreeNodeDirectory(pasted.GetParent().GetPath());
                controllerTriggerExplorer.RecursePopulate(controllerTriggerExplorer.GetCurrentExplorer(), parent, pasted);
            }
            else if (e.Key == Key.F && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                OpenSearchField();
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

            ControllerProject controller = new ControllerProject();

            menuPaste.IsEnabled = controller.GetCopiedElement() != null;
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
            ControllerProject controller = new ControllerProject();
            controller.CopyExplorerElement(currentElement.Ielement, true);
        }

        private void menuCopy_Click(object sender, RoutedEventArgs e)
        {
            ControllerProject controller = new ControllerProject();
            controller.CopyExplorerElement(currentElement.Ielement);
        }

        private void menuPaste_Click(object sender, RoutedEventArgs e)
        {
            ControllerProject controller = new ControllerProject();
            IExplorerElement pasted = controller.PasteExplorerElement(currentElement.Ielement);

            ControllerTriggerExplorer controllerTriggerExplorer = new ControllerTriggerExplorer();
            var parent = controllerTriggerExplorer.FindTreeNodeDirectory(pasted.GetParent().GetPath());
            controllerTriggerExplorer.RecursePopulate(controllerTriggerExplorer.GetCurrentExplorer(), parent, pasted);
        }

        private void menuRename_Click(object sender, RoutedEventArgs e)
        {
            currentElement.ShowRenameBox();
        }

        private void menuDelete_Click(object sender, RoutedEventArgs e)
        {
            ControllerFileSystem.Delete(currentElement.Ielement.GetPath());
        }

        private void menuNewCategory_Click(object sender, RoutedEventArgs e)
        {
            ControllerFolder.Create();
        }

        private void menuNewTrigger_Click(object sender, RoutedEventArgs e)
        {
            ControllerTrigger.Create();
        }

        private void menuNewScript_Click(object sender, RoutedEventArgs e)
        {
            ControllerScript.Create();
        }

        private void menuNewVariable_Click(object sender, RoutedEventArgs e)
        {
            ControllerVariable.Create();
        }

        private void menuElementEnabled_Click(object sender, RoutedEventArgs e)
        {
            // TODO: This will not update the checkmark in the editor view.
            ControllerProject controller = new ControllerProject();
            controller.SetElementEnabled(currentElement.Ielement, !currentElement.Ielement.GetEnabled());
            if (currentElement.editor != null)
                currentElement.editor.OnStateChange();
        }

        private void menuElementInitiallyOn_Click(object sender, RoutedEventArgs e)
        {
            ControllerProject controller = new ControllerProject();
            controller.SetElementInitiallyOn(currentElement.Ielement, !currentElement.Ielement.GetInitiallyOn());
            if (currentElement.editor != null)
                currentElement.editor.OnStateChange();
        }

        private void menuOpenInExplorer_Click(object sender, RoutedEventArgs e)
        {
            ControllerFileSystem.OpenInExplorer(currentElement.Ielement.GetPath());
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
            Search();
        }

        private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search();
            else if (e.Key == Key.Escape)
                CloseSearchField();
        }

        private void Search()
        {
            TreeItemBT treeItem = SearchForElement(treeViewTriggerExplorer.Items[0] as TreeItemBT);
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
                treeItem.Focus();
            }
        }

        private TreeItemBT SearchForElement(TreeItemBT parent)
        {
            if (parent.GetHeaderText().ToLower().Contains(searchTextBox.Text.ToLower()))
                return parent;

            TreeItemBT treeItem = null;
            if (parent.Items.Count > 0)
            {
                foreach (var item in parent.Items)
                {
                    treeItem = SearchForElement(item as TreeItemBT);
                    if (treeItem != null)
                        break;
                }
            }
            return treeItem;
        }

    }
}

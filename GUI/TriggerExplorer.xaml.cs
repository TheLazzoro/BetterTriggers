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
            while (traversedTarget == null)
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
            DataObject data = null;

            data = new DataObject("inadt", dragItem);

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
            ControllerFileSystem controller = new ControllerFileSystem();
            controller.MoveFile(dragItem.Ielement.GetPath(), dropTarget.Ielement.GetPath(), this.insertIndex);

            // focus select item again
            dragItem.IsSelected = true;
        }

        private void treeViewTriggerExplorer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                TreeItemExplorerElement selectedElement = treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement;
                if (selectedElement == null || selectedElement == map)
                    return;

                ControllerReferences controllerRef = new ControllerReferences();
                List<ExplorerElementTrigger> refs = controllerRef.GetReferrers(selectedElement.Ielement);
                if (refs.Count > 0)
                {
                    DialogBoxReferences dialogBox = new DialogBoxReferences(refs, ExplorerAction.Delete);
                    dialogBox.ShowDialog();
                    if (!dialogBox.OK)
                        return;
                }

                ControllerFileSystem controller = new ControllerFileSystem();
                controller.DeleteElement(selectedElement.Ielement.GetPath());
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
            IExplorerElement pasted =  controller.PasteExplorerElement(currentElement.Ielement);

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
            ControllerFileSystem controller = new ControllerFileSystem();
            controller.DeleteElement(currentElement.Ielement.GetPath());
        }

        private void menuNewCategory_Click(object sender, RoutedEventArgs e)
        {
            ControllerFolder controller = new ControllerFolder();
            controller.CreateFolder();
        }

        private void menuNewTrigger_Click(object sender, RoutedEventArgs e)
        {
            ControllerTrigger controller = new ControllerTrigger();
            controller.CreateTrigger();
        }

        private void menuNewScript_Click(object sender, RoutedEventArgs e)
        {
            ControllerScript controller = new ControllerScript();
            controller.CreateScript();
        }

        private void menuNewVariable_Click(object sender, RoutedEventArgs e)
        {
            ControllerVariable controller = new ControllerVariable();
            controller.CreateVariable();
        }

        private void menuElementEnabled_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Crashes when right-click enable/disabling a trigger element that isn't in the TabControl.
            currentElement.editor.SetElementEnabled(!currentElement.Ielement.GetEnabled());
        }

        private void menuElementInitiallyOn_Click(object sender, RoutedEventArgs e)
        {
            currentElement.editor.SetElementInitiallyOn(!currentElement.Ielement.GetInitiallyOn());
        }

        private void menuOpenInExplorer_Click(object sender, RoutedEventArgs e)
        {
            ControllerFileSystem controller = new ControllerFileSystem();
            controller.OpenInExplorer(currentElement.Ielement.GetPath());
        }

    }
}

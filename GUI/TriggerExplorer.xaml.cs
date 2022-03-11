using Model.Data;
using GUI.Utility;
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
using Model.EditorData.Enums;
using BetterTriggers.Controllers;
using BetterTriggers.Containers;
using GUI.Components;

namespace GUI
{
    /// <summary>
    /// Interaction logic for TriggerExplorer.xaml
    /// </summary>
    /// 
    public partial class TriggerExplorer : UserControl
    {
        public TreeItemExplorerElement map;
        public TreeItemExplorerElement currentElement;

        // Drag and drop fields
        Point _startPoint;
        TreeItemExplorerElement dragItem;
        bool _IsDragging = false;

        int insertIndex = 0; // used when a file is moved from one location to the other.
                             // We can use it when the user wants to drop a file at a specific index.

        // attaches to a treeviewitem
        AdornerLayer adorner;
        TreeItemAdorner lineIndicator;

        public TriggerExplorer()
        {
            InitializeComponent();

            ContainerProject.OnCreated += ContainerProject_OnElementCreated;
            ContainerProject.OnMoved += ContainerProject_OnElementMoved;
        }

        // This function is invoked by a method in the container when a new file is created.
        private void ContainerProject_OnElementCreated(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                ControllerTriggerExplorer controller = new ControllerTriggerExplorer();
                controller.OnCreateElement(this, ContainerProject.createdPath); // hack
            });
        }

        private void ContainerProject_OnElementMoved(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                ControllerTriggerExplorer controller = new ControllerTriggerExplorer();
                controller.OnMoveElement(this, ContainerProject.createdPath, insertIndex); // hack
            });
        }

        /*
         * I am not sure why this is here.
         * We have the same function in MainWindow,
         * but I'm thinking we need it here too at some point.
         */
        private void treeViewTriggerExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ControllerProject controller = new ControllerProject();
            //controller.OnClick_ExplorerElement(treeViewTriggerExplorer.SelectedItem as TreeViewItem, new Grid());

            e.Handled = true; // prevents event from firing up the parent items
        }

        public static bool IsMouseInFirstHalf(FrameworkElement container, Point mousePosition, Orientation orientation)
        {
            if (orientation == Orientation.Vertical)
            {
                return mousePosition.Y < container.ActualHeight / 2;
            }
            return mousePosition.X < container.ActualWidth / 2;
        }

        /// <summary>
        /// // It is necessary to traverse the item's parents since drag & drop picks up
        /// things like 'TextBlock' and 'Border' on the drop target when dropping the 
        /// dragged element.
        /// </summary>
        /// <returns></returns>
        private TreeItemExplorerElement GetTraversedItem(UIElement uiElement)
        {
            FrameworkElement target = uiElement as FrameworkElement;

            if (target is TreeView)
                return null;

            TreeItemExplorerElement traversedTarget = null;
            if (target != null && !(target is TreeView))
            {
                while (traversedTarget == null)
                {
                    target = target.Parent as FrameworkElement;
                    if (target is TreeItemExplorerElement)
                    {
                        traversedTarget = (TreeItemExplorerElement)target;
                    }
                }
            }

            return traversedTarget;
        }

        private void treeViewItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed && !_IsDragging)
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

        private void treeViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
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

        private void treeViewItem_PreviewDrop(object sender, DragEventArgs e)
        {
            /*
            if (_IsDragging && dragItem != null)
            {
                var parent = (TreeViewItem) dragItem.Parent;

                parent.Items.Remove(dragItem);

                var dropTarget = (TreeViewItem) e.OriginalSource;
                dropTarget.Items.Insert(0, dragItem);
            }
            */
        }

        public void CreateRootItem()
        {
            this.map = new TreeItemExplorerElement(ContainerProject.projectFiles[0]);
            treeViewTriggerExplorer.Items.Add(this.map);
            this.map.IsExpanded = true;
            this.map.IsSelected = true;
            //item.AllowDrop = true; // maybe needed?
        }

        private void treeViewItem_PreviewDragEnter(object sender, DragEventArgs e)
        {
            // Use this event to display feedback to the user when dragging?

            // var header = (TreeViewItem)treeViewTriggerExplorer.Items[0];
            //header.Header = ;

        }

        private void treeViewTriggerExplorer_Drop(object sender, DragEventArgs e)
        {
            if (_IsDragging && dragItem != null)
            {
                if(adorner != null)
                    adorner.Remove(lineIndicator);

                TreeItemExplorerElement dropTarget = GetTraversedItem(e.Source as FrameworkElement);
                if (dragItem == dropTarget || dropTarget == null)
                    return;

                var targetParent = (TreeItemExplorerElement)dropTarget.Parent;

                var dragItemParent = (TreeItemExplorerElement)dragItem.Parent;
                dragItemParent.Items.Remove(dragItem); // suspect
                var relativePos = e.GetPosition(dropTarget);
                bool inFirstHalf = IsMouseInFirstHalf(dropTarget, relativePos, Orientation.Vertical);
                if (inFirstHalf)
                    this.insertIndex = targetParent.Items.IndexOf(dropTarget);
                else
                    this.insertIndex = targetParent.Items.IndexOf(dropTarget) + 1;

                // We also insert the item here, in case the file didn't get moved to another location
                targetParent.Items.Insert(this.insertIndex, dragItem);
                if (dragItemParent == targetParent)
                {
                    ControllerProject controllerProject = new ControllerProject();
                    controllerProject.RearrangeElement(dragItem.Ielement, insertIndex);
                }

                ControllerFileSystem controller = new ControllerFileSystem();
                controller.MoveFile(dragItem.Ielement.GetPath(), dropTarget.Ielement.GetPath(), this.insertIndex);

                // focus select item again
                dragItem.IsSelected = true;
            }
        }

        private void treeViewTriggerExplorer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                TreeItemExplorerElement selectedElement = treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement;
                if (selectedElement == null || selectedElement == map)
                    return;

                ControllerFileSystem controller = new ControllerFileSystem();
                controller.DeleteElement(selectedElement.Ielement.GetPath());
            }
        }

        private void treeViewTriggerExplorer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeItemExplorerElement rightClickedElement = GetTraversedItem(e.Source as FrameworkElement);

            if (rightClickedElement == null)
                return;

            // Set selected item
            rightClickedElement.IsSelected = true;
            ContextMenuExplorer contextMenu = new ContextMenuExplorer(rightClickedElement);
            rightClickedElement.ContextMenu = contextMenu;
        }

        private void treeViewTriggerExplorer_DragOver(object sender, DragEventArgs e)
        {
            if (dragItem == null)
                return;

            TreeItemExplorerElement dropTarget = GetTraversedItem(e.Source as FrameworkElement);
            if (dropTarget == null)
                return;

            if (lineIndicator != null)
                adorner.Remove(lineIndicator);

            var relativePos = e.GetPosition(dropTarget);
            bool inFirstHalf = IsMouseInFirstHalf(dropTarget, relativePos, Orientation.Vertical);
            if (inFirstHalf)
            {
                adorner = AdornerLayer.GetAdornerLayer(dropTarget);
                lineIndicator = new TreeItemAdorner(dropTarget, true);
                adorner.Add(lineIndicator);
            }
            else
            {
                adorner = AdornerLayer.GetAdornerLayer(dropTarget);
                lineIndicator = new TreeItemAdorner(dropTarget, false);
                adorner.Add(lineIndicator);
            }
        }
    }
}

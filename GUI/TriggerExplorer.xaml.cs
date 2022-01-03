﻿using Model.Data;
using GUI.Utility;
using GUI.Containers;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for TriggerExplorer.xaml
    /// </summary>
    /// 
    public partial class TriggerExplorer : UserControl
    {
        public ExplorerElement map;
        Point _startPoint;
        TreeViewItem dragItem;
        bool _IsDragging = false;
        FileSystemWatcher fileSystemWatcher;

        public TriggerExplorer(string rootFolderPath)
        {
            InitializeComponent();

            fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = rootFolderPath;
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                string path = e.FullPath;
                ControllerFileSystem controller = new ControllerFileSystem();
                controller.CreateElement(this, path);
            });
            
        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                string path = e.FullPath;
                ControllerFileSystem controller = new ControllerFileSystem();
                controller.DeleteElement(this, path);
            });

        }


        /*
        private void treeViewTriggerExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ControllerProject controller = new ControllerProject();
            controller.OnClick_ExplorerElement(treeViewTriggerExplorer.SelectedItem as TreeViewItem, new Grid());

            e.Handled = true; // prevents event from firing up the parent items
        }
        */

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
            dragItem = this.treeViewTriggerExplorer.SelectedItem as TreeViewItem;
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

        public void CreateRootItem(string path, EnumCategory category)
        {
            this.map = new ExplorerElement(path, true);
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
                var parent = (TreeViewItem)dragItem.Parent;


                // It is necessary to traverse the item's parents since drag & drop picks up
                // things like 'TextBlock' and 'Border' on the drop target when dropping the 
                // dragged element.
                FrameworkElement dropTarget = e.Source as FrameworkElement;
                TreeViewItem traversedTarget = null;
                while (traversedTarget == null)
                {
                    dropTarget = dropTarget.Parent as FrameworkElement;
                    if (dropTarget is TreeViewItem)
                    {
                        traversedTarget = (TreeViewItem)dropTarget;
                    }
                }

                if (traversedTarget != dragItem)
                {
                    parent.Items.Remove(dragItem);
                    traversedTarget.Items.Insert(0, dragItem);
                }
            }
        }

    }
}
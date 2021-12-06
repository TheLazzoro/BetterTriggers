using Model.Data;
using GUI.Components.Utility;
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

namespace GUI.Components.TriggerExplorer
{
    /// <summary>
    /// Interaction logic for TriggerExplorer.xaml
    /// </summary>
    /// 
    public partial class TriggerExplorer : UserControl
    {
        TreeViewItem map;
        Point _startPoint;
        TreeViewItem dragItem;
        bool _IsDragging = false;

        public TriggerExplorer()
        {
            InitializeComponent();

            this.map = CreateTreeViewItem("MapName.w3x", EnumCategory.Map);
            treeViewTriggerExplorer.Items.Add(this.map);
        }

        public void CreateFolder()
        {
            string name = NameGenerator.GenerateCategoryName();

            var item = CreateTreeViewItem(name, EnumCategory.Folder);
            TriggerFolder script = new TriggerFolder(name, item);
        }

        public void CreateTrigger(TriggerControl triggerControl)
        {
            string name = NameGenerator.GenerateTriggerName();

            var item = CreateTreeViewItem(name, EnumCategory.Trigger);
            Trigger trig = new Trigger(name, item, triggerControl);
        }

        public void CreateScript(ICSharpCode.AvalonEdit.TextEditor textEditor)
        {
            string name = NameGenerator.GenerateScriptName();

            var item = CreateTreeViewItem(name, EnumCategory.AI);
            Script script = new Script(name, item, textEditor);
        }

        public void CreateVariable(VariableControl variableControl)
        {
            string name = NameGenerator.GenerateVariableName();

            var item = CreateTreeViewItem(name, EnumCategory.SetVariable);
            Variable script = new Variable(name, item, variableControl);
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

        private TreeViewItem CreateTreeViewItem(string text, EnumCategory category)
        {
            TreeViewItem item = new TreeViewItem();

            TreeViewItem selectedItem = (TreeViewItem)treeViewTriggerExplorer.SelectedItem;
            if (selectedItem != null && selectedItem.Tag is TriggerFolder)
                selectedItem.Items.Add(item);
            else if (selectedItem != null && selectedItem.Parent != null && !(selectedItem.Parent is TreeView))
            {
                TreeViewItem parent = (TreeViewItem)selectedItem.Parent;
                parent.Items.Insert(parent.Items.IndexOf(selectedItem) + 1, item);
            }
            else if (this.map != null)
                this.map.Items.Add(item);

            item.IsExpanded = true;
            item.IsSelected = true;
            //item.AllowDrop = true; // maybe needed?

            TreeViewManipulator.SetTreeViewItemAppearance(item, text, category);

            return item;
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

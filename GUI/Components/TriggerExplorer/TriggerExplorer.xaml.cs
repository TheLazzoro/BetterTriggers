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
    public partial class TriggerExplorer : UserControl
    {
        Point _lastMouseDown;
        TreeViewItem draggedItem, _target;

        public TriggerExplorer()
        {
            InitializeComponent();



            TreeViewItem map = new TreeViewItem();
            map = GetTreeView("MapName.w3x", "resources/document.png");

            treeViewTriggerExplorer.Items.Add(map);

            TreeViewItem subItem = new TreeViewItem();
            subItem.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFF");
            subItem.Header = "Trigger";
            map.Items.Add(subItem);

            TreeViewItem subItem2 = new TreeViewItem();
            subItem2.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFF");
            subItem2.Header = "Trigger2";
            map.Items.Add(subItem2);

            TreeViewItem subItem3 = new TreeViewItem();
            subItem3.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFF");
            subItem3.Header = "Trigger3";
            map.Items.Add(subItem3);

            TreeViewItem subItem4 = new TreeViewItem();
            subItem4.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFF");
            subItem4.Header = "Trigger3";
            subItem3.Items.Add(subItem4);
        }

        public void AddScript(string name)
        {
            treeViewTriggerExplorer.Items.Add(name);
        }

        private void treeViewTriggerExplorer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _lastMouseDown = e.GetPosition(treeViewTriggerExplorer);
            }
        }

        private void treeViewTriggerExplorer_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point currentPosition = e.GetPosition(treeViewTriggerExplorer);


                    if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                        (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                    {
                        draggedItem = (TreeViewItem)treeViewTriggerExplorer.SelectedItem;
                        if (draggedItem != null)
                        {
                            DragDropEffects finalDropEffect = DragDrop.DoDragDrop(treeViewTriggerExplorer, treeViewTriggerExplorer.SelectedValue,
                                DragDropEffects.Move);
                            //Checking target is not null and item is dragging(moving)
                            if ((finalDropEffect == DragDropEffects.Move) && (_target != null))
                            {
                                // A Move drop was accepted
                                if (!draggedItem.Header.ToString().Equals(_target.Header.ToString()))
                                {
                                    CopyItem(draggedItem, _target);
                                    _target = null;
                                    draggedItem = null;
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void treeViewTriggerExplorer_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                Point currentPosition = e.GetPosition(treeViewTriggerExplorer);

                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                   (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    //textBoxTriggerComment.Text = currentPosition.Y.ToString();



                    // Verify that this is a valid drop and then store the drop target
                    TreeViewItem item = GetNearestContainer
                    (e.OriginalSource as UIElement);
                    if (CheckDropTarget(draggedItem, item))
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        private bool IsInFirstHalf(FrameworkElement container, Point mousePosition, Orientation orientation)
        {
            if (orientation == Orientation.Vertical)
            {
                return mousePosition.Y < container.ActualHeight / 2;
            }
            return mousePosition.X < container.ActualWidth / 2;
        }

        private void CopyItem(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {

            //Asking user wether he want to drop the dragged TreeViewItem here or not
            if (MessageBox.Show("Would you like to drop " + _sourceItem.Header.ToString() + " into " + _targetItem.Header.ToString() + "", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    //adding dragged TreeViewItem in target TreeViewItem
                    addChild(_sourceItem, _targetItem);

                    //finding Parent TreeViewItem of dragged TreeViewItem 
                    TreeViewItem ParentItem = FindVisualParent<TreeViewItem>(_sourceItem);
                    // if parent is null then remove from TreeView else remove from Parent TreeViewItem
                    if (ParentItem == null)
                    {
                        treeViewTriggerExplorer.Items.Remove(_sourceItem);
                    }
                    else
                    {
                        ParentItem.Items.Remove(_sourceItem);
                    }
                }
                catch
                {

                }
            }

        }

        public void addChild(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            // add item in target TreeViewItem 
            TreeViewItem item1 = new TreeViewItem();
            item1.Header = _sourceItem.Header;
            _targetItem.Items.Add(item1);
            foreach (TreeViewItem item in _sourceItem.Items)
            {
                addChild(item, item1);
            }
        }

        private bool CheckDropTarget(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            //Check whether the target item is meeting your condition
            bool _isEqual = false;
            if (!_sourceItem.Header.ToString().Equals(_targetItem.Header.ToString()))
            {
                _isEqual = true;
            }
            return _isEqual;

        }



        private void treeViewTriggerExplorer_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                // Verify that this is a valid drop and then store the drop target
                TreeViewItem TargetItem = GetNearestContainer
                    (e.OriginalSource as UIElement);
                if (TargetItem != null && draggedItem != null)
                {
                    _target = TargetItem;
                    e.Effects = DragDropEffects.Move;
                }
            }
            catch (Exception)
            {
            }
        }

        static TObject FindVisualParent<TObject>(UIElement child) where TObject : UIElement
        {
            if (child == null)
            {
                return null;
            }

            UIElement parent = VisualTreeHelper.GetParent(child) as UIElement;

            while (parent != null)
            {
                TObject found = parent as TObject;
                if (found != null)
                {
                    return found;
                }
                else
                {
                    parent = VisualTreeHelper.GetParent(parent) as UIElement;
                }
            }

            return null;
        }

        private TreeViewItem GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }

        private TreeViewItem GetTreeView(string text, string imagePath)
        {
            TreeViewItem item = new TreeViewItem();

            item.IsExpanded = true;

            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            // create Image
            Image image = new Image();
            image.Source = new BitmapImage
                (new Uri(Directory.GetCurrentDirectory() + "/" + imagePath));
            image.Width = 16;
            image.Height = 16;

            // Label
            Label lbl = new Label();
            lbl.Content = text;
            lbl.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFF");

            // Add into stack
            stack.Children.Add(image);
            stack.Children.Add(lbl);


            // assign stack to header
            item.Header = stack;
            return item;
        }
    }
}

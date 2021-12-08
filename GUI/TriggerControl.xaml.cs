using GUI.Commands;
using GUI.Components.TriggerEditor;
using GUI.Components.Utility;
using System;
using System.Collections.Generic;
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


namespace GUI
{
    /// <summary>
    /// Interaction logic for TriggerControl.xaml
    /// </summary>
    public partial class TriggerControl : UserControl
    {
        NodeEvent categoryEvent;
        NodeCondition categoryCondition;
        NodeAction categoryAction;

        TextBlock currentParameterBlock;
        TextBlock currentDescriptionBlock;

        Point _startPoint;
        TreeViewItem dragItem;
        bool _IsDragging = false;

        public TriggerControl()
        {
            InitializeComponent();

            categoryEvent = new NodeEvent();
            categoryCondition = new NodeCondition();
            categoryAction = new NodeAction();

            treeViewTriggers.Items.Add(categoryEvent);
            treeViewTriggers.Items.Add(categoryCondition);
            treeViewTriggers.Items.Add(categoryAction);
        }

        public void CreateEvent()
        {
            var eventMenu = new EventMenuWindow();
            eventMenu.ShowDialog();
            Model.Natives.Function _event = eventMenu.selectedEvent;

            if(_event != null)
            {
                TriggerEvent item = new TriggerEvent(_event);
                categoryEvent.Items.Add(item);

                categoryEvent.IsExpanded = true;
            }

        }

        public void CreateCondition()
        {
            var conditionMenu = new ConditionMenuWindow();
            conditionMenu.ShowDialog();
            Model.Natives.Condition condition = conditionMenu.selectedContition;

            if (condition != null)
            {
                TriggerCondition item = new TriggerCondition(condition);
                categoryCondition.Items.Add(item);

                categoryCondition.IsExpanded = true;
            }
        }

        public void CreateAction()
        {
            var actionMenu = new ActionMenuWindow();
            actionMenu.ShowDialog();
            Model.Natives.Function action = actionMenu.selectedAction;

            if (action != null)
            {
                Components.TriggerEditor.TriggerAction item = new Components.TriggerEditor.TriggerAction(action);
                categoryAction.Items.Add(item);

                categoryAction.IsExpanded = true;
            }
        }

        private void treeViewTriggers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = treeViewTriggers.SelectedItem as TriggerElement;
            if (item != null) {
                var textBlockParameters = item.paramTextBlock;
                var textBlockDescription = item.descriptionTextBlock;

                if (currentParameterBlock != null && currentParameterBlock.Parent != null) { }
                    grid.Children.Remove(currentParameterBlock); // remove current active parameter text block so the new one can be added.
                if (currentDescriptionBlock != null && currentDescriptionBlock.Parent != null) { }
                    grid.Children.Remove(currentDescriptionBlock);


                // Display appropriate textblock
                grid.Children.Add(textBlockParameters);
                Grid.SetRow(textBlockParameters, 3);
                textBlockParameters.Margin = new Thickness(0, 0, 5, 0);

                grid.Children.Add(textBlockDescription);
                Grid.SetRow(textBlockDescription, 4);
                textBlockDescription.Margin = new Thickness(0, 0, 5, 0);

                currentParameterBlock = textBlockParameters;
                currentDescriptionBlock = textBlockDescription;
            }
        }

        private void treeViewTriggers_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void treeViewTriggers_PreviewMouseMove(object sender, MouseEventArgs e)
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

        private void StartDrag(MouseEventArgs e)
        {
            _IsDragging = true;
            dragItem = this.treeViewTriggers.SelectedItem as TreeViewItem;

            if (dragItem is NodeEvent || dragItem is NodeCondition || dragItem is NodeAction)
                return;

            DataObject data = null;

            data = new DataObject("inadt", dragItem);

            if (data != null)
            {
                DragDropEffects dde = DragDropEffects.Move;
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    dde = DragDropEffects.All;
                }
                DragDropEffects de = DragDrop.DoDragDrop(this.treeViewTriggers, data, dde);
            }
            _IsDragging = false;
        }

        private void treeViewTriggers_Drop(object sender, DragEventArgs e)
        {
            if (_IsDragging && dragItem != null)
            {
                var parent = (TreeViewItem)dragItem.Parent;

                // It is necessary to traverse the item's parents since drag & drop picks up
                // things like 'TextBlock' and 'Border' on the drop target when dropping the 
                // dragged element.
                TreeViewItem actionNode = DragItemIsAction(e);

                TreeViewItem whereItemWasDropped = GetTraversedTargetDropItem(e.Source as FrameworkElement);

                int indexInTree = 0;
                TreeViewItem parentToDropLocation;
                if (whereItemWasDropped is NodeAction)
                    indexInTree = 0;
                else
                {
                    parentToDropLocation = whereItemWasDropped.Parent as TreeViewItem;
                    indexInTree = parentToDropLocation.Items.IndexOf(whereItemWasDropped);
                }

                if (actionNode != dragItem)
                {
                    TriggerElementMoveCommand command = new TriggerElementMoveCommand(dragItem, parent, actionNode, indexInTree); // TODO !!!!!!!!!! change 0 to the dropped index
                    command.Execute();
                }
            }
        }

        private TreeViewItem GetTraversedTargetDropItem(FrameworkElement dropTarget)
        {
            TreeViewItem traversedTarget = null;
            while (traversedTarget == null)
            {
                dropTarget = dropTarget.Parent as FrameworkElement;
                if (dropTarget is TreeViewItem)
                {
                    traversedTarget = (TreeViewItem)dropTarget;
                }
            }

            return traversedTarget;
        }

        private TreeViewItem DragItemIsAction(DragEventArgs e)
        {
            FrameworkElement dropTarget = e.Source as FrameworkElement;
            TreeViewItem traversedTarget = GetTraversedTargetDropItem(dropTarget);

            if (traversedTarget is NodeAction)
                return traversedTarget;
            else
                traversedTarget = GetTraversedTargetDropItem(traversedTarget); // traverse one more time to get the action node

            return traversedTarget;
        }

        private void treeViewTriggers_PreviewDragEnter(object sender, DragEventArgs e)
        {
            
        }

        private void treeViewTriggers_PreviewDrop(object sender, DragEventArgs e)
        {
            // Use this event to display feedback to the user when dragging?
        }
    }
}

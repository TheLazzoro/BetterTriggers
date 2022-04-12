using BetterTriggers.Commands;
using BetterTriggers.Controllers;
using GUI.Components;
using GUI.Components.TriggerEditor;
using GUI.Components.TriggerExplorer;
using GUI.Utility;
using Model.EditorData;
using Model.SaveableData;
using Newtonsoft.Json;
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


namespace GUI.Components
{
    /// <summary>
    /// Interaction logic for TriggerControl.xaml
    /// </summary>
    public partial class TriggerControl : UserControl, IEditor
    {
        public ExplorerElementTrigger explorerElementTrigger; // needed to get file references to variables in TriggerElements

        public NodeEvent categoryEvent;
        public NodeCondition categoryCondition;
        public NodeAction categoryAction;

        TextBlock currentParameterBlock;
        TextBlock currentDescriptionBlock;

        Point _startPoint;
        TreeViewItem dragItem;
        bool _IsDragging = false;

        TreeViewTriggerElement copiedTriggerElement;
        private List<TreeItemExplorerElement> observers = new List<TreeItemExplorerElement>();

        public TriggerControl(ExplorerElementTrigger explorerElementTrigger)
        {
            InitializeComponent();

            this.explorerElementTrigger = explorerElementTrigger;

            categoryEvent = new NodeEvent("Events");
            categoryCondition = new NodeCondition("Conditions");
            categoryAction = new NodeAction("Actions");

            treeViewTriggers.Items.Add(categoryEvent);
            treeViewTriggers.Items.Add(categoryCondition);
            treeViewTriggers.Items.Add(categoryAction);

            LoadTrigger(explorerElementTrigger.trigger);
        }

        public void Refresh()
        {
            for (int i = 0; i < categoryEvent.Items.Count; i++)
            {
                var _event = categoryEvent.Items[i] as TreeViewTriggerElement;
                _event.FormatParameterText();
            }
            for (int i = 0; i < categoryCondition.Items.Count; i++)
            {
                var condition = categoryCondition.Items[i] as TreeViewTriggerElement;
                condition.FormatParameterText();
            }
            for (int i = 0; i < categoryAction.Items.Count; i++)
            {
                var action = categoryAction.Items[i] as TreeViewTriggerElement;
                action.FormatParameterText();
            }
        }

        private void LoadTrigger(Model.SaveableData.Trigger trigger)
        {
            this.categoryEvent.Items.Clear();
            this.categoryCondition.Items.Clear();
            this.categoryAction.Items.Clear();

            RecurseLoadTrigger(trigger.Events, this.categoryEvent);
            RecurseLoadTrigger(trigger.Conditions, this.categoryCondition);
            RecurseLoadTrigger(trigger.Actions, this.categoryAction);

            this.categoryEvent.ExpandSubtree();
            this.categoryCondition.ExpandSubtree();
            this.categoryAction.ExpandSubtree();
        }

        private void RecurseLoadTrigger(List<TriggerElement> functions, INode parent)
        {
            for (int i = 0; i < functions.Count; i++)
            {
                var function = functions[i];
                TreeViewTriggerElement triggerElement = new TreeViewTriggerElement(function, this);
                parent.Add(triggerElement);
                if (function is IfThenElse)
                {
                    var ifThenElse = (IfThenElse)function;

                    var nodeIf = new NodeCondition("If - Conditions");
                    var nodeThen = new NodeAction("Then - Actions");
                    var nodeElse = new NodeAction("Else - Actions");
                    triggerElement.Items.Add(nodeIf);
                    triggerElement.Items.Add(nodeThen);
                    triggerElement.Items.Add(nodeElse);
                    RecurseLoadTrigger(ifThenElse.If, nodeIf);
                    RecurseLoadTrigger(ifThenElse.Then, nodeThen);
                    RecurseLoadTrigger(ifThenElse.Else, nodeElse);
                }
            }
        }

        public Model.SaveableData.Trigger GenerateTriggerFromControl()
        {
            Model.SaveableData.Trigger trigger = new Model.SaveableData.Trigger()
            {
                Id = explorerElementTrigger.trigger.Id
            };

            RecurseGenerateTrigger(trigger.Events, this.categoryEvent);
            RecurseGenerateTrigger(trigger.Conditions, this.categoryCondition);
            RecurseGenerateTrigger(trigger.Actions, this.categoryAction);

            return trigger;
        }

        private void RecurseGenerateTrigger(List<TriggerElement> functions, INode node)
        {
            var treeViewTriggerElements = node.GetTreeViewTriggerElements();
            for (int i = 0; i < treeViewTriggerElements.Count; i++)
            {
                var triggerElement = treeViewTriggerElements[i];
                var function = triggerElement.triggerElement;
                functions.Add(function);

                if (function is IfThenElse)
                {
                    var ifThenElse = (IfThenElse)function;
                    var nodeIf = (NodeCondition)triggerElement.Items[0];
                    var nodeThen = (NodeAction)triggerElement.Items[1];
                    var nodeElse = (NodeAction)triggerElement.Items[2];
                    RecurseGenerateTrigger(ifThenElse.If, nodeIf);
                    RecurseGenerateTrigger(ifThenElse.Then, nodeThen);
                    RecurseGenerateTrigger(ifThenElse.Else, nodeElse);
                }
            }
        }

        public string GetSaveString()
        {
            var trigger = GenerateTriggerFromControl();
            return JsonConvert.SerializeObject(trigger);
        }

        public UserControl GetControl()
        {
            return this;
        }

        public void CreateEvent()
        {
            CreateTriggerElement(TriggerElementType.Event);
        }

        public void CreateCondition()
        {
            CreateTriggerElement(TriggerElementType.Condition);
        }

        public void CreateAction()
        {
            CreateTriggerElement(TriggerElementType.Action);
        }

        private void CreateTriggerElement(TriggerElementType type)
        {
            var menu = new TriggerElementMenuWindow(type);
            menu.ShowDialog();
            Function func = menu.selectedTriggerElement;
            TriggerElement triggerElement = new TriggerElement();
            triggerElement.function = func;

            List<TriggerElement> parent = null;
            var selected = treeViewTriggers.SelectedItem;
            int insertIndex = 0;
            if (selected is TriggerElement)
            {
                var treeItem = (TreeViewItem)selected;
                var node = (INode)treeItem.Parent;
                if (node.GetNodeType() == type)
                {
                    parent = node.GetTriggerElements();
                    insertIndex = parent.IndexOf((TriggerElement)selected);
                }
                else
                {
                    if (type == TriggerElementType.Event)
                        parent = categoryEvent.GetTriggerElements();
                    else if (type == TriggerElementType.Condition)
                        parent = categoryCondition.GetTriggerElements();
                    else if (type == TriggerElementType.Action)
                        parent = categoryAction.GetTriggerElements();

                    insertIndex = parent.Count;
                }
            }

            if (func != null)
            {
                CommandTriggerElementCreate command = new CommandTriggerElementCreate(triggerElement, parent, insertIndex);
                command.Execute();
            }
        }

        private void treeViewTriggers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (currentParameterBlock != null && currentParameterBlock.Parent != null) { }
            grid.Children.Remove(currentParameterBlock); // remove current active parameter text block so the new one can be added.
            if (currentDescriptionBlock != null && currentDescriptionBlock.Parent != null) { }
            grid.Children.Remove(currentDescriptionBlock);

            var item = treeViewTriggers.SelectedItem as TreeViewTriggerElement;
            if (item == null)
                return;

            var textBlockParameters = item.paramTextBlock;
            var textBlockDescription = item.descriptionTextBlock;
            // Display appropriate textblock
            grid.Children.Add(textBlockParameters);
            grid.Children.Add(textBlockDescription);
            currentParameterBlock = textBlockParameters;
            currentDescriptionBlock = textBlockDescription;
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
                INode node = GetCategoryTarget(e.Source as FrameworkElement);
                TreeViewItem whereItemWasDropped = GetTraversedTargetDropItem(e.Source as FrameworkElement);

                var item = (TreeViewTriggerElement)this.dragItem;
                if (whereItemWasDropped is TreeViewTriggerElement)
                {
                    var location = (TreeViewTriggerElement)whereItemWasDropped;
                    int insertIndex = node.GetTriggerElements().IndexOf(location.triggerElement);
                    int oldIndex = item.triggerElement.Parent.IndexOf(item.triggerElement);
                    bool isSameParent = location.triggerElement.Parent == item.triggerElement.Parent;

                    if (isSameParent && insertIndex == oldIndex)
                        return;

                }
                else if (whereItemWasDropped is NodeEvent || whereItemWasDropped is NodeCondition || whereItemWasDropped is NodeAction)
                {
                    if (item.triggerElement.Parent as )
                }







                int insertIndex = 0;
                TreeViewItem parentToDropLocation;
                if (whereItemWasDropped.GetType() == NodeAction)
                    insertIndex = 0;
                else
                {
                    parentToDropLocation = whereItemWasDropped.Parent as TreeViewItem;
                    insertIndex = parentToDropLocation.Items.IndexOf(whereItemWasDropped);
                }

                if (node != dragItem)
                {
                    CommandTriggerElementMove command = new CommandTriggerElementMove(this, this.dragItem, parent, node, insertIndex);
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

        private INode GetCategoryTarget(FrameworkElement dropTarget)
        {
            if (dropTarget is NodeEvent || dropTarget is NodeCondition || dropTarget is NodeAction)
                return (INode)dropTarget;

            TreeViewItem traversedTarget = GetTraversedTargetDropItem(dropTarget);
            if (traversedTarget is NodeEvent || traversedTarget is NodeCondition || traversedTarget is NodeAction)
                return (INode)traversedTarget;
            else
                traversedTarget = GetTraversedTargetDropItem(traversedTarget); // traverse one more time to get the action node

            return (INode)traversedTarget;
        }

        private void treeViewTriggers_PreviewDragEnter(object sender, DragEventArgs e)
        {

        }

        private void treeViewTriggers_PreviewDrop(object sender, DragEventArgs e)
        {
            // Use this event to display feedback to the user when dragging?
        }

        private void treeViewTriggers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                DeleteTriggerElement();
            else if (e.Key == Key.C && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                CopyTriggerElement();
            else if (e.Key == Key.V && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                PasteTriggerElement();
        }

        private void DeleteTriggerElement()
        {
            var selectedItem = (TreeViewItem)treeViewTriggers.SelectedItem;
            if (selectedItem == null || selectedItem is NodeEvent || selectedItem is NodeCondition || selectedItem is NodeAction)
                return;

            CommandTriggerElementDelete command = new CommandTriggerElementDelete(this, selectedItem as TreeViewTriggerElement, (TreeViewItem)selectedItem.Parent);
            command.Execute();
        }

        private void CopyTriggerElement()
        {
            var selectedItem = (TreeViewItem)treeViewTriggers.SelectedItem;
            if (selectedItem == null || selectedItem is NodeEvent || selectedItem is NodeCondition || selectedItem is NodeAction)
                return;

            var selectedItemCast = (TreeViewTriggerElement)selectedItem;
            this.copiedTriggerElement = selectedItemCast;
        }

        private void PasteTriggerElement()
        {
            throw new NotImplementedException();

            /*
            var selectedItem = (TreeViewItem)treeViewTriggers.SelectedItem;
            if (selectedItem == null || this.copiedTriggerElement == null)
                return;

            // Copy the actual values from the model-layer
            Function function = null;
            if (copiedTriggerElement is Components.TriggerEditor.TriggerEvent)
            {
                var element = (Components.TriggerEditor.TriggerEvent) this.copiedTriggerElement;
                function = (Function) element._event.Clone();
            }
            else if (copiedTriggerElement is Components.TriggerEditor.TriggerCondition)
            {
                var element = (Components.TriggerEditor.TriggerCondition)this.copiedTriggerElement;
                throw new NotImplementedException();
            }
            else if(copiedTriggerElement is Components.TriggerEditor.TriggerAction)
            {
                var element = (Components.TriggerEditor.TriggerAction)this.copiedTriggerElement;
                function = (Function)element.action.Clone();
            }

            // Determine where to place the pasted element
            TreeViewItem targetParentNode;
            int insertIndex = 0;
            if (selectedItem is NodeEvent || selectedItem is NodeCondition || selectedItem is NodeAction)
                targetParentNode = selectedItem;
            else
            {
                targetParentNode = selectedItem.Parent as TreeViewItem;
                insertIndex = targetParentNode.Items.IndexOf(selectedItem);
            }

            // Determine if the copied item is appropriate for the target node
            if (this.copiedTriggerElement is Components.TriggerEditor.TriggerEvent && !(targetParentNode is NodeEvent))
                return;
            if (this.copiedTriggerElement is Components.TriggerEditor.TriggerCondition && !(targetParentNode is NodeCondition))
                return;
            if (this.copiedTriggerElement is Components.TriggerEditor.TriggerAction && !(targetParentNode is NodeAction))
                return;

            CommandTriggerElementPaste command = new CommandTriggerElementPaste(function, targetParentNode, insertIndex);
            command.Execute();
            */
        }

        public void OnElementRename(string name)
        {
            throw new NotImplementedException();
        }

        public void OnRemoteChange()
        {
            throw new NotImplementedException();
        }

        public void Attach(TreeItemExplorerElement explorerElement)
        {
            this.observers.Add(explorerElement);
        }

        public void Detach(TreeItemExplorerElement explorerElement)
        {
            this.observers.Add(explorerElement);

        }

        public void OnStateChange()
        {
            foreach (var observer in observers)
            {
                observer.OnStateChange();
            }
        }

        public void SetElementEnabled(bool isEnabled)
        {
            checkBoxIsEnabled.IsChecked = isEnabled;
            ControllerProject controller = new ControllerProject();
            controller.SetElementEnabled(explorerElementTrigger, (bool)checkBoxIsEnabled.IsChecked);
            OnStateChange();
        }

        public void SetElementInitiallyOn(bool isInitiallyOn)
        {
            checkBoxIsInitiallyOn.IsChecked = isInitiallyOn;
            ControllerProject controller = new ControllerProject();
            controller.SetElementInitiallyOn(explorerElementTrigger, (bool)checkBoxIsInitiallyOn.IsChecked);
            OnStateChange();
        }

        private void checkBoxIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            ControllerProject controller = new ControllerProject();
            controller.SetElementEnabled(explorerElementTrigger, (bool)checkBoxIsEnabled.IsChecked);
            OnStateChange();
        }

        private void checkBoxIsInitiallyOn_Click(object sender, RoutedEventArgs e)
        {
            ControllerProject controller = new ControllerProject();
            controller.SetElementInitiallyOn(explorerElementTrigger, (bool)checkBoxIsInitiallyOn.IsChecked);
            OnStateChange();
        }
    }
}

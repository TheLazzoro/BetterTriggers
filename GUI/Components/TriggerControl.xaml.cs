using BetterTriggers.Commands;
using BetterTriggers.Controllers;
using GUI.Components.TriggerEditor;
using GUI.Components.TriggerExplorer;
using GUI.Container;
using GUI.Controllers;
using Model.EditorData;
using Model.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace GUI.Components
{
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

        private List<TreeItemExplorerElement> observers = new List<TreeItemExplorerElement>();
        private ControllerTriggerControl controllerTriggerControl = new ControllerTriggerControl();
        private TreeViewTriggerElement selectedElement;
        private TreeViewTriggerElement selectedElementEnd;
        private List<TreeViewTriggerElement> selectedItems = new List<TreeViewTriggerElement>();

        public TriggerControl(ExplorerElementTrigger explorerElementTrigger)
        {
            InitializeComponent();

            this.explorerElementTrigger = explorerElementTrigger;
            treeViewTriggers.SelectedItemChanged += TreeViewTriggers_SelectedItemChanged;

            categoryEvent = new NodeEvent("Events");
            categoryCondition = new NodeCondition("Conditions");
            categoryAction = new NodeAction("Actions");

            treeViewTriggers.Items.Add(categoryEvent);
            treeViewTriggers.Items.Add(categoryCondition);
            treeViewTriggers.Items.Add(categoryAction);


            LoadTrigger(explorerElementTrigger.trigger);
        }

        private void TreeViewTriggers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeViewTriggers.SelectedItem is INode)
            {
                this.selectedItems = controllerTriggerControl.SelectItemsMultiple(null, null);
                return;
            }

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                selectedElementEnd = (TreeViewTriggerElement)treeViewTriggers.SelectedItem;
            else
            {
                selectedElement = (TreeViewTriggerElement)treeViewTriggers.SelectedItem;
                selectedElementEnd = (TreeViewTriggerElement)treeViewTriggers.SelectedItem;
            }

            this.selectedItems = controllerTriggerControl.SelectItemsMultiple(selectedElement, selectedElementEnd);
        }

        public void Refresh()
        {
            for (int i = 0; i < categoryEvent.Items.Count; i++)
            {
                var _event = categoryEvent.Items[i] as TreeViewTriggerElement;
                _event.UpdateTreeItem();
            }
            for (int i = 0; i < categoryCondition.Items.Count; i++)
            {
                var condition = categoryCondition.Items[i] as TreeViewTriggerElement;
                condition.UpdateTreeItem();
            }
            for (int i = 0; i < categoryAction.Items.Count; i++)
            {
                var action = categoryAction.Items[i] as TreeViewTriggerElement;
                action.UpdateTreeItem();
            }
        }

        private void LoadTrigger(Model.SaveableData.Trigger trigger)
        {
            ControllerTriggerControl controller = new ControllerTriggerControl();
            controller.RecurseLoadTrigger(trigger.Events, this.categoryEvent);
            controller.RecurseLoadTrigger(trigger.Conditions, this.categoryCondition);
            controller.RecurseLoadTrigger(trigger.Actions, this.categoryAction);

            this.categoryEvent.ExpandSubtree();
            this.categoryCondition.ExpandSubtree();
            this.categoryAction.ExpandSubtree();
        }

        public string GetSaveString()
        {
            //var trigger = GenerateTriggerFromControl();
            return JsonConvert.SerializeObject(this.explorerElementTrigger.trigger);
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
            TriggerElement triggerElement = menu.createdTriggerElement;

            INode parent = null;
            List<TriggerElement> parentItems = null;
            var selected = treeViewTriggers.SelectedItem;
            int insertIndex = 0;
            if (selected is TreeViewTriggerElement)
            {
                var selectedTreeItem = (TreeViewTriggerElement)selected;
                var node = (INode)selectedTreeItem.Parent;
                if (node.GetNodeType() == type) // valid parent if 'created' matches 'selected' type
                {
                    var selectedTriggerElement = selectedTreeItem.triggerElement;
                    parent = node;
                    parentItems = parent.GetTriggerElements();
                    insertIndex = parentItems.IndexOf(selectedTriggerElement);
                }
            }
            else if (selected is INode)
            {
                var node = (INode)selected;
                if (node.GetNodeType() == type)
                {
                    parent = node;
                    insertIndex = 0;
                    parentItems = parent.GetTriggerElements();
                }
            }
            if (parent == null)
            {
                if (type == TriggerElementType.Event)
                    parent = categoryEvent;
                else if (type == TriggerElementType.Condition)
                    parent = categoryCondition;
                else if (type == TriggerElementType.Action)
                    parent = categoryAction;

                parentItems = parent.GetTriggerElements();
                insertIndex = parentItems.Count;
            }

            var parentAsItem = (TreeViewItem)parent;
            parentAsItem.IsExpanded = true;

            if (triggerElement != null)
            {
                CommandTriggerElementCreate command = new CommandTriggerElementCreate(triggerElement, parentItems, insertIndex);
                command.Execute();

                TreeViewTriggerElement treeViewTriggerElement = new TreeViewTriggerElement(triggerElement);
                this.treeViewTriggers.Items.Add(treeViewTriggerElement); // hack. This is to not make the below OnCreated method crash.

                triggerElement.Attach(treeViewTriggerElement);
                treeViewTriggerElement.OnCreated(insertIndex);
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
            dragItem = this.treeViewTriggers.SelectedItem as TreeViewItem;

            if (dragItem is NodeEvent || dragItem is NodeCondition || dragItem is NodeAction)
                return;

            DataObject data = null;

            if (dragItem == null)
                return;

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
            if (!_IsDragging || dragItem == null || e.Source is TreeView)
                return;

            var parent = (INode)dragItem.Parent;

            // It is necessary to traverse the item's parents since drag & drop picks up
            // things like 'TextBlock' and 'Border' on the drop target when dropping the 
            // dragged element.
            INode node = GetCategoryTarget(e.Source as FrameworkElement);
            TreeViewItem whereItemWasDropped = GetTraversedTargetDropItem(e.Source as FrameworkElement);
            var item = (TreeViewTriggerElement)this.dragItem;
            int oldIndex = item.triggerElement.Parent.IndexOf(item.triggerElement);
            int insertIndex;
            List<TriggerElement> newParent;
            if (whereItemWasDropped is TreeViewTriggerElement)
            {
                var location = (TreeViewTriggerElement)whereItemWasDropped;
                insertIndex = node.GetTriggerElements().IndexOf(location.triggerElement);
                newParent = location.triggerElement.Parent;
            }
            else
            {
                var location = (INode)whereItemWasDropped;
                insertIndex = 0;
                newParent = location.GetTriggerElements();
            }

            if (whereItemWasDropped is TreeViewTriggerElement)
            {
                bool isSameParent = newParent == item.triggerElement.Parent;
                if (isSameParent && insertIndex == oldIndex)
                    return;

                parent = node;

            }
            else if (whereItemWasDropped is NodeEvent && dragItem.Parent is NodeEvent)
                parent = (INode)whereItemWasDropped;
            else if (whereItemWasDropped is NodeCondition && dragItem.Parent is NodeCondition)
                parent = (INode)whereItemWasDropped;
            else if (whereItemWasDropped is NodeAction && dragItem.Parent is NodeAction)
                parent = (INode)whereItemWasDropped;



            if (whereItemWasDropped != dragItem)
            {
                CommandTriggerElementMove command = new CommandTriggerElementMove(item.triggerElement, parent.GetTriggerElements(), insertIndex);
                command.Execute();
            }
        }

        private TreeViewItem GetTraversedTargetDropItem(FrameworkElement dropTarget)
        {
            if (dropTarget == null || dropTarget is TreeView)
                return null;

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

        /// <summary>
        /// Gets either an 'Event', 'Condition' or 'Action' node
        /// based on the parent of the drop target.
        /// </summary>
        /// <param name="dropTarget"></param>
        /// <returns></returns>
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
            else if (e.Key == Key.X && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                CopyTriggerElement(true);
            else if (e.Key == Key.V && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                PasteTriggerElement();
        }

        public void DeleteTriggerElement()
        {
            List<TriggerElement> elementsToDelete = new List<TriggerElement>();
            for (int i = 0; i < selectedItems.Count; i++)
            {
                elementsToDelete.Add(selectedItems[i].triggerElement);
            }

            if (elementsToDelete.Count == 0)
                return;

            CommandTriggerElementDelete command = new CommandTriggerElementDelete(elementsToDelete);
            command.Execute();
        }

        private void CopyTriggerElement(bool isCut = false)
        {
            ControllerTrigger controller = new ControllerTrigger();
            List<TriggerElement> triggerElements = new List<TriggerElement>();
            for (int i = 0; i < selectedItems.Count; i++)
            {
                triggerElements.Add(selectedItems[i].triggerElement);
            }
            controller.CopyTriggerElements(triggerElements, isCut);

            var selected = (TreeViewItem)treeViewTriggers.SelectedItem;
            ContainerCopiedElementsGUI.copiedElementParent = (INode)selected.Parent;
        }

        private void PasteTriggerElement()
        {
            var selected = (TreeViewItem)treeViewTriggers.SelectedItem;
            INode attachTarget = null;
            int insertIndex = 0;
            if (selected is TreeViewTriggerElement)
            {
                attachTarget = (INode)selected.Parent;
                var parent = (TreeViewItem)selected.Parent;
                insertIndex = parent.Items.IndexOf(selected) + 1;
            }
            else if (selected is INode)
            {
                attachTarget = (INode)selected;
            }

            if (attachTarget.GetType() != ContainerCopiedElementsGUI.copiedElementParent.GetType()) // reject if TriggerElement types don't match. 
                return;


            ControllerTrigger controller = new ControllerTrigger();
            var pasted = controller.PasteTriggerElements(attachTarget.GetTriggerElements(), insertIndex);

            for (int i = 0; i < pasted.Count; i++)
            {
                TreeViewTriggerElement treeViewTriggerElement = new TreeViewTriggerElement(pasted[i]);
                this.treeViewTriggers.Items.Add(treeViewTriggerElement); // hack. This is to not make the below OnCreated method crash.

                pasted[i].Attach(treeViewTriggerElement);
                treeViewTriggerElement.OnCreated(insertIndex + i); // index hack. May want a "GetIndex" method on TriggerElements.
            }
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

        private void treeViewTriggers_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem rightClickedElement = GetTraversedTargetDropItem(e.Source as FrameworkElement);

            if (!(rightClickedElement is TreeViewItem))
                return;

            rightClickedElement.IsSelected = true;
            rightClickedElement.ContextMenu = contextMenu;

            if (rightClickedElement is INode)
            {
                ContextMenuDisableNodeTypes((INode)rightClickedElement);
                menuCut.IsEnabled = false;
                menuCopy.IsEnabled = false;
                menuDelete.IsEnabled = false;
                menuFunctionEnabled.IsEnabled = false;
                menuFunctionEnabled.IsChecked = false;
            }

            else
            {
                ContextMenuDisableNodeTypes((INode)rightClickedElement.Parent);
                menuCut.IsEnabled = true;
                menuCopy.IsEnabled = true;
                menuDelete.IsEnabled = true;
                var treeItemTriggerElement = (TreeViewTriggerElement)rightClickedElement;
                menuFunctionEnabled.IsEnabled = true;
                menuFunctionEnabled.IsChecked = treeItemTriggerElement.triggerElement.isEnabled;
            }
        }

        private void ContextMenuDisableNodeTypes(INode node)
        {
            if (node is NodeEvent)
            {
                menuEvent.IsEnabled = true;
                menuCondition.IsEnabled = false;
                menuAction.IsEnabled = false;
            }
            else if (node is NodeCondition)
            {
                menuEvent.IsEnabled = false;
                menuCondition.IsEnabled = true;
                menuAction.IsEnabled = false;
            }
            else if (node is NodeAction)
            {
                menuEvent.IsEnabled = false;
                menuCondition.IsEnabled = false;
                menuAction.IsEnabled = true;
            }
        }

        private void menuCut_Click(object sender, RoutedEventArgs e)
        {
            CopyTriggerElement(true);
        }

        private void menuCopy_Click(object sender, RoutedEventArgs e)
        {
            CopyTriggerElement();
        }

        private void menuCopyAsText_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void menuPaste_Click(object sender, RoutedEventArgs e)
        {
            PasteTriggerElement();
        }

        private void menuDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteTriggerElement();
        }

        private void menuEvent_Click(object sender, RoutedEventArgs e)
        {
            selectedElementEnd.GetTriggerControl().CreateEvent();
        }

        private void menuCondition_Click(object sender, RoutedEventArgs e)
        {
            selectedElementEnd.GetTriggerControl().CreateCondition();
        }

        private void menuAction_Click(object sender, RoutedEventArgs e)
        {
            selectedElementEnd.GetTriggerControl().CreateAction();
        }

        private void menuFunctionEnabled_Click(object sender, RoutedEventArgs e)
        {
            CommandTriggerElementEnableDisable command = new CommandTriggerElementEnableDisable(selectedElementEnd.triggerElement);
            command.Execute();
        }
    }
}

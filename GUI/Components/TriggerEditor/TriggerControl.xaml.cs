using BetterTriggers;
using BetterTriggers.Commands;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using Cake.Core.Scripting;
using GUI.Components.Shared;
using GUI.Components.TriggerEditor;
using GUI.Components.Return;
using GUI.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using GUI.Components.ParameterEditor;
using BetterTriggers.Models.EditorData.TriggerEditor;

namespace GUI.Components
{
    public partial class TriggerControl : UserControl
    {
        public TriggerControlViewModel ViewModel { get; }

        private TextEditor TextEditor;
        private ExplorerElement explorerElement; // needed to get file references to variables in TriggerElements
        private ExplorerElementEnum explorerElementType;

        private Point _startPoint;
        private TreeViewItem dragItem;
        private bool _IsDragging = false;
        private int insertIndex = 0;
        private TreeViewItem _treeItemParentDropTarget;

        private TriggerElement selectedElement;
        private TriggerElement selectedElementEnd;
        private List<TriggerElement> selectedElements = new();
        private List<TriggerElement> selectedItems = new();

        // attaches to a treeviewitem
        private AdornerLayer adorner;
        private TreeItemAdornerLine lineIndicator;
        private TreeItemAdornerSquare squareIndicator;

        private VariableControl _variableControl;
        private ReturnTypeControl _returnTypeControl;
        private ParameterDefinitionControl _parameterDefinitionControl;

        public TriggerControl(ExplorerElement explorerElement)
        {
            InitializeComponent();

            ViewModel = new TriggerControlViewModel(explorerElement);
            DataContext = ViewModel;

            EditorSettings settings = EditorSettings.Load();
            if (settings.triggerEditorMode == 1)
            {
                bottomControl.Visibility = Visibility.Hidden;
                bottomSplitter.Visibility = Visibility.Hidden;
                Grid.SetRowSpan(treeViewTriggers, 3);
            }

            this.explorerElement = explorerElement;
            explorerElementType = explorerElement.ElementType;

            if (explorerElementType == ExplorerElementEnum.Trigger)
            {
                checkBoxIsEnabled.IsChecked = explorerElement.IsEnabled;
                checkBoxIsInitiallyOn.IsChecked = explorerElement.IsInitiallyOn;
                checkBoxIsCustomScript.IsChecked = explorerElement.trigger.IsScript;
                checkBoxRunOnMapInit.IsChecked = explorerElement.trigger.RunOnMapInit;
                ShowTextEditor(explorerElement.trigger.IsScript);
            }
            else if (explorerElementType == ExplorerElementEnum.ActionDefinition
                    || explorerElementType == ExplorerElementEnum.ConditionDefinition
                    || explorerElementType == ExplorerElementEnum.FunctionDefinition)
            {
                checkBoxIsEnabled.Visibility = Visibility.Hidden;
                checkBoxIsInitiallyOn.Visibility = Visibility.Hidden;
                checkBoxIsCustomScript.Visibility = Visibility.Hidden;
                checkBoxRunOnMapInit.Visibility = Visibility.Hidden;
            }

            treeViewTriggers.SelectedItemChanged += TreeViewTriggers_SelectedItemChanged;
            explorerElement.OnChanged += ExplorerElement_OnChanged;
            explorerElement.OnToggleEnable += ExplorerElement_OnToggleEnable;
            explorerElement.OnToggleInitiallyOn += ExplorerElement_OnToggleInitiallyOn;

            KeyDown += TriggerControl_KeyDown;
        }

        public TriggerElement? GetTriggerElementFromItem(TreeViewItem? item)
        {
            var parent = TreeViewItemHelper.GetTreeItemParent(item);
            var triggerElement = parent.ItemContainerGenerator.ItemFromContainer(item) as TriggerElement;
            return triggerElement;
        }

        public TreeViewItem? GetTreeViewItemFromTriggerElement(TriggerElement triggerElement)
        {
            List<TriggerElement> list = new();
            TreeViewItem? treeViewItem = null;

            // walks up the element hierarchy until a parent attached to the TreeView is found.
            while (treeViewItem == null)
            {
                treeViewItem = treeViewTriggers.ItemContainerGenerator.ContainerFromItem(triggerElement) as TreeViewItem;
                if (treeViewItem == null)
                {
                    list.Add(triggerElement);
                    triggerElement = triggerElement.GetParent();
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

        private void TreeViewTriggers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var triggerElement = treeViewTriggers.SelectedItem as TriggerElement;
            if (triggerElement == null)
            {
                this.selectedItems = SelectItemsMultiple(null, null);
                return;
            }

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                selectedElementEnd = (TriggerElement)treeViewTriggers.SelectedItem;
            else
            {
                selectedElement = triggerElement;
                selectedElementEnd = triggerElement;
            }

            this.selectedItems = SelectItemsMultiple(selectedElement, selectedElementEnd);
        }

        /// <summary>
        /// When mouse hovers over trigger element, determine whether top node is an action.
        /// </summary>
        private void TreeViewItem_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var treeItem = e.Source as TreeViewItem;
            if (treeItem == null)
                return;

            var triggerElement = treeItem.DataContext as TriggerElement;
            IncludeLocalsInParameterMenu(triggerElement);
        }

        private void bottomControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (treeViewTriggers.SelectedItem is TriggerElement triggerElement)
                IncludeLocalsInParameterMenu(triggerElement);
        }

        private void IncludeLocalsInParameterMenu(TriggerElement triggerElement)
        {
            bool isActionOrConditionalInAction = false;
            TriggerElementCollection actions = null;
            switch (explorerElementType)
            {
                case ExplorerElementEnum.Trigger:
                    actions = explorerElement.trigger.Actions;
                    break;
                case ExplorerElementEnum.ActionDefinition:
                    actions = explorerElement.actionDefinition.Actions;
                    break;
                case ExplorerElementEnum.ConditionDefinition:
                    actions = explorerElement.conditionDefinition.Actions;
                    break;
                case ExplorerElementEnum.FunctionDefinition:
                    actions = explorerElement.functionDefinition.Actions;
                    break;
                default:
                    return;
            }

            while (triggerElement != null)
            {
                if (triggerElement == actions)
                {
                    isActionOrConditionalInAction = true;
                    break;
                }

                triggerElement = triggerElement.GetParent();
            }

            Variables.includeLocals = isActionOrConditionalInAction;
        }

        private void Refresh()
        {
            RefreshBottomControls();
        }

        public void CreateEvent()
        {
            if (explorerElementType == ExplorerElementEnum.Trigger)
            {
                CreateTriggerElement(TriggerElementType.Event);
            }
        }

        public void CreateCondition()
        {
            CreateTriggerElement(TriggerElementType.Condition);
        }

        public void CreateAction()
        {
            CreateTriggerElement(TriggerElementType.Action);
        }

        public void CreateLocalVariable()
        {
            CreateTriggerElement(TriggerElementType.LocalVariable);
        }

        public void CreateParameter()
        {
            CreateTriggerElement(TriggerElementType.ParameterDef);
        }

        private void CreateTriggerElement(TriggerElementType type)
        {
            int insertIndex = 0;

            if (type == TriggerElementType.LocalVariable)
            {
                switch (explorerElementType)
                {
                    case ExplorerElementEnum.Trigger:
                        insertIndex = explorerElement.trigger.LocalVariables.Count();
                        break;
                    case ExplorerElementEnum.ActionDefinition:
                        insertIndex = explorerElement.actionDefinition.LocalVariables.Count();
                        break;
                    case ExplorerElementEnum.ConditionDefinition:
                        insertIndex = explorerElement.conditionDefinition.LocalVariables.Count();
                        break;
                    case ExplorerElementEnum.FunctionDefinition:
                        insertIndex = explorerElement.functionDefinition.LocalVariables.Count();
                        break;
                    default:
                        break;
                }

                Project.CurrentProject.Variables.CreateLocalVariable(explorerElement, insertIndex);
                return;
            }
            else if (type == TriggerElementType.ParameterDef)
            {
                ParameterDefinitionCollection parameterDefCollection = null;
                switch (explorerElementType)
                {
                    case ExplorerElementEnum.ActionDefinition:
                        insertIndex = explorerElement.actionDefinition.LocalVariables.Count();
                        parameterDefCollection = explorerElement.actionDefinition.Parameters;
                        break;
                    case ExplorerElementEnum.ConditionDefinition:
                        insertIndex = explorerElement.conditionDefinition.LocalVariables.Count();
                        parameterDefCollection = explorerElement.conditionDefinition.Parameters;
                        break;
                    case ExplorerElementEnum.FunctionDefinition:
                        insertIndex = explorerElement.functionDefinition.LocalVariables.Count();
                        parameterDefCollection = explorerElement.functionDefinition.Parameters;
                        break;
                    default:
                        break;
                }

                parameterDefCollection.CreateParameterDefinition(explorerElement);
                return;
            }

            var menu = new TriggerElementMenuWindow(explorerElement, type);
            menu.ShowDialog();
            ECA eca = menu.createdTriggerElement;

            TriggerElement parent = null;
            var selected = treeViewTriggers.SelectedItem as TriggerElement;
            if (selected == null)
            {
                switch (type)
                {
                    case TriggerElementType.Event:
                        parent = explorerElement.trigger.Events;
                        break;
                    case TriggerElementType.Condition:
                        parent = explorerElement.trigger.Conditions;
                        break;
                    case TriggerElementType.Action:
                        parent = explorerElement.trigger.Actions;
                        break;
                }
            }
            if (selected is ECA)
            {
                var selectedTreeItem = (ECA)selected;
                var node = selectedTreeItem.GetParent();
                if (node.ElementType == type) // valid parent if 'created' matches 'selected' type
                {
                    parent = node;
                    insertIndex = parent.IndexOf(selected) + 1;
                }
            }
            else if (selected is TriggerElementCollection node)
            {
                if (node.ElementType == type)
                {
                    parent = node;
                    insertIndex = 0;
                }
            }
            if (parent == null)
            {
                switch (type)
                {
                    case TriggerElementType.Event:
                        parent = explorerElement.trigger.Events;
                        break;
                    case TriggerElementType.Condition:
                        parent = explorerElement.trigger.Conditions;
                        break;
                    case TriggerElementType.Action:
                        parent = explorerElement.trigger.Actions;
                        break;
                }

                insertIndex = parent.Count();
            }

            var parentAsItem = GetTreeViewItemFromTriggerElement(parent);
            parentAsItem.IsExpanded = true;

            if (eca != null)
            {
                CommandTriggerElementCreate command = new CommandTriggerElementCreate(explorerElement, eca, parent, insertIndex);
                command.Execute();
                eca.IsSelected = true;
            }
        }

        private void RefreshBottomControls()
        {
            EditorSettings settings = EditorSettings.Load();
            if (settings.triggerEditorMode == 1)
            {
                bottomControl.Visibility = Visibility.Hidden;
                bottomSplitter.Visibility = Visibility.Hidden;
                Grid.SetRowSpan(treeViewTriggers, 3);
            }
            else
            {
                bottomControl.Visibility = Visibility.Visible;
                bottomSplitter.Visibility = Visibility.Visible;
                Grid.SetRowSpan(treeViewTriggers, 1);
            }

            var triggerElement = treeViewTriggers.SelectedItem as TriggerElement;
            textblockParams.Inlines.Clear();
            textblockDescription.Text = string.Empty;

            if (grid.Children.Contains(_variableControl))
            {
                _variableControl.OnChange -= OnChange;
                _variableControl.Dispose();
                grid.Children.Remove(_variableControl);
            }
            if (grid.Children.Contains(_returnTypeControl))
            {
                grid.Children.Remove(_returnTypeControl);
            }
            if (grid.Children.Contains(_parameterDefinitionControl))
            {
                _parameterDefinitionControl.OnChanged -= OnChange;
                grid.Children.Remove(_parameterDefinitionControl);
            }

            if (triggerElement == null)
                return;

            if (triggerElement is ECA eca)
            {
                ParamTextBuilder controllerTriggerTreeItem = new ParamTextBuilder();
                var inlines = controllerTriggerTreeItem.GenerateParamText(explorerElement, eca);
                textblockParams.Inlines.AddRange(inlines);
                textblockDescription.Text = Locale.Translate(eca.function.value);
                eca.DisplayText = controllerTriggerTreeItem.GenerateTreeItemText(explorerElement, eca);
            }
            else if (triggerElement is LocalVariable localVar)
            {
                _variableControl = new VariableControl(localVar.variable);
                _variableControl.OnChange += OnChange;
                grid.Children.Add(_variableControl);
                Grid.SetRow(_variableControl, 3);
                Grid.SetRowSpan(_variableControl, 2);
            }
            else if (triggerElement is ReturnType returnType)
            {
                _returnTypeControl = new(explorerElement.functionDefinition, returnType);
                grid.Children.Add(_returnTypeControl);
                Grid.SetRow(_returnTypeControl, 3);
                Grid.SetRowSpan(_returnTypeControl, 2);
            }
            else if (triggerElement is ParameterDefinition paramDef)
            {
                _parameterDefinitionControl = new ParameterDefinitionControl(paramDef);
                _parameterDefinitionControl.OnChanged += OnChange;
                grid.Children.Add(_parameterDefinitionControl);
                Grid.SetRow(_parameterDefinitionControl, 3);
                Grid.SetRowSpan(_parameterDefinitionControl, 2);
            }
        }

        private void OnChange()
        {
            explorerElement.AddToUnsaved();
        }

        // TODO: There are two 'SelectedItemChanged' functions?
        private void treeViewTriggers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RefreshBottomControls();
        }

        private void treeViewItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (
                e.LeftButton == MouseButtonState.Pressed
                && !_IsDragging
                && !contextMenu.IsVisible
                && e.Source is not HyperlinkParameterTrigger // Needed because of weird WPF bug with unclickable links on treeitems when selected.
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
            _IsDragging = true;
            var triggerElement = treeViewTriggers.SelectedItem as TriggerElement;
            if (triggerElement is InvalidECA)
            {
                return;
            }

            var treeItem = GetTreeViewItemFromTriggerElement(treeViewTriggers.SelectedItem as TriggerElement);
            dragItem = treeItem;

            if (dragItem == null || triggerElement.IsRenaming)
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
                DragDropEffects de = DragDrop.DoDragDrop(this.treeViewTriggers, data, dde);
            }
            _IsDragging = false;
        }


        /// <summary>
        /// Event happens when trigger element is dragged.
        /// All rules for dragging and inserting happens here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewItem_DragOver(object sender, DragEventArgs e)
        {
            if (dragItem == null)
                return;

            if (!dragItem.IsKeyboardFocused)
                return;

            var dragItemTriggerElement = GetTriggerElementFromItem(dragItem);
            if (dragItemTriggerElement is TriggerElementCollection)
                return;

            var currentParent = TreeViewItemHelper.GetTreeItemParent(dragItem);
            TreeViewItem dropTarget = TreeViewItemHelper.GetTraversedTargetDropItem(e.Source as DependencyObject);
            int currentIndex = currentParent.Items.IndexOf(dragItemTriggerElement);
            if (dropTarget == null)
                return;

            if (lineIndicator != null)
                adorner.Remove(lineIndicator);
            if (squareIndicator != null)
                adorner.Remove(squareIndicator);

            if (dropTarget == dragItem)
            {
                _treeItemParentDropTarget = null;
                e.Handled = true;
                return;
            }

            if (UIUtility.IsCircularParent(dragItem, dropTarget))
                return;

            var triggerElementDragItem = GetTriggerElementFromItem(dragItem);
            var triggerElementDropTarget = GetTriggerElementFromItem(dropTarget);
            var parentOfDragItem = triggerElementDragItem.GetParent();
            var parentOfDropTarget = triggerElementDropTarget.GetParent();

            if (triggerElementDropTarget is not TriggerElementCollection && parentOfDragItem.ElementType != parentOfDropTarget.ElementType)
            {
                _treeItemParentDropTarget = null;
                return;
            }
            if (dragItemTriggerElement.ElementType != triggerElementDropTarget.ElementType)
            {
                _treeItemParentDropTarget = null;
                return;
            }

            if (triggerElementDropTarget is ECA || triggerElementDropTarget is LocalVariable)
            {
                var relativePos = e.GetPosition(dropTarget);
                bool inFirstHalf = UIUtility.IsMouseInFirstHalf(dropTarget, relativePos, Orientation.Vertical);
                if (inFirstHalf)
                {
                    adorner = AdornerLayer.GetAdornerLayer(dropTarget);
                    lineIndicator = new TreeItemAdornerLine(dropTarget, true);
                    adorner.Add(lineIndicator);

                    var parentDropTarget = triggerElementDropTarget.GetParent();
                    insertIndex = parentDropTarget.IndexOf(triggerElementDropTarget);
                }
                else
                {
                    adorner = AdornerLayer.GetAdornerLayer(dropTarget);
                    lineIndicator = new TreeItemAdornerLine(dropTarget, false);
                    adorner.Add(lineIndicator);

                    var parentDropTarget = triggerElementDropTarget.GetParent();
                    insertIndex = parentDropTarget.IndexOf(triggerElementDropTarget) + 1;
                    var treeItemParentDropTarget = GetTreeViewItemFromTriggerElement(parentDropTarget);
                    _treeItemParentDropTarget = treeItemParentDropTarget;
                }

                // We detach the item before inserting, so the index goes one down.
                if (dropTarget.Parent == dragItem.Parent && insertIndex > currentIndex)
                    insertIndex--;
            }
            else if (triggerElementDropTarget is TriggerElementCollection)
            {
                adorner = AdornerLayer.GetAdornerLayer(dropTarget);
                squareIndicator = new TreeItemAdornerSquare(dropTarget);
                adorner.Add(squareIndicator);

                _treeItemParentDropTarget = dropTarget;
                insertIndex = 0;
            }

            e.Handled = true;
        }


        private void treeViewTriggers_Drop(object sender, DragEventArgs e)
        {
            if (!_IsDragging || dragItem == null)
                return;

            if (!dragItem.IsKeyboardFocused)
                return;

            if (adorner != null)
            {
                if (lineIndicator != null)
                    adorner.Remove(lineIndicator);
                if (squareIndicator != null)
                    adorner.Remove(squareIndicator);
            }

            if (_treeItemParentDropTarget == null)
                return;

            if (UIUtility.IsCircularParent(dragItem, _treeItemParentDropTarget))
                return;

            var triggerElement = GetTriggerElementFromItem(dragItem);
            var parent = GetTriggerElementFromItem(_treeItemParentDropTarget) as TriggerElementCollection;

            /* Fix for jumpy trigger elements.
             * When creating a new trigger element on double-click
             * and then holding and dragging after the dialog menu closed,
             * the element could jump to the last 'parentDropTarget',
             * which could be an invalid trigger element location. */
            _treeItemParentDropTarget = null;

            CommandTriggerElementMove command = new CommandTriggerElementMove(explorerElement, triggerElement, parent, insertIndex);
            command.Execute();
        }

        /// <summary>
        /// Returns a list of selected elements in the editor.
        /// Returns only the first selected element if their 'Parents' don't match.
        /// </summary>
        /// <param name="startElement"></param>
        /// <param name="endElement"></param>
        /// <returns></returns>
        public List<TriggerElement> SelectItemsMultiple(TriggerElement startElement, TriggerElement endElement)
        {
            // deselect old items
            for (int i = 0; i < selectedElements.Count; i++)
            {
                selectedElements[i].IsSelected_Multi = false;
            }
            if (startElement == null && endElement == null)
                return null;

            // Prepare selection
            selectedElements = new List<TriggerElement>();
            var startTreeItem = GetTreeViewItemFromTriggerElement(startElement);
            var endTreeItem = GetTreeViewItemFromTriggerElement(endElement);
            var startParent = TreeViewItemHelper.GetTreeItemParent(startTreeItem);
            var endParent = TreeViewItemHelper.GetTreeItemParent(endTreeItem);
            if (startParent == endParent)
            {
                var parent = startParent;

                // in case selected goes from bottom to top.
                TriggerElement correctedStartElement;
                TriggerElement correctedEndElement;
                if (parent.Items.IndexOf(startElement) < parent.Items.IndexOf(endElement))
                {
                    correctedStartElement = startElement;
                    correctedEndElement = endElement;
                }
                else
                {
                    correctedStartElement = endElement;
                    correctedEndElement = startElement;
                }

                int startIndex = parent.Items.IndexOf(correctedStartElement);
                int size = parent.Items.IndexOf(correctedEndElement) - parent.Items.IndexOf(correctedStartElement);
                for (int i = 0; i <= size; i++)
                {
                    selectedElements.Add((TriggerElement)parent.Items[startIndex + i]);
                }
            }
            else
                selectedElements.Add(endElement);

            // select elements
            for (int i = 0; i < selectedElements.Count; i++)
            {
                selectedElements[i].IsSelected_Multi = true;
            }

            return selectedElements;
        }

        private void treeViewItem_IsExpanded(object sender, RoutedEventArgs e)
        {
            var treeItem = sender as TreeViewItem;
            bool isDisconnected = VisualTreeHelper.GetParent(treeItem) == null;
            if (isDisconnected)
                return;

            var triggerElement = GetTriggerElementFromItem(sender as TreeViewItem);
            triggerElement.IsExpanded = true;
        }

        private void treeViewItem_IsCollapsed(object sender, RoutedEventArgs e)
        {
            var treeItem = sender as TreeViewItem;
            bool isDisconnected = VisualTreeHelper.GetParent(treeItem) == null;
            if (isDisconnected)
                return;

            var triggerElement = GetTriggerElementFromItem(sender as TreeViewItem);
            triggerElement.IsExpanded = false;
        }

        /*
        /// <summary>
        /// Custom arrow key navigation. WPF's built-in TreeView navigation is slow and buggy once treeitems get complex headers.
        /// 
        /// TODO: REFACTOR - Is this even needed now when the items are generated from a collection? I'm not sure.
        /// Have to test performance once we get everything working.
        /// </summary>
        private void treeViewTriggers_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && treeViewTriggers.SelectedItem != null)
            {
                var current = (TreeViewItem)treeViewTriggers.SelectedItem;
                if (current.Items.Count > 0 && current.IsExpanded)
                {
                    var item = (TreeViewItem)current.Items[0];
                    item.IsSelected = true;
                    e.Handled = true;
                }
                else
                {
                    if (current.Parent is TreeViewItem parent)
                    {
                        int curIndex = parent.Items.IndexOf(current);
                        int newIndex = curIndex + 1;
                        if (newIndex != parent.Items.Count)
                        {
                            var newItem = (TreeViewItem)parent.Items[newIndex];
                            newItem.IsSelected = true;
                            e.Handled = true;
                        }
                        else
                        {
                            //TODO: Moving out of the end of an item collection causes lag
                        }
                    }
                }
            }

            else if (e.Key == Key.Up && treeViewTriggers.SelectedItem != null)
            {
                var current = (TreeViewItem)treeViewTriggers.SelectedItem;
                if (current.Parent is TreeViewItem parent)
                {
                    int curIndex = parent.Items.IndexOf(current);
                    int newIndex = curIndex - 1;
                    if (newIndex < 0)
                    {
                        parent.IsSelected = true;
                        e.Handled = true;
                    }
                    else
                    {
                        var newItem = (TreeViewItem)parent.Items[newIndex];
                        var tmp = newItem;
                        if (tmp.HasItems)
                        {
                            bool didSelect = false;
                            while (!didSelect)
                            {
                                tmp = (TreeViewItem)tmp.Items[tmp.Items.Count - 1];
                                if (!tmp.HasItems || !tmp.IsExpanded)
                                {
                                    didSelect = true;
                                    newItem = tmp;
                                }
                            }
                        }

                        newItem.IsSelected = true;
                        e.Handled = true;
                    }
                }
            }
        }
        */

        private void TriggerControl_KeyDown(object sender, KeyEventArgs e)
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
            var selectedElement = treeViewTriggers.SelectedItem as TriggerElement;
            if (selectedElement == null || selectedElement is TriggerElementCollection || selectedElement is ParameterDefinitionCollection)
                return;

            TriggerElementCollection elementsToDelete = new(selectedElement.ElementType);
            for (int i = 0; i < selectedItems.Count; i++)
            {
                var triggerElement = selectedItems[i];
                if (triggerElement != null)
                    elementsToDelete.Elements.Add(triggerElement);
            }

            if (elementsToDelete.Count() == 0)
                return;

            // local variables
            List<LocalVariable> localsInUse = new List<LocalVariable>();
            List<ParameterDefinition> paramDefsInUse = new List<ParameterDefinition>();
            elementsToDelete.Elements.ForEach(el =>
            {
                var localVar = el as LocalVariable;
                var paramDef = el as ParameterDefinition;
                if (localVar != null)
                {
                    List<ExplorerElement> refs = Project.CurrentProject.References.GetReferrers(localVar.variable);
                    if (refs.Count > 0)
                        localsInUse.Add(localVar);
                }
                else if (paramDef != null)
                {
                    List<ExplorerElement> refs = Project.CurrentProject.References.GetReferrers(paramDef);
                    if (refs.Count > 0)
                        paramDefsInUse.Add(paramDef);
                }
            });
            if (localsInUse.Count > 0)
            {
                InUseWindow window = new(localsInUse);
                window.ShowDialog();
                if (!window.OK)
                    return;

                localsInUse.ForEach(v => Project.CurrentProject.Variables.RemoveLocalVariable(v));
                TriggerValidator validator = new TriggerValidator(explorerElement);
                validator.RemoveInvalidReferences();
            }
            else if (paramDefsInUse.Count > 0)
            {
                InUseWindow window = new(paramDefsInUse);
                window.ShowDialog();
                if (!window.OK)
                    return;

            }

            TriggerElement ToSelectAfterDeletion = null;
            TriggerElement BottomSelected = elementsToDelete.Elements.Last();
            var parent = elementsToDelete.Elements[0].GetParent();
            if (elementsToDelete.Elements.Count() == parent.Elements.Count)
            {
                // All elements in the parent are deleted, we select the parent.
                ToSelectAfterDeletion = parent;
            }
            else if (parent.Elements.IndexOf(BottomSelected) < parent.Elements.Count - 1)
            {
                // Selects the element right below the bottom selected one.
                int index = parent.Elements.IndexOf(BottomSelected) + 1;
                ToSelectAfterDeletion = parent.Elements[index];
            }
            else if (parent.Elements.IndexOf(BottomSelected) == parent.Elements.Count - 1)
            {
                // Selects the element coming before all the selected ones.
                TriggerElement topSelected = elementsToDelete.Elements.First();
                int index = parent.Elements.IndexOf(topSelected) - 1;
                ToSelectAfterDeletion = parent.Elements[index];
            }

            CommandTriggerElementDelete command = new CommandTriggerElementDelete(explorerElement, elementsToDelete);
            command.Execute();

            if (ToSelectAfterDeletion != null)
            {
                ToSelectAfterDeletion.IsSelected = true;
            }
        }

        private void CopyTriggerElement(bool isCut = false)
        {
            var selected = (TriggerElement)treeViewTriggers.SelectedItem;
            if (selected == null)
                return;

            TriggerElementCollection triggerElements = new(selected.ElementType);
            for (int i = 0; i < selectedItems.Count; i++)
            {
                var triggerElement = selectedItems[i];
                if (triggerElement != null)
                    triggerElements.Elements.Add(triggerElement);
            }
            Project.CurrentProject.CopyTriggerElements(explorerElement, triggerElements, isCut);
        }

        private void PasteTriggerElement()
        {
            var selected = (TriggerElement)treeViewTriggers.SelectedItem;
            if (selected == null)
                return;

            TriggerElement? attachTarget = null;
            int insertIndex = 0;
            if (selectedElement is TriggerElementCollection collection)
            {
                attachTarget = collection;
            }
            else
            {
                attachTarget = selectedElement.GetParent();
                insertIndex = selected.GetParent().IndexOf(selected) + 1;
            }

            if (attachTarget == null)
                return;
            if (attachTarget.ElementType != CopiedElements.CopiedTriggerElements.ElementType) // reject if TriggerElement types don't match. 
                return;

            var pasted = Project.CurrentProject.PasteTriggerElements(explorerElement, attachTarget, insertIndex);
        }

        private void checkBoxIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            var trigger = explorerElement.trigger;
            explorerElement.IsEnabled = (bool)checkBoxIsEnabled.IsChecked;
        }

        private void checkBoxIsInitiallyOn_Click(object sender, RoutedEventArgs e)
        {
            explorerElement.IsInitiallyOn = (bool)checkBoxIsInitiallyOn.IsChecked;
        }

        private void checkBoxRunOnMapInit_Click(object sender, RoutedEventArgs e)
        {
            explorerElement.trigger.RunOnMapInit = (bool)checkBoxRunOnMapInit.IsChecked;
        }

        private void checkBoxIsCustomScript_Click(object sender, RoutedEventArgs e)
        {
            bool isScript = (bool)checkBoxIsCustomScript.IsChecked;
            if (!isScript)
            {
                Dialogs.DialogBox dialog = new Dialogs.DialogBox("Confirmation", "All changes made in the custom script will be lost when you switch. This cannot be undone.\n\nDo you wish to continue?");
                dialog.ShowDialog();
                if (!dialog.OK)
                {
                    explorerElement.trigger.Script = "";
                    checkBoxIsCustomScript.IsChecked = true;
                    e.Handled = true;
                    return;
                }
            }

            ShowTextEditor(isScript);
            explorerElement.AddToUnsaved();
        }

        private void ShowTextEditor(bool doShow)
        {
            explorerElement.trigger.IsScript = doShow;
            if (doShow)
            {
                if (!explorerElement.trigger.IsScript)
                {
                    // Only generates a new script when the user has clicked the 'Custom Script' checkbox.
                    ScriptGenerator scriptGenerator = new ScriptGenerator(Info.GetLanguage());
                    string script = scriptGenerator.ConvertGUIToJass(explorerElement, new List<string>());
                    explorerElement.trigger.Script = script;
                }

                TextEditor = new TextEditor(explorerElement.trigger.Script, Info.GetLanguage());
                TextEditor.avalonEditor.Text = explorerElement.trigger.Script;
                TextEditor.avalonEditor.TextChanged += delegate
                {
                    explorerElement.trigger.Script = TextEditor.avalonEditor.Text;
                    explorerElement.AddToUnsaved();
                };

                if (!grid.Children.Contains(TextEditor))
                    grid.Children.Add(TextEditor);
                Grid.SetRow(TextEditor, 2);
                Grid.SetRowSpan(TextEditor, 3);
                checkBoxList.Items.Remove(checkBoxIsInitiallyOn);
                checkBoxList.Items.Remove(checkBoxRunOnMapInit);
                checkBoxList.Items.Add(checkBoxRunOnMapInit);

                explorerElement.trigger.RunOnMapInit = false;
                for (int i = 0; i < explorerElement.trigger.Events.Count(); i++)
                {
                    var _event = (ECA)explorerElement.trigger.Events.Elements[i];
                    if (_event.function.value == "MapInitializationEvent")
                    {
                        explorerElement.trigger.RunOnMapInit = true;
                        checkBoxRunOnMapInit.IsChecked = true;
                        break;
                    }
                }
            }
            else
            {
                if (grid.Children.Contains(TextEditor))
                    grid.Children.Remove(TextEditor);

                checkBoxList.Items.Remove(checkBoxIsCustomScript);
                checkBoxList.Items.Remove(checkBoxIsInitiallyOn);
                checkBoxList.Items.Remove(checkBoxRunOnMapInit);
                checkBoxList.Items.Add(checkBoxIsInitiallyOn);
                checkBoxList.Items.Add(checkBoxIsCustomScript);
            }
        }

        private void treeViewTriggers_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // It is necessary to traverse the item's parents since drag & drop picks up
            // things like 'TextBlock' and 'Border' on the drop target when dropping the 
            // dragged element.
            TreeViewItem rightClickedElement = TreeViewItemHelper.GetTraversedTargetDropItem(e.Source as FrameworkElement);

            if (!(rightClickedElement is TreeViewItem))
                return;

            rightClickedElement.IsSelected = true;
            var triggerElement = GetTriggerElementFromItem(rightClickedElement);

            if (triggerElement is TriggerElementCollection collection)
            {
                ContextMenuDisableNodeTypes(collection);
                menuCut.IsEnabled = false;
                menuCopy.IsEnabled = false;
                menuDelete.IsEnabled = false;
                menuFunctionEnabled.IsEnabled = false;
                menuFunctionEnabled.IsChecked = false;
            }
            else
            {
                ContextMenuDisableNodeTypes(triggerElement.GetParent());
                menuCut.IsEnabled = true;
                menuCopy.IsEnabled = true;
                menuDelete.IsEnabled = true;
                menuFunctionEnabled.IsEnabled = true;
                if (triggerElement is ECA eca)
                {
                    menuFunctionEnabled.IsChecked = eca.IsEnabled;
                }
            }

            contextMenu.IsOpen = true;

            e.Handled = true;
        }

        private void ContextMenuDisableNodeTypes(TriggerElement node)
        {
            if (node.ElementType == TriggerElementType.Event)
            {
                menuEvent.IsEnabled = true;
                menuCondition.IsEnabled = false;
                menuAction.IsEnabled = false;
            }
            else if (node.ElementType == TriggerElementType.Action)
            {
                menuEvent.IsEnabled = false;
                menuCondition.IsEnabled = true;
                menuAction.IsEnabled = false;
            }
            else if (node.ElementType == TriggerElementType.Action)
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

        private string indent = string.Empty;
        private void menuCopyAsText_Click(object sender, RoutedEventArgs e)
        {
            string text = string.Empty;
            indent = string.Empty;
            //text += RecurseCopyText(categoryEvent);
            //text += RecurseCopyText(categoryCondition);
            //text += RecurseCopyText(categoryAction);

            Clipboard.SetText(text);
        }

        //private string RecurseCopyText(TreeItemBT item)
        //{
        //    string output = string.Empty;
        //    output += $"{indent}{item.GetHeaderText()}\n";

        //    if (item.Items.Count > 0)
        //    {
        //        indent += "  ";
        //        foreach (var child in item.Items)
        //        {
        //            output += RecurseCopyText((TreeItemBT)child);
        //        }
        //        indent = indent.Substring(0, indent.Length - 2);
        //    }

        //    return output;
        //}

        private void menuPaste_Click(object sender, RoutedEventArgs e)
        {
            PasteTriggerElement();
        }

        private void menuDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteTriggerElement();
        }

        private void menuRename_Click(object sender, RoutedEventArgs e)
        {
            var triggerElement = selectedElementEnd;
            if (selectedElementEnd == null || triggerElement is not LocalVariable)
                return;

            ShowRenameBox();
        }

        private void menuEvent_Click(object sender, RoutedEventArgs e)
        {
            CreateEvent();
        }

        private void menuLocalVar_Click(object sender, RoutedEventArgs e)
        {
            CreateLocalVariable();
        }

        private void menuCondition_Click(object sender, RoutedEventArgs e)
        {
            CreateCondition();
        }

        private void menuAction_Click(object sender, RoutedEventArgs e)
        {
            CreateAction();
        }

        private void menuFunctionEnabled_Click(object sender, RoutedEventArgs e)
        {
            if (selectedElementEnd == null)
                return;

            var eca = selectedElementEnd as ECA;
            CommandTriggerElementEnableDisable command = new CommandTriggerElementEnableDisable(explorerElement, eca);
            command.Execute();
        }

        private void textBoxComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            switch (explorerElementType)
            {
                case ExplorerElementEnum.Trigger:
                    explorerElement.trigger.Comment = textBoxComment.Text;
                    break;
                case ExplorerElementEnum.ActionDefinition:
                    explorerElement.actionDefinition.Comment = textBoxComment.Text;
                    break;
                case ExplorerElementEnum.ConditionDefinition:
                    explorerElement.conditionDefinition.Comment = textBoxComment.Text;
                    break;
                case ExplorerElementEnum.FunctionDefinition:
                    explorerElement.functionDefinition.Comment = textBoxComment.Text;
                    break;
                default:
                    break;
            }
            explorerElement.AddToUnsaved();
        }

        private void treeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var treeItem = sender as TreeViewItem;
            if (treeItem == null)
                return;

            if (treeViewTriggers.SelectedItem == treeItem.DataContext) // prevents the parent TreeItems from running the same logic
            {
                ReplaceTriggerElement(selectedElementEnd);
                e.Handled = true;
            }
        }

        private void treeViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            var selected = treeViewTriggers.SelectedItem as TriggerElement;

            if (e.Key == Key.Enter && selected.IsRenaming)
            {
                if (selected is LocalVariable localVar)
                {
                    var variables = Project.CurrentProject.Variables;
                    try
                    {
                        variables.RenameLocalVariable(explorerElement, localVar, localVar.RenameText);
                    }
                    catch (Exception ex)
                    {
                        Dialogs.MessageBox dialog = new Dialogs.MessageBox("Rename local variable", ex.Message);
                        dialog.ShowDialog();
                    }
                    selected.RenameBoxVisibility = Visibility.Hidden;
                }
                else if (selected is ParameterDefinition parameterDef)
                {
                    var parent = parameterDef.GetParent() as ParameterDefinitionCollection;
                    try
                    {
                        parent.RenameParameterDefinition(explorerElement, parameterDef);
                    }
                    catch (Exception ex)
                    {
                        Dialogs.MessageBox dialog = new Dialogs.MessageBox("Rename local variable", ex.Message);
                        dialog.ShowDialog();
                    }
                    selected.RenameBoxVisibility = Visibility.Hidden;
                }
            }
            else if (e.Key == Key.Enter)
            {
                ReplaceTriggerElement(selectedElementEnd);
                e.Handled = true;
            }
            else if (e.Key == Key.F2)
            {
                ShowRenameBox();
            }
            else if (e.Key == Key.Escape)
            {
                selectedElementEnd.CancelRename();
            }
        }

        private void ReplaceTriggerElement(TriggerElement toReplace)
        {
            var eca = toReplace as ECA;
            if (eca == null)
                return;


            TriggerElementType elementType;
            if (eca.ElementType == TriggerElementType.Event)
                elementType = TriggerElementType.Event;
            else if (eca.ElementType == TriggerElementType.Condition)
                elementType = TriggerElementType.Condition;
            else if (eca.ElementType == TriggerElementType.Action)
                elementType = TriggerElementType.Action;
            else
                return;

            TriggerElementMenuWindow window = new TriggerElementMenuWindow(explorerElement, elementType, eca);
            window.ShowDialog();
            ECA selected = window.createdTriggerElement;

            if (selected == null || selected.function.value == eca.function.value)
                return;

            CommandTriggerElementReplace command = new CommandTriggerElementReplace(explorerElement, eca, selected);
            command.Execute();
        }

        /// <summary>
        /// Prevents horizontal scroll when selecting a long TreeViewItem.
        /// </summary>
        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            // Cancel the current scroll attempt
            e.Handled = true;
        }

        private void ShowRenameBox()
        {
            if(selectedElementEnd == null) return;

            if (selectedElementEnd is LocalVariable || selectedElementEnd is ParameterDefinition)
            {
                selectedElementEnd.RenameBoxVisibility = Visibility.Visible;
                var treeItem = GetTreeViewItemFromTriggerElement(selectedElementEnd);
                var textBox = TreeViewItemHelper.FindChild<TextBox>(treeItem, "renameBox");
                textBox.Focus();
                textBox.SelectAll();
            }
        }

        private void treeViewTriggers_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            bool selectionIsNull = selectedElementEnd == null;
            menuCut.IsEnabled = !selectionIsNull;
            menuCopy.IsEnabled = !selectionIsNull;
            menuPaste.IsEnabled = !selectionIsNull;
            menuDelete.IsEnabled = !selectionIsNull;
            menuRename.IsEnabled = !selectionIsNull;
            menuFunctionEnabled.IsEnabled = !selectionIsNull;
            if (selectionIsNull)
                return;

            var triggerElement = selectedElementEnd;

            bool isECA = triggerElement is ECA;
            bool isRenameEnabled = triggerElement is LocalVariable || triggerElement is ParameterDefinition;
            menuFunctionEnabled.IsEnabled = isECA;
            menuFunctionEnabled.IsChecked = triggerElement.IsEnabled;
            menuRename.IsEnabled = isRenameEnabled;
        }

        private void ExplorerElement_OnChanged()
        {
            RefreshBottomControls();
        }

        private void ExplorerElement_OnToggleEnable()
        {
            checkBoxIsEnabled.IsChecked = explorerElement.IsEnabled;
        }

        private void ExplorerElement_OnToggleInitiallyOn()
        {
            checkBoxIsInitiallyOn.IsChecked = explorerElement.IsInitiallyOn;
        }


        private void treeViewTriggers_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedElementEnd == null)
                return;

            if (selectedElementEnd.IsRenaming)
            {
                selectedElementEnd.CancelRename();
            }
        }

    }
}

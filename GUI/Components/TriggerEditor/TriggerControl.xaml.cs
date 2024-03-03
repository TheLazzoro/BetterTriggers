using BetterTriggers;
using BetterTriggers.Commands;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.WorldEdit;
using Cake.Core.Scripting;
using GUI.Components.Shared;
using GUI.Components.TriggerEditor;
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

namespace GUI.Components
{
    public partial class TriggerControl : UserControl
    {
        public TriggerControlViewModel ViewModel { get; }

        public TextEditor TextEditor;
        public ExplorerElement explorerElementTrigger; // needed to get file references to variables in TriggerElements
        public static BetterTriggers.Models.EditorData.Trigger TriggerInFocus;

        Point _startPoint;
        TreeViewItem dragItem;
        bool _IsDragging = false;
        int insertIndex = 0;
        TreeViewItem treeItemParentDropTarget;

        private TriggerElement selectedElement;
        private TriggerElement selectedElementEnd;
        private List<TriggerElement> selectedElements = new ();
        private List<TriggerElement> selectedItems = new ();

        // attaches to a treeviewitem
        AdornerLayer adorner;
        TreeItemAdornerLine lineIndicator;
        TreeItemAdornerSquare squareIndicator;

        VariableControl variableControl;

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

            this.explorerElementTrigger = explorerElement;


            checkBoxIsEnabled.IsChecked = explorerElement.GetEnabled();
            checkBoxIsInitiallyOn.IsChecked = explorerElement.GetInitiallyOn();
            checkBoxIsCustomScript.IsChecked = explorerElement.trigger.IsScript;
            checkBoxRunOnMapInit.IsChecked = explorerElement.trigger.RunOnMapInit;
            ShowTextEditor(explorerElement.trigger.IsScript);

            treeViewTriggers.SelectedItemChanged += TreeViewTriggers_SelectedItemChanged;
            explorerElement.OnChanged += ExplorerElement_OnChanged;

            // TODO: REFACTOR
            //TreeViewItem.OnMouseEnter += TreeViewItem_OnMouseEnter;
        }

        internal void Dispose()
        {
            // TODO: REFACTOR
            // TreeViewItem.OnMouseEnter -= TreeViewItem_OnMouseEnter;
        }

        public TriggerElement? GetTriggerElementFromItem(TreeViewItem? item)
        {
            var parent = GetTreeItemParent(item);
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
                if(treeViewItem == null)
                {
                    list.Add(triggerElement);
                    triggerElement = triggerElement.GetParent();
                }
            }

            // walks down the tree until the TreeViewItem has been pulled out.
            for (int i = list.Count-1; i >= 0; i--)
            {
                var element = list[i];
                treeViewItem = treeViewItem.ItemContainerGenerator.ContainerFromItem(element) as TreeViewItem;
            }
            
            return treeViewItem;
        }

        private ItemsControl GetTreeItemParent(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as ItemsControl;
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
        private void TreeViewItem_OnMouseEnter(TreeViewItem obj)
        {
            Variables.includeLocals = IsActionOrConditionalInAction(obj);
        }

        private void bottomControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (treeViewTriggers.SelectedItem is TreeViewItem treeItem)
                Variables.includeLocals = IsActionOrConditionalInAction(treeItem);
        }

        private bool IsActionOrConditionalInAction(TreeViewItem item)
        {
            var triggerElement = GetTriggerElementFromItem(item);
            bool isActionOrConditionalInAction = false;
            while (!isActionOrConditionalInAction && item != null)
            {
                if (triggerElement != null && triggerElement.ElementType == TriggerElementType.Action)
                    isActionOrConditionalInAction = true;
                else
                    item = item.Parent as TreeViewItem;
            }

            return isActionOrConditionalInAction;
        }

        private void Refresh()
        {
            RefreshBottomControls();
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

        public void CreateLocalVariable()
        {
            CreateTriggerElement(TriggerElementType.LocalVariable);
        }

        private void CreateTriggerElement(TriggerElementType type)
        {
            int insertIndex = 0;

            if (type == TriggerElementType.LocalVariable)
            {
                insertIndex = explorerElementTrigger.trigger.LocalVariables.Count();
                var trigger = explorerElementTrigger.trigger;
                Project.CurrentProject.Variables.CreateLocalVariable(trigger, insertIndex);
                return;
            }

            var menu = new TriggerElementMenuWindow(explorerElementTrigger, type);
            menu.ShowDialog();
            ECA eca = menu.createdTriggerElement;

            TriggerElement parent = null;
            var selected = treeViewTriggers.SelectedItem as TriggerElement;
            if (selected == null)
            {
                switch (type)
                {
                    case TriggerElementType.Event:
                        parent = explorerElementTrigger.trigger.Events;
                        break;
                    case TriggerElementType.Condition:
                        parent = explorerElementTrigger.trigger.Conditions;
                        break;
                    case TriggerElementType.Action:
                        parent = explorerElementTrigger.trigger.Actions;
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
                    insertIndex = parent.IndexOf(selected);
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
                        parent = explorerElementTrigger.trigger.Events;
                        break;
                    case TriggerElementType.Condition:
                        parent = explorerElementTrigger.trigger.Conditions;
                        break;
                    case TriggerElementType.Action:
                        parent = explorerElementTrigger.trigger.Actions;
                        break;
                }

                insertIndex = parent.Count();
            }

            var parentAsItem = GetTreeViewItemFromTriggerElement(parent);
            parentAsItem.IsExpanded = true;

            if (eca != null)
            {
                CommandTriggerElementCreate command = new CommandTriggerElementCreate(eca, parent, insertIndex);
                command.Execute();
            }
        }

        public void RefreshBottomControls()
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

            if (grid.Children.Contains(variableControl))
            {
                variableControl.OnChange -= VariableControl_OnChange;
                variableControl.Dispose();
                grid.Children.Remove(variableControl);
            }

            if (triggerElement == null)
                return;

            if (triggerElement is ECA eca)
            {
                ParamTextBuilder controllerTriggerTreeItem = new ParamTextBuilder();
                var inlines = controllerTriggerTreeItem.GenerateParamText(explorerElementTrigger, eca);
                textblockParams.Inlines.AddRange(inlines);
                textblockDescription.Text = Locale.Translate(eca.function.value);
                eca.DisplayText = controllerTriggerTreeItem.GenerateTreeItemText(eca);
            }
            else if (triggerElement is LocalVariable localVar)
            {
                variableControl = new VariableControl(localVar.variable);
                variableControl.OnChange += VariableControl_OnChange;
                grid.Children.Add(variableControl);
                Grid.SetRow(variableControl, 3);
                Grid.SetRowSpan(variableControl, 2);
            }
        }

        private void VariableControl_OnChange()
        {
            explorerElementTrigger.AddToUnsaved();
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

            var currentParent = GetTreeItemParent(dragItem);
            TreeViewItem dropTarget = GetTraversedTargetDropItem(e.Source as DependencyObject);
            int currentIndex = currentParent.Items.IndexOf(dragItemTriggerElement);
            if (dropTarget == null)
                return;

            if (lineIndicator != null)
                adorner.Remove(lineIndicator);
            if (squareIndicator != null)
                adorner.Remove(squareIndicator);

            if (dropTarget == dragItem)
            {
                treeItemParentDropTarget = null;
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
                treeItemParentDropTarget = null;
                return;
            }
            if (dragItemTriggerElement.ElementType != triggerElementDropTarget.ElementType)
            {
                treeItemParentDropTarget = null;
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

                treeItemParentDropTarget = dropTarget;
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

            if (treeItemParentDropTarget == null)
                return;

            if (UIUtility.IsCircularParent(dragItem, treeItemParentDropTarget))
                return;

            var triggerElement = GetTriggerElementFromItem(dragItem);
            var parent = GetTriggerElementFromItem(treeItemParentDropTarget) as TriggerElementCollection;

            /* Fix for jumpy trigger elements.
             * When creating a new trigger element on double-click
             * and then holding and dragging after the dialog menu closed,
             * the element could jump to the last 'parentDropTarget',
             * which could be an invalid trigger element location. */
            treeItemParentDropTarget = null;

            CommandTriggerElementMove command = new CommandTriggerElementMove(explorerElementTrigger.trigger, triggerElement, parent, insertIndex);
            command.Execute();
        }


        private TreeViewItem GetTraversedTargetDropItem(DependencyObject dropTarget)
        {
            if (dropTarget == null || dropTarget is TreeView)
                return null;

            TreeViewItem traversedTarget = null;
            while (traversedTarget == null)
            {
                if (dropTarget is not TextElement)
                    dropTarget = VisualTreeHelper.GetParent(dropTarget);
                else
                {
                    var d = (TextElement)dropTarget;
                    dropTarget = d.Parent;
                }


                if (dropTarget is TreeViewItem)
                {
                    traversedTarget = (TreeViewItem)dropTarget;
                }

                if (dropTarget == null)
                    return null;
            }

            return traversedTarget;
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
                selectedElements[i].IsSelected = false;
            }
            if (startElement == null && endElement == null)
                return null;

            // Prepare selection
            selectedElements = new List<TriggerElement>();
            var startTreeItem = GetTreeViewItemFromTriggerElement(startElement);
            var endTreeItem = GetTreeViewItemFromTriggerElement(endElement);
            var startParent = GetTreeItemParent(startTreeItem);
            var endParent = GetTreeItemParent(endTreeItem);
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
                selectedElements[i].IsSelected = true;
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

        /// <summary>
        /// Custom arrow key navigation. WPF's built-in TreeView navigation is slow and buggy once treeitems get complex headers.
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

        private TriggerElement? GetSelectedTriggerElement()
        {
            var selected = (TreeViewItem)treeViewTriggers.SelectedItem;
            TriggerElement? selectedElement = GetTriggerElementFromItem(selected);
            return selectedElement;
        }

        public void DeleteTriggerElement()
        {
            var selectedElement = GetSelectedTriggerElement();
            if (selectedElement == null)
                return;

            TriggerElementCollection elementsToDelete = new (selectedElement.ElementType);
            for (int i = 0; i < selectedItems.Count; i++)
            {
                var triggerElement = selectedItems[i];
                if (triggerElement != null)
                    elementsToDelete.Elements.Add(triggerElement);
            }

            if (elementsToDelete.Count() == 0)
                return;

            // local variables
            List<LocalVariable> inUse = new List<LocalVariable>();
            elementsToDelete.Elements.ForEach(v =>
            {
                var localVar = v as LocalVariable;
                if (localVar != null)
                {
                    List<ExplorerElement> refs = Project.CurrentProject.References.GetReferrers(localVar.variable);
                    if (refs.Count > 0)
                        inUse.Add(localVar);
                }

            });
            if (inUse.Count > 0)
            {
                LocalsInUseWindow window = new LocalsInUseWindow(inUse);
                window.ShowDialog();
                if (!window.OK)
                    return;

                inUse.ForEach(v => Project.CurrentProject.Variables.RemoveLocalVariable(v));
                Project.CurrentProject.Triggers.RemoveInvalidReferences(explorerElementTrigger);
            }

            CommandTriggerElementDelete command = new CommandTriggerElementDelete(explorerElementTrigger, elementsToDelete);
            command.Execute();
        }

        private void CopyTriggerElement(bool isCut = false)
        {
            var selected = (TreeViewItem)treeViewTriggers.SelectedItem;
            if (selected == null)
                return;

            var selectedElement = GetTriggerElementFromItem(selected);
            if (selectedElement == null)
                return;

            TriggerElementCollection triggerElements = new(selectedElement.ElementType);
            for (int i = 0; i < selectedItems.Count; i++)
            {
                var triggerElement = selectedItems[i];
                if (triggerElement != null)
                    triggerElements.Elements.Add(triggerElement);
            }
            Project.CurrentProject.Triggers.CopyTriggerElements(explorerElementTrigger, triggerElements, isCut);
        }

        private void PasteTriggerElement()
        {
            var selected = (TreeViewItem)treeViewTriggers.SelectedItem;
            if (selected == null)
                return;

            TriggerElement? attachTarget = null;
            int insertIndex = 0;
            var selectedElement = GetTriggerElementFromItem(selected);
            if (selectedElement is TriggerElementCollection collection)
            {
                attachTarget = collection;
            }
            else if (selected is TreeViewItem parent)
            {
                attachTarget = selectedElement.GetParent();
                insertIndex = parent.Items.IndexOf(selected) + 1;
            }

            if (attachTarget == null)
                return;
            if (attachTarget.ElementType != CopiedElements.CopiedTriggerElements.ElementType) // reject if TriggerElement types don't match. 
                return;

            var pasted = Project.CurrentProject.Triggers.PasteTriggerElements(explorerElementTrigger, attachTarget, insertIndex);
        }

        public void SetElementEnabled(bool isEnabled)
        {
            checkBoxIsEnabled.IsChecked = isEnabled;
            explorerElementTrigger.SetEnabled((bool)checkBoxIsEnabled.IsChecked);
            //OnStateChange();
        }

        public void SetElementInitiallyOn(bool isInitiallyOn)
        {
            checkBoxIsInitiallyOn.IsChecked = isInitiallyOn;
            explorerElementTrigger.SetEnabled((bool)checkBoxIsInitiallyOn.IsChecked);
            //OnStateChange();
        }

        private void checkBoxIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            SetElementEnabled((bool)checkBoxIsEnabled.IsChecked);
        }

        private void checkBoxIsInitiallyOn_Click(object sender, RoutedEventArgs e)
        {
            SetElementInitiallyOn((bool)checkBoxIsInitiallyOn.IsChecked);
        }

        private void checkBoxRunOnMapInit_Click(object sender, RoutedEventArgs e)
        {
            explorerElementTrigger.trigger.RunOnMapInit = (bool)checkBoxRunOnMapInit.IsChecked;
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
                    explorerElementTrigger.trigger.Script = "";
                    checkBoxIsCustomScript.IsChecked = true;
                    e.Handled = true;
                    return;
                }
            }

            ShowTextEditor(isScript);
            //OnStateChange();
        }

        private void ShowTextEditor(bool doShow)
        {
            if (doShow)
            {
                ScriptGenerator scriptGenerator = new ScriptGenerator(Info.GetLanguage());
                string script = scriptGenerator.ConvertGUIToJass(explorerElementTrigger, new List<string>());
                explorerElementTrigger.trigger.Script = script;
                explorerElementTrigger.trigger.IsScript = doShow;

                TextEditor = new TextEditor(explorerElementTrigger.trigger.Script, Info.GetLanguage());
                TextEditor.avalonEditor.Text = script;
                TextEditor.avalonEditor.TextChanged += delegate
                {
                    explorerElementTrigger.trigger.Script = TextEditor.avalonEditor.Text;
                    //OnStateChange();
                };

                if (!grid.Children.Contains(TextEditor))
                    grid.Children.Add(TextEditor);
                Grid.SetRow(TextEditor, 2);
                Grid.SetRowSpan(TextEditor, 3);
                checkBoxList.Items.Remove(checkBoxIsInitiallyOn);
                checkBoxList.Items.Remove(checkBoxRunOnMapInit);
                checkBoxList.Items.Add(checkBoxRunOnMapInit);

                explorerElementTrigger.trigger.RunOnMapInit = false;
                for (int i = 0; i < explorerElementTrigger.trigger.Events.Count(); i++)
                {
                    var _event = (ECA)explorerElementTrigger.trigger.Events.Elements[i];
                    if (_event.function.value == "MapInitializationEvent")
                    {
                        explorerElementTrigger.trigger.RunOnMapInit = true;
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
            TreeViewItem rightClickedElement = GetTraversedTargetDropItem(e.Source as FrameworkElement);

            if (!(rightClickedElement is TreeViewItem))
                return;

            rightClickedElement.IsSelected = true;
            rightClickedElement.ContextMenu = contextMenu;
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
                    menuFunctionEnabled.IsChecked = eca.isEnabled;
                }
            }
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

            triggerElement.IsRenaming = true;
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
            CommandTriggerElementEnableDisable command = new CommandTriggerElementEnableDisable(eca);
            command.Execute();
        }

        private void textBoxComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            explorerElementTrigger.trigger.Comment = textBoxComment.Text;
            //OnStateChange();
        }

        private void treeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ReplaceTriggerElement(selectedElementEnd);
        }

        private void treeViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ReplaceTriggerElement(selectedElementEnd);
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

            TriggerElementMenuWindow window = new TriggerElementMenuWindow(explorerElementTrigger, elementType, eca);
            window.ShowDialog();
            ECA selected = window.createdTriggerElement;

            if (selected == null || selected.function.value == eca.function.value)
                return;

            CommandTriggerElementReplace command = new CommandTriggerElementReplace(eca, selected);
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

        public void OnRemoteChange()
        {
            Refresh();
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
            bool isLocalVar = triggerElement is LocalVariable;
            menuFunctionEnabled.IsEnabled = isECA;
            menuRename.IsEnabled = isLocalVar;
        }

        private void ExplorerElement_OnChanged()
        {
            RefreshBottomControls();
        }
    }
}

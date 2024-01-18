using BetterTriggers;
using BetterTriggers.Commands;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using Cake.Core.Scripting;
using GUI.Components.Shared;
using GUI.Components.TriggerEditor;
using GUI.Container;
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
    public partial class TriggerControl : UserControl, IEditor
    {
        public TextEditor TextEditor;
        public ExplorerElementTrigger explorerElementTrigger; // needed to get file references to variables in TriggerElements

        public NodeEvent categoryEvent;
        public NodeCondition categoryCondition;
        public NodeLocalVariable categoryLocalVariable;
        public NodeAction categoryAction;

        public static BetterTriggers.Models.SaveableData.Trigger TriggerInFocus;

        Point _startPoint;
        TreeViewTriggerElement dragItem;
        bool _IsDragging = false;
        int insertIndex = 0;
        TreeViewItem parentDropTarget;

        private List<TreeItemExplorerElement> observers = new List<TreeItemExplorerElement>();
        private TreeViewTriggerElement selectedElement;
        private TreeViewTriggerElement selectedElementEnd;
        private List<TreeViewTriggerElement> selectedElements = new List<TreeViewTriggerElement>();
        private List<TreeViewTriggerElement> selectedItems = new List<TreeViewTriggerElement>();

        // attaches to a treeviewitem
        AdornerLayer adorner;
        TreeItemAdornerLine lineIndicator;
        TreeItemAdornerSquare squareIndicator;

        VariableControl variableControl;

        public TriggerControl(ExplorerElementTrigger explorerElementTrigger)
        {
            InitializeComponent();

            EditorSettings settings = EditorSettings.Load();
            if (settings.triggerEditorMode == 1)
            {
                bottomControl.Visibility = Visibility.Hidden;
                bottomSplitter.Visibility = Visibility.Hidden;
                Grid.SetRowSpan(treeViewTriggers, 3);
            }

            this.explorerElementTrigger = explorerElementTrigger;
            

            checkBoxIsEnabled.IsChecked = explorerElementTrigger.GetEnabled();
            checkBoxIsInitiallyOn.IsChecked = explorerElementTrigger.GetInitiallyOn();
            checkBoxIsCustomScript.IsChecked = explorerElementTrigger.trigger.IsScript;
            checkBoxRunOnMapInit.IsChecked = explorerElementTrigger.trigger.RunOnMapInit;
            ShowTextEditor(explorerElementTrigger.trigger.IsScript);

            treeViewTriggers.SelectedItemChanged += TreeViewTriggers_SelectedItemChanged;

            categoryEvent = new NodeEvent("Events");
            categoryCondition = new NodeCondition("Conditions");
            categoryLocalVariable = new NodeLocalVariable("Local Variables");
            categoryAction = new NodeAction("Actions");

            treeViewTriggers.Items.Add(categoryEvent);
            treeViewTriggers.Items.Add(categoryCondition);
            treeViewTriggers.Items.Add(categoryLocalVariable);
            treeViewTriggers.Items.Add(categoryAction);

            LoadTrigger(explorerElementTrigger.trigger);

            TreeViewTriggerElement.OnMouseEnter += TreeViewTriggerElement_OnMouseEnter;
        }

        internal void Dispose()
        {
            TreeViewTriggerElement.OnMouseEnter -= TreeViewTriggerElement_OnMouseEnter;
        }

        private void TreeViewTriggers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeViewTriggers.SelectedItem is INode)
            {
                this.selectedItems = SelectItemsMultiple(null, null);
                return;
            }

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                selectedElementEnd = (TreeViewTriggerElement)treeViewTriggers.SelectedItem;
            else
            {
                selectedElement = (TreeViewTriggerElement)treeViewTriggers.SelectedItem;
                selectedElementEnd = (TreeViewTriggerElement)treeViewTriggers.SelectedItem;
            }

            this.selectedItems = SelectItemsMultiple(selectedElement, selectedElementEnd);
        }

        /// <summary>
        /// When mouse hovers over trigger element, determine whether top node is an action.
        /// </summary>
        private void TreeViewTriggerElement_OnMouseEnter(TreeViewTriggerElement obj)
        {
            Variables.includeLocals = IsActionOrConditionalInAction(obj);
        }

        private void bottomControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (treeViewTriggers.SelectedItem is TreeViewTriggerElement treeItem)
                Variables.includeLocals = IsActionOrConditionalInAction(treeItem);
        }

        private bool IsActionOrConditionalInAction(TreeViewItem item)
        {
            bool isActionOrConditionalInAction = false;
            while (!isActionOrConditionalInAction && item != null)
            {
                if (item == categoryAction)
                    isActionOrConditionalInAction = true;
                else
                    item = item.Parent as TreeViewItem;
            }

            return isActionOrConditionalInAction;
        }

        private void Refresh()
        {
            Refresh(categoryEvent);
            Refresh(categoryCondition);
            Refresh(categoryLocalVariable);
            Refresh(categoryAction);
            RefreshBottomControls();
        }

        private void Refresh(TreeViewItem treeViewItem)
        {
            for (int i = 0; i < treeViewItem.Items.Count; i++)
            {
                var child = (TreeViewItem)treeViewItem.Items[i];
                if (child is TreeViewTriggerElement)
                {
                    var item = treeViewItem.Items[i] as TreeViewTriggerElement;
                    item.UpdateTreeItem();
                }

                if (child.Items.Count > 0)
                    Refresh(child);
            }
        }

        private void LoadTrigger(BetterTriggers.Models.SaveableData.Trigger trigger)
        {
            this.textBoxComment.Text = trigger.Comment;
            TreeViewTriggerElement.RecurseLoadTrigger(trigger.Events, this.categoryEvent);
            TreeViewTriggerElement.RecurseLoadTrigger(trigger.Conditions, this.categoryCondition);
            TreeViewTriggerElement.RecurseLoadTrigger(trigger.LocalVariables, this.categoryLocalVariable);
            TreeViewTriggerElement.RecurseLoadTrigger(trigger.Actions, this.categoryAction);

            this.categoryEvent.IsExpanded = true;
            this.categoryCondition.IsExpanded = true;
            this.categoryLocalVariable.IsExpanded = true;
            this.categoryAction.IsExpanded = true;
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
                insertIndex = categoryLocalVariable.GetTriggerElements().Count;
                LocalVariable localVariable = new LocalVariable(explorerElementTrigger.trigger); // TODO: This is probably not supposed to be here.
                Project.CurrentProject.Variables.CreateLocalVariable(explorerElementTrigger.trigger, localVariable, categoryLocalVariable.GetTriggerElements(), insertIndex);
                TreeViewTriggerElement treeViewTriggerElement = new TreeViewTriggerElement(localVariable);
                this.treeViewTriggers.Items.Add(treeViewTriggerElement); // hack. This is to not make the below OnCreated method crash.

                localVariable.Attach(treeViewTriggerElement);
                treeViewTriggerElement.OnCreated(insertIndex);
                return;
            }

            var menu = new TriggerElementMenuWindow(type);
            menu.ShowDialog();
            ECA triggerElement = menu.createdTriggerElement;

            INode parent = null;
            List<TriggerElement> parentItems = null;
            var selected = treeViewTriggers.SelectedItem;
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

            var item = treeViewTriggers.SelectedItem as TreeViewTriggerElement;
            textblockParams.Inlines.Clear();
            textblockDescription.Text = string.Empty;

            if (grid.Children.Contains(variableControl))
            {
                variableControl.OnChange -= VariableControl_OnChange;
                variableControl.Dispose();
                grid.Children.Remove(variableControl);
            }

            if (item == null)
                return;

            if (item.triggerElement is ECA)
            {
                ParamTextBuilder controllerTriggerTreeItem = new ParamTextBuilder();
                var inlines = controllerTriggerTreeItem.GenerateParamText(item);
                var element = (ECA)item.triggerElement;
                textblockParams.Inlines.AddRange(inlines);
                textblockDescription.Text = Locale.Translate(element.function.value);
            }
            else if (item.triggerElement is LocalVariable)
            {
                var element = (LocalVariable)item.triggerElement;
                variableControl = new VariableControl(element.variable);
                variableControl.OnChange += VariableControl_OnChange;
                grid.Children.Add(variableControl);
                Grid.SetRow(variableControl, 3);
                Grid.SetRowSpan(variableControl, 2);
            }
        }

        private void VariableControl_OnChange()
        {
            OnStateChange();
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
            dragItem = this.treeViewTriggers.SelectedItem as TreeViewTriggerElement;

            if (dragItem == null || dragItem.IsRenaming())
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

            TreeViewItem currentParent = (TreeViewItem)dragItem.Parent;
            TreeViewItem dropTarget = GetTraversedTargetDropItem(e.Source as DependencyObject);
            int currentIndex = currentParent.Items.IndexOf(dragItem);
            if (dropTarget == null)
                return;

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

            if (dropTarget is TreeViewTriggerElement)
            {
                if (
                    dragItem.Parent is NodeEvent && !(dropTarget.Parent is NodeEvent) ||
                    dragItem.Parent is NodeCondition && !(dropTarget.Parent is NodeCondition) ||
                    dragItem.Parent is NodeAction && !(dropTarget.Parent is NodeAction) ||
                    dragItem.Parent is NodeLocalVariable && !(dropTarget.Parent is NodeLocalVariable)
                    )
                {
                    parentDropTarget = null;
                    return;
                }


                var relativePos = e.GetPosition(dropTarget);
                bool inFirstHalf = UIUtility.IsMouseInFirstHalf(dropTarget, relativePos, Orientation.Vertical);
                if (inFirstHalf)
                {
                    adorner = AdornerLayer.GetAdornerLayer(dropTarget);
                    lineIndicator = new TreeItemAdornerLine(dropTarget, true);
                    adorner.Add(lineIndicator);

                    parentDropTarget = (TreeViewItem)dropTarget.Parent;
                    insertIndex = parentDropTarget.Items.IndexOf(dropTarget);
                }
                else
                {
                    adorner = AdornerLayer.GetAdornerLayer(dropTarget);
                    lineIndicator = new TreeItemAdornerLine(dropTarget, false);
                    adorner.Add(lineIndicator);

                    parentDropTarget = (TreeViewItem)dropTarget.Parent;
                    insertIndex = parentDropTarget.Items.IndexOf(dropTarget) + 1;
                }

                // We detach the item before inserting, so the index goes one down.
                if (dropTarget.Parent == dragItem.Parent && insertIndex > currentIndex)
                    insertIndex--;
            }
            else if (dropTarget is INode)
            {
                if (dragItem.Parent is NodeEvent && !(dropTarget is NodeEvent))
                    return;
                else if (dragItem.Parent is NodeCondition && !(dropTarget is NodeCondition))
                    return;
                else if (dragItem.Parent is NodeAction && !(dropTarget is NodeAction))
                    return;
                else if (dragItem.Parent is NodeLocalVariable && !(dropTarget is NodeLocalVariable))
                    return;

                adorner = AdornerLayer.GetAdornerLayer(dropTarget);
                squareIndicator = new TreeItemAdornerSquare(dropTarget);
                adorner.Add(squareIndicator);

                parentDropTarget = dropTarget;
                insertIndex = 0;
            }
            else
            {
                parentDropTarget = null;
            }
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

            if (parentDropTarget == null)
                return;

            if (UIUtility.IsCircularParent(dragItem, parentDropTarget))
                return;

            TreeViewTriggerElement item = (TreeViewTriggerElement)dragItem;
            INode targetParentGUI = (INode)parentDropTarget;

            /* Fix for jumpy trigger elements.
             * When creating a new trigger element on double-click
             * and then holding and dragging after the dialog menu closed,
             * the element could jump to the last 'parentDropTarget',
             * which could be an invalid trigger element location. */
            parentDropTarget = null;

            CommandTriggerElementMove command = new CommandTriggerElementMove(explorerElementTrigger.trigger, item.triggerElement, targetParentGUI.GetTriggerElements(), insertIndex);
            command.Execute();
        }


        private TreeViewItem GetTraversedTargetDropItem(DependencyObject dropTarget)
        {
            if (dropTarget == null || dropTarget is TreeView)
                return null;

            TreeViewItem traversedTarget = null;
            while (traversedTarget == null)
            {
                if(dropTarget is not TextElement)
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
        public List<TreeViewTriggerElement> SelectItemsMultiple(TreeViewTriggerElement startElement, TreeViewTriggerElement endElement)
        {
            // visually deselect old items
            for (int i = 0; i < selectedElements.Count; i++)
            {
                selectedElements[i].Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }
            if (startElement == null && endElement == null)
                return null;

            selectedElements = new List<TreeViewTriggerElement>();
            if (startElement.Parent == endElement.Parent)
            {
                var parent = (TreeViewItem)startElement.Parent;

                TreeViewTriggerElement correctedStartElement;
                TreeViewTriggerElement correctedEndElement;
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
                    selectedElements.Add((TreeViewTriggerElement)parent.Items[startIndex + i]);
                }
            }
            else
                selectedElements.Add(endElement);

            // visually select elements
            for (int i = 0; i < selectedElements.Count; i++)
            {
                selectedElements[i].Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#aa357EC7");
            }

            return selectedElements;
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

        public void DeleteTriggerElement()
        {
            List<TriggerElement> elementsToDelete = new List<TriggerElement>();
            for (int i = 0; i < selectedItems.Count; i++)
            {
                elementsToDelete.Add(selectedItems[i].triggerElement);
            }

            if (elementsToDelete.Count == 0)
                return;

            // local variables
            List<LocalVariable> inUse = new List<LocalVariable>();
            elementsToDelete.ForEach(v =>
            {
                var localVar = v as LocalVariable;
                if (localVar != null)
                {
                    List<ExplorerElementTrigger> refs = Project.CurrentProject.References.GetReferrers(localVar.variable);
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

            List<TriggerElement> triggerElements = new List<TriggerElement>();
            for (int i = 0; i < selectedItems.Count; i++)
            {
                triggerElements.Add(selectedItems[i].triggerElement);
            }
            Project.CurrentProject.Triggers.CopyTriggerElements(explorerElementTrigger, triggerElements, isCut);

            ContainerCopiedElementsGUI.copiedElementParent = (INode)selected.Parent;
        }

        private void PasteTriggerElement()
        {
            var selected = (TreeViewItem)treeViewTriggers.SelectedItem;
            if (selected == null || ContainerCopiedElementsGUI.copiedElementParent == null)
                return;

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

            var pasted = Project.CurrentProject.Triggers.PasteTriggerElements(explorerElementTrigger, attachTarget.GetTriggerElements(), insertIndex);

            for (int i = 0; i < pasted.Count; i++)
            {
                TreeViewTriggerElement treeViewTriggerElement = new TreeViewTriggerElement(pasted[i]);
                this.treeViewTriggers.Items.Add(treeViewTriggerElement); // hack. This is to not make the below OnCreated method crash.

                pasted[i].Attach(treeViewTriggerElement);
                treeViewTriggerElement.OnCreated(pasted[i].GetParent().IndexOf(pasted[i]));
            }
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
            explorerElementTrigger.SetEnabled((bool)checkBoxIsEnabled.IsChecked);
            OnStateChange();
        }

        public void SetElementInitiallyOn(bool isInitiallyOn)
        {
            checkBoxIsInitiallyOn.IsChecked = isInitiallyOn;
            explorerElementTrigger.SetEnabled((bool)checkBoxIsInitiallyOn.IsChecked);
            OnStateChange();
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
            OnStateChange();
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
                    OnStateChange();
                };

                if (!grid.Children.Contains(TextEditor))
                    grid.Children.Add(TextEditor);
                Grid.SetRow(TextEditor, 2);
                Grid.SetRowSpan(TextEditor, 3);
                checkBoxList.Items.Remove(checkBoxIsInitiallyOn);
                checkBoxList.Items.Remove(checkBoxRunOnMapInit);
                checkBoxList.Items.Add(checkBoxRunOnMapInit);

                explorerElementTrigger.trigger.RunOnMapInit = false;
                for (int i = 0; i < explorerElementTrigger.trigger.Events.Count; i++)
                {
                    var _event = (ECA)explorerElementTrigger.trigger.Events[i];
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
                if (treeItemTriggerElement.triggerElement is ECA)
                {
                    var element = (ECA)treeItemTriggerElement.triggerElement;
                    menuFunctionEnabled.IsChecked = element.isEnabled;
                }
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

        private string indent = string.Empty;
        private void menuCopyAsText_Click(object sender, RoutedEventArgs e)
        {
            string text = string.Empty;
            indent = string.Empty;
            text += RecurseCopyText(categoryEvent);
            text += RecurseCopyText(categoryCondition);
            text += RecurseCopyText(categoryAction);

            Clipboard.SetText(text);
        }

        private string RecurseCopyText(TreeItemBT item)
        {
            string output = string.Empty;
            output += $"{indent}{item.GetHeaderText()}\n";

            if (item.Items.Count > 0)
            {
                indent += "  ";
                foreach (var child in item.Items)
                {
                    output += RecurseCopyText((TreeItemBT)child);
                }
                indent = indent.Substring(0, indent.Length - 2);
            }

            return output;
        }

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
            if (selectedElementEnd == null || selectedElementEnd.triggerElement is not LocalVariable)
                return;

            selectedElementEnd.ShowRenameBox();
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

            CommandTriggerElementEnableDisable command = new CommandTriggerElementEnableDisable((ECA)selectedElementEnd.triggerElement);
            command.Execute();
        }

        private void textBoxComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            explorerElementTrigger.trigger.Comment = textBoxComment.Text;
            OnStateChange();
        }

        private void treeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewTriggerElement doubleClicked = selectedElementEnd;
            ReplaceTriggerElement(doubleClicked);
        }

        private void treeViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ReplaceTriggerElement(selectedElementEnd);
        }

        private void ReplaceTriggerElement(TreeViewTriggerElement toReplace)
        {
            if (toReplace == null)
                return;

            TriggerElementType elementType;
            if (toReplace.Parent is NodeEvent)
                elementType = TriggerElementType.Event;
            else if (toReplace.Parent is NodeCondition)
                elementType = TriggerElementType.Condition;
            else if (toReplace.Parent is NodeAction)
                elementType = TriggerElementType.Action;
            else
                return;

            var triggerElement = (ECA)toReplace.triggerElement;

            TriggerElementMenuWindow window = new TriggerElementMenuWindow(elementType, (ECA)toReplace.triggerElement);
            window.ShowDialog();
            ECA selected = window.createdTriggerElement;

            if (selected == null || selected.function.value == triggerElement.function.value)
                return;

            TreeViewItem parent = toReplace.Parent as TreeViewItem;
            int index = parent.Items.IndexOf(toReplace);
            CommandTriggerElementReplace command = new CommandTriggerElementReplace(triggerElement, selected);
            command.Execute();

            TreeViewTriggerElement treeViewTriggerElement = new TreeViewTriggerElement(selected);
            this.treeViewTriggers.Items.Add(treeViewTriggerElement); // hack. This is to not make the below OnCreated method crash.

            selected.Attach(treeViewTriggerElement);
            treeViewTriggerElement.OnCreated(index);
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

            bool isECA = selectedElementEnd.triggerElement is ECA;
            bool isLocalVar = selectedElementEnd.triggerElement is LocalVariable;
            menuFunctionEnabled.IsEnabled = isECA;
            menuRename.IsEnabled = isLocalVar;
        }

    }
}

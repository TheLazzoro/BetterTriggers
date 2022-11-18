using BetterTriggers;
using BetterTriggers.Commands;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using GUI.Components.Shared;
using GUI.Components.TriggerEditor;
using GUI.Components.TriggerExplorer;
using GUI.Container;
using GUI.Controllers;
using GUI.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;


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

        Point _startPoint;
        TreeViewItem dragItem;
        bool _IsDragging = false;
        int insertIndex = 0;
        TreeViewItem parentDropTarget;

        private List<TreeItemExplorerElement> observers = new List<TreeItemExplorerElement>();
        private ControllerTriggerControl controllerTriggerControl = new ControllerTriggerControl();
        private TreeViewTriggerElement selectedElement;
        private TreeViewTriggerElement selectedElementEnd;
        private List<TreeViewTriggerElement> selectedItems = new List<TreeViewTriggerElement>();

        // attaches to a treeviewitem
        AdornerLayer adorner;
        TreeItemAdornerLine lineIndicator;
        TreeItemAdornerSquare squareIndicator;

        VariableControl variableControl;

        public TriggerControl(ExplorerElementTrigger explorerElementTrigger)
        {
            InitializeComponent();

            this.explorerElementTrigger = explorerElementTrigger;
            TextEditor = new TextEditor(explorerElementTrigger.trigger.Script, Info.GetLanguage());
            TextEditor.avalonEditor.TextChanged += delegate
            {
                explorerElementTrigger.trigger.Script = TextEditor.avalonEditor.Text;
                OnStateChange();
            };

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

        private void Refresh()
        {
            Refresh(categoryEvent);
            Refresh(categoryCondition);
            Refresh(categoryLocalVariable);
            Refresh(categoryAction);
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
            ControllerTriggerControl controller = new ControllerTriggerControl();
            this.textBoxComment.Text = trigger.Comment;
            controller.RecurseLoadTrigger(trigger.Events, this.categoryEvent);
            controller.RecurseLoadTrigger(trigger.Conditions, this.categoryCondition);
            controller.RecurseLoadTrigger(trigger.LocalVariables, this.categoryLocalVariable);
            controller.RecurseLoadTrigger(trigger.Actions, this.categoryAction);

            this.categoryEvent.ExpandSubtree();
            this.categoryCondition.ExpandSubtree();
            this.categoryLocalVariable.ExpandSubtree();
            this.categoryAction.ExpandSubtree();
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
                // TODO:
                insertIndex = categoryLocalVariable.GetTriggerElements().Count;
                LocalVariable localVariable = new LocalVariable();
                CommandTriggerElementCreate command = new CommandTriggerElementCreate(localVariable,categoryLocalVariable.GetTriggerElements(), insertIndex );
                command.Execute();

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

        public void RefreshParameterBlock()
        {
            var item = treeViewTriggers.SelectedItem as TreeViewTriggerElement;
            textblockParams.Inlines.Clear();
            textblockDescription.Text = string.Empty;
            if (item == null)
                return;

            ControllerParamText controllerTriggerTreeItem = new ControllerParamText();
            var inlines = controllerTriggerTreeItem.GenerateParamText(item);

            if (item.triggerElement is ECA)
            {
                var element = (ECA)item.triggerElement;
                if (!grid.Children.Contains(textblockParams))
                {
                    grid.Children.Add(textblockParams);
                    grid.Children.Add(textblockDescription);
                }
                textblockParams.Inlines.AddRange(inlines);
                textblockDescription.Text = Locale.Translate(element.function.value);
            }
            else if (item.triggerElement is LocalVariable)
            {
                var element = (LocalVariable)item.triggerElement;
                grid.Children.Remove(textblockParams);
                grid.Children.Remove(textblockDescription);
                variableControl = new VariableControl(element.variable);
            }
        }

        // TODO: There are two 'SelectedItemChanged' functions?
        private void treeViewTriggers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RefreshParameterBlock();
        }

        private void treeViewItem_PreviewMouseMove(object sender, MouseEventArgs e)
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
            {
                _IsDragging = false;
                return;
            }

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
            TreeViewItem dropTarget = GetTraversedTargetDropItem(e.Source as FrameworkElement);
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
                if (dragItem.Parent is NodeEvent && !(dropTarget.Parent is NodeEvent))
                    return;
                else if (dragItem.Parent is NodeCondition && !(dropTarget.Parent is NodeCondition))
                    return;
                else if (dragItem.Parent is NodeAction && !(dropTarget.Parent is NodeAction))
                    return;

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

            CommandTriggerElementMove command = new CommandTriggerElementMove(item.triggerElement, targetParentGUI.GetTriggerElements(), insertIndex);
            command.Execute();
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

                if (dropTarget == null)
                    return null;
            }

            return traversedTarget;
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

            CommandTriggerElementDelete command = new CommandTriggerElementDelete(explorerElementTrigger, elementsToDelete);
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
            controller.CopyTriggerElements(explorerElementTrigger, triggerElements, isCut);

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
            var pasted = controller.PasteTriggerElements(explorerElementTrigger, attachTarget.GetTriggerElements(), insertIndex);

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
                DialogBox dialog = new DialogBox("Confirmation", "All changes made in the custom script will be lost when you switch. This cannot be undone.\n\nDo you wish to continue?");
                dialog.ShowDialog();
                if (!dialog.OK)
                {
                    explorerElementTrigger.trigger.Script = "";
                    checkBoxIsCustomScript.IsChecked = true;
                    e.Handled = true;
                    return;
                }
            }

            ScriptGenerator scriptGenerator = new ScriptGenerator(Info.GetLanguage());
            string script = scriptGenerator.ConvertGUIToJass(explorerElementTrigger, new List<string>());
            TextEditor.avalonEditor.Text = script;
            explorerElementTrigger.trigger.Script = script;
            explorerElementTrigger.trigger.IsScript = isScript;
            ShowTextEditor(isScript);
            OnStateChange();
        }

        private void ShowTextEditor(bool doShow)
        {
            if (doShow)
            {
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

        private void menuEvent_Click(object sender, RoutedEventArgs e)
        {
            CreateEvent();
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
            TreeViewTriggerElement doubleClicked = GetTraversedTargetDropItem(e.OriginalSource as FrameworkElement) as TreeViewTriggerElement;
            if (doubleClicked == null)
                return;

            ReplaceTriggerElement(doubleClicked);
        }

        private void treeViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ReplaceTriggerElement(selectedElement);
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
    }
}

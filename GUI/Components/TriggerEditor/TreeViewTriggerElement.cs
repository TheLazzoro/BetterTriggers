using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using GUI.Components.Shared;
using GUI.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using static GUI.Components.Shared.TreeItemHeader;

namespace GUI.Components.TriggerEditor
{
    public class TreeViewTriggerElement : TreeItemBT, ITriggerElementUI
    {
        internal TriggerElement triggerElement { get; }
        internal string paramText { get; set; }
        protected string category { get; set; }
        private TriggerControl triggerControl;

        internal static event Action<TreeViewTriggerElement> OnMouseEnter;

        public TreeViewTriggerElement(TriggerElement triggerElement)
        {
            this.treeItemHeader = new TreeItemHeader();
            this.Header = treeItemHeader;
            this.triggerElement = triggerElement;
            this.category = TriggerData.GetCategoryTriggerElement(triggerElement);
            if (triggerElement is ECA)
            {
                this.paramText = TriggerData.GetParamText(triggerElement);
                this.UpdateTreeItem();
                CreateSpecialTriggerElement(this);
            }
            else if (triggerElement is LocalVariable localVar)
            {
                this.UpdateTreeItem();
                localVar.variable.ValuesChanged += delegate
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        this.UpdateTreeItem();
                    });
                };
            }

            this.KeyDown += TreeViewTriggerElement_KeyDown;
            this.treeItemHeader.RenameBox.KeyDown += RenameBox_KeyDown;
            this.MouseEnter += new MouseEventHandler(delegate (object sender, MouseEventArgs e)
            {
                e.Handled = true;
                OnMouseEnter?.Invoke(this);
            }); // TODO: Memory leak because of static event.
        }

        private static void CreateSpecialTriggerElement(TreeViewTriggerElement treeViewTriggerElement)
        {
            if (treeViewTriggerElement.triggerElement is IfThenElse)
            {
                var function = (IfThenElse)treeViewTriggerElement.triggerElement;

                var If = new NodeCondition("If - Conditions");
                var Then = new NodeAction("Then - Actions");
                var Else = new NodeAction("Else - Actions");
                If.SetTriggerElements(function.If);
                Then.SetTriggerElements(function.Then);
                Else.SetTriggerElements(function.Else);
                treeViewTriggerElement.Items.Add(If);
                treeViewTriggerElement.Items.Add(Then);
                treeViewTriggerElement.Items.Add(Else);

                ControllerTriggerControl.RecurseLoadTrigger(If.GetTriggerElements(), If);
                ControllerTriggerControl.RecurseLoadTrigger(Then.GetTriggerElements(), Then);
                ControllerTriggerControl.RecurseLoadTrigger(Else.GetTriggerElements(), Else);
                If.IsExpanded = true;
                Then.IsExpanded = true;
                Else.IsExpanded = true;
                treeViewTriggerElement.IsExpanded = true;
            }
            else if (treeViewTriggerElement.triggerElement is AndMultiple)
            {
                var function = (AndMultiple)treeViewTriggerElement.triggerElement;
                var And = new NodeCondition("Conditions");
                And.SetTriggerElements(function.And);
                treeViewTriggerElement.Items.Add(And);

                ControllerTriggerControl.RecurseLoadTrigger(And.GetTriggerElements(), And);
                And.IsExpanded = true;
                treeViewTriggerElement.IsExpanded = true;
            }
            else if (treeViewTriggerElement.triggerElement is OrMultiple)
            {
                var function = (OrMultiple)treeViewTriggerElement.triggerElement;
                var Or = new NodeCondition("Conditions");
                Or.SetTriggerElements(function.Or);
                treeViewTriggerElement.Items.Add(Or);

                ControllerTriggerControl.RecurseLoadTrigger(Or.GetTriggerElements(), Or);
                Or.IsExpanded = true;
                treeViewTriggerElement.IsExpanded = true;
            }
            else if (treeViewTriggerElement.triggerElement is ForGroupMultiple)
            {
                var function = (ForGroupMultiple)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions);
                treeViewTriggerElement.Items.Add(Actions);

                ControllerTriggerControl.RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
                Actions.IsExpanded = true;
                treeViewTriggerElement.IsExpanded = true;
            }
            else if (treeViewTriggerElement.triggerElement is ForForceMultiple)
            {
                var function = (ForForceMultiple)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions);
                treeViewTriggerElement.Items.Add(Actions);

                ControllerTriggerControl.RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
                Actions.IsExpanded = true;
                treeViewTriggerElement.IsExpanded = true;
            }
            else if (treeViewTriggerElement.triggerElement is ForLoopAMultiple)
            {
                var function = (ForLoopAMultiple)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions);
                treeViewTriggerElement.Items.Add(Actions);

                ControllerTriggerControl.RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
                Actions.IsExpanded = true;
                treeViewTriggerElement.IsExpanded = true;
            }
            else if (treeViewTriggerElement.triggerElement is ForLoopBMultiple)
            {
                var function = (ForLoopBMultiple)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions);
                treeViewTriggerElement.Items.Add(Actions);

                ControllerTriggerControl.RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
                Actions.IsExpanded = true;
                treeViewTriggerElement.IsExpanded = true;
            }
            else if (treeViewTriggerElement.triggerElement is ForLoopVarMultiple)
            {
                var function = (ForLoopVarMultiple)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions);
                treeViewTriggerElement.Items.Add(Actions);

                ControllerTriggerControl.RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
                Actions.IsExpanded = true;
                treeViewTriggerElement.IsExpanded = true;
            }
            else if (treeViewTriggerElement.triggerElement is EnumDestructablesInRectAllMultiple)
            {
                var function = (EnumDestructablesInRectAllMultiple)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions);
                treeViewTriggerElement.Items.Add(Actions);

                ControllerTriggerControl.RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
                Actions.IsExpanded = true;
                treeViewTriggerElement.IsExpanded = true;
            }
            else if (treeViewTriggerElement.triggerElement is EnumDestructiblesInCircleBJMultiple)
            {
                var function = (EnumDestructiblesInCircleBJMultiple)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions);
                treeViewTriggerElement.Items.Add(Actions);

                ControllerTriggerControl.RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
                Actions.IsExpanded = true;
                treeViewTriggerElement.IsExpanded = true;
            }
            else if (treeViewTriggerElement.triggerElement is EnumItemsInRectBJ)
            {
                var function = (EnumItemsInRectBJ)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions);
                treeViewTriggerElement.Items.Add(Actions);

                ControllerTriggerControl.RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
                Actions.IsExpanded = true;
                treeViewTriggerElement.IsExpanded = true;
            }
        }



        /// <summary>
        /// Gets the TriggerControl the item is attached to.
        /// </summary>
        /// <returns></returns>
        public TriggerControl GetTriggerControl()
        {
            if (this.triggerControl != null)
                return this.triggerControl;

            // hack
            if (this.Parent is TreeView)
            {
                var treeView = (TreeView)this.Parent;
                var grid = (Grid)treeView.Parent;
                return (TriggerControl)grid.Parent;
            }

            TriggerControl triggerControl = null;
            FrameworkElement treeViewParent = (FrameworkElement)this.Parent;
            while (triggerControl == null && treeViewParent != null)
            {
                var parent = treeViewParent.Parent;
                if (parent is TriggerControl)
                    triggerControl = (TriggerControl)parent;
                else
                    treeViewParent = (FrameworkElement)parent;
            }

            this.triggerControl = triggerControl;

            return triggerControl;
        }

        public ExplorerElementTrigger GetExplorerElementTrigger()
        {
            return GetTriggerControl().explorerElementTrigger;
        }

        // TODO: Clean up.
        public void UpdateTreeItem()
        {
            ControllerParamText controllerTriggerTreeItem = new ControllerParamText();
            string text = string.Empty;
            bool isEnabled = true;
            TreeItemState state = TreeItemState.Normal;
            if (this.triggerElement is ECA eca)
            {
                text = controllerTriggerTreeItem.GenerateTreeItemText(this);
                bool areParametersValid = true;
                List<Parameter> parameters = eca.function.parameters;
                areParametersValid = Project.CurrentProject.Triggers.VerifyParameters(parameters) == 0;
                isEnabled = eca.isEnabled;
                state = areParametersValid ? TreeItemState.Normal : TreeItemState.HasErrors;

                Settings settings = Settings.Load();
                if (settings.triggerEditorMode == 0)
                {
                    this.treeItemHeader.Refresh(text, category, state, isEnabled);
                }
                else if (settings.triggerEditorMode == 1)
                {
                    List<Inline> inlines = new();
                    inlines = controllerTriggerTreeItem.GenerateParamText(this);
                    this.treeItemHeader.Refresh(inlines, category, state, isEnabled);
                }
            }
            else if (triggerElement is LocalVariable localVar)
            {
                text = localVar.variable.Name;
                //text = localVar.variable.Name + $" <{Types.GetDisplayName(localVar.variable.Type)}>"; // TODO: type text also gets into the rename field.
                this.treeItemHeader.Refresh(text, category, state, isEnabled);
            }

        }

        public void UpdatePosition()
        {
            int insertIndex = triggerElement.GetParent().IndexOf(triggerElement);
            var parent = (INode)this.Parent;
            var treeView = this.GetTriggerControl().treeViewTriggers;
            parent.Remove(this);
            INode newParent = null;
            for (int i = 0; i < treeView.Items.Count; i++)
            {
                newParent = FindParent(treeView.Items[i] as TreeViewItem, this);
                if (newParent != null)
                    break;
            }
            if (newParent == null)
                throw new Exception("Target 'Parent' was not found.");

            newParent.Insert(this, insertIndex);


            GetTriggerControl().OnStateChange();

            this.IsSelected = true;
            this.Focus();
        }

        public void UpdateParams()
        {
            ControllerParamText controllerTriggerElement = new ControllerParamText();
            controllerTriggerElement.GenerateParamText(this);
            UpdateTreeItem();
            GetTriggerControl().RefreshBottomControls();
            GetTriggerControl().OnStateChange();
        }

        public void UpdateEnabled()
        {
            UpdateTreeItem();
            GetTriggerControl().OnStateChange();
        }

        public void OnDeleted()
        {
            var parent = (TreeViewItem)this.Parent;
            TreeViewItem nextToSelect = null;
            if (parent.Items.Count > 1 && parent.Items.IndexOf(this) < parent.Items.Count - 1)
                nextToSelect = (TreeViewItem)parent.Items[parent.Items.IndexOf(this) + 1];
            else if (parent.Items.Count > 1)
                nextToSelect = (TreeViewItem)parent.Items[parent.Items.Count - 2];
            else
                nextToSelect = parent;

            this.GetTriggerControl().OnStateChange();
            parent.Items.Remove(this);

            nextToSelect.IsSelected = true;
            nextToSelect.Focus();
        }

        public void OnCreated(int insertIndex)
        {
            ControllerTriggerControl controller = new ControllerTriggerControl();
            var triggerControl = this.GetTriggerControl();
            INode parent = null;
            for (int i = 0; i < triggerControl.treeViewTriggers.Items.Count; i++)
            {
                var node = triggerControl.treeViewTriggers.Items[i];
                parent = FindParent(node as TreeViewItem, this);
                if (parent != null)
                    break;
            }
            if (this.Parent != null) // needed because of another hack. Basically, the item is already attached, so we need to detach it.
            {
                if (this.Parent is TreeView)
                {
                    var unwantedParent = (TreeView)this.Parent;
                    unwantedParent.Items.Remove(this);
                }
                else if (this.Parent is TreeViewItem)
                {
                    var unwantedParent = (TreeViewItem)this.Parent;
                    unwantedParent.Items.Remove(this);
                }
            }
            var parentTreeItem = (TreeViewItem)parent;
            parentTreeItem.Items.Insert(insertIndex, this);

            triggerControl.OnStateChange();
            this.IsSelected = true;
            this.Focus();
        }

        public void ShowRenameBox()
        {
            this.treeItemHeader.ShowRenameBox(true);
        }

        public bool IsRenaming()
        {
            return this.treeItemHeader.isRenaming;
        }

        private void TreeViewTriggerElement_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.F2 && triggerElement is LocalVariable)
            {
                ShowRenameBox();
                e.Handled = true;
            }
        }

        private void RenameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string renameText = this.treeItemHeader.GetRenameText();
                var localVar = (LocalVariable)triggerElement;
                try
                {
                    Project.CurrentProject.Variables.RenameLocalVariable(GetTriggerControl().explorerElementTrigger.trigger, localVar, renameText);
                    this.treeItemHeader.ShowRenameBox(false);
                    this.treeItemHeader.SetDisplayText(renameText);
                    GetTriggerControl().OnStateChange();
                }
                catch (Exception ex)
                {
                    MessageBox messageBox = new MessageBox("Error", ex.Message);
                    messageBox.ShowDialog();
                }
            }
            else if (e.Key == Key.Escape)
            {
                this.treeItemHeader.ShowRenameBox(false);
                this.Focus();
            }
        }

        private INode FindParent(TreeViewItem parent, TreeViewTriggerElement treeViewTriggerElement)
        {
            INode node = null;

            // Ugly code
            // we need to check the parent right away, because the loop below
            // will never check it.
            if (parent is INode)
            {
                var tmpNode = (INode)parent;
                if (tmpNode.GetTriggerElements() == treeViewTriggerElement.triggerElement.GetParent())
                    return tmpNode;
            }

            for (int i = 0; i < parent.Items.Count; i++)
            {
                var treeItem = (TreeViewItem)parent.Items[i];
                if (treeItem is INode)
                {
                    var tmpNode = (INode)treeItem;
                    if (tmpNode.GetTriggerElements() == treeViewTriggerElement.triggerElement.GetParent())
                    {
                        return tmpNode;
                    }
                }

                if (treeItem.Items.Count > 0)
                    node = FindParent(treeItem, treeViewTriggerElement);

                if (node != null)
                    return node;
            }

            return node;
        }
    }
}

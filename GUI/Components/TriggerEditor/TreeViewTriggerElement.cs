using BetterTriggers.Containers;
using BetterTriggers.Controllers;
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

        public TreeViewTriggerElement(TriggerElement triggerElement)
        {
            this.treeItemHeader = new TreeItemHeader();
            this.Header = treeItemHeader;
            this.triggerElement = triggerElement;
            this.category = ControllerTriggerData.GetCategoryTriggerElement(triggerElement);
            if (triggerElement is ECA)
            {
                this.paramText = ControllerTriggerData.GetParamText(triggerElement);
                this.UpdateTreeItem();
                ControllerTriggerControl controllerTriggerControl = new ControllerTriggerControl();
                controllerTriggerControl.CreateSpecialTriggerElement(this);
            }
            else if(triggerElement is LocalVariable)
                this.UpdateTreeItem();

            this.KeyDown += TreeViewTriggerElement_KeyDown;
            this.treeItemHeader.RenameBox.KeyDown += RenameBox_KeyDown;
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
            if (this.triggerElement is ECA)
                text = controllerTriggerTreeItem.GenerateTreeItemText(this);
            else if (triggerElement is LocalVariable localVar)
                text = localVar.variable.Name;

            bool isEnabled = true;
            TreeItemState state = TreeItemState.Normal;
            if (this.triggerElement is not LocalVariable)
            {
                bool areParametersValid = true;
                var _triggerElement = (ECA)this.triggerElement;
                List<Parameter> parameters = _triggerElement.function.parameters;
                areParametersValid = ControllerTrigger.VerifyParameters(parameters) == 0;
                isEnabled = _triggerElement.isEnabled;
                state = areParametersValid ? TreeItemState.Normal : TreeItemState.HasErrors;
            }

            this.treeItemHeader.Refresh(text, category, state, isEnabled);
        }

        public void UpdatePosition()
        {
            ControllerTriggerControl controller = new ControllerTriggerControl();
            controller.OnTriggerElementMove(this, triggerElement.GetParent().IndexOf(triggerElement));
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
                parent = controller.FindParent(node as TreeViewItem, this);
                if (parent != null)
                    break;
            }
            controller.OnTriggerElementCreate(this, parent, insertIndex);
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
                    ControllerVariable.RenameLocalVariable(GetTriggerControl().explorerElementTrigger.trigger, localVar, renameText);
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
    }
}

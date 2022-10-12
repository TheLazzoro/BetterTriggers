using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using GUI.Components.Shared;
using GUI.Controllers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using static GUI.Components.Shared.TreeItemHeader;

namespace GUI.Components.TriggerEditor
{
    public class TreeViewTriggerElement : TreeItemBT, ITriggerElementUI
    {
        internal TriggerElement triggerElement;
        internal string paramText;
        protected string category;
        private TriggerControl triggerControl;

        public TreeViewTriggerElement(TriggerElement triggerElement)
        {
            this.triggerElement = triggerElement;

            ControllerTriggerData controller = new ControllerTriggerData();
            this.paramText = controller.GetParamText(triggerElement.function);
            this.category = controller.GetCategoryTriggerElement(triggerElement.function);

            this.UpdateTreeItem();

            ControllerTriggerControl controllerTriggerControl = new ControllerTriggerControl();
            controllerTriggerControl.CreateSpecialTriggerElement(this);
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
            ControllerTrigger controllerTrigger = new ControllerTrigger();
            ControllerParamText controllerTriggerTreeItem = new ControllerParamText();
            string text = controllerTriggerTreeItem.GenerateTreeItemText(this);

            List<Parameter> parameters = this.triggerElement.function.parameters;

            bool areParametersValid = controllerTrigger.VerifyParameters(parameters) == 0;
            bool isEnabled = triggerElement.isEnabled;

            TreeItemState state = areParametersValid == true ? TreeItemState.Normal : TreeItemState.HasErrors;

            TreeItemHeader header = new TreeItemHeader(text, category, state, isEnabled);
            this.treeItemHeader = header;
            this.Header = header;
        }

        public void UpdatePosition()
        {
            ControllerTriggerControl controller = new ControllerTriggerControl();
            controller.OnTriggerElementMove(this, triggerElement.Parent.IndexOf(triggerElement));
            GetTriggerControl().OnStateChange();

            this.IsSelected = true;
            this.Focus();
        }

        public void UpdateParams()
        {
            ControllerParamText controllerTriggerElement = new ControllerParamText();
            controllerTriggerElement.GenerateParamText(this);
            UpdateTreeItem();
            GetTriggerControl().RefreshParameterBlock();
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
    }
}

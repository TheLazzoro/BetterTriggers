using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using GUI.Controllers;
using GUI.Utility;
using Model.EditorData;
using Model.EditorData.Enums;
using Model.SaveableData;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Components.TriggerEditor
{
    public class TreeViewTriggerElement : TreeViewItem, ITriggerElementUI
    {
        internal TriggerElement triggerElement;
        public TextBlock paramTextBlock;
        public TextBlock descriptionTextBlock;
        internal string paramText;
        protected Category category;
        private string formattedParamText = string.Empty;
        private TriggerControl triggerControl;

        public TreeViewTriggerElement(TriggerElement triggerElement)
        {
            this.triggerElement = triggerElement;

            this.paramTextBlock = new TextBlock();
            this.paramTextBlock.Margin = new Thickness(5, 0, 5, 0);
            this.paramTextBlock.FontSize = 18;
            this.paramTextBlock.TextWrapping = TextWrapping.Wrap;
            this.paramTextBlock.Foreground = Brushes.White;
            Grid.SetRow(this.paramTextBlock, 3);

            this.descriptionTextBlock = new TextBlock();
            this.descriptionTextBlock.FontSize = 12;
            this.descriptionTextBlock.TextWrapping = TextWrapping.Wrap;
            this.descriptionTextBlock.Margin = new Thickness(5, 0, 5, 5);
            this.descriptionTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 200));
            this.descriptionTextBlock.Background = new SolidColorBrush(Color.FromRgb(40, 40, 40));
            Grid.SetRow(this.descriptionTextBlock, 4);

            ControllerTriggerData controller = new ControllerTriggerData();
            this.paramText = controller.GetParamText(triggerElement.function);
            this.descriptionTextBlock.Text = controller.GetDescription(triggerElement.function);
            this.category = controller.GetCategoryTriggerElement(triggerElement.function);

            ControllerTriggerTreeItem controllerTriggerElement = new ControllerTriggerTreeItem(this);
            controllerTriggerElement.GenerateParamText();
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

        public void UpdateTreeItem()
        {
            List<Parameter> parameters = this.triggerElement.function.parameters;

            bool areParametersValid = IsParameterListValid(parameters);
            if (parameters.Count == 1 && parameters[0].returnType == "nothing") // hack
                areParametersValid = true;
            bool isEnabled = triggerElement.isEnabled;

            Inline[] inlines = new Inline[paramTextBlock.Inlines.Count];
            paramTextBlock.Inlines.CopyTo(inlines, 0);
            List<TextRange> textRanges = new List<TextRange>();
            for (int i = 0; i < inlines.Length; i++)
            {
                textRanges.Add(new TextRange(inlines[i].ContentStart, inlines[i].ContentEnd));
            }
            string text = string.Empty;
            textRanges.ForEach(element => text += element.Text);

            TreeItemHeader header = new TreeItemHeader(text, category, areParametersValid, isEnabled);
            this.Header = header;
        }

        private bool IsParameterListValid(List<Parameter> parameters, bool isValid = true)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                var param = parameters[i];
                if (param is Function)
                {
                    var func = (Function)param;
                    if (func.parameters.Count > 0)
                    {
                        isValid = IsParameterListValid(func.parameters, isValid);
                    }
                }

                if (param.identifier == null)
                    isValid = false;
            }

            return isValid;
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
            ControllerTriggerTreeItem controllerTriggerElement = new ControllerTriggerTreeItem(this);
            controllerTriggerElement.GenerateParamText();
            UpdateTreeItem();
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
            INode parent = null;
            for (int i = 0; i < GetTriggerControl().treeViewTriggers.Items.Count; i++)
            {
                var node = GetTriggerControl().treeViewTriggers.Items[i];
                parent = controller.FindParent(node as TreeViewItem, this);
                if (parent != null)
                    break;
            }
            controller.OnTriggerElementCreate(this, parent, insertIndex);
            this.GetTriggerControl().OnStateChange();

            this.IsSelected = true;
            this.Focus();
        }
    }
}

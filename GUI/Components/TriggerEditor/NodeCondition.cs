using Model.Data;
using GUI.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Model.EditorData.Enums;
using Model.SaveableData;

namespace GUI.Components.TriggerEditor
{
    public class NodeCondition : TreeViewItem, INode
    {
        public List<TriggerElement> TriggerElements;

        public NodeCondition(string text)
        {
            TreeViewManipulator.SetTreeViewItemAppearance(this, text, Category.Condition);
        }

        public void Add(TreeViewTriggerElement triggerElement)
        {
            this.Items.Add(triggerElement);
        }

        public void Remove(TreeViewTriggerElement triggerElement)
        {
            this.Items.Remove(triggerElement);
        }

        public List<TreeViewTriggerElement> GetTreeViewTriggerElements()
        {
            List<TreeViewTriggerElement> triggerElements = new List<TreeViewTriggerElement>();
            for (int i = 0; i < Items.Count; i++)
            {
                triggerElements.Add((TreeViewTriggerElement)Items[i]);
            }

            return triggerElements;
        }

        public List<TriggerElement> GetTriggerElements()
        {
            return TriggerElements;
        }

        public TriggerElementType GetNodeType()
        {
            return TriggerElementType.Condition;
        }
    }
}

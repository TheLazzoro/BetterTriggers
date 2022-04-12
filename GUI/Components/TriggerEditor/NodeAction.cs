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
    public class NodeAction : TreeViewItem, INode
    {
        public List<TriggerElement> TriggerElements;
        
        public NodeAction(string text)
        {
            TreeViewManipulator.SetTreeViewItemAppearance(this, text, Category.Action);
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
            return TriggerElementType.Action;
        }
    }
}

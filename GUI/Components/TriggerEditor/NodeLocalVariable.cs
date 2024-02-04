using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using GUI.Components.Shared;
using BetterTriggers.Models.SaveableData;
using System.Linq;

namespace GUI.Components.TriggerEditor
{
    public class NodeLocalVariable : TreeItemBT, INode
    {
        public List<TriggerElement_Saveable> LocalVariables = new List<TriggerElement_Saveable>();

        public NodeLocalVariable(string text)
        {
            TreeItemHeader header = new TreeItemHeader(text, TriggerCategory.TC_LOCAL_VARIABLE);
            this.treeItemHeader = header;
            this.Header = header;
        }

        public void Add(TreeViewTriggerElement triggerElement)
        {
            this.Items.Add(triggerElement);
        }

        public void Insert(TreeViewTriggerElement treeViewTriggerElement, int insertIndex)
        {
            this.Items.Insert(insertIndex, treeViewTriggerElement);
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

        public List<TriggerElement_Saveable> GetTriggerElements()
        {
            return LocalVariables;
        }

        public TriggerElementType GetNodeType()
        {
            return TriggerElementType.LocalVariable;
        }

        public void SetTriggerElements(List<TriggerElement_Saveable> triggerElements)
        {
            this.LocalVariables = triggerElements;
        }
    }
}

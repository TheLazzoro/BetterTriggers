using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.TriggerEditor
{
    public interface INode
    {
        void SetTriggerElements(List<ITriggerElement> triggerElements);
        List<ITriggerElement> GetTriggerElements();
        TriggerElementType GetNodeType();
        List<TreeViewTriggerElement> GetTreeViewTriggerElements();
        void Add(TreeViewTriggerElement triggerElement);
        void Insert(TreeViewTriggerElement treeViewTriggerElement, int insertIndex);
        void Remove(TreeViewTriggerElement triggerElement);
    }
}

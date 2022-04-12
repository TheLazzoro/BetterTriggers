using Model.SaveableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.TriggerEditor
{
    public interface INode
    {
        List<TriggerElement> GetTriggerElements();
        TriggerElementType GetNodeType();
        List<TreeViewTriggerElement> GetTreeViewTriggerElements();
        void Add(TreeViewTriggerElement triggerElement);
        void Remove(TreeViewTriggerElement triggerElement);
    }
}

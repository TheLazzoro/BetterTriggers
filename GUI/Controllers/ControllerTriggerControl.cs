using BetterTriggers.Models.SaveableData;
using GUI.Components.TriggerEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI.Controllers
{
    public class ControllerTriggerControl
    {
        public static void RecurseLoadTrigger(List<TriggerElement> triggerElements, INode parentNode)
        {
            TreeViewItem item = (TreeViewItem)parentNode;
            parentNode.SetTriggerElements(triggerElements);
            for (int i = 0; i < triggerElements.Count; i++)
            {
                var triggerElement = triggerElements[i];
                triggerElement.SetParent(triggerElements);
                TreeViewTriggerElement treeItem = new TreeViewTriggerElement(triggerElement);
                triggerElement.Attach(treeItem);
                parentNode.Add(treeItem);
            }
        }
    }
}

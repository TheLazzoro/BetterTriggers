using GUI.Components;
using GUI.Components.TriggerEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerTriggerControl
    {
        public void OnTriggerElementCreate(TreeViewTriggerElement item, INode parent, int insertIndex)
        {
            var parentTreeItem = (TreeViewItem)parent;
            parentTreeItem.Items.Insert(insertIndex, item);
        }

        /// <summary>
        /// Moves a 'TreeViewTriggerElement' to its correct location based on the 'TriggerElement'.
        /// </summary>
        /// <param name="treeViewTriggerElement"></param>
        /// <param name="insertIndex"></param>
        internal void OnTriggerElementMove(TreeViewTriggerElement treeViewTriggerElement, int insertIndex)
        {
            var parent = (INode)treeViewTriggerElement.Parent;
            var triggerElement = treeViewTriggerElement.triggerElement;
            var treeView = treeViewTriggerElement.triggerControl.treeViewTriggers;
            parent.Remove(treeViewTriggerElement);

            INode newParent = null;
            for (int i = 0; i < treeView.Items.Count; i++)
            {
                newParent = FindParent(treeView.Items[i] as INode, treeViewTriggerElement);
                if (newParent != null)
                    break;
            }
            if (newParent == null)
                throw new Exception("Target 'Parent' was not found.");

            newParent.Insert(treeViewTriggerElement, insertIndex);
        }

        /// <summary>
        /// Finds the parent to attach a TreeViewTriggerElement to.
        /// This assumes the item has 'Parent', otherwise expect a crash.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="treeViewTriggerElement"></param>
        /// <returns></returns>
        internal INode FindParent(INode parent, TreeViewTriggerElement treeViewTriggerElement)
        {
            if (parent == null)
                return null;

            if (parent.GetTriggerElements() == treeViewTriggerElement.triggerElement.Parent)
                return parent;
            
            var items = parent.GetTreeViewTriggerElements();
            for(int i = 0; i < items.Count; i++)
            {
                var par = FindParent(items[i] as INode, treeViewTriggerElement);
                if (par != null)
                    return par;
            }

            return null;
        }


        /*
        internal TreeItemExplorerElement FindTreeNodeDirectory(TreeItemExplorerElement parent, string directory)
        {
            TreeItemExplorerElement node = null;

            for (int i = 0; i < parent.Items.Count; i++)
            {
                TreeItemExplorerElement element = parent.Items[i] as TreeItemExplorerElement;
                if (Directory.Exists(element.Ielement.GetPath()) && node == null)
                {
                    if (element.Ielement.GetPath() == directory)
                    {
                        node = element;
                        break;
                    }
                    else
                    {
                        node = FindTreeNodeDirectory(element, directory);
                    }
                }
            }

            return node;
        }
        */
    }
}

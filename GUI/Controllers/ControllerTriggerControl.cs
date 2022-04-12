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
            item.OnCreated();
        }

        private TreeViewItem FindParent(INode parent, TreeViewTriggerElement triggerElement)
        {
            throw new NotImplementedException();
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

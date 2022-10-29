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
        List<TreeViewTriggerElement> selectedElements = new List<TreeViewTriggerElement>();

        public void OnTriggerElementCreate(TreeViewTriggerElement item, INode parent, int insertIndex)
        {
            if (item.Parent != null) // needed because of another hack. Basically, the item is already attached, so we need to detach it.
            {
                if (item.Parent is TreeView)
                {
                    var unwantedParent = (TreeView)item.Parent;
                    unwantedParent.Items.Remove(item);
                }
                else if (item.Parent is TreeViewItem)
                {
                    var unwantedParent = (TreeViewItem)item.Parent;
                    unwantedParent.Items.Remove(item);
                }
            }

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
            var treeView = treeViewTriggerElement.GetTriggerControl().treeViewTriggers;
            parent.Remove(treeViewTriggerElement);

            INode newParent = null;
            for (int i = 0; i < treeView.Items.Count; i++)
            {
                newParent = FindParent(treeView.Items[i] as TreeViewItem, treeViewTriggerElement);
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
        internal INode FindParent(TreeViewItem parent, TreeViewTriggerElement treeViewTriggerElement)
        {
            INode node = null;

            // Ugly code
            // we need to check the parent right away, because the loop below
            // will never check it.
            if (parent is INode)
            {
                var tmpNode = (INode)parent;
                if (tmpNode.GetTriggerElements() == treeViewTriggerElement.triggerElement.GetParent())
                    return tmpNode;
            }

            for (int i = 0; i < parent.Items.Count; i++)
            {
                var treeItem = (TreeViewItem)parent.Items[i];
                if (treeItem is INode)
                {
                    var tmpNode = (INode)treeItem;
                    if (tmpNode.GetTriggerElements() == treeViewTriggerElement.triggerElement.GetParent())
                    {
                        return tmpNode;
                    }
                }

                if (treeItem.Items.Count > 0)
                    node = FindParent(treeItem, treeViewTriggerElement);

                if (node != null)
                    return node;
            }

            return node;
        }

        internal void CreateSpecialTriggerElement(TreeViewTriggerElement treeViewTriggerElement)
        {
            if (treeViewTriggerElement.triggerElement is IfThenElse)
            {
                treeViewTriggerElement.ExpandSubtree();
                var function = (IfThenElse)treeViewTriggerElement.triggerElement;

                var If = new NodeCondition("If - Conditions");
                var Then = new NodeAction("Then - Actions");
                var Else = new NodeAction("Else - Actions");
                If.SetTriggerElements(function.If.Cast<ITriggerElement>().ToList());
                Then.SetTriggerElements(function.Then.Cast<ITriggerElement>().ToList());
                Else.SetTriggerElements(function.Else.Cast<ITriggerElement>().ToList());
                treeViewTriggerElement.Items.Add(If);
                treeViewTriggerElement.Items.Add(Then);
                treeViewTriggerElement.Items.Add(Else);

                RecurseLoadTrigger(If.GetTriggerElements(), If);
                RecurseLoadTrigger(Then.GetTriggerElements(), Then);
                RecurseLoadTrigger(Else.GetTriggerElements(), Else);
            }
            else if (treeViewTriggerElement.triggerElement is AndMultiple)
            {
                treeViewTriggerElement.ExpandSubtree();
                var function = (AndMultiple)treeViewTriggerElement.triggerElement;
                var And = new NodeCondition("Conditions");
                And.SetTriggerElements(function.And.Cast<ITriggerElement>().ToList());
                treeViewTriggerElement.Items.Add(And);

                RecurseLoadTrigger(And.GetTriggerElements(), And);
            }
            else if (treeViewTriggerElement.triggerElement is OrMultiple)
            {
                treeViewTriggerElement.ExpandSubtree();
                var function = (OrMultiple)treeViewTriggerElement.triggerElement;
                var Or = new NodeCondition("Conditions");
                Or.SetTriggerElements(function.Or.Cast<ITriggerElement>().ToList());
                treeViewTriggerElement.Items.Add(Or);

                RecurseLoadTrigger(Or.GetTriggerElements(), Or);
            }
            else if (treeViewTriggerElement.triggerElement is ForGroupMultiple)
            {
                treeViewTriggerElement.ExpandSubtree();
                var function = (ForGroupMultiple)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions.Cast<ITriggerElement>().ToList());
                treeViewTriggerElement.Items.Add(Actions);

                RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
            }
            else if (treeViewTriggerElement.triggerElement is ForForceMultiple)
            {
                treeViewTriggerElement.ExpandSubtree();
                var function = (ForForceMultiple)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions.Cast<ITriggerElement>().ToList());
                treeViewTriggerElement.Items.Add(Actions);

                RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
            }
            else if (treeViewTriggerElement.triggerElement is ForLoopAMultiple)
            {
                treeViewTriggerElement.ExpandSubtree();
                var function = (ForLoopAMultiple)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions.Cast<ITriggerElement>().ToList());
                treeViewTriggerElement.Items.Add(Actions);

                RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
            }
            else if (treeViewTriggerElement.triggerElement is ForLoopBMultiple)
            {
                treeViewTriggerElement.ExpandSubtree();
                var function = (ForLoopBMultiple)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions.Cast<ITriggerElement>().ToList());
                treeViewTriggerElement.Items.Add(Actions);

                RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
            }
            else if (treeViewTriggerElement.triggerElement is ForLoopVarMultiple)
            {
                treeViewTriggerElement.ExpandSubtree();
                var function = (ForLoopVarMultiple)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions.Cast<ITriggerElement>().ToList());
                treeViewTriggerElement.Items.Add(Actions);

                RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
            }
            else if (treeViewTriggerElement.triggerElement is EnumDestructablesInRectAllMultiple)
            {
                treeViewTriggerElement.ExpandSubtree();
                var function = (EnumDestructablesInRectAllMultiple)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions.Cast<ITriggerElement>().ToList());
                treeViewTriggerElement.Items.Add(Actions);

                RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
            }
            else if (treeViewTriggerElement.triggerElement is EnumDestructiblesInCircleBJMultiple)
            {
                treeViewTriggerElement.ExpandSubtree();
                var function = (EnumDestructiblesInCircleBJMultiple)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions.Cast<ITriggerElement>().ToList());
                treeViewTriggerElement.Items.Add(Actions);

                RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
            }
            else if (treeViewTriggerElement.triggerElement is EnumItemsInRectBJ)
            {
                treeViewTriggerElement.ExpandSubtree();
                var function = (EnumItemsInRectBJ)treeViewTriggerElement.triggerElement;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions.Cast<ITriggerElement>().ToList());
                treeViewTriggerElement.Items.Add(Actions);

                RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
            }
        }

        public void RecurseLoadTrigger(List<ITriggerElement> triggerElements, INode parentNode)
        {
            TreeViewItem item = (TreeViewItem)parentNode;
            item.ExpandSubtree();
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

        /// <summary>
        /// Returns a list of selected elements in the editor.
        /// Returns only the first selected element if their 'Parents' don't match.
        /// </summary>
        /// <param name="startElement"></param>
        /// <param name="endElement"></param>
        /// <returns></returns>
        public List<TreeViewTriggerElement> SelectItemsMultiple(TreeViewTriggerElement startElement, TreeViewTriggerElement endElement)
        {
            // visually deselect old items
            for (int i = 0; i < selectedElements.Count; i++)
            {
                selectedElements[i].Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }

            if (startElement == null && endElement == null)
                return null;

            selectedElements = new List<TreeViewTriggerElement>();

            if (startElement.Parent == endElement.Parent)
            {
                var parent = (TreeViewItem)startElement.Parent;

                TreeViewTriggerElement correctedStartElement;
                TreeViewTriggerElement correctedEndElement;
                if (parent.Items.IndexOf(startElement) < parent.Items.IndexOf(endElement))
                {
                    correctedStartElement = startElement;
                    correctedEndElement = endElement;
                }
                else
                {
                    correctedStartElement = endElement;
                    correctedEndElement = startElement;
                }

                int startIndex = parent.Items.IndexOf(correctedStartElement);
                int size = parent.Items.IndexOf(correctedEndElement) - parent.Items.IndexOf(correctedStartElement);
                for (int i = 0; i <= size; i++)
                {
                    selectedElements.Add((TreeViewTriggerElement)parent.Items[startIndex + i]);
                }
            }
            else
                selectedElements.Add(endElement);

            // visually select elements
            for (int i = 0; i < selectedElements.Count; i++)
            {
                selectedElements[i].Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#aa357EC7");
            }

            return selectedElements;
        }
    }
}

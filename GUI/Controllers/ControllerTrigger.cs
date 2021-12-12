using GUI.Components.TriggerEditor;
using GUI.Components.TriggerExplorer;
using GUI.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerTrigger
    {
        public void CreateTrigger(TriggerExplorer triggerExplorer, Grid mainGrid)
        {
            var triggerControl = new TriggerControl();
            triggerControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            triggerControl.VerticalContentAlignment = VerticalAlignment.Stretch;
            Grid.SetColumn(triggerControl, 1);
            Grid.SetRow(triggerControl, 3);
            Grid.SetRowSpan(triggerControl, 2);
            mainGrid.Children.Add(triggerControl);

            string name = NameGenerator.GenerateTriggerName();

            TreeViewItem item = new TreeViewItem();
            ExplorerTrigger trig = new ExplorerTrigger(name, item, triggerControl);

            triggerExplorer.CreateTreeViewItem(item, name, Model.Data.EnumCategory.Trigger);
        }

        public void CreateTriggerWithElements(TriggerExplorer triggerExplorer, Grid mainGrid, string name, Model.Trigger trigger)
        {
            var triggerControl = new TriggerControl();
            triggerControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            triggerControl.VerticalContentAlignment = VerticalAlignment.Stretch;
            Grid.SetColumn(triggerControl, 1);
            Grid.SetRow(triggerControl, 3);
            Grid.SetRowSpan(triggerControl, 2);
            mainGrid.Children.Add(triggerControl);

            TreeViewItem item = new TreeViewItem();
            ExplorerTrigger trig = new ExplorerTrigger(name, item, triggerControl);
            triggerExplorer.CreateTreeViewItem(item, name, Model.Data.EnumCategory.Trigger);

            // Generate trigger elements
            for(int i = 0; i < trigger.Events.Count; i++)
            {
                var _event = trigger.Events[i];
                TriggerElement triggerElement = new TriggerElement(_event);
                triggerControl.categoryEvent.Items.Add(triggerElement);
            }
            for (int i = 0; i < trigger.Conditions.Count; i++)
            {
                var condition = trigger.Conditions[i];
                TriggerElement triggerElement = new TriggerElement(condition);
                triggerControl.categoryCondition.Items.Add(triggerElement);
            }
            for (int i = 0; i < trigger.Actions.Count; i++)
            {
                var action = trigger.Actions[i];
                TriggerElement triggerElement = new TriggerElement(action);
                triggerControl.categoryAction.Items.Add(triggerElement);
            }
        }
    }
}

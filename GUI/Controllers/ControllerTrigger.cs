using GUI.Components.TriggerEditor;
using GUI.Components.TriggerExplorer;
using GUI.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerTrigger
    {
        public void CreateTrigger(TriggerExplorer triggerExplorer, TabControl tabControl)
        {
            var triggerControl = new TriggerControl();
            triggerControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            triggerControl.VerticalContentAlignment = VerticalAlignment.Stretch;
            Grid.SetColumn(triggerControl, 1);
            Grid.SetRow(triggerControl, 3);
            Grid.SetRowSpan(triggerControl, 2);


            string name = NameGenerator.GenerateTriggerName();

            TreeViewItem item = new TreeViewItem();

            triggerExplorer.CreateTreeViewItem(item, name, Model.Data.EnumCategory.Trigger);
        }

        public TriggerControl CreateTriggerWithElements(TabControl tabControl, Model.Trigger trigger)
        {
            var triggerControl = CreateTriggerControl(tabControl);
            TriggerControl trig = new TriggerControl();
            GenerateTriggerElements(triggerControl, trigger);

            return trig;
        }

        public Model.Trigger LoadTriggerFromFile(string filename)
        {
            var file = File.ReadAllText(filename);
            Model.Trigger trigger = JsonConvert.DeserializeObject<Model.Trigger>(file);

            return trigger;
        }

        private TriggerControl CreateTriggerControl(TabControl tabControl)
        {
            var triggerControl = new TriggerControl();
            triggerControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            triggerControl.VerticalContentAlignment = VerticalAlignment.Stretch;
            Grid.SetColumn(triggerControl, 1);
            Grid.SetRow(triggerControl, 3);
            Grid.SetRowSpan(triggerControl, 2);
            tabControl.Items.Add(triggerControl);

            return triggerControl;
        }

        private void GenerateTriggerElements(TriggerControl triggerControl, Model.Trigger trigger)
        {
            // Generate trigger elements
            for (int i = 0; i < trigger.Events.Count; i++)
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

            triggerControl.categoryEvent.ExpandSubtree();
            triggerControl.categoryCondition.ExpandSubtree();
            triggerControl.categoryAction.ExpandSubtree();
        }
    }
}

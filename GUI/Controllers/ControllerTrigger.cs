using GUI.Components.TriggerEditor;
using GUI.Components.TriggerExplorer;
using GUI.Containers;
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
        public void CreateTrigger(TriggerExplorer explorer)
        {
            NewExplorerElementWindow createExplorerElementWindow = new NewExplorerElementWindow(EnumExplorerElement.Trigger);
            createExplorerElementWindow.ShowDialog();
            if(createExplorerElementWindow.ElementName != null)
            {
                string name = createExplorerElementWindow.ElementName;

                ControllerProject controllerProject = new ControllerProject();
                string directory = controllerProject.GetDirectoryFromSelection(explorer.treeViewTriggerExplorer);

                Trigger trigger = new Trigger();
                string json = JsonConvert.SerializeObject(trigger);

                File.WriteAllText(directory + "/" + name + ".json", json);
            }
        }

        public TriggerControl CreateTriggerWithElements(TabControl tabControl, Model.Trigger trigger)
        {
            var triggerControl = CreateTriggerControl(tabControl);
            GenerateTriggerElements(triggerControl, trigger);

            return triggerControl;
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

using GUI.Components.TriggerEditor;
using Model.Natives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerExplorer
{
    public class ExplorerTrigger : TriggerExplorerElement, ITriggerExplorerElement
    {
        public TriggerControl triggerControl;
        public bool IsEnabled;

        public ExplorerTrigger(string name, TreeViewItem treeViewItem, TriggerControl triggerControl) : base(treeViewItem)
        {
            this.Name = name;
            this.IsEnabled = true;
            this.triggerControl = triggerControl;
        }

        public void Hide()
        {
            triggerControl.Visibility = Visibility.Hidden;
        }

        public void OnElementClick()
        {
            if (currentTriggerElement != null)
                currentTriggerElement.Hide();

            this.Show();

            currentTriggerElement = this;
        }

        public void Show()
        {
            triggerControl.Visibility = Visibility.Visible;
        }

        public string GetScript()
        {
            throw new NotImplementedException();
        }

        public string GetSaveString()
        {
            Model.Trigger trigger = new Model.Trigger();

            var nodeEvents = triggerControl.categoryEvent;
            for (int i = 0; i < nodeEvents.Items.Count; i++)
            {
                var _event = (TriggerElement)nodeEvents.Items[i];
                trigger.Events.Add(_event.function);
            }

            var nodeConditions = triggerControl.categoryCondition;
            for (int i = 0; i < nodeConditions.Items.Count; i++)
            {
                var condition = (TriggerElement)nodeConditions.Items[i];
                trigger.Conditions.Add(condition.function);
            }

            var nodeActions = triggerControl.categoryAction;
            for (int i = 0; i < nodeActions.Items.Count; i++)
            {
                var action = (TriggerElement)nodeActions.Items[i];
                trigger.Actions.Add(action.function);
            }

            return JsonConvert.SerializeObject(trigger);
        }

        public UserControl GetControl()
        {
            return this.triggerControl;
        }
    }
}

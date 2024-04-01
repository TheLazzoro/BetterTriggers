using BetterTriggers.Models.EditorData;
using BetterTriggers.WorldEdit;
using GUI.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.TriggerEditor
{
    public class TriggerControlViewModel
    {
        private ExplorerElement _explorerElement;
        private ObservableCollection<TriggerElement> _triggers;
        public ObservableCollection<TriggerElement> Elements
        {
            get
            {
                return _triggers;
            }
        }

        public TriggerControlViewModel(ExplorerElement explorerElement)
        {
            _explorerElement = explorerElement;
            var trigger = explorerElement.trigger;
            var actionDefinition = explorerElement.actionDefinition;
            var conditionDefinition = explorerElement.conditionDefinition;
            var functionDefinition = explorerElement.functionDefinition;

            if (trigger != null)
            {
                _triggers = new()
                {
                    trigger.Events,
                    trigger.Conditions,
                    trigger.LocalVariables,
                    trigger.Actions,
                };

                InitTriggerElementVisuals(trigger.Events.Elements);
                InitTriggerElementVisuals(trigger.Conditions.Elements);
                InitTriggerElementVisuals(trigger.LocalVariables.Elements);
                InitTriggerElementVisuals(trigger.Actions.Elements);
            }
            else if(actionDefinition != null)
            {
                _triggers = new()
                {
                    actionDefinition.Parameters,
                    actionDefinition.LocalVariables,
                    actionDefinition.Actions
                };
            }
        }

        private void InitTriggerElementVisuals(ObservableCollection<TriggerElement> elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                var triggerElement = elements[i];
                string category = TriggerData.GetCategoryTriggerElement(triggerElement);
                var paramBuilder = new ParamTextBuilder();
                triggerElement.IconImage = Category.Get(category).Icon;
                if (triggerElement is ECA eca)
                {
                    triggerElement.DisplayText = paramBuilder.GenerateTreeItemText(eca);
                }
                if (triggerElement.Elements != null && triggerElement.Elements.Count > 0)
                {
                    foreach (TriggerElementCollection collection in triggerElement.Elements)
                    {
                        InitTriggerElementVisuals(collection.Elements);
                    }
                }
            }
        }
    }
}

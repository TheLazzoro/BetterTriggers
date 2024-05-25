using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.EditorData.TriggerEditor;
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
    public class TriggerControlViewModel : IDisposable
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
            ExplorerElement.CurrentToRender = explorerElement;
            var trigger = explorerElement.trigger;
            var actionDefinition = explorerElement.actionDefinition;
            var conditionDefinition = explorerElement.conditionDefinition;
            var functionDefinition = explorerElement.functionDefinition;

            switch (explorerElement.ElementType)
            {
                case ExplorerElementEnum.Trigger:
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
                    break;
                case ExplorerElementEnum.ActionDefinition:
                    _triggers = new()
                    {
                        actionDefinition.Parameters,
                        actionDefinition.LocalVariables,
                        actionDefinition.Actions
                    };

                    InitTriggerElementVisuals(actionDefinition.Parameters.Elements);
                    InitTriggerElementVisuals(actionDefinition.LocalVariables.Elements);
                    InitTriggerElementVisuals(actionDefinition.Actions.Elements);
                    break;
                case ExplorerElementEnum.ConditionDefinition:
                    _triggers = new()
                    {
                        conditionDefinition.Parameters,
                        conditionDefinition.LocalVariables,
                        conditionDefinition.Actions
                    };

                    InitTriggerElementVisuals(conditionDefinition.Parameters.Elements);
                    InitTriggerElementVisuals(conditionDefinition.LocalVariables.Elements);
                    InitTriggerElementVisuals(conditionDefinition.Actions.Elements);
                    break;
                case ExplorerElementEnum.FunctionDefinition:
                    _triggers = new()
                    {
                        functionDefinition.ReturnType,
                        functionDefinition.Parameters,
                        functionDefinition.LocalVariables,
                        functionDefinition.Actions
                    };

                    InitTriggerElementVisuals(functionDefinition.Parameters.Elements);
                    InitTriggerElementVisuals(functionDefinition.LocalVariables.Elements);
                    InitTriggerElementVisuals(functionDefinition.Actions.Elements);
                    break;
                default:
                    break;
            }
        }

        private void InitTriggerElementVisuals(ObservableCollection<TriggerElement> elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                var triggerElement = elements[i];
                string category = TriggerData.GetCategoryTriggerElement(triggerElement);
                if (triggerElement is ActionDefinitionRef)
                {
                    category = TriggerCategory.TC_ACTION_DEF;
                }
                else if (triggerElement is ConditionDefinitionRef condDefRef)
                {
                    category = TriggerCategory.TC_CONDITION_DEF;
                }
                else if (triggerElement is ParameterDefinition paramDef)
                {
                    category = TriggerCategory.TC_PARAMETER;
                }
                var paramBuilder = new ParamTextBuilder();
                triggerElement.IconImage = Category.Get(category).Icon;
                if (triggerElement is ECA eca)
                {
                    triggerElement.DisplayText = paramBuilder.GenerateTreeItemText(_explorerElement, eca);
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

        public void Dispose()
        {
            _triggers.Clear();
        }
    }
}

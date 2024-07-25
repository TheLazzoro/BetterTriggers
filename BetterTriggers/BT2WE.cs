using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.Build.Script;
using BetterTriggers.Utility;

namespace BetterTriggers
{
    internal class BT2WE
    {
        private Dictionary<ExplorerElement, int> folderIds = new Dictionary<ExplorerElement, int>();

        /// <summary>
        /// Converts all project files from Better Triggers to triggers used by the World Editor.
        /// </summary>
        internal void Convert()
        {
            Project project = Project.CurrentProject;
            var triggers = project.Triggers.GetAll();
            var variables = project.Variables.GetAll();
            var actionDefinitions = project.ActionDefinitions.GetAll();
            var conditionDefinitions = project.ConditionDefinitions.GetAll();
            var functionDefinitions = project.FunctionDefinitions.GetAll();

            int localVarCount = triggers.Select(t => t.GetLocalVariables()).Count();
            int varCustomFeaturesCount = variables.Where(v => v.IsTwoDimensions).Count();

            if (
                localVarCount > 0
                || varCustomFeaturesCount > 0
                || actionDefinitions.Count > 0
                || conditionDefinitions.Count > 0
                || functionDefinitions.Count > 0
                )
            {
                throw new Exception("Map contains custom Better Triggers data. Conversion is not possible.");
            }

            RecurseThroughTriggers(project.GetRoot());
        }

        internal void RecurseThroughTriggers(ExplorerElement parent)
        {
            Project project = Project.CurrentProject;
            folderIds.TryGetValue(parent, out int parentId);
            var children = parent.GetExplorerElements();
            for (int i = 0; i < children.Count; i++)
            {
                var explorerElement = children[i];
                switch (explorerElement.ElementType)
                {
                    case ExplorerElementEnum.Folder:
                        int id = project.GenerateId();
                        folderIds.Add(explorerElement, id);
                        RecurseThroughTriggers(explorerElement);
                        break;
                    case ExplorerElementEnum.GlobalVariable:
                        break;
                    case ExplorerElementEnum.Root:
                        break;
                    case ExplorerElementEnum.Script:
                        break;
                    case ExplorerElementEnum.Trigger:
                        var trigger = explorerElement.trigger;
                        var triggerDefinition = new TriggerDefinition(TriggerItemType.Gui);
                        triggerDefinition.Id = explorerElement.GetId();
                        triggerDefinition.Name = explorerElement.GetName();
                        triggerDefinition.Description = explorerElement.trigger.Comment;
                        triggerDefinition.ParentId = parentId;
                        triggerDefinition.RunOnMapInit = trigger.RunOnMapInit;
                        triggerDefinition.Functions.AddRange(ConvertTriggerElements(trigger.Events));
                        triggerDefinition.Functions.AddRange(ConvertTriggerElements(trigger.Conditions));
                        triggerDefinition.Functions.AddRange(ConvertTriggerElements(trigger.Actions));
                        break;
                    default:
                        continue;
                }

            }
        }

        internal List<TriggerFunction> ConvertTriggerElements(TriggerElementCollection triggerElementCollection)
        {
            for (int i = 0; i < triggerElementCollection.Count(); i++)
            {
                var eca = triggerElementCollection.Elements[i] as ECA;
                TriggerFunction triggerFunction = new TriggerFunction();
                triggerFunction.

                if (eca.Elements != null && eca.Elements.Count > 0)
                {
                    foreach (var collection in eca.Elements)
                    {
                        ConvertTriggerElements(collection as TriggerElementCollection);
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Controllers
{
    public class ControllerReferences
    {
        /// <summary>
        /// Updates a trigger with all variable and trigger refs.
        /// </summary>
        public void UpdateReferences(ExplorerElementTrigger t)
        {
            ControllerTrigger controller = new ControllerTrigger();
            List<ExplorerElementVariable> explorerElementVariables = new List<ExplorerElementVariable>();

            ContainerReferences.RemoveReferrer(t);

            var parameters = controller.GetParametersFromTrigger(t);
            parameters.ForEach(p =>
            {
                if (p is VariableRef)
                {
                    VariableRef varRef = (VariableRef)p;
                    ExplorerElementVariable element = ContainerVariables.FindExplorerVariableById(varRef.VariableId);
                    ContainerReferences.AddReferrer(t, element);
                }
                else if (p is TriggerRef)
                {
                    TriggerRef tRef = (TriggerRef)p;
                    ExplorerElementTrigger element = ContainerTriggers.FindById(tRef.TriggerId);
                    ContainerReferences.AddReferrer(t, element);
                }
            });
        }

        public void UpdateReferencesAll() {
            ControllerTrigger controller = new ControllerTrigger();
            var triggers = controller.GetTriggersAll();
            triggers.ForEach(trigger => UpdateReferences(trigger));
        }

        public List<ExplorerElementTrigger> GetReferrers(IExplorerElement explorerElement) {
            return ContainerReferences.GetReferreres(explorerElement);
        }

        public void RemoveReferences(ExplorerElementTrigger t)
        {
            ContainerReferences.RemoveReferrer(t);
        }
    }
}
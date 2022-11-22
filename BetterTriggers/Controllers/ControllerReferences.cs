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

            References.RemoveReferrer(t);

            var parameters = controller.GetParametersFromTrigger(t);
            parameters.ForEach(p =>
            {
                if (p is VariableRef)
                {
                    VariableRef varRef = (VariableRef)p;
                    Variable element = Variables.GetVariableById(varRef.VariableId);
                    References.AddReferrer(t, element);
                }
                else if (p is TriggerRef)
                {
                    TriggerRef tRef = (TriggerRef)p;
                    ExplorerElementTrigger element = Triggers.FindById(tRef.TriggerId);
                    References.AddReferrer(t, element.trigger);
                }
            });
        }

        public void UpdateReferencesAll() {
            ControllerTrigger controller = new ControllerTrigger();
            var triggers = controller.GetTriggersAll();
            triggers.ForEach(trigger => UpdateReferences(trigger));
        }

        public List<ExplorerElementTrigger> GetReferrers(IReferable element) {
            return References.GetReferreres(element);
        }

        public void RemoveReferences(ExplorerElementTrigger t)
        {
            References.RemoveReferrer(t);
        }
    }
}
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.SaveableData;
using System.Collections.Generic;

namespace BetterTriggers.Models.EditorData
{
    /// <summary>
    /// A collection of variable or trigger references with the given Referable.
    /// Refs can be removed and re-added to the Function they were attached to.
    /// </summary>
    internal class RefCollection
    {
        List<RefParent> refParents = new List<RefParent>();
        List<ExplorerElementTrigger> triggersToUpdate;

        internal RefCollection(Variable variable)
        {
            CreateVarRefs(variable);
        }
     
        internal RefCollection(Trigger trigger)
        {
            CreateTrigRefs(trigger);
        }

        internal RefCollection(IExplorerElement explorerElement)
        {
            if (explorerElement is ExplorerElementVariable exVar)
                CreateVarRefs(exVar.variable);
            else if (explorerElement is ExplorerElementTrigger exTrig)
                CreateTrigRefs(exTrig.trigger);
        }

        private void CreateVarRefs(Variable variable)
        {
            this.triggersToUpdate = References.GetReferreres(variable);
            ControllerTrigger controllerProject = new ControllerTrigger();
            var functions = controllerProject.GetFunctionsAll();
            functions.ForEach(f =>
            {
                f.parameters.ForEach(p =>
                {
                    if (p is VariableRef varRef)
                    {
                        if (varRef.VariableId == variable.Id)
                        {
                            var refParent = new RefParent(varRef, f);
                            refParents.Add(refParent);
                        }
                    }
                });
            });
        }

        private void CreateTrigRefs(Trigger trigger)
        {
            this.triggersToUpdate = References.GetReferreres(trigger);
            ControllerTrigger controllerProject = new ControllerTrigger();
            var functions = controllerProject.GetFunctionsAll();
            functions.ForEach(f =>
            {
                f.parameters.ForEach(p =>
                {
                    if (p is TriggerRef trigRef)
                    {
                        if (trigRef.TriggerId == trigger.Id)
                        {
                            var refParent = new RefParent(trigRef, f);
                            refParents.Add(refParent);
                        }
                    }
                });
            });
        }

        internal void RemoveRefsFromParent()
        {
            refParents.ForEach(r => r.RemoveFromParent());
            triggersToUpdate.ForEach(t => t.Notify());
        }

        internal void AddRefsToParent()
        {
            refParents.ForEach(r => r.AddToParent());
            triggersToUpdate.ForEach(t => t.Notify());
        }

        internal void Notify()
        {
            triggersToUpdate.ForEach(t => t.Notify());
        }
    }

    internal class RefParent
    {
        Parameter parameter;
        Function parent;
        int index;
        internal RefParent(Parameter parameter, Function parent)
        {
            this.parameter = parameter;
            this.parent = parent;
            this.index = parent.parameters.IndexOf(parameter);
        }

        internal void RemoveFromParent()
        {
            parent.parameters.Remove(parameter);
            parent.parameters.Insert(index, new Parameter());
        }

        internal void AddToParent()
        {
            parent.parameters.RemoveAt(index);
            parent.parameters.Insert(index, parameter);
        }
    }
}

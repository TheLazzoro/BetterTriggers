using BetterTriggers.Containers;
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
        List<ExplorerElement> triggersToUpdate = new();

        internal RefCollection(Variable variable)
        {
            CreateVarRefs(variable);
        }

        internal RefCollection(Variable variable, string newType)
        {
            CreateVarRefs(variable, newType);
        }

        internal RefCollection(Trigger trigger)
        {
            CreateTrigRefs(trigger);
        }

        internal RefCollection(ExplorerElement explorerElement)
        {
            if (explorerElement.ElementType == ExplorerElementEnum.GlobalVariable)
                CreateVarRefs(explorerElement.variable);
            else if (explorerElement.ElementType == ExplorerElementEnum.Trigger)
                CreateTrigRefs(explorerElement.trigger);
        }

        private void CreateVarRefs(Variable variable, string newType = null)
        {
            this.triggersToUpdate = Project.CurrentProject.References.GetReferrers(variable);
            var functions = Project.CurrentProject.GetFunctionsAll();
            functions.ForEach(f =>
            {
                f.parameters.ForEach(p =>
                {
                    if (p is VariableRef varRef)
                    {
                        if (varRef.VariableId == variable.Id)
                        {
                            var refParent = new RefParent(varRef, f, newType);
                            refParents.Add(refParent);
                        }
                    }
                });
            });
        }

        private void CreateTrigRefs(Trigger trigger)
        {
            this.triggersToUpdate = Project.CurrentProject.References.GetReferrers(trigger);
            var functions = Project.CurrentProject.GetFunctionsAll();
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
        Parameter setvarOldValue; // hack for 'SetVariable' value undo/redo
        Function parent;
        int index;
        internal RefParent(Parameter parameter, Function parent, string newType = null)
        {
            this.parameter = parameter;
            this.parent = parent;
            this.index = parent.parameters.IndexOf(parameter);
            if (newType != null && parent.value == "SetVariable" && parameter == parent.parameters[0])
            {
                var varRef = (VariableRef)parameter;
                var variable = Project.CurrentProject.Variables.GetByReference(varRef);
                if (variable.Type != newType)
                    setvarOldValue = parent.parameters[1];
            }
        }

        internal void RemoveFromParent()
        {
            parent.parameters.Remove(parameter);
            parent.parameters.Insert(index, new Parameter());
            if(setvarOldValue != null)
                parent.parameters[1] = new Parameter();
        }

        internal void AddToParent()
        {
            parent.parameters.RemoveAt(index);
            parent.parameters.Insert(index, parameter);
            if (setvarOldValue != null)
                parent.parameters[1] = setvarOldValue;
        }
    }
}

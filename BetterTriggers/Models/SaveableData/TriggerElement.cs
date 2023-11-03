using BetterTriggers.Containers;
using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.SaveableData
{
    [JsonConverter(typeof(BaseConverterTriggerElement))]
    public class TriggerElement
    {
        /// <summary>
        /// Only directly set this field when the trigger loads the first time.
        /// Otherwise use SetParent().
        /// </summary>
        [JsonIgnore]
        private List<TriggerElement> Parent;
        [JsonIgnore]
        protected List<ITriggerElementUI> triggerElementUIs = new List<ITriggerElementUI>(); // TODO: Make this event-based instead of observer pattern. This should not know the UI in any sense.

        public virtual TriggerElement Clone()
        {
            return new TriggerElement();
        }

        public void SetParent(List<TriggerElement> Parent, int insertIndex)
        {
            Parent.Insert(insertIndex, this);
            this.Parent = Parent;
        }

        public void RemoveFromParent()
        {
            this.Parent.Remove(this);
            this.Parent = null;
        }

        public List<TriggerElement> GetParent()
        {
            return this.Parent;
        }

        public void SetParent(List<TriggerElement> parent)
        {
            this.Parent = parent;
        }

        public void Attach(ITriggerElementUI elementUI)
        {
            triggerElementUIs.Add(elementUI);
        }

        public void Detach(ITriggerElementUI elementUI)
        {
            triggerElementUIs.Remove(elementUI);
        }

        public void ChangedParams()
        {
            for (int i = 0; i < triggerElementUIs.Count; i++)
            {
                triggerElementUIs[i].UpdateParams();
            }
        }

        public void ChangedPosition()
        {
            for (int i = 0; i < triggerElementUIs.Count; i++)
            {
                triggerElementUIs[i].UpdatePosition();
            }
        }

        public void ChangedEnabled()
        {
            for (int i = 0; i < triggerElementUIs.Count; i++)
            {
                triggerElementUIs[i].UpdateEnabled();
            }
        }

        public void Created(int insertIndex)
        {
            if (this is LocalVariable localVar)
            {
                var variables = Project.CurrentProject.Variables;
                variables.AddLocalVariable(localVar);
            }
            for (int i = 0; i < triggerElementUIs.Count; i++)
            {
                triggerElementUIs[i].OnCreated(insertIndex);
            }
        }

        public void Deleted()
        {
            if (this is LocalVariable localVar)
            {
                var variables = Project.CurrentProject.Variables;
                variables.RemoveLocalVariable(localVar);
            }
            for (int i = 0; i < triggerElementUIs.Count; i++)
            {
                triggerElementUIs[i].OnDeleted();
            }
        }
    }
}

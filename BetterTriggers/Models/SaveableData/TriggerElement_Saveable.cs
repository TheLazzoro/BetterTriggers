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
    public class TriggerElement_Saveable
    {
        /// <summary>
        /// Only directly set this field when the trigger loads the first time.
        /// Otherwise use SetParent().
        /// </summary>
        [JsonIgnore]
        private List<TriggerElement_Saveable> Parent;
        [JsonIgnore]
        protected List<ITriggerElementUI_Saveable> triggerElementUIs = new List<ITriggerElementUI_Saveable>(); // TODO: Make this event-based instead of observer pattern. This should not know the UI in any sense.

        public virtual TriggerElement_Saveable Clone()
        {
            return new TriggerElement_Saveable();
        }

        public void SetParent(List<TriggerElement_Saveable> Parent, int insertIndex)
        {
            Parent.Insert(insertIndex, this);
            this.Parent = Parent;
        }

        public void RemoveFromParent()
        {
            this.Parent.Remove(this);
            this.Parent = null;
        }

        public List<TriggerElement_Saveable> GetParent()
        {
            return this.Parent;
        }

        public void SetParent(List<TriggerElement_Saveable> parent)
        {
            this.Parent = parent;
        }

        public void Attach(ITriggerElementUI_Saveable elementUI)
        {
            triggerElementUIs.Add(elementUI);
        }

        public void Detach(ITriggerElementUI_Saveable elementUI)
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
            if (this is LocalVariable_Saveable localVar)
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
            if (this is LocalVariable_Saveable localVar)
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

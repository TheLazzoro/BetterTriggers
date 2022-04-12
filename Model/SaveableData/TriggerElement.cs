using Model.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.SaveableData
{
    public class TriggerElement : ITriggerElement
    {
        public bool isEnabled;
        public Function function;

        [JsonIgnore]
        public List<TriggerElement> Parent;
        [JsonIgnore]
        private List<ITriggerElementUI> triggerElementUIs = new List<ITriggerElementUI>();

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
                triggerElementUIs[i].UpdateEnabled(isEnabled);
            }
        }
        
        public void Created()
        {
            for (int i = 0; i < triggerElementUIs.Count; i++)
            {
                triggerElementUIs[i].OnCreated();
            }
        }

        public void Deleted()
        {
            for (int i = 0; i < triggerElementUIs.Count; i++)
            {
                triggerElementUIs[i].OnDeleted();
            }
        }

    }
}

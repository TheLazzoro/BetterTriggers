using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ECA : TriggerElement
    {
        public bool isEnabled = true;
        public Function function = new Function();

        
        public ECA() { }
        public ECA(string value)
        {
            this.function.value = value;
        }

        public override ECA Clone()
        {
            ECA clone = new ECA();
            clone.isEnabled = isEnabled;
            clone.function = function.Clone();

            return clone;
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
            for (int i = 0; i < triggerElementUIs.Count; i++)
            {
                triggerElementUIs[i].OnCreated(insertIndex);
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

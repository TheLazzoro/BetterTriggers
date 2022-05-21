using Newtonsoft.Json;
using System.Collections.Generic;

namespace Model.SaveableData
{
    public class TriggerElement : ITriggerElement
    {
        public bool isEnabled = true;
        public Function function;

        /// <summary>
        /// Only directly set this field when the trigger loads the first time.
        /// Otherwise use SetParent().
        /// </summary>
        [JsonIgnore]
        public List<TriggerElement> Parent;
        [JsonIgnore]
        private List<ITriggerElementUI> triggerElementUIs = new List<ITriggerElementUI>();

        public TriggerElement Clone()
        {
            TriggerElement clone = new TriggerElement();
            clone.isEnabled = isEnabled;
            Function fClone;
            if (function is IfThenElse)
            {
                var ifThenElse = (IfThenElse)function;
                fClone = ifThenElse.Clone();
            }
            else if (function is ForLoopAMultiple)
            {
                var forLoopA = (ForLoopAMultiple)function;
                fClone = forLoopA.Clone();
            }
            else if (function is ForLoopBMultiple)
            {
                var forLoopB = (ForLoopBMultiple)function;
                fClone = forLoopB.Clone();
            }
            else if (function is ForLoopVarMultiple)
            {
                var forLoopVar = (ForLoopVarMultiple)function;
                fClone = forLoopVar.Clone();
            }
            else if (function is AndMultiple)
            {
                var AndMultiple = (AndMultiple)function;
                fClone = AndMultiple.Clone();
            }
            else if (function is OrMultiple)
            {
                var OrMultiple = (OrMultiple)function;
                fClone = OrMultiple.Clone();
            }
            else if (function is SetVariable)
            {
                var setVariable = (SetVariable)function;
                fClone = setVariable.Clone();
            }
            else if (function is EnumDestructablesInRectAllMultiple)
            {
                var enumDest = (EnumDestructablesInRectAllMultiple)function;
                fClone = enumDest.Clone();
            }
            else if (function is EnumDestructiblesInCircleBJMultiple)
            {
                var enumDest = (EnumDestructiblesInCircleBJMultiple)function;
                fClone = enumDest.Clone();
            }
            else
            {
                fClone = function.Clone();
            }

            clone.function = fClone;

            return clone;
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

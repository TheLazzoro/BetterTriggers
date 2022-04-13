using System;
using System.Collections.Generic;
using System.Text;

namespace Model.SaveableData
{
    public interface ITriggerElementUI
    {
        void UpdatePosition();
        void UpdateParams();
        void UpdateEnabled(bool isEnabled);
        void OnCreated(int insertIndex);
        void OnDeleted();
    }
}

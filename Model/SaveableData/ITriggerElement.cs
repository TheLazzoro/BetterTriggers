using System;
using System.Collections.Generic;
using System.Text;

namespace Model.SaveableData
{
    public interface ITriggerElement
    {
        void Attach(ITriggerElementUI elementUI);
        void Detach(ITriggerElementUI elementUI);
        void ChangedPosition();
        void ChangedParams();
        void ChangedEnabled();
        void Deleted();
        void Created();
    }
}

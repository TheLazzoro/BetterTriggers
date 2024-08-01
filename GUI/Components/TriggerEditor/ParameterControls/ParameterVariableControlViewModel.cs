using GUI.Components.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public class ParameterVariableControlViewModel : ViewModelBase
    {
        public ObservableCollection<VariableItem> Variables = new ObservableCollection<VariableItem>();
    }
}

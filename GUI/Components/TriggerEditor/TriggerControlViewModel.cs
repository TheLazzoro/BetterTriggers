using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.TriggerEditor
{
    public class TriggerControlViewModel
    {
        private ExplorerElement _explorerElement;
        private ObservableCollection<TriggerElement> _triggers;
        public ObservableCollection<TriggerElement> Elements
        {
            get
            {
                return _triggers;
            }
        }

        public TriggerControlViewModel(ExplorerElement explorerElement)
        {
            _explorerElement = explorerElement;
            var trigger = explorerElement.trigger;
            _triggers = new()
            {
                trigger.Events,
                trigger.Conditions,
                trigger.LocalVariables,
                trigger.Actions,
            };
        }
    }
}

using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.OpenMap
{
    public class OpenWar3MapViewModel
    {

        public ObservableCollection<TreeNodeBase> Maps { get; set; } = new();

        public OpenWar3MapViewModel()
        {
            
        }
    }
}

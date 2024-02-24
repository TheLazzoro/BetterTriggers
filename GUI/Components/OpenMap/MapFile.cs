using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.OpenMap
{
    public class MapFile : TreeNodeBase
    {
        public string DisplayName { get; private set; }
        public string FullPath { get; private set; }

        public MapFile(string fullPath)
        {
            FullPath = fullPath;
            DisplayName = Path.GetFileName(fullPath);
            SetCategory(TriggerCategory.TC_MAP);
        }
    }
}

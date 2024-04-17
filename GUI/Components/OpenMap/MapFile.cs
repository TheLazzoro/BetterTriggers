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
        public string FullPath { get; private set; }

        public MapFile(string fullPath)
        {
            FullPath = fullPath;
            DisplayText = Path.GetFileName(fullPath);
            CategoryStr = TriggerCategory.TC_MAP;
        }
    }
}

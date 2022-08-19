using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class War3ProjectFileEntry
    {
        public bool isEnabled;
        public bool isInitiallyOn;
        public bool isExpanded;
        public string path;
        public List<War3ProjectFileEntry> Files = new List<War3ProjectFileEntry>();
    }
}

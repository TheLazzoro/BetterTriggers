using System.Collections.Generic;

namespace Model.SaveableData
{
    public class War3ProjectFileEntry
    {
        public bool isEnabled;
        public bool isInitiallyOn;
        public string path;
        public List<War3ProjectFileEntry> Files = new List<War3ProjectFileEntry>();
    }
}

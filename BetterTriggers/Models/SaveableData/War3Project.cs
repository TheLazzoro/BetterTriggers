using Newtonsoft.Json;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class War3Project
    {
        public string Name;
        public int Version;
        public string Language;
        public string Comment;
        public string Header;
        public string War3MapDirectory;
        public List<War3ProjectFileEntry> Files;

        [JsonIgnore]
        public static readonly int EditorVersion = 0;
    }
}

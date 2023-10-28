using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class War3Project
    {
        public string Name;
        public int Version;
        public Version GameVersion;
        public string Language;
        public bool UseRelativeMapDirectory;
        public string Comment;
        public string Header;
        public string War3MapDirectory;
        public List<War3ProjectFileEntry> Files;

        /// <summary>
        /// Specifies the current editor version. Increment when new features to the file format are added.
        /// </summary>
        [JsonIgnore]
        public const int EditorVersion = 3;
    }
}

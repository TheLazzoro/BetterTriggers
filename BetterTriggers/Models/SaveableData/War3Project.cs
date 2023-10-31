using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using BetterTriggers.WorldEdit;
using Newtonsoft.Json.Linq;

namespace BetterTriggers.Models.SaveableData
{
    public class War3Project
    {
        public string Name;
        public int Version;
        public Version GameVersion;
        public string Language
        {
            get
            {
                // hack
                if (_language == "jass")
                    Info.SetLanguage(War3Net.Build.Info.ScriptLanguage.Jass);
                else
                    Info.SetLanguage(War3Net.Build.Info.ScriptLanguage.Lua);

                return _language;
            }
            set
            {
                if (value == "jass")
                    Info.SetLanguage(War3Net.Build.Info.ScriptLanguage.Jass);
                else
                    Info.SetLanguage(War3Net.Build.Info.ScriptLanguage.Lua);

                _language = value;
            }
        }
        public bool UseRelativeMapDirectory;
        public string Comment;
        public string Header;
        public string War3MapDirectory;
        public List<War3ProjectFileEntry> Files;

        private string _language;

        /// <summary>
        /// Specifies the current editor version. Increment when new features to the file format are added.
        /// </summary>
        [JsonIgnore]
        public const int EditorVersion = 3;
    }
}

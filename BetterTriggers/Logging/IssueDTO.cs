using BetterTriggers.WorldEdit.GameDataReader;
using System;

namespace BetterTriggers.Logging
{
    public class IssueDTO
    {
        public string Message { get; set; }
        public Version AppVersion { get; set; }
        public Version GameVersion { get; set; }

        public IssueDTO(string message)
        {
            Message = message;

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            AppVersion = new Version(version);
            GameVersion = WarcraftStorageReader.GameVersion;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.IO.Mpq;

namespace BetterTriggers.WorldEdit.GameDataReader
{
    internal class GameMpq
    {
        private Dictionary<string, MpqArchive> archives = null;
        private string[] files = new string[]{
                        "war3.mpq",
                        "War3x.mpq",
                        "War3xLocal.mpq",
                        "War3Patch.mpq" };

        public (bool, string) Load(string path)
        {
            string war3exe = Path.Combine(path, "Warcraft III.exe");
            var info = FileVersionInfo.GetVersionInfo(war3exe);
            WarcraftStorageReader.GameVersion = new Version(info.FileVersion.Replace(" ", string.Empty).Replace(',', '.'));

            string errorMsg = string.Empty;
            try
            {
                archives = new Dictionary<string, MpqArchive>();

                for (var i = 0; i < files.Length; i++)
                {
                    string fullPath = Path.Combine(path, files[i]);
                    try
                    {
                        var mpqArchive = MpqArchive.Open(fullPath);
                        archives.Add(files[i], mpqArchive);
                    }
                    catch (Exception)
                    {

                    }
                }

                return (true, errorMsg);
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
            }

            return (false, errorMsg);
        }

        public bool FileExists(string path)
        {
            foreach (var archive in archives.Values)
            {
                if (archive.FileExists(path))
                {
                    return true;
                }
            }
            return false;
        }

        public Stream Open(string path, string archiveName = null)
        {
            if(archiveName != null)
            {
                foreach (var archive in archives)
                {
                    if (archive.Key == archiveName && archive.Value.FileExists(path))
                    {
                        return MpqFile.OpenRead(archive.Value, path);
                    }
                }
            }

            foreach (var archive in archives.Values)
            {
                if (archive.FileExists(path))
                {
                    return MpqFile.OpenRead(archive, path);
                }
            }
            return null;
        }
    }
}

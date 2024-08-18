using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.IO.Mpq;

namespace BetterTriggers.WorldEdit.GameDataReader
{
    internal class GameMpq
    {
        private MpqArchive[] archives = null;
        private string[] files = new string[]{
                        "war3.mpq",
                        "War3x.mpq",
                        "War3xLocal.mpq",
                        "War3Patch.mpq" };

        public (bool, string) Load(string path)
        {
            string errorMsg = string.Empty;
            try
            {
                archives = new MpqArchive[4];
                
                for (var i = 0; i < files.Length; i++)
                {
                    archives[i] = MpqArchive.Open(files[i]);
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
            foreach (var archive in archives)
            {
                if (archive.FileExists(path))
                {
                    return true;
                }
            }
            return false;
        }

        public Stream Open(string path)
        {
            foreach (var archive in archives)
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

using BetterTriggers.WorldEdit.GameDataReader;
using CASCLib;

namespace BetterTriggers.Extensions;
public static class CascHandlerExtension
{
    public static bool FileExistsExt(this CASCHandler handler, string path)
    {
        path = path.Replace("\\", "/");
        var chunks = path.Split("/");
        CASCFolder? folder = null;
        if (WarcraftStorageReader.GameVersion >= WarcraftVersion._1_31)
        {
            folder = Casc.GetWar3ModFolder();
        }
        else if (WarcraftStorageReader.GameVersion >= WarcraftVersion._1_30)
        {
            folder = Casc.GetWar3MpqFolder_1_30();
        }

        if (folder == null) return false;

        foreach (var chunk in chunks)
        {
            var folderExists = folder.Folders.ContainsKey(chunk);
            if (folderExists)
            {
                folder = folder.Folders[chunk];
            }
            else
            {
                var fileExists = folder.Files.ContainsKey(chunk);
                if (fileExists) return true;
            }
        }

        return false;
    }
}

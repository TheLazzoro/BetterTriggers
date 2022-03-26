using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CASCLib;

namespace BetterTriggers.Casc
{
    public class Casc
    {
        bool _onlineMode = false;
        string product = "w3";
        string path = @"E:\Programmer\Warcraft III";

        public void test()
        {
            CASCConfig.LoadFlags |= LoadFlags.Install;
            CASCConfig config = _onlineMode ? CASCConfig.LoadOnlineStorageConfig(product, "eu") : CASCConfig.LoadLocalStorageConfig(path, product);

            var casc = CASCHandler.OpenStorage(config);

            casc.Root.SetFlags(LocaleFlags.enGB, false, false);


            using (var _ = new PerfCounter("LoadListFile()"))
            {
                casc.Root.LoadListFile("listfile.csv");
            }

            var fldr = casc.Root.SetFlags(LocaleFlags.enGB, false);
            casc.Root.MergeInstall(casc.Install);

            var war3_w3mod = (CASCFolder) fldr.Entries["War3.w3mod"];
            var replaceableTextures = (CASCFolder) war3_w3mod.Entries["replaceabletextures"];
            var commandButtons = (CASCFolder)replaceableTextures.Entries["commandbuttons"];
            var files = CASCFolder.GetFiles(commandButtons.Entries.Select(kv => kv.Value)).ToList();

            foreach (var entry in files)
            {
                var file = casc.OpenFile(entry.FullName);
                casc.SaveFileTo(entry.FullName, @"C:\Users\Lasse Dam\Desktop\Ny mappe (2)\");
                //file.read
            }



            GC.Collect();
        }
    }
}

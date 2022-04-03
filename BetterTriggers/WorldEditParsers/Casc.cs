using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CASCLib;

namespace BetterTriggers.WorldEditParsers
{
    public class Casc
    {
        private static bool _onlineMode = false;
        private static string product = "w3";
        private static string path = @"E:\Programmer\Warcraft III"; // we need to change this
        private static CASCFolder war3_w3mod;
        private static CASCHandler casc;

        public static CASCHandler GetCasc()
        {
            if (casc == null)
            {
                CASCConfig.LoadFlags |= LoadFlags.Install;
                CASCConfig config = _onlineMode ? CASCConfig.LoadOnlineStorageConfig(product, "eu") : CASCConfig.LoadLocalStorageConfig(path, product);

                casc = CASCHandler.OpenStorage(config);

                casc.Root.SetFlags(LocaleFlags.enGB, false, false);


                using (var _ = new PerfCounter("LoadListFile()"))
                {
                    casc.Root.LoadListFile("listfile.csv");
                }
            }

            return casc;
        }

        public static CASCFolder GetWar3ModFolder()
        {
            if (war3_w3mod == null)
            {
                var casc = GetCasc();
                var fldr = casc.Root.SetFlags(LocaleFlags.enGB, false);
                casc.Root.MergeInstall(casc.Install);
                war3_w3mod = (CASCFolder)fldr.Entries["War3.w3mod"];
            }

            return war3_w3mod;
        }


        public void test()
        {
            /*
            var replaceableTextures = (CASCFolder)war3_w3mod.Entries["replaceabletextures"];
            var commandButtons = (CASCFolder)replaceableTextures.Entries["commandbuttons"];
            files = CASCFolder.GetFiles(commandButtons.Entries.Select(kv => kv.Value)).ToList();

            foreach (var entry in files)
            {
                var file = casc.OpenFile(entry.FullName);
                //casc.SaveFileTo(entry.FullName, @"C:\Users\Lasse Dam\Desktop\Ny mappe (2)\");
                //file.read
            }
            */


            GC.Collect();
        }
    }
}

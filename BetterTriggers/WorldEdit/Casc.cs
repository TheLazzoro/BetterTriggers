using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CASCLib;

namespace BetterTriggers.WorldEdit
{
    public class Casc
    {
        private static bool _onlineMode = false;
        private static string product = "w3";
        private static CASCFolder war3_w3mod;
        private static CASCHandler casc;

        public static CASCHandler GetCasc()
        {
            if (casc == null)
            {
                // settings from BetterTriggers
                Settings settings = Settings.Load();

                CASCConfig.LoadFlags |= LoadFlags.Install;
                CASCConfig config = _onlineMode ? CASCConfig.LoadOnlineStorageConfig(product, "eu") : CASCConfig.LoadLocalStorageConfig(settings.war3root, product);

                casc = CASCHandler.OpenStorage(config);

                casc.Root.SetFlags(LocaleFlags.All, false, false);


                using (var _ = new PerfCounter("LoadListFile()"))
                {
                    casc.Root.LoadListFile("listfile.csv");
                }
            }

            return casc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Valid if true</returns>
        public static bool VerifyWc3Storage()
        {
            bool isValid = false;
            
            try
            {
                Settings settings = Settings.Load();

                CASCConfig.LoadFlags |= LoadFlags.Install;
                CASCConfig config = _onlineMode ? CASCConfig.LoadOnlineStorageConfig(product, "eu") : CASCConfig.LoadLocalStorageConfig(settings.war3root, product);
                var casc = CASCHandler.OpenStorage(config);
                casc.Root.SetFlags(LocaleFlags.enGB, false, false);
                using (var _ = new PerfCounter("LoadListFile()"))
                {
                    casc.Root.LoadListFile("listfile.csv");
                }

                isValid = true;
            }
            catch (Exception ex)
            {
            }

            return isValid;
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

        public static CASCFolder Getx86Folder()
        {
            if (war3_w3mod == null)
            {
                var casc = GetCasc();
                var fldr = casc.Root.SetFlags(LocaleFlags.enGB, false);
                casc.Root.MergeInstall(casc.Install);
                war3_w3mod = (CASCFolder)fldr.Entries["x86_64"];
            }

            return war3_w3mod;
        }

        public static void SaveFile(CASCFile file, string fullPath)
        {
            var stream = Casc.GetCasc().OpenFile(file.FullName);

            var dir = Path.GetDirectoryName(fullPath);
            var name = Path.GetFileName(fullPath);

            FileStream fileStream = File.Create(Path.Combine(dir, name), (int)stream.Length);
            byte[] bytesInStream = new byte[stream.Length];
            stream.Read(bytesInStream, 0, bytesInStream.Length);
            fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            fileStream.Close();
        } 

    }
}

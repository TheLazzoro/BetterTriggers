using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using War3Net.Build.Info;
using War3Net.Build;
using War3Net.IO.Mpq;

namespace BetterTriggers.TestMap
{
    public class Builder
    {
        public (bool, string) GenerateScript()
        {
            War3Project project = Project.CurrentProject.war3project;
            if (project == null)
                return (false, null);

            ScriptLanguage language = project.Language == "lua" ? ScriptLanguage.Lua : ScriptLanguage.Jass;

            ScriptGenerator scriptGenerator = new ScriptGenerator(language);
            bool success = scriptGenerator.GenerateScript();

            return (success, scriptGenerator.GeneratedScript);
        }

        string archivePath;
        /// <summary>
        /// Builds an MPQ archive.
        /// </summary>
        /// <returns>Full path of the archive.</returns>
        public bool BuildMap(string destinationDir = null)
        {
            (bool wasVerified, string script) = GenerateScript();
            if (!wasVerified)
                return false;

            War3Project project = Project.CurrentProject.war3project;
            ScriptLanguage language = project.Language == "lua" ? ScriptLanguage.Lua : ScriptLanguage.Jass;

            string mapDir = Project.CurrentProject.GetFullMapPath();
            var map = Map.Open(mapDir);
            map.Info.ScriptLanguage = language;
            map.Script = script;

            // We need to add all arbitrary files into to the builder.
            MapBuilder builder = new MapBuilder(map);
            if (Directory.Exists(mapDir))
                builder.AddFiles(mapDir, "*", SearchOption.AllDirectories);
            else
            {
                var mpqArchive = MpqArchive.Open(mapDir, true);
                builder.AddFiles(mpqArchive);
            }


            var archiveCreateOptions = new MpqArchiveCreateOptions
            {
                ListFileCreateMode = MpqFileCreateMode.Overwrite,
                AttributesCreateMode = MpqFileCreateMode.Prune,
                //BlockSize = 3,
            };

            string src = Path.GetDirectoryName(Project.CurrentProject.src);
            if (destinationDir == null)
                archivePath = Path.Combine(src, Path.Combine("dist", Path.GetFileName(mapDir)));
            else
            {
                EditorSettings settings = EditorSettings.Load();
                archivePath = Path.Combine(destinationDir, settings.CopyLocation + ".w3x");
            }

            bool didWrite = false;
            int attemptLimit = 1000;
            while (attemptLimit > 0 && !didWrite)
            {
                try
                {
                    builder.Build(archivePath, archiveCreateOptions);
                    didWrite = true;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(5);
                    attemptLimit--;
                }
            }
            if (!didWrite)
                throw new Exception("Could not write to map.");


            return true;
        }

        public void TestMap()
        {
            string destinationDir = Path.GetTempPath();
            bool success = BuildMap(destinationDir);
            if (!success)
                return;

            EditorSettings settings = EditorSettings.Load();
            string war3Exe = Path.Combine(settings.war3root, "_retail_/x86_64/Warcraft III.exe");

            int difficulty = settings.Difficulty;
            string windowMode;
            switch (settings.WindowMode)
            {
                case 0:
                    windowMode = "windowed";
                    break;
                case 1:
                    windowMode = "windowedfullscreen";
                    break;
                default:
                    windowMode = "fullscreen";
                    break;
            }
            int hd = settings.HD;
            int teen = settings.Teen;
            string playerProfile = settings.PlayerProfile;
            int fixedseed = settings.FixedRandomSeed == true ? 1 : 0;
            string nowfpause = settings.NoWindowsFocusPause == true ? "-nowfpause " : "";

            string launchArgs = $"-launch " +
                $"-mapdiff {difficulty} " +
                $"-windowmode {windowMode} " +
                $"-hd {hd} " +
                $"-teen {teen} " +
                $"-testmapprofile {playerProfile} " +
                $"-fixedseed {fixedseed} " +
                $"{nowfpause}";

            Process.Start($"\"{war3Exe}\" {launchArgs} -loadfile \"{archivePath}\"");
        }

    }
}

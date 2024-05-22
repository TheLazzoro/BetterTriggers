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
using JassObfuscator;
using War3Net.IO.Compression;

namespace BetterTriggers.TestMap
{
    public class Builder
    {
        private ScriptLanguage _language;

        public Builder()
        {
            War3Project project = Project.CurrentProject.war3project;
            _language = project.Language == "lua" ? ScriptLanguage.Lua : ScriptLanguage.Jass;
        }

        public (bool, string) GenerateScript()
        {
            War3Project project = Project.CurrentProject.war3project;
            if (project == null)
                return (false, null);

            ScriptGenerator scriptGenerator = new ScriptGenerator(_language);
            bool success = scriptGenerator.GenerateScript();

            return (success, scriptGenerator.GeneratedScript);
        }

        string archivePath;
        /// <summary>
        /// Builds an MPQ archive.
        /// </summary>
        public bool BuildMap(string destinationDir = null, bool includeMPQSettings = false)
        {
            EditorSettings settings = EditorSettings.Load();
            (bool wasVerified, string script) = GenerateScript();
            if (!wasVerified)
                return false;

            if (includeMPQSettings)
            {
                if (settings.Export_Obfuscate && _language == ScriptLanguage.Jass)
                {
                    string commonJ = ScriptGenerator.PathCommonJ;
                    string blizzardJ = ScriptGenerator.PathBlizzardJ;
                    script = Obfuscator.Obfuscate(script, commonJ, blizzardJ);
                }
            }

            string mapDir = Project.CurrentProject.GetFullMapPath();
            var map = Map.Open(mapDir);
            map.Info.ScriptLanguage = _language;
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

            // MPQ protection
            if (includeMPQSettings)
            {
                if (settings.Export_RemoveTriggerData)
                {
                    map.Triggers = null;
                }

            }

            ushort blockSize = 3;
            if (settings.Export_Compress)
                blockSize = 8;
            if (settings.Export_Compress && settings.Export_Compress_Advanced)
                blockSize = settings.Export_Compress_BlockSize;

            var archiveCreateOptions = new MpqArchiveCreateOptions
            {
                ListFileCreateMode = MpqFileCreateMode.Overwrite,
                AttributesCreateMode = MpqFileCreateMode.Prune,
                BlockSize = blockSize,
            };

            string src = Path.GetDirectoryName(Project.CurrentProject.src);
            if (destinationDir == null)
                archivePath = Path.Combine(src, Path.Combine("dist", Path.GetFileName(mapDir)));
            else
            {
                archivePath = Path.Combine(destinationDir, settings.CopyLocation + ".w3x");
            }

            bool didWrite = false;
            int attemptLimit = 10;
            Exception err = new Exception();
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
                    if (attemptLimit <= 0)
                    {
                        err = ex;
                    }
                }
            }
            if (!didWrite)
                throw err;

            if (includeMPQSettings)
            {
                if (settings.Export_RemoveListfile)
                {
                    string tempFile = Path.Combine(GetProgramTempPath(), "tempfile.w3x");
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);

                    IEnumerable<MpqFile> mpqFiles = null;
                    using (var stream = new FileStream(archivePath, FileMode.Open))
                    {
                        File.Copy(archivePath, tempFile);
                        using (var stream2 = new FileStream(tempFile, FileMode.Open))
                        {
                            var archive = MpqArchive.Open(stream2, false);
                            mpqFiles = archive.GetMpqFiles();
                        }
                    }
                    File.Delete(archivePath);
                    using (var s = new FileStream(archivePath, FileMode.Create))
                    {
                        MpqArchive.Create(s, mpqFiles, archiveCreateOptions);
                    }
                }
            }

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


        private string GetProgramTempPath()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "BetterTriggers");
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            return tempPath;
        }
    }
}

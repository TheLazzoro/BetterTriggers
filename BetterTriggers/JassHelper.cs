using System;
using System.Diagnostics;
using System.IO;

namespace BetterTriggers
{
    public static class JassHelper
    {
        public static void SaveVJassScript(string path, string script)
        {
            File.WriteAllText(path, script);
        }

        public static void RunJassHelper(string JassHelperEXE, string commonJ, string blizzardJ, string inputScriptFile, string outputScriptFile)
        {
            Process p = Process.Start($"{JassHelperEXE}", $"--scriptonly {commonJ} {blizzardJ} {inputScriptFile} {outputScriptFile}");
            p.WaitForExit();
        }
    }
}

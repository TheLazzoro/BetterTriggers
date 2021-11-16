using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers2
{
    public static class JassHelper
    {
        public static void SaveScript(string JassHelperEXE, string commonJ, string blizzardJ, string inputScriptFile, string outputScriptFile)
        {
            Process p = Process.Start($"{JassHelperEXE}", $"--scriptonly {commonJ} {blizzardJ} {inputScriptFile} {outputScriptFile}");
            p.WaitForExit();
        }
    }
}

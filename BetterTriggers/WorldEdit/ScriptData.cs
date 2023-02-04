using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BetterTriggers.WorldEdit
{
    public static class ScriptData
    {
        public class Native
        {
            public string displayText { get; internal set; }
            public string description { get; internal set; }

            public Native()
            {
                Natives.Add(this);
            }
        }

        public static List<Native> Natives = new List<Native>();

        internal static void Load()
        {
            string[] commonJ = File.ReadAllLines(TriggerData.pathCommonJ);
            List<string> types = new List<string>();
            List<string> constantNatives = new List<string>();
            List<string> constants = new List<string>();
            List<string> natives = new List<string>();
            for (int i = 0; i < commonJ.Length; i++)
            {
                commonJ[i] = Regex.Replace(commonJ[i], @"\s+", " ");
                if (commonJ[i].StartsWith("type"))
                {
                    types.Add(commonJ[i]);
                }
                else if (commonJ[i].StartsWith("constant native"))
                {
                    constantNatives.Add(commonJ[i]);
                }
                else if (commonJ[i].StartsWith("constant"))
                {
                    constants.Add(commonJ[i]);
                }
                else if (commonJ[i].StartsWith("native"))
                {
                    natives.Add(commonJ[i]);
                }
            }

            // Types
            for (int i = 0; i < types.Count; i++)
            {
                string[] type = types[i].Split(" ");
                new Native
                {
                    displayText = type[1],
                    description = types[i],
                };
            }

            // Constant Natives
            for (int i = 0; i < constantNatives.Count; i++)
            {
                string[] constantNative = constantNatives[i].Split(" ");
                new Native
                {
                    displayText = constantNative[2],
                    description = constantNatives[i],
                };
            }

            // Constants
            for (int i = 0; i < constants.Count; i++)
            {
                string[] constant = constants[i].Split(" ");
                new Native
                {
                    displayText = constant[1],
                    description = constants[i],
                };
            }

            // Natives
            for (int i = 0; i < natives.Count; i++)
            {
                string[] native = natives[i].Split(" ");
                new Native
                {
                    displayText = native[1],
                    description = natives[i],
                };
            }
        }
    }
}

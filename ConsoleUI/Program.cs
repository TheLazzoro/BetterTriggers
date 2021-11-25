using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {

            // want to find difference between a map where rifleman is and isn't selected in the trigger editor

            var noRifleman = File.ReadAllText(@"C:\Users\Lasse Dam\Desktop\mapa3.w3x - Kopi\war3map.wtg");
            var withRifleman = File.ReadAllText(@"C:\Users\Lasse Dam\Desktop\mapa3.w3x\war3map.wtg");
            var withRiflemanAndHero = File.ReadAllText(@"C:\Users\Lasse Dam\Desktop\mapa4.w3x\war3map.wtg");

            bool foundDiff = false;
            int i = 0;
            while (!foundDiff && i < noRifleman.Length)
            {
                char noRif = noRifleman[i];
                char withRif = withRifleman[i];

                if (noRif != withRif)
                    foundDiff = true;

                    i++;
            }

            Console.WriteLine("Found diff at: " + i);
            Console.WriteLine("-----------------------------");
            Console.WriteLine(withRifleman.Substring(i, withRifleman.Length - i - 1));
            Console.ReadLine();


            /*

            //Reader.ReadCommonJ();
            //Console.ReadLine();

            string commonJ = "\"C:/Users/Lasse Dam/Desktop/JassHelper Experiement/common.j\"";
            string blizzardJ = "\"C:/Users/Lasse Dam/Desktop/JassHelper Experiement/Blizzard.j\"";
            string input = "\"C:/Users/Lasse Dam/Desktop/JassHelper Experiement/test.j\"";
            string output = "\"C:/Users/Lasse Dam/Desktop/JassHelper Experiement/result.j\"";

            Process p = Process.Start("C:/Users/Lasse Dam/Desktop/JassHelper Experiement/jasshelper.exe", $"--scriptonly {commonJ} {blizzardJ} {input} {output}");
            p.WaitForExit();

            bool exists = File.Exists("C:/Users/Lasse Dam/Desktop/JassHelper Experiement/result.j");

            Console.WriteLine("finished");
            Console.WriteLine(exists);
            Console.ReadLine();
            */
        }
    }
}

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
        }
    }
}

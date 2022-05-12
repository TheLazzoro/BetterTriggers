using BetterTriggers;
using BetterTriggers.WorldEdit;
using System;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings settings = new Settings();
            settings.war3root = @"E:\Programmer\Warcraft III";
            Settings.Save(settings);

            BetterTriggers.Init.Initialize();
            
            CustomMapData.Load();

            Casc c = new Casc();
            c.test();

            Console.WriteLine("Hello World!");
        }
    }
}

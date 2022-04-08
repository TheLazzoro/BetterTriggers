using BetterTriggers.WorldEdit;
using System;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Units.Load();
            Cameras.Load();
            BetterTriggers.Init.Initialize();

            Casc c = new Casc();
            c.test();

            Console.WriteLine("Hello World!");
        }
    }
}

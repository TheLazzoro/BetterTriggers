using BetterTriggers.WorldEdit;
using System;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            BetterTriggers.Init.Initialize();
            
            Casc c = new Casc();
            c.test();

            Console.WriteLine("Hello World!");

            Units parser = new Units();
            parser.ParseUnits();
        }
    }
}

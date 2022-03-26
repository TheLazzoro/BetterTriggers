using BetterTriggers.Casc;
using System;
using WorldEditParser;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Casc c = new Casc();
            c.test();


            Console.WriteLine("Hello World!");

            UnitParser parser = new UnitParser();
            parser.ParseUnits();
        }
    }
}

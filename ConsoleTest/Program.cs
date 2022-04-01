using BetterTriggers.WorldEditParsers;
using System;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            UnitDataParser unitDataParser = new UnitDataParser();
            unitDataParser.ParseUnitData();
            
            Casc c = new Casc();
            c.test();

            Console.WriteLine("Hello World!");

            UnitParser parser = new UnitParser();
            parser.ParseUnits();
        }
    }
}

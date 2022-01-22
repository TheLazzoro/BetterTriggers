using System;
using WorldEditParser;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            UnitParser parser = new UnitParser();
            parser.ParseUnits();
        }
    }
}

using BetterTriggers.WorldEdit;
using System;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AbilityData abilityDataParser = new AbilityData();
            abilityDataParser.ParseAbilityData();

            UnitData unitDataParser = new UnitData();
            unitDataParser.ParseUnitData();
            
            Casc c = new Casc();
            c.test();

            Console.WriteLine("Hello World!");

            Units parser = new Units();
            parser.ParseUnits();
        }
    }
}

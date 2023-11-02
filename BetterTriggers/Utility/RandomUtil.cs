using System;

namespace BetterTriggers.Utility
{
    internal static class RandomUtil
    {
        internal static int GenerateInt()
        {
            Random rand = new Random();
            return rand.Next();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BetterTriggers
{
    public class Event : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

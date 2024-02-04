using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.SaveableData
{
    public class InvalidECA_Saveable : ECA_Saveable
    {
        public InvalidECA_Saveable()
        {
            function.value = "InvalidECA";
        }
    }
}

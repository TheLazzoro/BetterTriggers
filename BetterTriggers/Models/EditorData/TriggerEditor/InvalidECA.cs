﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class InvalidECA : ECA
    {
        public InvalidECA()
        {
            function.value = "InvalidECA";
        }
    }
}
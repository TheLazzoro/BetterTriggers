﻿using BetterTriggers.JsonBaseConverter;
using BetterTriggers.Models.EditorData;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class LocalVariable_Saveable : TriggerElement_Saveable
    {
        public readonly int LocalVar = 1; // DO NOT CHANGE
        public Variable_Saveable variable = new Variable_Saveable();

        public LocalVariable_Saveable()
        {
            variable.IsArray = false; // forces locals to be non-arrays
        }

        public LocalVariable_Saveable(Trigger_Saveable trig)
        {
            variable.IsArray = false; // forces locals to be non-arrays
        }
    }
}

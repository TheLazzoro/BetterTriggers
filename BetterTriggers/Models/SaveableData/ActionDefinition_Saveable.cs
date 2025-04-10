﻿using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.SaveableData
{
    public class ActionDefinition_Saveable
    {
        public int Id;
        public string Comment;
        public string Category = TriggerCategory.TC_NOTING; // default
        public List<ParameterDefinition_Saveable> Parameters = new();
        public List<TriggerElement_Saveable> Actions = new();
        public List<TriggerElement_Saveable> LocalVariables = new();
    }
}
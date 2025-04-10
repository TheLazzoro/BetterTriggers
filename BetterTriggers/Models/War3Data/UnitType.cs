﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace BetterTriggers.Models.War3Data
{
    public class UnitType
    {
        public string Id;
        public UnitName Name;
        public string Sort; // category
        public string Race;
        public string Icon;
        public string Model;
        public bool isSpecial;
        public bool isCampaign;
        public byte[] Image;
    }
}

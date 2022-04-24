using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Model.War3Data
{
    public class UnitType
    {
        public string Id;
        public string Sort; // category
        public string Race;
        public string Icon;
        public string Model;
        public bool isSpecial;
        public Stream Image;
    }
}

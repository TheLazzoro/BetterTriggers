using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Utility.IniParser
{
    internal class IniSection
    {
        internal string SectionName;
        internal List<IniKey> Keys = new List<IniKey>();

    }
}

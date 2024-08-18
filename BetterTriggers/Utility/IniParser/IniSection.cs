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
        internal Dictionary<string, IniKey> Keys = new Dictionary<string, IniKey>();

    }
}

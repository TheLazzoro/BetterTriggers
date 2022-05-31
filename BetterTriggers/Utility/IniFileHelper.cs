using IniParser.Model;
using IniParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Utility
{
    internal static class IniFileHelper
    {
        internal static IniData Parse(string content)
        {
            IniDataParser parser = new IniDataParser();
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowDuplicateKeys = true;
            return parser.Parse(content);
        }
    }
}

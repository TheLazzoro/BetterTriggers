using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace BetterTriggers.Utility.IniParser
{
    internal class IniSection
    {
        internal string SectionName { get; }
        internal List<IniKey> Keys = new List<IniKey>();

        public IniSection(string sectionName)
        {
            SectionName = sectionName;
        }

        public string this[string keyName]
        {
            get
            {
                var iniKey = Keys.FirstOrDefault(x => x.KeyName == keyName);
                if (iniKey != null)
                {
                    return iniKey.Value;
                }

                return null;
            }
        }
    }
}

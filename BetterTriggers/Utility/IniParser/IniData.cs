using System.Collections.Generic;

namespace BetterTriggers.Utility.IniParser
{
    internal class IniData
    {
        internal Dictionary<string, IniSection> Sections = new Dictionary<string, IniSection>();

        public IniData(string data)
        {
            // fix blizzard ini file
            string[] lines = data.Split("\r\n");
            string initFile = string.Empty;
            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].Contains("=") && !lines[i].Contains("["))
                    lines[i] = "";
            }

            IniSection section = null;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.StartsWith("//") || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (line.StartsWith("["))
                {
                    
                    int nameEndIndex = 0;
                    for (int k = 0; k < line.Length; k++)
                    {
                        var c = line[k];
                        if (c == ']')
                        {
                            nameEndIndex = k;
                            break;
                        }
                    }

                    var sectionName = line.Substring(1, nameEndIndex - 1);
                    section = new IniSection(sectionName);
                    Sections.TryAdd(section.SectionName, section);
                }

                // read keys

                int keyEndIndex = 0;
                int valueStartIndex = 0;
                for (int k = 0; k < line.Length; k++)
                {
                    var c = line[k];
                    if (c == '=')
                    {
                        keyEndIndex = k;
                        valueStartIndex = k + 1;
                        break;
                    }
                }

                string key = line.Substring(0, keyEndIndex);
                if (key == string.Empty) continue;
                string value = line.Substring(valueStartIndex, line.Length - valueStartIndex);
                var iniKey = new IniKey(key, value);
                section.Keys.Add(iniKey);
            }
        }

        internal bool ContainsSection(string sectionName)
        {
            return Sections.ContainsKey(sectionName);
        }

        public bool AddSection(string keyName)
        {
            if (!ContainsSection(keyName))
            {
                Sections.Add(keyName, new IniSection(keyName));
                return true;
            }

            return false;
        }

        public IniSection? this[string sectionName]
        {
            get
            {
                if (!ContainsSection(sectionName))
                {
                    //if (!Configuration.AllowCreateSectionsOnFly)
                    //{
                    //    return null;
                    //}

                    AddSection(sectionName);
                }

                return Sections.GetValueOrDefault(sectionName);
            }
        }

    }
}

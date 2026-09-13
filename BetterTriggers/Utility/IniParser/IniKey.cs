namespace BetterTriggers.Utility.IniParser
{
    internal class IniKey
    {
        internal string KeyName;
        internal string Value;

        public IniKey(string key, string value)
        {
            KeyName = key;
            Value = value;
        }
    }
}

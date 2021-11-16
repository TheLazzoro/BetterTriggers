using System.Collections;

namespace BetterTriggers.Data
{
    public class Types
    {
        public static Hashtable collection = new Hashtable();
        public string name;

        public Types(string name)
        {
            this.name = name;
            collection.Add(name, this);
        }
    }
}

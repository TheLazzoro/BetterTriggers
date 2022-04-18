using Model.SaveableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Utility
{
    public class SearchObject
    {
        public object Object;
        public List<string> Words;
    }

    public class SearchObjects
    {
        private List<SearchObject> Items; 

        public SearchObjects(List<SearchObject> searchObjects)
        {
            Items = searchObjects;
        }

        public List<SearchObject> Search(string word)
        {
            string wordToLower = word.ToLower();
            List<SearchObject> list = new List<SearchObject>();

            for (int i = 0; i < Items.Count; i++)
            {
                int j = 0;
                bool isMatch = false;
                while(j < Items[i].Words.Count && !isMatch)
                {
                    if(Items[i].Words[j].Contains(wordToLower))
                    {
                        isMatch = true;
                        list.Add(Items[i]);
                    }
                    j++;
                }
            }

            return list;
        }
    }
}

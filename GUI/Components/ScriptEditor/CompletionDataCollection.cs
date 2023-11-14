using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.ScriptEditor
{
    public class CompletionDataCollection
    {
        List<CompletionData> completionData;

        public CompletionDataCollection(List<CompletionData> completionData)
        {
            this.completionData = completionData;
        }

        public List<CompletionData> Search(string word)
        {
            List<CompletionData> list = new List<CompletionData>();
            word = word.ToLower();
            for (int i = 0; i < completionData.Count; i++)
            {
                if (completionData[i].Text.ToLower().Contains(word) || word == string.Empty)
                    list.Add(completionData[i]);
            }

            return list;
        }
    }
}

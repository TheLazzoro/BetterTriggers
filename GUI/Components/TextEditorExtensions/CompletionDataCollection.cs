using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.TextEditorExtensions
{
    public class CompletionDataCollection
    {
        List<MyCompletionData> completionData;

        public CompletionDataCollection(List<MyCompletionData> completionData)
        {
            this.completionData = completionData;
        }

        public List<MyCompletionData> Search(string word)
        {
            List<MyCompletionData> list = new List<MyCompletionData>();
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

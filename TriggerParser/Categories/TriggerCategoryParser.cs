using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerParser.Categories
{
    public static class TriggerCategoryParser
    {
        public static void ParseCategories(string file)
        {
            IEnumerable<string> lines = File.ReadLines(file);

            foreach (string line in lines)
            {
                string key = string.Empty;
                string displayText = string.Empty;
                string iconImageFile = string.Empty;
                string isDisabled = "";

                int memberIndex = 0;

                // read line
                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];
                    bool isSeperator = false;

                    if (c == '=' || c == ',')
                        isSeperator = true;

                    if (!isSeperator)
                    {
                        switch (memberIndex)
                        {
                            case 0:
                                key += c;
                                break;
                            case 1:
                                displayText += c;
                                break;
                            case 2:
                                iconImageFile += c;
                                break;
                            case 3:
                                isDisabled += c;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        memberIndex++;
                    }
                }

                int intIsDisabled = 0;
                if(isDisabled != "")
                    intIsDisabled = int.Parse(isDisabled);

                var category = new TriggerCategory()
                {
                    key = key,
                    displayText = displayText,
                    iconImageFile = iconImageFile,
                    isDisabled = intIsDisabled
                };

                TriggerCategoryContainer.container.Add(category);
            }
        }
    }
}

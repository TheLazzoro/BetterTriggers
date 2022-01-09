using Model.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace GUI.Controllers
{
    /// <summary>
    /// Responisble for loading trigger data from .json files
    /// </summary>
    public class ControllerTriggerData
    {
        public List<ComboBoxItem> LoadVariableTypes()
        {
            string file = File.ReadAllText(@"C:\Users\Lasse Dam\Desktop\JSON\types.json");
            List<TriggerType> triggerTypes = JsonConvert.DeserializeObject<List<TriggerType>>(file);

            List<ComboBoxItem> list = new List<ComboBoxItem>();
            for (int i = 0; i < triggerTypes.Count; i++)
            {
                // TODO: We may want to make this combobox item a class that inherits ComboBoxItem,
                // so we can specify custom fields like 'key' which is used by the script for type checking
                ComboBoxItem item = new ComboBoxItem()
                {
                    Content = triggerTypes[i].displayname,
                    Tag = triggerTypes[i].key, 
                };

                list.Add(item);
            }

            return list;
        }
    }
}

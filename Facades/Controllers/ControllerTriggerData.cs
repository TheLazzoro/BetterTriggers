using Model.Data;
using Model.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Facades.Controllers
{
    /// <summary>
    /// Responisble for loading trigger data from .json files
    /// </summary>
    public class ControllerTriggerData
    {
        /*
        public List<ComboBoxItemType> LoadVariableTypes()
        {
            string file = File.ReadAllText(@"C:\Users\Lasse Dam\Desktop\JSON\types.json");
            List<TriggerType> triggerTypes = JsonConvert.DeserializeObject<List<TriggerType>>(file);

            List<ComboBoxItemType> list = new List<ComboBoxItemType>();
            for (int i = 0; i < triggerTypes.Count; i++)
            {
                // TODO: We may want to make this combobox item a class that inherits ComboBoxItem,
                // so we can specify custom fields like 'key' which is used by the script for type checking
                ComboBoxItemType item = new ComboBoxItemType()
                {
                    Content = triggerTypes[i].displayname,
                    Type = triggerTypes[i].key,
                };

                list.Add(item);
            }

            return list;
        }
        */

        /// <summary>
        ///     
        /// </summary>
        /// <param name="filepath">Expects a .json file</param>
        /// <returns></returns>
        public List<FunctionTemplate> LoadAllEvents(string filepath)
        {
            string filePlainText = File.ReadAllText(filepath);
            List<FunctionTemplate> list = JsonConvert.DeserializeObject<List<FunctionTemplate>>(filePlainText);

            return list;
        }

        public List<FunctionTemplate> LoadAllFunctions(string filepath)
        {
            string filePlainText = File.ReadAllText(filepath);
            List<FunctionTemplate> list = JsonConvert.DeserializeObject<List<FunctionTemplate>>(filePlainText);

            return list;
        }

        public List<ConstantTemplate> LoadAllConstants(string filepath)
        {
            string filePlainText = File.ReadAllText(filepath);
            List<ConstantTemplate> list = JsonConvert.DeserializeObject<List<ConstantTemplate>>(filePlainText);

            return list;
        }

        public List<FunctionTemplate> LoadAllConditions(string filepath)
        {
            string filePlainText = File.ReadAllText(filepath);
            List<FunctionTemplate> list = JsonConvert.DeserializeObject<List<FunctionTemplate>>(filePlainText);

            return list;
        }
    }
}

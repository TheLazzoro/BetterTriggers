using Model.Natives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class LoadData
    {
        /// <summary>
        ///     
        /// </summary>
        /// <param name="filepath">Expects a .json file</param>
        /// <returns></returns>
        public static List<Function> LoadAllEvents(string filepath)
        {
            string filePlainText = File.ReadAllText(filepath);
            List<Function> list = JsonConvert.DeserializeObject<List<Function>>(filePlainText);

            return list;
        }

        public static List<Function> LoadAllFunctions(string filepath)
        {
            string filePlainText = File.ReadAllText(filepath);
            List<Function> list = JsonConvert.DeserializeObject<List<Function>>(filePlainText);

            return list;
        }

        public static List<Constant> LoadAllConstants(string filepath)
        {
            string filePlainText = File.ReadAllText(filepath);
            List<Constant> list = JsonConvert.DeserializeObject<List<Constant>>(filePlainText);

            return list;
        }

        public static List<Function> LoadAllConditions(string filepath)
        {
            string filePlainText = File.ReadAllText(filepath);
            List<Function> list = JsonConvert.DeserializeObject<List<Function>>(filePlainText);

            return list;
        }
    }
}

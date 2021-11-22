using DataAccess.Natives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public static class LoadData
    {
        /// <summary>
        ///     
        /// </summary>
        /// <param name="filepath">Expects a .json file</param>
        /// <returns></returns>
        public static List<Event> LoadAllEvents(string filepath)
        {
            string filePlainText = File.ReadAllText(filepath);
            List<Event> list = JsonConvert.DeserializeObject<List<Event>>(filePlainText);

            return list;
        }
    }
}

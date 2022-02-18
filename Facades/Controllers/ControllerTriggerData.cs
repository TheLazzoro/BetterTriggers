using Facades.Containers;
using Model.Data;
using Model.EditorData;
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
        public List<VariableType> LoadVariableTypes()
        {
            return ContainerTriggerData.VariableTypes;
        }

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

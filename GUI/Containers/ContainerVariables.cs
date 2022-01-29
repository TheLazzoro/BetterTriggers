using GUI.Components.TriggerExplorer;
using Model.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerVariables
    {
        private static List<ExplorerElement> triggerVariableContainer = new List<ExplorerElement>();
        private static List<Variable> variableContainer = new List<Variable>(); // mirror indexing with above

        public static void AddTriggerElement(ExplorerElement variable)
        {
            triggerVariableContainer.Add(variable);

            string json = File.ReadAllText(variable.FilePath);
            var model = JsonConvert.DeserializeObject<Variable>(json);

            variableContainer.Add(model);

        }

        public static int Count()
        {
            return triggerVariableContainer.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public static bool Contains(string name)
        {
            bool found = false;

            foreach (var item in triggerVariableContainer)
            {
                if (item.ElementName == name)
                {
                    found = true;
                }
            }

            return found;
        }

        public static int GenerateId()
        {
            int generatedId = 0;
            bool isIdValid = false;
            while (!isIdValid)
            {
                int i = 0;
                bool doesIdExist = false;
                while (!doesIdExist && i < triggerVariableContainer.Count)
                {
                    if (variableContainer[i].Id == generatedId)
                        doesIdExist = true;
                    else
                        i++;
                }

                if (!doesIdExist)
                    isIdValid = true;
                else
                    generatedId++;
            }

            return generatedId;
        }

        public static string GetVariableNameById(int Id)
        {
            string name = string.Empty;

            bool found = false;
            int i = 0;
            while (!found && i < triggerVariableContainer.Count)
            {
                if(variableContainer[i].Id == Id)
                {
                    name = Path.GetFileNameWithoutExtension(triggerVariableContainer[i].FilePath);
                    found = true;
                }

                i++;
            }

            return name;
        }

        public static ExplorerElement Get(int index)
        {
            return triggerVariableContainer[index];
        }

        public static void Remove(ExplorerElement explorerElement)
        {
            int index = triggerVariableContainer.IndexOf(explorerElement);
            triggerVariableContainer.Remove(explorerElement);
            variableContainer.RemoveAt(index);
        }

        public static void RemoveByFilePath(string filePath)
        {
            for (int i = 0; i < triggerVariableContainer.Count; i++)
            {
                var item = triggerVariableContainer[i];
                if (item.FilePath == filePath)
                {
                    Remove(item);
                }
            }
        }
    }
}

using Model.EditorData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Facades.Containers
{
    public static class ContainerVariables
    {
        public static List<ExplorerElementVariable> variableContainer = new List<ExplorerElementVariable>();

        public static void AddVariable(ExplorerElementVariable variable)
        {
            variableContainer.Add(variable);
        }

        public static int Count()
        {
            return variableContainer.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public static bool Contains(string name)
        {
            bool found = false;

            foreach (var item in variableContainer)
            {
                if (item.GetName() == name)
                {
                    found = true;
                }
            }

            return found;
        }

        public static ExplorerElementVariable Get(int index)
        {
            return variableContainer[index];
        }

        public static int GenerateId()
        {
            int generatedId = 0;
            bool isIdValid = false;
            while (!isIdValid)
            {
                int i = 0;
                bool doesIdExist = false;
                while (!doesIdExist && i < variableContainer.Count)
                {
                    if (variableContainer[i].variable.Id == generatedId)
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
            while (!found && i < variableContainer.Count)
            {
                if(variableContainer[i].variable.Id == Id)
                {
                    name = Path.GetFileNameWithoutExtension(variableContainer[i].path);
                    found = true;
                }

                i++;
            }

            return name;
        }

        public static void RemoveByFilePath(string filePath)
        {
            for (int i = 0; i < variableContainer.Count; i++)
            {
                var item = variableContainer[i];
                if (item.path == filePath)
                {
                    variableContainer.Remove(item);
                }
            }
        }
    }
}

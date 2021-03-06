using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BetterTriggers.Containers
{
    public static class Variables
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
                if (item.GetName().ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
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

        internal static ExplorerElementVariable FindExplorerVariableById(int Id)
        {
            ExplorerElementVariable var = null;

            bool found = false;
            int i = 0;
            while (!found && i < variableContainer.Count)
            {
                if (variableContainer[i].variable.Id == Id)
                {
                    var = variableContainer[i];
                    found = true;
                }

                i++;
            }

            return var;
        }
        
        internal static Variable GetVariableById(int Id)
        {
            Variable var = null;

            bool found = false;
            int i = 0;
            while (!found && i < variableContainer.Count)
            {
                if (variableContainer[i].variable.Id == Id)
                {
                    var = variableContainer[i].variable;
                    found = true;
                }

                i++;
            }

            return var;
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

        public static void Remove(ExplorerElementVariable explorerElementVariable)
        {
            variableContainer.Remove(explorerElementVariable);
        }

        internal static void Clear()
        {
            variableContainer.Clear();
        }
    }
}

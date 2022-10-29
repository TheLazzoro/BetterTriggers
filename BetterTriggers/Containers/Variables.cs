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
        public static List<Variable> variableContainer = new List<Variable>();

        public static void AddVariable(Variable variable)
        {
            variableContainer.Add(variable);
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
                if (item.Name.ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
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

        internal static Variable FindVariableById(int Id)
        {
            Variable var = null;

            bool found = false;
            int i = 0;
            while (!found && i < variableContainer.Count)
            {
                if (variableContainer[i].Id == Id)
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
                if (variableContainer[i].Id == Id)
                {
                    var = variableContainer[i];
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
                if(variableContainer[i].Id == Id)
                {
                    name = variableContainer[i].Name;
                    found = true;
                }

                i++;
            }

            return name;
        }

        public static void Remove(Variable variable)
        {
            variableContainer.Remove(variable);
        }

        internal static void Clear()
        {
            variableContainer.Clear();
        }
    }
}

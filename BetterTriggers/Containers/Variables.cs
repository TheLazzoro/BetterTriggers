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
        public static List<Variable> localVariableContainer = new List<Variable>();

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

        /// <summary>
        /// Returns true if an element with the given id exists in the container.
        /// </summary>
        public static bool Contains(int id)
        {
            bool found = true;
            foreach (var variable in variableContainer)
            {
                if (variable.Id == id)
                    found = true;
            }
            foreach (var variable in localVariableContainer)
            {
                if (variable.Id == id)
                    found = true;
            }

            return found;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blacklist">Id's that cannot be used.</param>
        /// <returns></returns>
        public static int GenerateId(List<int> blacklist = null)
        {
            int generatedId = 0;
            bool isIdValid = false;
            while (!isIdValid)
            {
                bool doesIdExist = false;
                foreach (var variable in variableContainer)
                {
                    if (variable.Id == generatedId)
                        doesIdExist = true;
                }
                foreach (var variable in localVariableContainer)
                {
                    if (variable.Id == generatedId)
                        doesIdExist = true;
                }
                if (blacklist != null)
                {
                    foreach (var id in blacklist)
                    {
                        if (generatedId == id)
                            doesIdExist = true;
                    }
                }

                if (!doesIdExist)
                    isIdValid = true;
                else
                    generatedId++;
            }

            return generatedId;
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
            i = 0;
            while (!found && i < localVariableContainer.Count)
            {
                if (localVariableContainer[i].Id == Id)
                {
                    var = localVariableContainer[i];
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
                if (variableContainer[i].Id == Id)
                {
                    name = variableContainer[i].Name;
                    found = true;
                }
                i++;
            }
            i = 0;
            while (!found && i < localVariableContainer.Count)
            {
                if (localVariableContainer[i].Id == Id)
                {
                    name = localVariableContainer[i].Name;
                    found = true;
                }
                i++;
            }

            return name;
        }

        internal static void Remove(Variable variable)
        {
            variableContainer.Remove(variable);
        }

        internal static void Clear()
        {
            variableContainer.Clear();
        }

        internal static void AddLocalVariable(LocalVariable localVariable)
        {
            localVariableContainer.Add(localVariable.variable);
        }

        internal static void RemoveLocalVariable(LocalVariable localVariable)
        {
            localVariableContainer.Remove(localVariable.variable);
        }
    }
}

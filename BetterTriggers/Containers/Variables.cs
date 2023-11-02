using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BetterTriggers.Containers
{
    public static class Variables
    {
        public static HashSet<ExplorerElementVariable> variableContainer = new HashSet<ExplorerElementVariable>();
        public static HashSet<Variable> localVariableContainer = new HashSet<Variable>();

        public static void AddVariable(ExplorerElementVariable variable)
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
                if (item.variable.Name.ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
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
                if (variable.variable.Id == id)
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
                    if (variable.variable.Id == generatedId)
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
                    generatedId = RandomUtil.GenerateInt();
            }

            return generatedId;
        }

        internal static string GenerateLocalName(Trigger trig, string oldName = "UntitledVariable")
        {
            string baseName = oldName;
            string name = baseName;
            int i = 0;
            bool validName = false;
            while (!validName && trig.LocalVariables.Count > 0)
            {
                foreach (LocalVariable localVar in trig.LocalVariables)
                {
                    validName = name != localVar.variable.Name;
                    if (!validName)
                    {
                        name = baseName + i;
                        i++;
                        break;
                    }
                }
            }
            return name;
        }

        internal static List<Variable> GetAll()
        {
            List<Variable> list = new();
            variableContainer.ToList().ForEach(el =>
            {
                list.Add(el.variable);
            });
            list.AddRange(localVariableContainer.ToList());
            return list;
        }

        internal static List<ExplorerElementVariable> GetGlobals()
        {
            return variableContainer.ToList();
        }

        internal static Variable GetVariableById(int Id, Trigger trig = null)
        {
            Variable var = null;

            bool found = false;
            var enumerator = variableContainer.GetEnumerator();
            while (!found && enumerator.MoveNext())
            {
                if (enumerator.Current.variable.Id == Id)
                {
                    var = enumerator.Current.variable;
                    found = true;
                }
            }

            if (trig == null)
                trig = ControllerTrigger.SelectedTrigger;

            if (trig != null) // for local variables
            {
                for (int i = 0; i < trig.LocalVariables.Count; i++)
                {
                    var localVar = (LocalVariable)trig.LocalVariables[i];
                    if (localVar.variable.Id == Id)
                    {
                        var = localVar.variable;
                        break;
                    }
                }
            }

            return var;
        }

        /// <summary>
        /// Should only be used for script generation.
        /// </summary>
        internal static Variable GetVariableById_AllLocals(int Id)
        {
            Variable var = null;

            bool found = false;
            var enumerator = variableContainer.GetEnumerator();
            while (!found && enumerator.MoveNext())
            {
                if (enumerator.Current.variable.Id == Id)
                {
                    var = enumerator.Current.variable;
                    found = true;
                }
            }
            var enumerator2 = localVariableContainer.GetEnumerator();
            while (!found && enumerator2.MoveNext())
            {
                if (enumerator2.Current.Id == Id)
                {
                    var = enumerator2.Current;
                    found = true;
                }
            }

            return var;
        }

        public static string GetVariableNameById(int Id)
        {
            string name = string.Empty;

            bool found = false;
            var enumerator = variableContainer.GetEnumerator();
            while (!found && enumerator.MoveNext())
            {
                if (enumerator.Current.variable.Id == Id)
                {
                    name = enumerator.Current.variable.Name;
                    found = true;
                }
            }
            var enumerator2 = localVariableContainer.GetEnumerator();
            while (!found && enumerator2.MoveNext())
            {
                if (enumerator2.Current.Id == Id)
                {
                    name = enumerator2.Current.Name;
                    found = true;
                }
            }

            return name;
        }

        internal static void Remove(ExplorerElementVariable variable)
        {
            variableContainer.Remove(variable);
        }

        internal static void Clear()
        {
            variableContainer.Clear();
            localVariableContainer.Clear();
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

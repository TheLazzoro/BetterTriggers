using BetterTriggers.Commands;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using War3Net.Build.Script;

namespace BetterTriggers.Containers
{
    public class Variables
    {
        public static bool includeLocals { get; set; } = true; // hack
        public HashSet<ExplorerElement> variableContainer = new HashSet<ExplorerElement>();
        public HashSet<Variable> localVariableContainer = new HashSet<Variable>();

        /// <returns>Full path.</returns>
        public string Create()
        {
            var project = Project.CurrentProject;
            string directory = project.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = GenerateName();

            // Default variable is always an integer on creation.
            Variable_Saveable variable = new Variable_Saveable()
            {
                Id = GenerateId(),
                Name = name,
                Type = "integer",
                InitialValue = new Value_Saveable() { value = "0" },
                ArraySize = new int[] { 1, 1 },
            };
            string json = JsonConvert.SerializeObject(variable);
            string fullPath = Path.Combine(directory, name + ".var");
            File.WriteAllText(fullPath, json);

            return fullPath;
        }

        public LocalVariable CreateLocalVariable(Trigger trig, int insertIndex)
        {
            TriggerElementCollection parent = trig.LocalVariables;
            LocalVariable localVariable = new LocalVariable();
            localVariable.variable.Name = GenerateLocalName(trig);
            localVariable.variable.Id = GenerateId();
            localVariable.variable.Type = "integer";
            localVariable.variable.ArraySize = new int[] { 1, 1 };
            localVariable.variable.InitialValue = new Value() { value = "0" };
            localVariable.DisplayText = localVariable.variable.Name;
            localVariable.IconImage = Category.Get(TriggerCategory.TC_LOCAL_VARIABLE).Icon;

            CommandTriggerElementCreate command = new CommandTriggerElementCreate(localVariable, parent, insertIndex);
            command.Execute();

            return localVariable;
        }

        public void RenameLocalVariable(Trigger trig, LocalVariable variable, string newName)
        {
            if (newName == variable.variable.Name)
                return;

            foreach (LocalVariable v in trig.LocalVariables.Elements)
            {
                if (v.variable.Name == newName)
                    throw new Exception($"Local variable with name '{newName}' already exists.");
            }

            CommandLocalVariableRename command = new CommandLocalVariableRename(variable, newName);
            command.Execute();
        }

        /// <summary>
        /// Returns true if initial value was removed.
        /// </summary>
        internal bool RemoveInvalidReference(ExplorerElement ExplorerElement)
        {
            Variable variable = ExplorerElement.variable;
            if (variable.InitialValue is Value value)
            {
                bool dataExists = CustomMapData.ReferencedDataExists(value, variable.Type);
                if (!dataExists)
                {
                    variable.InitialValue = new Parameter();
                    return true;
                }
            }

            return false;
        }

        private List<Variable> GetVariables(string returnType, Trigger trig, bool includeLocals)
        {
            List<Variable> list = new List<Variable>();
            List<Variable> all = new List<Variable>();
            all.AddRange(GetGlobals().Select(v => v.variable)); // globals
            if (includeLocals)
            {
                bool isParameterFromVariableInitialValue = trig == null;
                if (isParameterFromVariableInitialValue == false)
                {
                    trig.LocalVariables.Elements.ForEach(e =>
                    { // locals
                        var lv = (LocalVariable)e;
                        all.Add(lv.variable);
                    });
                }
            }

            for (int i = 0; i < all.Count; i++)
            {
                if (returnType != "AnyGlobal" && all[i].Type != returnType)
                    continue;

                var variable = all[i];
                list.Add(variable);
            }

            return list;
        }

        /// <summary>
        /// Creates a list of saveable variable refs
        /// </summary>
        /// <param name="returnType"></param>
        /// <returns></returns>
        public List<VariableRef> GetVariableRefs(string returnType, Trigger trig, bool includeLocals)
        {
            bool wasIntegervar = false;
            if (returnType == "integervar")
            {
                wasIntegervar = true;
                returnType = "integer";
            }

            List<Variable> variables = GetVariables(returnType, trig, includeLocals);
            List<VariableRef> list = new List<VariableRef>();

            for (int i = 0; i < variables.Count; i++)
            {
                VariableRef varRef = new VariableRef()
                {
                    VariableId = variables[i].Id,
                };
                varRef.arrayIndexValues.Add(new Value() { value = "0" });
                varRef.arrayIndexValues.Add(new Value() { value = "0" });

                list.Add(varRef);
            }

            return list;
        }


        internal string GenerateName(string name = "UntitledVariable")
        {
            string generatedName = name;
            bool ok = false;
            int i = 0;
            while (!ok)
            {
                if (!Contains(generatedName))
                    ok = true;
                else
                {
                    generatedName = name + i;
                }

                i++;
            }
            return generatedName;
        }


        internal void AddVariable(ExplorerElement variable)
        {
            variableContainer.Add(variable);
        }

        /// <summary>
        /// Returns true if an element with the given file name exists in the container.
        /// </summary>
        internal bool Contains(string name)
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
        internal bool Contains(int id)
        {
            bool found = false;
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
        internal int GenerateId(List<int> blacklist = null)
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

        internal string GenerateLocalName(Trigger trig, string oldName = "UntitledVariable")
        {
            string baseName = oldName;
            string name = baseName;
            int i = 0;
            bool validName = false;
            while (!validName && trig.LocalVariables.Elements.Count > 0)
            {
                foreach (LocalVariable localVar in trig.LocalVariables.Elements)
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

        internal List<Variable> GetAll()
        {
            List<Variable> list = new();
            variableContainer.ToList().ForEach(el =>
            {
                list.Add(el.variable);
            });
            list.AddRange(localVariableContainer.ToList());
            return list;
        }

        public List<ExplorerElement> GetGlobals()
        {
            return variableContainer.ToList();
        }

        public Variable GetById(int Id, Trigger trig = null)
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
                trig = Project.CurrentProject.Triggers.SelectedTrigger;

            if (trig != null) // for local variables
            {
                for (int i = 0; i < trig.LocalVariables.Elements.Count; i++)
                {
                    var localVar = (LocalVariable)trig.LocalVariables.Elements[i];
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
        internal Variable GetVariableById_AllLocals(int Id)
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

        public string GetVariableNameById(int Id)
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

        internal void Remove(ExplorerElement variable)
        {
            variableContainer.Remove(variable);
        }

        internal void AddLocalVariable(LocalVariable localVariable)
        {
            localVariableContainer.Add(localVariable.variable);
        }

        public void RemoveLocalVariable(LocalVariable localVariable)
        {
            localVariableContainer.Remove(localVariable.variable);
        }

        public Variable GetByReference(VariableRef variableRef)
        {
            return GetById(variableRef.VariableId);
        }
    }
}

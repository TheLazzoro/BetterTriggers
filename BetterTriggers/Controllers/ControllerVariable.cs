using BetterTriggers.Containers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Commands;

namespace BetterTriggers.Controllers
{
    public class ControllerVariable
    {
        /// <summary>
        /// </summary>
        /// <returns>Full path.</returns>
        public string CreateVariable()
        {
            string directory = ContainerProject.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = GenerateName();

            // Default variable is always an integer on creation.
            Variable variable = new Variable()
            {
                Id = Variables.GenerateId(),
                Name = name,
                Type = "integer",
                InitialValue = new Value() { value = "0" },
                ArraySize = new int[] { 1, 1 },
            };
            string json = JsonConvert.SerializeObject(variable);
            string fullPath = Path.Combine(directory, name + ".var");
            File.WriteAllText(fullPath, json);

            return fullPath;
        }

        public string GenerateName()
        {
            string name = "UntitledVariable";

            bool ok = false;
            int i = 0;
            while (!ok)
            {
                if (!Variables.Contains(name))
                    ok = true;
                else
                {
                    name = "UntitledVariable" + i;
                }

                i++;
            }
            return name;
        }

        public void CreateLocalVariable(Trigger trig, LocalVariable localVariable, List<TriggerElement> parent, int insertIndex)
        {
            string baseName = "UntitledVariable";
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
            localVariable.variable.Name = name;
            localVariable.variable.Id = Variables.GenerateId();
            localVariable.variable.Type = "integer";
            localVariable.variable.ArraySize = new int[] { 1, 1 };
            localVariable.variable.InitialValue = new Value() { value = "0" };

            CommandTriggerElementCreate command = new CommandTriggerElementCreate(localVariable, parent, insertIndex);
            command.Execute();
        }

        public void RenameLocalVariable(Trigger trig, LocalVariable variable, string name)
        {
            if (name == variable.variable.Name)
                return;

            foreach (LocalVariable v in trig.LocalVariables)
            {
                if (v.variable.Name == name)
                    throw new Exception($"Local variable with name '{name}' already exists.");
            }
            variable.variable.Name = name;
        }

        public List<Variable> GetVariablesAll()
        {
            return Variables.variableContainer;
        }

        private List<Variable> GetVariables(string returnType, Trigger trig)
        {
            List<Variable> list = new List<Variable>();
            List<Variable> all = new List<Variable>();
            all.AddRange(Variables.variableContainer); // globals
            all.AddRange(Variables.localVariableContainer); // locals

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
        public List<VariableRef> GetVariableRefs(string returnType, Trigger trig)
        {
            bool wasIntegervar = false;
            if (returnType == "integervar")
            {
                wasIntegervar = true;
                returnType = "integer";
            }

            List<Variable> variables = GetVariables(returnType, trig);
            List<VariableRef> list = new List<VariableRef>();

            for (int i = 0; i < variables.Count; i++)
            {
                VariableRef varRef = new VariableRef()
                {
                    VariableId = variables[i].Id,
                };
                varRef.arrayIndexValues.Add(new Value());
                varRef.arrayIndexValues.Add(new Value());

                list.Add(varRef);
            }

            return list;
        }

        public string GetVariableNameById(int id)
        {
            return Variables.GetVariableNameById(id);
        }

        
        public Variable GetById(int id)
        {
            return Variables.GetVariableById(id);
        }

        public Variable GetByReference(VariableRef variableRef)
        {
            return GetById(variableRef.VariableId);
        }

        public void RemoveVariableRefFromTriggers(Variable variable)
        {
            References.ResetVariableReferences(variable);
        }

    }
}

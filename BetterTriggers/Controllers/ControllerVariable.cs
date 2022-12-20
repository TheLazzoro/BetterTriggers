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
using System.Linq;

namespace BetterTriggers.Controllers
{
    public class ControllerVariable
    {
        /// <summary>
        /// </summary>
        /// <returns>Full path.</returns>
        public static string Create()
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

        public static string GenerateName(string name = "UntitledVariable")
        {
            string generatedName = name;
            bool ok = false;
            int i = 0;
            while (!ok)
            {
                if (!Variables.Contains(generatedName))
                    ok = true;
                else
                {
                    generatedName = name + i;
                }

                i++;
            }
            return generatedName;
        }

        public static void CreateLocalVariable(Trigger trig, LocalVariable localVariable, List<TriggerElement> parent, int insertIndex)
        {
            localVariable.variable.Name = Variables.GenerateLocalName(trig);
            localVariable.variable.Id = Variables.GenerateId();
            localVariable.variable.Type = "integer";
            localVariable.variable.ArraySize = new int[] { 1, 1 };
            localVariable.variable.InitialValue = new Value() { value = "0" };

            CommandTriggerElementCreate command = new CommandTriggerElementCreate(localVariable, parent, insertIndex);
            command.Execute();
        }

        public static void RenameLocalVariable(Trigger trig, LocalVariable variable, string newName)
        {
            if (newName == variable.variable.Name)
                return;

            foreach (LocalVariable v in trig.LocalVariables)
            {
                if (v.variable.Name == newName)
                    throw new Exception($"Local variable with name '{newName}' already exists.");
            }

            CommandLocalVariableRename command = new CommandLocalVariableRename(variable, newName);
            command.Execute();
        }

        public static List<Variable> GetVariablesAll()
        {
            return Variables.variableContainer.Select(x => x).ToList();
        }

        private static List<Variable> GetVariables(string returnType, Trigger trig, bool includeLocals)
        {
            List<Variable> list = new List<Variable>();
            List<Variable> all = new List<Variable>();
            all.AddRange(Variables.variableContainer); // globals
            if (includeLocals)
            {
                trig.LocalVariables.ForEach(e =>
                { // locals
                    var lv = (LocalVariable)e;
                    all.Add(lv.variable);
                });
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
        public static List<VariableRef> GetVariableRefs(string returnType, Trigger trig, bool includeLocals)
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
                varRef.arrayIndexValues.Add(new Value());
                varRef.arrayIndexValues.Add(new Value());

                list.Add(varRef);
            }

            return list;
        }

        public static string GetVariableNameById(int id)
        {
            return Variables.GetVariableNameById(id);
        }

        public static Variable GetById(int id)
        {
            return Variables.GetVariableById(id);
        }

        public static Variable GetByReference(VariableRef variableRef)
        {
            return GetById(variableRef.VariableId);
        }

        public static void RemoveLocalVariable(LocalVariable localVariable)
        {
            Variables.RemoveLocalVariable(localVariable);
            // TODO: RemoveVariableRefFromTriggers(localVariable.variable);
        }
    }
}

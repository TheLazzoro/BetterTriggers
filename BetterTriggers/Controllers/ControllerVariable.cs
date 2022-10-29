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

        public List<Variable> GetVariablesAll()
        {
            return Variables.variableContainer;
        }

        public List<Variable> GetVariables(string returnType)
        {
            List<Variable> list = new List<Variable>();

            for (int i = 0; i < Variables.variableContainer.Count; i++)
            {
                if (returnType != "AnyGlobal" && Variables.variableContainer[i].Type != returnType)
                    continue;

                var variable = Variables.variableContainer[i];
                list.Add(variable);
            }

            return list;
        }

        /// <summary>
        /// Creates a list of saveable variable refs
        /// </summary>
        /// <param name="returnType"></param>
        /// <returns></returns>
        public List<VariableRef> GetVariableRefs(string returnType)
        {
            bool wasIntegervar = false;
            if (returnType == "integervar")
            {
                wasIntegervar = true;
                returnType = "integer";
            }

            List<Variable> variables = GetVariables(returnType);
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

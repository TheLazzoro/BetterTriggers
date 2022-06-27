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
        public void CreateVariable()
        {
            string directory = ContainerProject.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = GenerateName();

            // Default variable is always an integer on creation.
            Variable variable = new Variable()
            {
                Id = ContainerVariables.GenerateId(),
                Name = name,
                Type = "integer",
                InitialValue = "0",
                ArraySize = new int[] { 1, 1 },
            };
            string json = JsonConvert.SerializeObject(variable);

            File.WriteAllText(directory + @"\" + name + ".var", json);
        }

        public string GenerateName()
        {
            string name = "UntitledVariable";

            bool ok = false;
            int i = 0;
            while (!ok)
            {
                if (!ContainerVariables.Contains(name))
                    ok = true;
                else
                {
                    name = "UntitledVariable" + i;
                }

                i++;
            }
            return name;
        }

        public List<Variable> GetVariables(string returnType)
        {
            List<Variable> list = new List<Variable>();

            for (int i = 0; i < ContainerVariables.variableContainer.Count; i++)
            {
                if (returnType != "AnyGlobal" && ContainerVariables.variableContainer[i].variable.Type != returnType)
                    continue;

                var explorerElement = ContainerVariables.variableContainer[i];
                string name = System.IO.Path.GetFileNameWithoutExtension(explorerElement.GetPath());
                Variable variable = explorerElement.variable;
                variable.Name = name;
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
                    returnType = wasIntegervar == false ? variables[i].Type : "integervar",
                    VariableId = variables[i].Id,
                };
                varRef.arrayIndexValues.Add(new Value() { returnType = "integer", identifier = "0" });
                varRef.arrayIndexValues.Add(new Value() { returnType = "integer", identifier = "0" });

                list.Add(varRef);
            }

            return list;
        }

        public string GetVariableNameById(int id)
        {
            return ContainerVariables.GetVariableNameById(id);
        }

        
        public Variable GetById(int id)
        {
            return ContainerVariables.GetVariableById(id);
        }

        public Variable GetByReference(VariableRef variableRef)
        {
            return GetById(variableRef.VariableId);
        }

        public ExplorerElementVariable GetExplorerElementVariableInMemory(string filepath)
        {
            ExplorerElementVariable variable = null;
            int i = 0;
            bool found = false;

            while (!found)
            {
                if (ContainerVariables.variableContainer[i].GetPath() == filepath)
                {
                    found = true;
                    variable = ContainerVariables.variableContainer[i];
                }

                i++;
            }


            return variable;
        }

        public void RemoveVariableRefFromTriggers(ExplorerElementVariable explorerElementVariable)
        {
            ContainerReferences.ResetVariableReferences(explorerElementVariable);
        }


        /*
        public VariableControl CreateVariableWithElements(TabControl tabControl, Model.Data.Variable variable)
        {
            var variableControl = new VariableControl(variable.Id);
            variableControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            variableControl.VerticalContentAlignment = VerticalAlignment.Stretch;
            GenerateVariableElements(variableControl, variable);

            return variableControl;
        }

        private void GenerateVariableElements(VariableControl variableControl, Model.Data.Variable variable)
        {
            variableControl.OnElementRename(variable.Name);

            ControllerTriggerData controller = new ControllerTriggerData();
            List<ComboBoxItemType> list = controller.LoadVariableTypes();

            for (int i = 0; i < list.Count; i++)
            {
                variableControl.comboBoxVariableType.Items.Add(list[i]);
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Type == variable.Type)
                    variableControl.comboBoxVariableType.SelectedItem = list[i];
            }


            variableControl.checkBoxIsArray.IsChecked = variable.IsArray;

            if (variable.IsTwoDimensions)
                variableControl.comboBoxArrayDimensions.SelectedIndex = 1;
            else
                variableControl.comboBoxArrayDimensions.SelectedIndex = 0;

            variableControl.textBoxArraySize0.Text = variable.ArraySize[0].ToString();
            variableControl.textBoxArraySize1.Text = variable.ArraySize[1].ToString();
        }
        */
    }
}

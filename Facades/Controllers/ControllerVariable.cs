using Facades.Containers;
using Model.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Facades.Controllers
{
    public class ControllerVariable
    {
        public void CreateVariable()
        {
            string directory = ContainerProject.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

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

            // Default variable is always an integer on creation.
            Variable variable = new Variable()
            {
                Id = ContainerVariables.GenerateId(),
                Name = name,
                Type = "integer",
                InitialValue = "0",
            };
            string json = JsonConvert.SerializeObject(variable);

            File.WriteAllText(directory + @"\" + name + ".var", json);
        }

        public Variable LoadVariableFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            Variable variable = JsonConvert.DeserializeObject<Variable>(json);

            return variable;
        }

        public Variable GetVariableInMemory(string filepath)
        {
            Variable variable = null;
            int i = 0;
            bool found = false;

            while (!found)
            {
                if (ContainerVariables.variableContainer[i].GetPath() == filepath)
                {
                    found = true;
                    variable = ContainerVariables.variableContainer[i].variable;
                }

                i++;
            }


            return variable;
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

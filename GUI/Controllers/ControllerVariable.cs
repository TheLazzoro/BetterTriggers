using GUI.Components.TriggerExplorer;
using GUI.Components.VariableEditor;
using GUI.Utility;
using Model.Data;
using Model.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerVariable
    {
        public void CreateVariable(TriggerExplorer triggerExplorer)
        {
            NewExplorerElementWindow createExplorerElementWindow = new NewExplorerElementWindow(ExplorerElementType.Variable);
            createExplorerElementWindow.ShowDialog();
            if (createExplorerElementWindow.ElementName != null)
            {
                string name = createExplorerElementWindow.ElementName;

                ControllerProject controllerProject = new ControllerProject();
                string directory = controllerProject.GetDirectoryFromSelection(triggerExplorer.treeViewTriggerExplorer);

                Variable variable = new Variable()
                {
                    Name = name,
                    Type = "integer",
                };
                string json = JsonConvert.SerializeObject(variable);

                File.WriteAllText(directory + "/" + name + ".var", json);
            }
        }

        public Variable LoadVariableFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            Variable variable = JsonConvert.DeserializeObject<Variable>(json);

            return variable;
        }

        public VariableControl CreateVariableWithElements(TabControl tabControl, Model.Data.Variable variable)
        {
            var variableControl = CreateTriggerControl(tabControl);
            GenerateVariableElements(variableControl, variable);

            return variableControl;
        }

        private VariableControl CreateTriggerControl(TabControl tabControl)
        {
            var variableControl = new VariableControl();
            variableControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            variableControl.VerticalContentAlignment = VerticalAlignment.Stretch;

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
                if ((string)list[i].Tag == variable.Type)
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
    }
}

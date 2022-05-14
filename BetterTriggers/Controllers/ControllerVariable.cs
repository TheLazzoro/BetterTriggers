using BetterTriggers.Containers;
using Model.EditorData;
using Model.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace BetterTriggers.Controllers
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

        public List<ExplorerElementVariable> GetExplorerElementAll()
        {
            return ContainerVariables.variableContainer;
        }

        public string GetVariableNameById(int id)
        {
            return ContainerVariables.GetVariableNameById(id);
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

        public void SetReferenceToVariable(int variableId, int triggerId)
        {
            Variable var = ContainerVariables.GetVariableById(variableId);
            var.TriggersUsing.Add(triggerId);
        }

        public void RemoveReferenceToVariable(int variableId, int triggerId)
        {
            Variable var = ContainerVariables.GetVariableById(variableId);
            var.TriggersUsing.Remove(triggerId);
        }

        /// <summary>
        /// This is used when a parameter with nested chains of parameters get un-done.
        /// </summary>
        public void RecurseRemoveVariableRefs(Function function, int triggerId)
        {
            for (int i = 0; i < function.parameters.Count; i++)
            {
                var param = function.parameters[i];
                if (param is Function)
                    RecurseRemoveVariableRefs((Function)param, triggerId);
                else if (param is VariableRef)
                {
                    var variableRef = (VariableRef)param;
                    ControllerVariable controller = new ControllerVariable();
                    controller.RemoveReferenceToVariable(variableRef.VariableId, triggerId);
                }
            }
        }

        /// <summary>
        /// This is used when a parameter with nested chains of parameters gets re-done.
        /// </summary>
        public void RecurseAddVariableRefs(Function function, int triggerId)
        {
            for (int i = 0; i < function.parameters.Count; i++)
            {
                var param = function.parameters[i];
                if (param is Function)
                    RecurseAddVariableRefs((Function)param, triggerId);
                else if (param is VariableRef)
                {
                    var variableRef = (VariableRef)param;
                    ControllerVariable controller = new ControllerVariable();
                    controller.SetReferenceToVariable(variableRef.VariableId, triggerId);
                }
            }
        }

        public void RemoveVariableRefFromTriggers(ExplorerElementVariable explorerElementVariable)
        {
            for (int i = 0; i < explorerElementVariable.variable.TriggersUsing.Count; i++)
            {
                // TODO !!! Right now triggers elements are not saved in ExplorerElementTriggers (in memory)
                // This is supposed to be called when a variable changes properties, i.e. type, isArray, arrayDimensions etc.
            }
            explorerElementVariable.variable.TriggersUsing.Clear();
        }

        public Variable GetById(int id)
        {
            return ContainerVariables.GetVariableById(id);
        }

        public Variable GetByReference(VariableRef variableRef)
        {
            return GetById(variableRef.VariableId);
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

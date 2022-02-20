using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GUI.Components.TriggerEditor;
using Model.EditorData;
using Model.SaveableData;

namespace GUI.Commands
{
    public class CommandVariableModifyType : ICommand
    {
        string commandName = "Modify Variable Type";
        ExplorerElementVariable explorerElement;
        string selectedType;
        string previousType;

        public CommandVariableModifyType(ExplorerElementVariable explorerElement, string selectedType, string previousType)
        {
            this.explorerElement = explorerElement;
            this.selectedType = selectedType;
            this.previousType = previousType;
        }

        public void Execute()
        {
            explorerElement.variable.Type = selectedType;
            explorerElement.Notify();

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            explorerElement.variable.Type = selectedType;
            explorerElement.Notify();
        }

        public void Undo()
        {
            explorerElement.variable.Type = previousType;
            explorerElement.Notify();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}

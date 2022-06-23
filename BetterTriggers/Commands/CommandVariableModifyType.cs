using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Controllers;
using Model.EditorData;
using Model.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyType : ICommand
    {
        string commandName = "Modify Variable Type";
        ExplorerElementVariable explorerElement;
        string selectedType;
        string previousType;
        ControllerTriggerData controllerTriggerData = new ControllerTriggerData();

        public CommandVariableModifyType(ExplorerElementVariable explorerElement, string selectedType, string previousType)
        {
            this.explorerElement = explorerElement;
            this.selectedType = selectedType;
            this.previousType = previousType;
        }

        public void Execute()
        {
            explorerElement.variable.InitialValue = controllerTriggerData.GetTypeInitialValue(selectedType);
            explorerElement.variable.Type = selectedType;
            explorerElement.Notify();

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            explorerElement.variable.InitialValue = controllerTriggerData.GetTypeInitialValue(selectedType);
            explorerElement.variable.Type = selectedType;
            explorerElement.Notify();
        }

        public void Undo()
        {
            explorerElement.variable.InitialValue = controllerTriggerData.GetTypeInitialValue(previousType);
            explorerElement.variable.Type = previousType;
            explorerElement.Notify();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}

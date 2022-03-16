using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using GUI.Components;
using GUI.Components.TriggerEditor;
using GUI.Components.TriggerExplorer;
using GUI.Controllers;
using Model.EditorData;
using Model.SaveableData;

namespace GUI.Commands
{
    public class CommandExplorerElementCreate : ICommand
    {
        string commandName = "Create Explorer Element";
        TriggerExplorer te;
        string fullPath;
        IExplorerElement createdElement;
        string fileContent = string.Empty;

        public CommandExplorerElementCreate(TriggerExplorer te, string fullPath)
        {
            this.te = te;
            this.fullPath = fullPath;
        }

        public void Execute()
        {
            ControllerProject controllerProject = new ControllerProject();
            ControllerTriggerExplorer controllerTriggerExplorer = new ControllerTriggerExplorer();
            TreeItemExplorerElement parent = controllerTriggerExplorer.FindTreeNodeDirectory(te.map, Path.GetDirectoryName(fullPath));
            if (parent == null)
                parent = te.map;

            controllerTriggerExplorer.RecurseCreateElement(ContainerProject.projectFiles[0], parent, fullPath, false);
            this.createdElement = controllerProject.FindExplorerElement(ContainerProject.projectFiles[0], fullPath);

            CommandManager.AddCommand(this);

            //triggerControl.OnStateChange();
        }

        public void Redo()
        {
            ControllerProject controllerProject = new ControllerProject();
            controllerProject.SetEnableFileEvents(false);

            ControllerTriggerExplorer controllerTriggerExplorer = new ControllerTriggerExplorer();
            IExplorerElement folder = controllerProject.FindExplorerElementFolder(ContainerProject.projectFiles[0], fullPath);
            TreeItemExplorerElement parent = controllerTriggerExplorer.FindTreeNodeDirectory(te.map, Path.GetDirectoryName(fullPath));
            if (parent == null)
                parent = te.map;

            controllerProject.RecurseCreateElementsWithContent(createdElement);
            controllerTriggerExplorer.RecurseCreateElement(folder, parent, fullPath);

            controllerProject.SetEnableFileEvents(true);

            //triggerControl.OnStateChange();
        }

        public void Undo()
        {
            ControllerProject controllerProject = new ControllerProject();
            controllerProject.SetEnableFileEvents(false);

            ControllerFileSystem controllerFileSystem = new ControllerFileSystem();
            controllerFileSystem.DeleteElement(fullPath);
            controllerProject.OnDeleteElement(fullPath);

            ControllerTriggerExplorer controllerTriggerExplorer = new ControllerTriggerExplorer();
            var element = controllerTriggerExplorer.FindTreeNodeElement(te.map, fullPath);
            var parent = controllerTriggerExplorer.FindTreeNodeDirectory(te.map, Path.GetDirectoryName(fullPath));

            parent.Items.Remove(element);

            controllerProject.SetEnableFileEvents(true);

            //triggerControl.OnStateChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
        
    }
}

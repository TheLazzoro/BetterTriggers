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
    public class CommandExplorerElementDelete : ICommand
    {
        string commandName = "Delete Explorer Element";
        TriggerExplorer te;
        string fullPath;
        IExplorerElement deletedElement;
        IExplorerElement parent;
        string fileContent = string.Empty;
        int index;

        public CommandExplorerElementDelete(TriggerExplorer te, string fullPath)
        {
            this.te = te;
            this.fullPath = fullPath;
        }

        public void Execute()
        {
            ControllerProject controllerProject = new ControllerProject();
            ControllerTriggerExplorer controllerTriggerExplorer = new ControllerTriggerExplorer();

            this.deletedElement = controllerProject.FindExplorerElement(ContainerProject.projectFiles[0], fullPath);
            this.parent = controllerProject.FindExplorerElementFolder(ContainerProject.projectFiles[0], Path.GetDirectoryName(fullPath));
            TreeItemExplorerElement element = controllerTriggerExplorer.FindTreeNodeElement(te.map, fullPath);
            TreeItemExplorerElement parent = controllerTriggerExplorer.FindTreeNodeDirectory(te.map, Path.GetDirectoryName(fullPath));
            if (parent == null)
                parent = te.map;

            this.fileContent = deletedElement.GetSaveableString();

            this.index = parent.Items.IndexOf(element);
            parent.Items.Remove(element);

            CommandManager.AddCommand(this);

            //triggerControl.OnStateChange();
        }

        public void Redo()
        {
            ControllerProject controllerProject = new ControllerProject();
            controllerProject.SetEnableFileEvents(false);

            ControllerTriggerExplorer controllerTriggerExplorer = new ControllerTriggerExplorer();

            this.deletedElement = controllerProject.FindExplorerElement(ContainerProject.projectFiles[0], fullPath);
            this.parent = controllerProject.FindExplorerElementFolder(ContainerProject.projectFiles[0], Path.GetDirectoryName(fullPath));
            TreeItemExplorerElement element = controllerTriggerExplorer.FindTreeNodeElement(te.map, fullPath);
            TreeItemExplorerElement parent = controllerTriggerExplorer.FindTreeNodeDirectory(te.map, Path.GetDirectoryName(fullPath));
            if (parent == null)
                parent = te.map;

            //this.index = parent.Items.IndexOf(element);
            parent.Items.Remove(element);

            ControllerFileSystem controllerFileSystem = new ControllerFileSystem();
            controllerFileSystem.DeleteElement(fullPath);

            controllerProject.SetEnableFileEvents(true);

            //triggerControl.OnStateChange();
        }

        public void Undo() // hacky?
        {
            ControllerProject controller = new ControllerProject();
            controller.SetEnableFileEvents(false);

            if(parent is ExplorerElementRoot)
            {
                var root = (ExplorerElementRoot)parent;
                root.explorerElements.Insert(this.index, deletedElement);
            } else if (parent is ExplorerElementFolder)
            {
                var folder = (ExplorerElementFolder)parent;
                folder.explorerElements.Insert(this.index, deletedElement);
            }

            // This is unfinished.
            // If we want to undo the deletion of a folder,
            // we need to recreate all contents of the folder as well.
            // Same thing must be done in the Create command.
            ControllerFileSystem controllerFileSystem = new ControllerFileSystem();
            controllerFileSystem.SaveFile(fullPath, this.fileContent);

            ControllerTriggerExplorer controllerTriggerExplorer = new ControllerTriggerExplorer();
            TreeItemExplorerElement uiParent = controllerTriggerExplorer.FindTreeNodeDirectory(te.map, Path.GetDirectoryName(fullPath));
            controllerTriggerExplorer.RecurseCreateElement(parent, uiParent, fullPath);
            // TODO: ALSO MAKE UI INSERT ON CORRECT INDEX!!!!


            controller.SetEnableFileEvents(true);
            //triggerControl.OnStateChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}

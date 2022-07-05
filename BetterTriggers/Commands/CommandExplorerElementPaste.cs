
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandExplorerElementPaste : ICommand
    {
        string commandName = "Paste Explorer Element";
        int pastedIndex = 0;
        int cutIndex = 0;
        IExplorerElement toCut;
        IExplorerElement toPaste;
        IExplorerElement cutParent;
        IExplorerElement pasteParent;

        public CommandExplorerElementPaste(IExplorerElement elementToPaste, IExplorerElement pasteParent, int pastedIndex)
        {
            this.toCut = CopiedElements.CutExplorerElement;
            if (toCut != null)
            {
                this.cutParent = toCut.GetParent();
                this.cutIndex = toCut.GetParent().GetExplorerElements().IndexOf(toCut);
            }


            this.toPaste = elementToPaste;
            this.pasteParent = pasteParent;
            this.pastedIndex = pastedIndex;
        }

        public void Execute()
        {
            ControllerProject controllerProject = new ControllerProject();
            ControllerFileSystem controllerFileSystem = new ControllerFileSystem();
            controllerProject.SetEnableFileEvents(false);
            CreatePastedElements(toPaste, pasteParent);

            if (toCut != null)
            {
                CopiedElements.CutExplorerElement = null;
                controllerFileSystem.DeleteElement(toCut.GetPath());
                toCut.RemoveFromParent();
                toCut.Deleted();
            }
            toPaste.SetParent(pasteParent, pastedIndex);
            toPaste.Created(pastedIndex);

            controllerProject.SetEnableFileEvents(true);


            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            ControllerProject controllerProject = new ControllerProject();
            ControllerFileSystem controllerFileSystem = new ControllerFileSystem();
            controllerProject.SetEnableFileEvents(false);
            CreatePastedElements(toPaste, pasteParent);

            if (toCut != null)
            {
                controllerFileSystem.DeleteElement(toCut.GetPath());
                toCut.RemoveFromParent();
                toCut.Deleted();
            }
            toPaste.SetParent(pasteParent, pastedIndex);
            toPaste.Created(pastedIndex);

            controllerProject.SetEnableFileEvents(true);
        }

        public void Undo()
        {
            ControllerProject controllerProject = new ControllerProject();
            ControllerFileSystem controllerFileSystem = new ControllerFileSystem();
            controllerProject.SetEnableFileEvents(false);
            controllerFileSystem.DeleteElement(toPaste.GetPath());
            RemovePastedElements(toPaste);

            if (toCut != null)
            {
                controllerProject.RecurseCreateElementsWithContent(toCut);
                toCut.SetParent(cutParent, cutIndex);
                toCut.Created(cutIndex);
            }
            toPaste.RemoveFromParent();
            toPaste.Deleted();

            controllerProject.SetEnableFileEvents(true);
        }

        public string GetCommandName()
        {
            return commandName;
        }

        /// <summary>
        /// Adjusts the final file path and creates element(s).
        /// </summary>
        /// <param name="toPaste"></param>
        /// <param name="pasteParent"></param>
        private void CreatePastedElements(IExplorerElement toPaste, IExplorerElement pasteParent)
        {
            string name = Path.GetFileName(toPaste.GetPath());
            string dir = pasteParent.GetPath();
            string finalPath = Path.Combine(dir, name);
            toPaste.SetPath(finalPath);
            ControllerProject controller = new ControllerProject();
            controller.AddElementToContainer(toPaste);

            if (toPaste is IExplorerSaveable)
            {
                var saveable = (IExplorerSaveable)toPaste;
                File.WriteAllText(finalPath, saveable.GetSaveableString());
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(dir, name));
                var children = toPaste.GetExplorerElements();
                for (int i = 0; i < children.Count; i++)
                {
                    CreatePastedElements(children[i], toPaste);
                }
            }
        }

        /// <summary>
        /// Removes elements from their respective container.
        /// </summary>
        private void RemovePastedElements(IExplorerElement toRemove)
        {
            ControllerProject controller = new ControllerProject();
            controller.RemoveElementFromContainer(toRemove);

            if(toRemove is ExplorerElementFolder)
            {
                toRemove.GetExplorerElements().ForEach(element => RemovePastedElements(element));
            }
        }
    }
}
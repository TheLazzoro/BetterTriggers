
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;

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
        Project project;

        public CommandExplorerElementPaste(IExplorerElement elementToPaste, IExplorerElement pasteParent, int pastedIndex)
        {
            this.project = Project.CurrentProject;
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
            project.EnableFileEvents(false);
            CreatePastedElements(toPaste, pasteParent);

            if (toCut != null)
            {
                CopiedElements.CutExplorerElement = null;
                FileSystemUtil.Delete(toCut.GetPath());
                toCut.RemoveFromParent();
                toCut.Deleted();
            }
            toPaste.SetParent(pasteParent, pastedIndex);
            toPaste.Created(pastedIndex);

            project.EnableFileEvents(true);


            project.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            project.EnableFileEvents(false);
            CreatePastedElements(toPaste, pasteParent);

            if (toCut != null)
            {
                FileSystemUtil.Delete(toCut.GetPath());
                toCut.RemoveFromParent();
                toCut.Deleted();
            }
            toPaste.SetParent(pasteParent, pastedIndex);
            toPaste.Created(pastedIndex);

            project.EnableFileEvents(true);
        }

        public void Undo()
        {
            project.EnableFileEvents(false);
            FileSystemUtil.Delete(toPaste.GetPath());
            RemovePastedElements(toPaste);

            if (toCut != null)
            {
                Project.CurrentProject.RecurseCreateElementsWithContent(toCut);
                toCut.SetParent(cutParent, cutIndex);
                toCut.Created(cutIndex);
            }
            toPaste.RemoveFromParent();
            toPaste.Deleted();

            project.EnableFileEvents(true);
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
            project.AddElementToContainer(toPaste);
            
            if(toPaste is ExplorerElementScript script)
            {
                finalPath = project.Scripts.GenerateName(script);
                toPaste.SetPath(finalPath);
            }

            if (toPaste is IExplorerSaveable)
            {
                var saveable = (IExplorerSaveable)toPaste;
                File.WriteAllText(finalPath, saveable.GetSaveableString());
            }
            else
            {
                string folder = project.Folders.GenerateName(Path.Combine(dir, name));
                toPaste.SetPath(folder);
                Directory.CreateDirectory(folder);
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
            project.RemoveElementFromContainer(toRemove);

            if(toRemove is ExplorerElementFolder)
            {
                toRemove.GetExplorerElements().ForEach(element => RemovePastedElements(element));
            }
        }
    }
}
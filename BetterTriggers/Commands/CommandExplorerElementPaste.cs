using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;
using System.IO;

namespace BetterTriggers.Commands
{
    public class CommandExplorerElementPaste : ICommand
    {
        Project _project;
        string commandName = "Paste Explorer Element";
        int pastedIndex = 0;
        int cutIndex = 0;
        ExplorerElement toCut;
        ExplorerElement toPaste;
        ExplorerElement cutParent;
        ExplorerElement pasteParent;

        public CommandExplorerElementPaste(Project project, ExplorerElement elementToPaste, ExplorerElement pasteParent, int pastedIndex)
        {
            _project = project;
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
            _project.EnableFileEvents(false);
            CreatePastedElements(toPaste, pasteParent);

            if (toCut != null)
            {
                CopiedElements.CutExplorerElement = null;
                FileSystemUtil.Delete(toCut.GetPath());
                toCut.RemoveFromParent();
            }
            toPaste.SetParent(pasteParent, pastedIndex);

            _project.EnableFileEvents(true);


            _project.CommandManager.AddCommand(this);
            toPaste.IsSelected = true;
        }

        public void Redo()
        {
            _project.EnableFileEvents(false);
            CreatePastedElements(toPaste, pasteParent);

            if (toCut != null)
            {
                FileSystemUtil.Delete(toCut.GetPath());
                toCut.RemoveFromParent();
            }
            toPaste.SetParent(pasteParent, pastedIndex);

            _project.EnableFileEvents(true);
            toPaste.IsSelected = true;
        }

        public void Undo()
        {
            _project.EnableFileEvents(false);
            FileSystemUtil.Delete(toPaste.GetPath());
            RemovePastedElements(toPaste);

            if (toCut != null)
            {
                _project.RecurseCreateElementsWithContent(toCut);
                toCut.SetParent(cutParent, cutIndex);
            }
            toPaste.RemoveFromParent();

            _project.EnableFileEvents(true);
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
        private void CreatePastedElements(ExplorerElement toPaste, ExplorerElement pasteParent)
        {
            string name = Path.GetFileName(toPaste.GetPath());
            string dir = pasteParent.GetPath();
            string finalPath = Path.Combine(dir, name);
            toPaste.SetPath(finalPath);
            _project.AddElementToContainer(toPaste);
            
            if(toPaste.ElementType == ExplorerElementEnum.Script)
            {
                finalPath = _project.Scripts.GenerateName(toPaste);
                toPaste.SetPath(finalPath);
            }

            if (toPaste.ElementType != ExplorerElementEnum.Folder)
            {
                toPaste.Save();
            }
            else
            {
                string folder = _project.Folders.GenerateName(Path.Combine(dir, name));
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
        private void RemovePastedElements(ExplorerElement toRemove)
        {
            _project.RemoveElementFromContainer_WhenDeleting(toRemove);

            if(toRemove.ElementType == ExplorerElementEnum.Folder)
            {
                var elementsToRemove = toRemove.GetExplorerElements();
                foreach (var element in elementsToRemove)
                {
                    RemovePastedElements(element);
                }
            }
        }
    }
}
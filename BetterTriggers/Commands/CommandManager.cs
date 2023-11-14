using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    /// <summary>
    /// Tracks all undo- and redoable actions made in a project.
    /// </summary>
    public class CommandManager
    {
        private Stack<ICommand> undoStack = new Stack<ICommand>();
        private Stack<ICommand> redoStack = new Stack<ICommand>();
        private string nameCommandToUndo;
        private string nameCommandToRedo;

        public void AddCommand(ICommand command)
        {
            undoStack.Push(command);
            redoStack.Clear();

            nameCommandToUndo = command.GetCommandName();
        }

        public void Reset()
        {
            undoStack.Clear();
            redoStack.Clear();
        }

        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                var command = undoStack.Pop();
                command.Undo();

                redoStack.Push(command);
            }

            SetCommandNames();
        }

        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                var command = redoStack.Pop();
                command.Redo();

                undoStack.Push(command);
            }

            SetCommandNames();
        }

        public bool CanUndo()
        {
            return undoStack.Count > 0;
        }

        public bool CanRedo()
        {
            return redoStack.Count > 0;
        }

        public string GetNameCommandToUndo()
        {
            return nameCommandToUndo;
        }

        public string GetNameCommandToRedo()
        {
            return nameCommandToRedo;
        }

        private void SetCommandNames()
        {
            if (undoStack.Count > 0)
                nameCommandToUndo = undoStack.Peek().GetCommandName();
            if (redoStack.Count > 0)
                nameCommandToRedo = redoStack.Peek().GetCommandName();
        }
    }
}

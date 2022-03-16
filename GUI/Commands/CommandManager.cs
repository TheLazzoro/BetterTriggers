using System.Collections.Generic;

namespace GUI.Commands
{
    public static class CommandManager
    {
        private static Stack<ICommand> undoStack = new Stack<ICommand>();
        private static Stack<ICommand> redoStack = new Stack<ICommand>();
        private static string nameCommandToUndo;
        private static string nameCommandToRedo;

        public static void AddCommand(ICommand command)
        {
            undoStack.Push(command);
            redoStack.Clear();

            nameCommandToUndo = command.GetCommandName();
        }

        public static void Undo()
        {
            if (undoStack.Count > 0)
            {
                var command = undoStack.Pop();
                command.Undo();

                redoStack.Push(command);
            }

            SetCommandNames();
        }

        public static void Redo()
        {
            if (redoStack.Count > 0)
            {
                var command = redoStack.Pop();
                command.Redo();

                undoStack.Push(command);
            }

            SetCommandNames();
        }

        public static bool CanUndo()
        {
            return undoStack.Count > 0;
        }

        public static bool CanRedo()
        {
            return redoStack.Count > 0;
        }

        public static string GetNameCommandToUndo()
        {
            return nameCommandToUndo;
        }

        public static string GetNameCommandToRedo()
        {
            return nameCommandToRedo;
        }

        private static void SetCommandNames()
        {
            if (undoStack.Count > 0)
                nameCommandToUndo = undoStack.Peek().GetCommandName();
            if (redoStack.Count > 0)
                nameCommandToRedo = redoStack.Peek().GetCommandName();
        }
    }
}

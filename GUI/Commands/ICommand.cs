using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Commands
{
    public interface ICommand
    {
        void Execute();
        void Undo();
        void Redo();
        string GetCommandName();
    }
}

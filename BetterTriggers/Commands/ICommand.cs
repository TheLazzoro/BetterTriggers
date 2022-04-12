using System;
using System.Collections.Generic;
using System.Text;

namespace BetterTriggers.Commands
{
    public interface ICommand
    {
        void Execute();
        void Undo();
        void Redo();
        string GetCommandName();
    }
}

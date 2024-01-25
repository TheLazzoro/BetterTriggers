using System.Threading;
using System;
using System.Windows.Input;

namespace GUI.Commands;

public class RelayCommand<T> : ICommand
{
    private Predicate<T> _canExecute;
    private Action<T> _execute;

    public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    private void Execute(T parameter)
    {
        _execute(parameter);
    }

    private bool CanExecute(T parameter)
    {
        return _canExecute == null ? true : _canExecute(parameter);
    }

    public bool CanExecute(object parameter)
    {
        return parameter == null ? false : CanExecute((T)parameter);
    }

    public void Execute(object parameter)
    {
        _execute((T)parameter);
    }

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        var temp = Volatile.Read(ref CanExecuteChanged);

        if (temp != null)
            temp(this, new EventArgs());
    }
}
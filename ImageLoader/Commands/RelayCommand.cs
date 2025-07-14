using System.Windows.Input;

namespace ImageLoader.Commands;


public class RelayCommand: ICommand
{
    private readonly Func<object, Task> _execute;
    private readonly Func<object, bool> _canExecute;
    public RelayCommand(Func<object, Task> execute, Func<object, bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    // Реализация интерфейса ICommand
    public bool CanExecute(object? parameter)
    {
        return (_canExecute?.Invoke(parameter) ?? true);
    }

    public async void Execute(object? parameter)
    {
        if (CanExecute(parameter))
        {
            try
            {
                CommandManager.InvalidateRequerySuggested();
                await _execute(parameter);
            }
            finally
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
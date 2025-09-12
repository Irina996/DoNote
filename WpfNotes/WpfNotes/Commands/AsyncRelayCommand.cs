using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfNotes.Commands
{
    internal class AsyncRelayCommand : ICommand
    {
        private readonly Func<object, Task> _executeAction;
        private readonly Func<bool> _canExecuteAction;

        public AsyncRelayCommand(Func<object, Task> executeAction, Func<bool> canExecuteAction)
        {
            _executeAction = executeAction;
            _canExecuteAction = canExecuteAction;
        }

        public AsyncRelayCommand(Func<object, Task> executeAction)
        {
            _executeAction = executeAction;
            _canExecuteAction = () => true;
        }

        public AsyncRelayCommand(Func<Task> executeAction, Func<bool> canExecuteAction)
        {
            _executeAction = _ => executeAction();
            _canExecuteAction = canExecuteAction;
        }

        public AsyncRelayCommand(Func<Task> executeAction)
        {
            _executeAction = _ => executeAction();
            _canExecuteAction = () => true;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecuteAction();
        }

        public void Execute(object? parameter)
        {
            _executeAction(parameter);
        }
    }
}

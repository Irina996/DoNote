using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfNotes.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _executeAction;
        private readonly Func<bool> _canExecuteAction;

        public RelayCommand(Action<object> executeAction) {
            _executeAction = executeAction;
            _canExecuteAction = null;
        }

        public RelayCommand(Action<object> executeAction, Func<bool> canExecuteAction) {
            _executeAction = executeAction;
            _canExecuteAction = canExecuteAction;
        }

        public RelayCommand(Action executeAction) : this(_ => executeAction())
        {
        }

        public RelayCommand(Action executeAction, Func<bool> canExecuteAction)
        {
            _executeAction = _ => executeAction();
            _canExecuteAction = canExecuteAction;
        }

        public event EventHandler CanExecuteChanged 
        {
            add {CommandManager.RequerySuggested += value;}
            remove { CommandManager.RequerySuggested -= value;}
        }

        public bool CanExecute(object parameter) 
        {
            return _canExecuteAction == null ? true : _canExecuteAction();
        }

        public void Execute(object parameter) 
        {
            _executeAction(parameter);   
        }
    }
}

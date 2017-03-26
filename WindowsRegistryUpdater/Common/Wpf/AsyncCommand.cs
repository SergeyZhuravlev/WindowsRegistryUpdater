using Common.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Common.Wpf
{
    public class AsyncCommand : ICommand
    {
        private bool _executeNow;
        private Predicate<object> CanExecutePredicate { get; set; }
        private Func<object, Task> _execute;

        public AsyncCommand(Func<Task> execute)
            : this(async _ => await execute(), _ => true)
        { }

        public AsyncCommand(Func<Task> execute, Func<bool> canExecute)
            : this(async _ => await execute(), _ => canExecute())
        { }

        public AsyncCommand(Func<object, Task> execute, Predicate<object> canExecute)
        {
            CanExecutePredicate = _ => !_executeNow && canExecute(_);
            _execute = async _ =>
            {
                _executeNow = true;
                CommandManager.InvalidateRequerySuggested();
                await execute(_);
                _executeNow = false;
                CommandManager.InvalidateRequerySuggested();
            };
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return CanExecutePredicate(parameter);
        }

        public async void Execute(object parameter)
        {
            await _execute(parameter);
        }
    }
}

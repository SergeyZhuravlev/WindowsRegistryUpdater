using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Common.Wpf
{
    public class RelayCommand : ICommand
    {
        public Predicate<object> CanExecutePredicate { get; set; }

        private Action<object> _execute;

        public RelayCommand(Action<object> execute)
            : this(execute, _ => true)
        { }

        public RelayCommand(Action execute)
            : this(_ => execute(), _ => true)
        { }

        public RelayCommand(params Action[] executes)
            : this(_ => { foreach (var execute in executes) execute(); }, _ => true)
        { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.CanExecutePredicate = canExecute;
            this._execute = execute;
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

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp2.ViewModel
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged { add { } remove { } }
        private readonly Action _action;

        public DelegateCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged { add { } remove { } }
        private readonly Action<T> _action;

        public DelegateCommand(Action<T> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action((T)parameter);
        }
    }
}

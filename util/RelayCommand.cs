using System;
using System.Windows.Input;

namespace DN.NSC.RecentRepairs.util
{
    namespace DN.NSC.RecentRepairs
    {
        public class RelayCommand : ICommand
        {
            private readonly Func<object, bool> _canexecuteMethod;
            private readonly Action<object> _execteMethod;

            public RelayCommand(Action<object> execteMethod, Func<object, bool> canexecuteMethod)
            {
                _execteMethod = execteMethod;
                _canexecuteMethod = canexecuteMethod;
            }

            public bool CanExecute(object parameter)
            {
                if (_canexecuteMethod != null)
                    return _canexecuteMethod(parameter);
                return false;
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public void Execute(object parameter)
            {
                _execteMethod(parameter);
            }
        }
    }
}
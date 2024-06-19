using System;
using System.Windows.Input;

namespace NoSleepHD
{
    public class StringCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public Action<string> ExecuteAction;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter == null) {
                return;
            }

            ExecuteAction.Invoke((string) parameter);
        }

        public StringCommand(Action<string> action)
        {
            ExecuteAction = action;
        }
    }
}

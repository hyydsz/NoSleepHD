using System;
using System.Windows.Input;

namespace NoSleepHD.Assets
{
    internal class MyCommand : ICommand
    {
        private readonly Action<object> execAction;

        public MyCommand(Action<object> execAction)
        {
            this.execAction = execAction;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.execAction.Invoke(parameter);
        }
    }

    internal class MainCommand
    {
        public ICommand MyCommand => new MyCommand(AllButtonHandler);

        private void AllButtonHandler(object parameter)
        {
            if (MainWindow.ButtonHandler != null)
            {
                MainWindow.ButtonHandler.Invoke(parameter);
            }
        }
    }
}

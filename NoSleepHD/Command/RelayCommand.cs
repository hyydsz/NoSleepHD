﻿using System;
using System.Windows.Input;

namespace NoSleepHD.Command
{
    public class RelayCommand<T> : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private Action<T> action;
        public RelayCommand(Action<T> action)
        {
            this.action = action;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter != null)
                action.Invoke((T)parameter);
        }
    }
}

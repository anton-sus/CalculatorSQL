using System;
using System.Windows.Input;

// Реализация команды, используемой в WPF для связывания действия пользователя с логикой приложения
namespace CalculatorSQL
{   
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        // Конструктор принимает действие и опционально условие
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        // Событие, которое генерируется при изменении состояния, указывающего, может ли команда быть выполнена
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Метод, который определяет, может ли команда выполняться
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        // Метод, который выполняет действие, связанное с командой
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}

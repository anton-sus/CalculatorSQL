using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;


namespace CalculatorSQL
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        // контекст базы данных
        AppContext db;

        // событие изменения свойства
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        // команды для кнопок
        public ICommand DigitButtonCommand { get; }
        public ICommand OperationButtonCommand { get; }
        public ICommand FunctionalButtonCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ShowCommand { get; }

        // переменные для хранения информации
        private string lastOperation;
        private bool newDisplayRequired = false;
        private string fullExpression;
        private string display;

        // объект библиотеки расчетов
        private CalculationsLib calculation;

        // свойства для доступа к переменным в объекте библиотеки расчетов
        public string FirstOperand
        {
            get { return calculation.FirstOperand; }
            set { calculation.FirstOperand = value; }
        }
        public string SecondOperand
        {
            get { return calculation.SecondOperand; }
            set { calculation.SecondOperand = value; }
        }
        public string Operation
        {
            get { return calculation.Operation; }
            set { calculation.Operation = value; }
        }
        public string LastOperation
        {
            get { return lastOperation; }
            set { lastOperation = value; }
        }
        public string Result
        {
            get { return calculation.Result; }
        }
        public string Display
        {
            get { return display; }
            set
            {
                display = value;
                OnPropertyChanged("Display");
            }
        }
        public string FullExpression
        {
            get { return fullExpression; }
            set
            {
                fullExpression = value;
                OnPropertyChanged("FullExpression");
            }
        }

        // конструктор класса
        public MainWindowViewModel()
        {
            // инициализация объектов
            this.calculation = new CalculationsLib();
            this.display = "0";
            this.FirstOperand = string.Empty;
            this.SecondOperand = string.Empty;
            this.Operation = string.Empty;
            this.lastOperation = string.Empty;
            this.fullExpression = string.Empty;

            // создание команд
            DigitButtonCommand = new RelayCommand(OnDigitButtonCommand);
            OperationButtonCommand = new RelayCommand(OnOperationButtonCommand);
            FunctionalButtonCommand = new RelayCommand(OnFunctionalButtonCommand);
            ClearCommand = new RelayCommand(OnClearCommand);
            ShowCommand = new RelayCommand(OnShowCommand);

            // создание контекста базы данных
            db = new AppContext();
        }


        /// <summary>
        /// Обработчик команды для функциональных кнопок (+/-, ., Del, C).
        /// </summary>
        /// <param name="operation">Операция, связанная с нажатой кнопкой</param>
        public void OnFunctionalButtonCommand(object operation)
        {
            try
            {
                FirstOperand = Display;
                Operation = operation as String;
                calculation.CalculateResult();

                // формируем выражение и сохраняем в базу данных
                FullExpression = Operation + "(" + Math.Round(Convert.ToDouble(FirstOperand), 10) + ") = "
                    + Math.Round(Convert.ToDouble(Result), 10);
                AddToDb(FullExpression);

                LastOperation = "=";
                Display = Result;
                FirstOperand = display;
                newDisplayRequired = true;
            }
            catch (Exception ex)
            {
                Display = Result == string.Empty ? "Error" : Result;
                LogExceptionInformation(ex);
            }
        }

        /// <summary>
        /// Обработчик нажатия на кнопку математической операции.
        /// </summary>
        /// <param name="operation">Операция, переданная как параметр.</param>
        private void OnOperationButtonCommand(object operation)
        {
            try
            {
                // Если первый операнд пустой или последняя операция была "=",
                // то присваиваем текущее значение дисплея к первому операнду и запоминаем текущую операцию
                if (FirstOperand == string.Empty || LastOperation == "=")
                {
                    FirstOperand = display;
                    LastOperation = operation as String;
                }
                else
                {
                    // Иначе, если первый операнд уже есть, то запоминаем текущее значение дисплея как второй операнд,
                    // выполняем предыдущую операцию и сохраняем полное выражение и результат в базу данных
                    SecondOperand = display;
                    Operation = lastOperation;
                    calculation.CalculateResult();

                    FullExpression = Math.Round(Convert.ToDouble(FirstOperand), 10) + " " + Operation + " "
                                    + Math.Round(Convert.ToDouble(SecondOperand), 10) + " = "
                                    + Math.Round(Convert.ToDouble(Result), 10);
                    AddToDb(FullExpression);

                    // Запоминаем текущую операцию, устанавливаем текущий результат в дисплей,
                    // и делаем текущее значение дисплея первым операндом
                    LastOperation = operation as String;
                    Display = Result;
                    FirstOperand = display;
                }
                newDisplayRequired = true; // Указываем, что новое значение дисплея нужно выводить
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, то выводим сообщение об ошибке на дисплей
                // и записываем информацию об ошибке в журнал
                Display = Result == string.Empty ? "Error" : Result;
                LogExceptionInformation(ex);
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку цифры или другую функцию калькулятора.
        /// </summary>
        /// <param name="button">Название кнопки, которую нажал пользователь.</param>
        private void OnDigitButtonCommand(object button)
        {
            switch (button as String)
            {
                case "C":
                    Display = "0";
                    FirstOperand = string.Empty;
                    SecondOperand = string.Empty;
                    Operation = string.Empty;
                    LastOperation = string.Empty;
                    FullExpression = string.Empty;
                    break;
                case "Del":
                    if (display.Length > 1)
                        Display = display.Substring(0, display.Length - 1);
                    else Display = "0";
                    break;
                case "+/-":
                    if (display.Contains("-") || display == "0")
                    {
                        Display = display.Remove(display.IndexOf("-"), 1);
                    }
                    else Display = "-" + display;
                    break;
                case ".":
                    if (newDisplayRequired)
                    {
                        Display = "0.";
                    }
                    else
                    {
                        if (!display.Contains("."))
                        {
                            Display = display + ".";
                        }
                    }
                    break;
                default:
                    if (display == "0" || newDisplayRequired)
                        Display = button as String;
                    else
                        Display = display + button;
                    break;
            }
            // После выполнения действия над дисплеем устанавливаем флаг newDisplayRequired на false,
            // чтобы следующее нажатие на цифровую кнопку начинало ввод нового числа, а не продолжало ввод текущего.
            newDisplayRequired = false;
        }

        /// <summary>
        /// Метод записывает информацию об исключении в лог и сохраняет ее в базу данных
        /// </summary>
        /// <param name="ex">Объект исключения</param>
        private void LogExceptionInformation(Exception ex)
        {
            FullExpression = ex.Message;
            AddToDb(FullExpression);
        }

        /// <summary>
        /// Метод добавляет информацию в базу данных в виде новой записи журнала.
        /// </summary>
        /// <param name="info">Строка с информацией для добавления в журнал.</param>
        /// <remarks>
        /// Создает новый экземпляр класса Log на основе переданной информации, добавляет его в базу данных и сохраняет изменения.
        /// </remarks>
        private void AddToDb(string info)
        {
            Log exeptionInfo = new Log(info);
            db.Logs.Add(exeptionInfo);
            db.SaveChanges();
        }

        /// <summary>
        /// Обработчик команды для отображения журнала логов. Получает список логов из базы данных,
        /// формирует текстовую строку и отображает ее в диалоговом окне сообщения.
        /// </summary>
        /// <param name="obj">Объект команды, переданный из пользовательского интерфейса.</param>
        private void OnShowCommand(object obj)
        {
            List<Log> logsToPopUp = db.Logs.ToList();
            string str = "";
            foreach (Log log in logsToPopUp)
                str += log.Info + "\n";

            string messageBoxText = str;
            string caption = "Logs";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBoxResult result;

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
        }

        /// <summary>
        /// Обработчик команды очистки лога.
        /// Удаляет все записи из базы данных.
        /// </summary>
        private void OnClearCommand(object obj)
        {
            var logsToRemove = db.Logs.Where(log => log.Info.Length > 0);
            db.Logs.RemoveRange(logsToRemove);
            db.SaveChanges();
        }
    }
}

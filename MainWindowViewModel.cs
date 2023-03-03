using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace CalculatorSQL
{
     class MainWindowViewModel : INotifyPropertyChanged
    {
        AppContext db;

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        public ICommand DigitButtonCommand { get; }
        public ICommand OperationButtonCommand { get; }
        public ICommand FunctionalButtonCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ShowCommand { get; }

        private string lastOperation;
        private bool newDisplayRequired = false;
        private string fullExpression;
        private string display;

        private CalculationsLib calculation;

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
        
        public MainWindowViewModel()
        {
            this.calculation = new CalculationsLib();
            this.display = "0";
            this.FirstOperand = string.Empty;
            this.SecondOperand = string.Empty;
            this.Operation = string.Empty;
            this.lastOperation = string.Empty;
            this.fullExpression = string.Empty;

            DigitButtonCommand = new RelayCommand(OnDigitButtonCommand);
            OperationButtonCommand = new RelayCommand(OnOperationButtonCommand);
            FunctionalButtonCommand = new RelayCommand(OnFunctionalButtonCommand);
            ClearCommand = new RelayCommand(OnClearCommand);
            ShowCommand = new RelayCommand(OnShowCommand);

            db = new AppContext();
        }

       

        public void OnFunctionalButtonCommand(object operation)
        {
            try
            {
                FirstOperand = Display;
                Operation = operation as String;
                calculation.CalculateResult();

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
        private void OnOperationButtonCommand(object operation)
        {
            try
            {
                if (FirstOperand == string.Empty || LastOperation == "=")
                {
                    FirstOperand = display;
                    LastOperation = operation as String;
                }
                else
                {
                    SecondOperand = display;
                    Operation = lastOperation;
                    calculation.CalculateResult();

                    FullExpression = Math.Round(Convert.ToDouble(FirstOperand), 10) + " " + Operation + " "
                                    + Math.Round(Convert.ToDouble(SecondOperand), 10) + " = "
                                    + Math.Round(Convert.ToDouble(Result), 10);
                    AddToDb(FullExpression);

                    LastOperation = operation as String;
                    Display = Result;
                    FirstOperand = display;
                }
                newDisplayRequired = true;
            }
            catch (Exception ex)
            {
                Display = Result == string.Empty ? "Error" : Result;
                LogExceptionInformation(ex);
            }
        }
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
            newDisplayRequired = false;
        }

        private void LogExceptionInformation(Exception ex)
        {
            FullExpression = ex.Message;
            AddToDb(FullExpression);
        }

        private void AddToDb(string info)
        {
            Log exeptionInfo = new Log(info);
            db.Logs.Add(exeptionInfo);
            db.SaveChanges();
        }

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

        private void OnClearCommand(object obj)
        {
            var logsToRemove = db.Logs.Where(log => log.Info.Length>0);
            db.Logs.RemoveRange(logsToRemove);
            db.SaveChanges();
        }
    }
}

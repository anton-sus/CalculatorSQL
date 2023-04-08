using System;

/// <summary>
/// Библиотека вычислений.
/// </summary>
namespace CalculatorSQL
{
    class CalculationsLib
    {
        #region Private members

        private string result;

        #endregion

        #region Constructors
        // Конструктор для двух операндов и операции.
        public CalculationsLib(string firstOperand, string secondOperand, string operation)
        {
            ValidateOperand(firstOperand);
            ValidateOperand(secondOperand);
            ValidateOperation(operation);

            FirstOperand = firstOperand;
            SecondOperand = secondOperand;
            Operation = operation;
            result = string.Empty;
        }

        // Конструктор для одного операнда и операции.
        public CalculationsLib(string firstOperand, string operation)
        {
            ValidateOperand(firstOperand);
            ValidateOperation(operation);

            FirstOperand = firstOperand;
            SecondOperand = string.Empty;
            Operation = operation;
            result = string.Empty;
        }

        // Конструктор по умолчанию.
        public CalculationsLib()
        {
            FirstOperand = string.Empty;
            SecondOperand = string.Empty;
            Operation = string.Empty;
            result = string.Empty;
        }

        #endregion

        #region Public properties and methods

        public string FirstOperand { get; set; }
        public string SecondOperand { get; set; }
        public string Operation { get; set; }
        public string Result { get { return result; } }

        /// <summary>
        /// Вычисление результата операции.
        /// </summary>
        public void CalculateResult()
        {
            ValidateData();

            try
            {
                switch (Operation)
                {
                    case ("+"):
                        result = (Convert.ToDouble(FirstOperand) + Convert.ToDouble(SecondOperand)).ToString();
                        break;

                    case ("-"):
                        result = (Convert.ToDouble(FirstOperand) - Convert.ToDouble(SecondOperand)).ToString();
                        break;

                    case ("*"):
                        result = (Convert.ToDouble(FirstOperand) * Convert.ToDouble(SecondOperand)).ToString();
                        break;

                    case ("/"):
                        result = (Convert.ToDouble(FirstOperand) / Convert.ToDouble(SecondOperand)).ToString();
                        break;

                    case ("%"):
                        result = (Convert.ToDouble(FirstOperand) / 100.0).ToString();
                        break;

                    case ("sqr"):
                        result = Math.Sqrt(Convert.ToDouble(FirstOperand)).ToString();
                        break;

                    case ("pow"):
                        double operand1 = Convert.ToDouble(FirstOperand);
                        int operand2 = Convert.ToInt32(SecondOperand);
                        result = Math.Pow(operand1, operand2).ToString();
                        break;
                }
            }
            catch (Exception)
            {
                result = "Error";
                throw;
            }
        }

        /// <summary>
        /// Метод, который проверяет, является ли входная строка операндом, конвертируя ее в double.
        /// Если строка не может быть конвертирована в double, метод выбрасывает исключение.
        /// </summary>
        /// <param name="operand">Строка, которую нужно проверить на то, является ли она операндом.</param>
        private void ValidateOperand(string operand)
        {
            try
            {
                Convert.ToDouble(operand);
            }
            catch (Exception)
            {
                result = "Invalid number: " + operand;
                throw;
            }
        }

        /// <summary>
        /// Метод, который проверяет, является ли входная строка операцией, которую можно использовать в калькуляторе.
        /// Если строка не соответствует допустимым операциям, метод выбрасывает исключение.
        /// </summary>
        /// <param name="operation">Строка, которую нужно проверить на то, является ли она операцией.</param>
        private void ValidateOperation(string operation)
        {
            switch (operation)
            {
                case "/":
                case "*":
                case "-":
                case "+":
                case "%":
                case "sqr":
                case "pow":
                    break;
                default:
                    result = "Unknown operation: " + operation;
                    throw new ArgumentException("Unknown Operation: " + operation, "operation");
            }
        }

        /// <summary>
        /// Метод, который проверяет данные перед их использованием в вычислениях.
        /// Если данные не соответствуют правилам, метод выбрасывает исключение.
        /// </summary>
        private void ValidateData()
        {
            switch (Operation)
            {
                case "/":
                case "*":
                case "-":
                case "+":
                case "pow":
                    ValidateOperand(FirstOperand);
                    ValidateOperand(SecondOperand);
                    break;
                case "%":
                case "sqr":
                    ValidateOperand(FirstOperand);
                    break;
                default:
                    result = "Unknown operation: " + Operation;
                    throw new ArgumentException("Unknown Operation: " + Operation, "operation");
            }
        }

        #endregion
    }
}


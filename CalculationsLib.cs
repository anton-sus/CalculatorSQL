using System;

namespace CalculatorSQL
{
    class CalculationsLib
    {
        #region Private members

        private string result;

        #endregion

        #region Constructors

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

        public CalculationsLib(string firstOperand, string operation)
        {
            ValidateOperand(firstOperand);
            ValidateOperation(operation);

            FirstOperand = firstOperand;
            SecondOperand = string.Empty;
            Operation = operation;
            result = string.Empty;
        }

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


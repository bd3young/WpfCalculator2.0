using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Input;

namespace WpfCalculator.ViewModels
{
    class CalculatorViewModel : ObservableObject
    {

        #region Enums

        private enum Operation
        {
            None,
            Add,
            Subtract,
            Multiply,
            Divide,
            Percent,
            Sin,
            Cos,
            Tan,
            Square,
            SquareRoot,
            Equal
        }

        private Dictionary<string, Operation> BinaryOperations = new Dictionary<string, Operation>()
        {
            { "+", Operation.Add },
            { "-", Operation.Subtract },
            { "*", Operation.Multiply },
            { "/", Operation.Divide },
            { "%", Operation.Percent },
            { "=", Operation.Equal }
        };

        private Dictionary<string, Operation> UnaryOperations = new Dictionary<string, Operation>()
        {
            { "Sin", Operation.Sin },
            { "Cos", Operation.Cos },
            { "Tan", Operation.Tan },
            { "Sqr", Operation.Square },
            { "SqrRt", Operation.SquareRoot }
        };

        #endregion
       
        #region PROPERTIES

        bool Counter = false;
        bool ClearDisplay = false;
        bool OpPressed = false;
        bool EqualPressed = false;
        bool EqualOn = false;
        double Operand;
        double OperandMathed;
        string OperationSymbol = "";

        private static string _operandString;
        private static double _operand1;
        private static double _operand2;
        private static Operation _binaryOperator;


        public ICommand ButtonNumberCommand { get; set; }
        public ICommand ButtonOperationCommand { get; set; }
        private Operation CurrentOperation { get; set; }

        private string _displayContent;




        #endregion

        #region CONSTRUCTORS

        #endregion

        #region FIELDs

        public string DisplayContent
        {
            get { return _displayContent; }
            set
            {
                _displayContent = value;
                OnPropertyChanged("DisplayContent");
            }
        }

        #endregion

        #region METHODS

        public CalculatorViewModel()
        {
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            _displayContent = "Enter Number";
            ButtonNumberCommand = new RelayCommand(new Action<object>(NumberPressed));
            ButtonOperationCommand = new RelayCommand(new Action<object>(OperationPressed));
        }

        private Operation CurrentOperator(string operationString)
        {
            if (BinaryOperations.ContainsKey(operationString))
            {
                return BinaryOperations[operationString];
            }
            else if (UnaryOperations.ContainsKey(operationString))
            {
                return UnaryOperations[operationString];
            }

            return Operation.None;
        }

        // Building Operand Methods

        private void NumberPressed(object obj)
        {
            if (obj.ToString() == "CE")
            {
                Clear();

            }
            else if (obj.ToString() == "CD")
            {
                if (OpPressed == false)
                {
                    if (EqualOn == false)
                    {
                        DisplayClear();
                    }

                }
            }

            else
            {
                if (EqualOn == false)
                {
                    BuildNumber(obj);
                    DisplayContent = _operandString;
                }

            }
            
        }

        private void BuildNumber(object obj)
        {
            if (ClearDisplay == true)
            {
                DisplayClear();
                Operand = 0;
                ClearDisplay = false;
            }
            _operandString += obj.ToString();
            double.TryParse(_operandString, out Operand);
            OpPressed = false;
        }

        //Operations method

        private void OperationPressed(object obj)
        {
            if (OpPressed == false)
            {
            
            Operation operation = CurrentOperator(obj.ToString());

            if (double.TryParse(_operandString, out double result))
            {
                if (BinaryOperations.ContainsValue(operation))
                {
                    if (operation == Operation.Equal)
                    {
                        if (EqualOn == false)
                        {
                            EqualIsPressed();
                            EqualOn = true;
                        }                       
                        EqualPressed = true;

                    }

                    else if (Counter == true)
                    {
                        if (EqualPressed == true)
                        {
                            CalculateOperand1();
                            CheckBinary(operation, result);
                            DisplayOperation();
                            EqualPressed = false;
                            ClearDisplay = true;
                            OpPressed = true;
                            EqualOn = false;
                        }
                        else
                        {
                            CalculateOperand2();
                            CheckBinary(operation, result);
                            DisplayOperation();
                            OpPressed = true;
                            EqualOn = false;
                        }


                    }


                    //need to move somewhere else
                    else
                    {

                        CheckBinary(operation, result);
                        CalculateOperand1();
                        Counter = true;
                        DisplayOperation();
                        OpPressed = true;
                        EqualPressed = false;
                        EqualOn = false;

                    }
                }
                else if (UnaryOperations.ContainsValue(operation))
                {
                        if (Counter == false)
                        {
                            UnaryCalculation(operation);
                        }


                }
                else
                {
                    DisplayContent = "Unknown Operation Encountered";
                }

            }
            else
            {
                DisplayContent = "Please enter a valid number.";
            }
            }
        }

        // Displays Operation Methods

        private void DisplayOperation()
        {
            DisplayContent = $"{OperandMathed} {OperationSymbol} ";
        }

        private void CheckBinary(Operation operation, double result)
        {
            if (Counter == true)
            {
                _operand1 = OperandMathed;
            }
            else
            {
                _operand1 = Operand;
            }
            _binaryOperator = operation;
            switch (operation)
            {
                case Operation.None:

                    break;
                case Operation.Add:
                    OperationSymbol = "+";
                    break;
                case Operation.Subtract:
                    OperationSymbol = "-";
                    break;
                case Operation.Multiply:
                    OperationSymbol = "*";
                    break;
                case Operation.Divide:
                    OperationSymbol = "/";
                    break;
                case Operation.Percent:
                    OperationSymbol = "%";
                    break;
                case Operation.Sin:
                    break;
                case Operation.Cos:
                    break;
                case Operation.Tan:
                    break;
                case Operation.Square:
                    break;
                case Operation.SquareRoot:
                    break;
                case Operation.Equal:
                    break;
                default:
                    break;
            }
        }

		private void PercentIsPressed()
		{
			_operand2 = Operand;
			_binaryOperator = Operation.Percent;
			DisplayContent = BinaryOperation(_binaryOperator).ToString();
		}

		private void EqualIsPressed()
		{
			_operand2 = Operand;
			DisplayContent = BinaryOperation(_binaryOperator).ToString();
			_binaryOperator = Operation.None;
			double.TryParse(DisplayContent, out OperandMathed);
            _operand1 = OperandMathed;           
		}

        //calculation methods

        private void CalculateOperand1()
        {
            if (Counter == true)
            {
                _operand1 = OperandMathed;
            }
            else
            {
                OperandMathed = _operand1;
            }

            ClearDisplay = true;
        }

        private void CalculateOperand2()
        {

            _operand2 = Operand;
            _operand1 = OperandMathed;
            DisplayContent = BinaryOperation(_binaryOperator).ToString();
            double.TryParse(DisplayContent, out OperandMathed);
            _binaryOperator = Operation.None;
            Operand = 0;
            ClearDisplay = true;

        }

        private void UnaryCalculation(Operation operation)
		{
            double.TryParse(DisplayContent, out _operand1);
            DisplayContent = UnaryOperation(operation).ToString();
            double.TryParse(DisplayContent, out OperandMathed);
        }

		private object UnaryOperation(Operation operation)
		{
			switch (operation)
			{
				case Operation.Sin:
					return Math.Sin(_operand1);
				case Operation.Cos:
					return Math.Cos(_operand1);
				case Operation.Tan:
					return Math.Tan(_operand1);
				case Operation.Square:
					return Math.Pow(_operand1, 2);
				case Operation.SquareRoot:
					return Math.Sqrt(_operand1);
				default:
					DisplayContent = "Unknown Operation Encountered";
					return 0;
			}
		}

		private object BinaryOperation(Operation binaryOperator)
		{

			switch (binaryOperator)
			{
				case Operation.Add:
					return _operand1 + _operand2;
				case Operation.Subtract:
					return _operand1 - _operand2;
				case Operation.Multiply:
					return _operand1 * _operand2;
				case Operation.Divide:
					return _operand1 / _operand2;
				case Operation.Percent:
					return _operand1 * (_operand2 / 100);
				default:
					DisplayContent = "Unknown Operation Encountered";
					return 0;
			}
		}

        // Both Clears

        private void DisplayClear()
        {
            _operandString = "";
            DisplayContent = "";
        }

        private void Clear()
        {
            _operandString = "";
            DisplayContent = "";
            Counter = false;
            ClearDisplay = false;
            OpPressed = false;
            EqualPressed = false;
            EqualOn = false;
        }
    }

		#endregion

}

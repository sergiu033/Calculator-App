using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Markup;
using System.Collections.ObjectModel;

namespace CalculatorApp
{
    enum CalculatorMode
    {
        Standard,
        Programmer
    }
    enum ProgrammerBase
    {
        Hex,
        Dec,
        Oct,
        Bin
    }

    internal class Calculator : INotifyPropertyChanged
    {
        private List<string> equationInput;
        private string equation;
        private string result;
        public ObservableCollection<string> memoryStack { get; set; }
        private bool digitGrouping;
        private CalculatorMode selectedMode;
        private ProgrammerBase selectedBase;

        private static string binaryNumberRegex = @"^1[01]*$";
        private static string octalNumberRegex = @"^[1-7][0-7]*$";
        private static string hexadecimalNumberRegex = @"^[1-9ABCDEF][0-9ABCDEF]*$";
        private static string numberRegex = @"^\-?\d+\.?\d*$";
        private static string operatorRegex = @"^(?:\+|\-|\*|\/|\%|\=)$";
        private static string functionRegex = @"^(?:sqrt|\^2|1/|\+/\-)$";
        public string Equation
        {
            get { return equation; }
            set
            {
                equation = value;
                OnPropertyChanged(nameof(Equation));
            }
        }
        public string Result
        {
            get { return result; }
            set
            {
                result = value;
                OnPropertyChanged(nameof(Result));
            }
        }
        public bool DigitGrouping
        {
            get { return digitGrouping; }
            set
            {
                digitGrouping = value;
                OnPropertyChanged(nameof(DigitGrouping));
            }
        }

        public CalculatorMode SelectedMode
        {
            get { return selectedMode; }
            set
            {
                selectedMode = value;
                OnPropertyChanged(nameof(SelectedMode));
            }
        }

        public ProgrammerBase SelectedBase
        {
            get { return selectedBase; }
            set
            {
                selectedBase = value;
                OnPropertyChanged(nameof(SelectedBase));
            }
        }

        public void SetSelectedMode(CalculatorMode mode)
        {
            SelectedMode = mode;
        }

        public void SetProgrammerBase(ProgrammerBase pBase)
        {
            SelectedBase = pBase;
        }

        public Calculator()
        {
            equationInput = new List<string>();
            memoryStack = new ObservableCollection<string>();
            digitGrouping = false;
            selectedMode = CalculatorMode.Standard;
            selectedBase = ProgrammerBase.Dec;
            equation = "";
            result = "0";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string UseDigitGrouping(string input)
        {
            string realPart = "";
            string floatPart = "";

            if (input.Contains("."))
            {
                string[] parts = input.Split(new[] { '.' }, 2);
                realPart = parts[0];
                floatPart = parts[1];
            }
            else
                realPart = input;

            string newInput = "";

            char[] charArray = realPart.ToCharArray();
            Array.Reverse(charArray);
            
            for (int i = 0; i < realPart.Length; i++)
            {
                if (i % 3 == 0)
                    newInput = ',' + newInput;
                newInput = charArray[i] + newInput;
            }

            if (newInput[newInput.Length - 1] == ',')
                newInput = newInput.Remove(newInput.Length - 1);

            if (!input.Contains("."))
                return newInput;
            else
                return newInput + "." + floatPart;
        }
        private void Evaluate()
        {
            double lastNumber = 0;
            string currentOperator = "";
            bool waitingOperation = false;
            bool dividedByZero = false;
            bool squaredRootNegativeNumber = false;
            foreach (var inp in equationInput)
            {
                if (Regex.IsMatch(inp, numberRegex))
                {
                    if (!waitingOperation)
                        lastNumber = Convert.ToDouble(inp);
                    else if (waitingOperation)
                    {
                        if (currentOperator == "+")
                        {
                            double temp = lastNumber + Convert.ToDouble(inp);
                            lastNumber = temp;
                        }
                        if (currentOperator == "-")
                        {
                            double temp = lastNumber - Convert.ToDouble(inp);
                            lastNumber = temp;
                        }
                        if (currentOperator == "*")
                        {
                            double temp = lastNumber * Convert.ToDouble(inp);
                            lastNumber = temp;
                        }
                        if (currentOperator == "/")
                        {
                            if (inp == "0")
                            {
                                Result = "Cannot divide by zero";
                                dividedByZero = true;
                            }
                            else
                            {
                                double temp = lastNumber / Convert.ToDouble(inp);
                                lastNumber = temp;
                            }
                        }
                        if (currentOperator == "%")
                        {
                            double temp = lastNumber % Convert.ToDouble(inp);
                            lastNumber = temp;
                        }
                        waitingOperation = false;
                    }
                }
                else if (Regex.IsMatch(inp, operatorRegex))
                {
                    if (inp == "+")
                        currentOperator = "+";
                    if (inp == "-")
                        currentOperator = "-";
                    if (inp == "*")
                        currentOperator = "*";
                    if (inp == "/")
                        currentOperator = "/";
                    if (inp == "%")
                        currentOperator = "%";
                    waitingOperation = true;
                }
                else if (inp == "^2")
                {
                    lastNumber *= lastNumber;
                }
                else if (inp == "sqrt")
                {
                    if (lastNumber < 0)
                    {
                        Result = "Cannot sqrt a negative number";
                        squaredRootNegativeNumber = true;
                    }
                    else
                    {
                        lastNumber = Math.Sqrt(lastNumber);
                    }
                }
                else if (inp == "1/")
                {
                    if (lastNumber == 0)
                    {
                        Result = "Cannot divide by zero";
                        dividedByZero = true;
                    }
                    else
                    {
                        lastNumber = 1 / lastNumber;
                    }
                }
                else if (inp == "+/-")
                {
                    if (lastNumber != 0)
                        lastNumber = -lastNumber;
                }
            }
            if (!dividedByZero && !squaredRootNegativeNumber)
            {
                string finalResult = Convert.ToString(lastNumber);
                if (!DigitGrouping)
                    Result = finalResult;
                else
                    Result = UseDigitGrouping(finalResult);
            }
        }

        private void BuildEquation()
        {
            Equation = "";
            foreach (string input in equationInput)
            {
                if (Regex.IsMatch(input, numberRegex))
                {
                    if (!DigitGrouping)
                        Equation += input;
                    else
                        Equation += UseDigitGrouping(input);
                }
                else if (Regex.IsMatch(input, operatorRegex))
                {
                    Equation += input;
                }
                else if (Regex.IsMatch(input, functionRegex))
                {
                    if (input == "^2")
                    {
                        Equation = "(" + Equation + ")^2";
                    }
                    if (input == "sqrt")
                    {
                        Equation = "sqrt(" + Equation + ")";
                    }
                    if (input == "1/")
                    {
                        Equation = "1/(" + Equation + ")";
                    }
                    if (input == "+/-")
                    {
                        Equation = "-(" + Equation + ")";
                    }
                }
            }
        }

        public void AddInput(string input)
        {
            //if the user has started the input
            if (equationInput.Count != 0)
            {
                if (Regex.IsMatch(input, operatorRegex))
                    Evaluate();
                if (equationInput.Last() == "=")
                {
                    Equation = "";
                    Result = "0";
                    equationInput.Clear();
                }
                else if (Regex.IsMatch(equationInput.Last(), numberRegex))
                {
                    if (input == "." && !equationInput.Last().Contains("."))
                    {
                        equationInput[equationInput.Count() - 1] += input;
                    }
                    else if (Regex.IsMatch(input, numberRegex))
                    {
                        //if last input is 0 we change the 0 into the input (to prevent multiple 0s in a row)
                        if (equationInput.Last() == "0")
                        {
                            equationInput[equationInput.Count() - 1] = input;
                        }
                        else
                        {
                            equationInput[equationInput.Count() - 1] += input;
                        }
                        if (!digitGrouping)
                            Result = equationInput.Last();
                        else
                            Result = UseDigitGrouping(equationInput.Last());
                    }
                    else if (Regex.IsMatch(input, operatorRegex))
                    {
                        equationInput.Add(input);
                    }
                    else if (Regex.IsMatch(input, functionRegex))
                    {
                        equationInput.Add(input);
                    }
                }
                else if (Regex.IsMatch(equationInput.Last(), operatorRegex))
                {
                    if (Regex.IsMatch(input, numberRegex))
                    {
                        equationInput.Add(input);
                    }
                    //if current input is an operator we switch it with the current operator
                    else if (Regex.IsMatch(input, operatorRegex))
                    {
                        equationInput[equationInput.Count() - 1] = input;
                    }
                    else if (Regex.IsMatch(input, functionRegex))
                    {
                        equationInput[equationInput.Count() - 1] = input;
                        Evaluate();
                    }
                }
                else if (Regex.IsMatch(input, operatorRegex))
                {
                    if (Regex.IsMatch(equationInput.Last(), operatorRegex))
                    {
                        equationInput.Add(input);
                        Evaluate();
                    }
                }
                else if (Regex.IsMatch(equationInput.Last(), functionRegex))
                {
                    if (Regex.IsMatch(input, functionRegex))
                    {
                        if (input == "+/-")
                        {
                            if (equationInput.Last() == "+/-")
                            {
                                equationInput.Remove(equationInput.Last());
                            }
                            else
                            {
                                equationInput.Add(input);
                            }
                            Evaluate();
                        }
                        else
                        {
                            equationInput.Add(input);
                            Evaluate();
                        }
                    }
                }
            }
            //if the user hasn't started the input
            else if (equationInput.Count == 0)
            {
                if (Regex.IsMatch(input, numberRegex))
                {
                    equationInput.Add(input);
                }
            }
            BuildEquation();
        }

        public void Backspace()
        {
            if (equationInput.Count == 1)
            {
                string lastInput = equationInput.Last();
                if (Regex.IsMatch(lastInput, numberRegex))
                {
                    if (lastInput.Length > 1)
                    {
                        lastInput = lastInput.Substring(0, lastInput.Length - 1);
                        equationInput[equationInput.Count - 1] = lastInput;
                    }
                    else
                    {
                        equationInput[equationInput.Count - 1] = "0";
                    }
                }
            }
            else if (equationInput.Count > 1)
            {
                string lastInput = equationInput.Last();
                int lastInputIndex = equationInput.Count - 1;
                if (Regex.IsMatch(lastInput, operatorRegex) || Regex.IsMatch(lastInput, functionRegex))
                {
                    equationInput.RemoveAt(lastInputIndex);
                }
                else if (Regex.IsMatch(lastInput, numberRegex))
                {
                    if (lastInput.Length > 1)
                    {
                        lastInput = lastInput.Substring(0, lastInput.Length - 1);
                        equationInput[equationInput.Count - 1] = lastInput;
                    }
                    else
                    {
                        equationInput.RemoveAt(lastInputIndex);
                    }
                }
            }
            BuildEquation();
            Evaluate();
        }

        public void Clear()
        {
            equationInput.Clear();
            BuildEquation();
            Evaluate();
        }

        public void ClearE()
        {
            if (equationInput.Count > 0)
            {
                if (Regex.IsMatch(equationInput.Last(), numberRegex))
                {
                    equationInput.RemoveAt(equationInput.Count - 1);
                    Result = "0";
                }
            }
            BuildEquation();
        }

        public void AddToMemoryStack()
        {
            memoryStack.Insert(0, equationInput.Last());
        }

        public void ClearMemoryStack()
        {
            memoryStack.Clear();
        }

        public void AdditionMemoryStack()
        {
            if (memoryStack.Count > 0)
            {
                double number = Convert.ToDouble(equationInput.Last()) + Convert.ToDouble(memoryStack[0]);
                memoryStack[0] = Convert.ToString(number);
            }
        }

        public void SubstractMemoryStack()
        {
            if (memoryStack.Count > 0)
            {
                double number = Convert.ToDouble(memoryStack[0]) - Convert.ToDouble(equationInput.Last());
                memoryStack[0] = Convert.ToString(number);
            }
        }

        public void TopMemoryStack()
        {
            if (!digitGrouping)
                Result = memoryStack[0];
            else
                Result = UseDigitGrouping(memoryStack[0]);
        }
    }
}

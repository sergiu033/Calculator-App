using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CalculatorApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Calculator Cal = new Calculator();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Cal;
        }
        private void SetStandardLayout()
        {
            Cal.SetSelectedMode(CalculatorMode.Standard);
            digitGrouping.Visibility = Visibility.Visible;
            MemoryPad.Visibility = Visibility.Visible;
            StandardPad.Visibility = Visibility.Visible;

            ProgrammerPad.Visibility = Visibility.Collapsed;
            BasePad.Visibility = Visibility.Collapsed;
            ProgrammerBase.Visibility = Visibility.Collapsed;
        }

        private void SetProgrammerLayout()
        {
            Cal.SetSelectedMode(CalculatorMode.Programmer);
            digitGrouping.Visibility = Visibility.Collapsed;
            MemoryPad.Visibility = Visibility.Collapsed;
            StandardPad.Visibility = Visibility.Collapsed;

            ProgrammerPad.Visibility = Visibility.Visible;
            BasePad.Visibility = Visibility.Visible;
            ProgrammerBase.Visibility = Visibility.Visible;
        }

        private void Add_Input_Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickButton = sender as Button;

            string content = clickButton.Content as string;
            Cal.AddInput(content);
        }
        private void Backspace_Button_Click(object sender, RoutedEventArgs e) { Cal.Backspace(); }
        private void Clear_Button_Click(object sender, RoutedEventArgs e) { Cal.Clear(); }
        private void ClearE_Button_Click(object sender, RoutedEventArgs e) { Cal.ClearE(); }
        private void Open_Memory_Stack_Button_Click(object sender, RoutedEventArgs e) 
        {
            if (StandardPad.Visibility == Visibility.Visible)
            {
                StandardPad.Visibility = Visibility.Collapsed;
                Stack.Visibility = Visibility.Visible;
            }
            else
            {
                StandardPad.Visibility = Visibility.Visible;
                Stack.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                // numbers (0-9) and operators %, *
                case Key.D0:
                case Key.NumPad0:
                    Cal.AddInput("0");
                    break;
                case Key.D1:
                case Key.NumPad1:
                    Cal.AddInput("1");
                    break;
                case Key.D2:
                case Key.NumPad2:
                    Cal.AddInput("2");
                    break;
                case Key.D3:
                case Key.NumPad3:
                    Cal.AddInput("3");
                    break;
                case Key.D4:
                case Key.NumPad4:
                    Cal.AddInput("4");
                    break;
                case Key.D5:
                case Key.NumPad5:
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                            Cal.AddInput("%");
                        else
                            Cal.AddInput("5");
                        break;
                    }
                case Key.D6:
                case Key.NumPad6:
                    Cal.AddInput("6");
                    break;
                case Key.D7:
                case Key.NumPad7:
                    Cal.AddInput("7");
                    break;
                case Key.D8:
                case Key.NumPad8:
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                            Cal.AddInput("*");
                        else
                            Cal.AddInput("8");
                        break;
                    }
                case Key.D9:
                case Key.NumPad9:
                    Cal.AddInput("9");
                    break;

                // operators (+, -, /, *)
                case Key.Add:
                case Key.OemPlus:
                    Cal.AddInput("+");
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    Cal.AddInput("-");
                    break;
                case Key.Divide:
                case Key.OemQuestion:
                    Cal.AddInput("/");
                    break;
                case Key.Multiply:
                    Cal.AddInput("*");
                    break;

                // functions (^2, sqrt, 1/)
                case Key.S:
                    Cal.AddInput("^2");
                    break;
                case Key.R:
                    Cal.AddInput("sqrt");
                    break;
                case Key.D:
                    Cal.AddInput("1/");
                    break;

                // = operator and backspace
                case Key.Enter:
                    Cal.AddInput("=");
                    break;
                case Key.Back:
                    Cal.Backspace();
                    break;

                default:
                    break;
            }
        }

        private void Store_In_Memory_Stack_Button_Click(object sender, RoutedEventArgs e)
        {
            Cal.AddToMemoryStack();
        }

        private void Clear_Memory_Stack_Button_Click(object sender, RoutedEventArgs e)
        {
            Cal.ClearMemoryStack();
        }

        private void Add_Memory_Stack_Button_Click(object sender, RoutedEventArgs e)
        {
            Cal.AdditionMemoryStack();
        }

        private void Substract_Memory_Stack_Button_Click(object sender, RoutedEventArgs e)
        {
            Cal.SubstractMemoryStack();
        }

        private void Display_Top_Memory_Stack_Button_Click(object sender, RoutedEventArgs e)
        {
            Cal.TopMemoryStack();
        }

        private void Open_Settings_Click(object sender, MouseButtonEventArgs e)
        {
            SettingsPopUp.IsOpen = !SettingsPopUp.IsOpen;
        }

        private void Set_Mode_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            string mode = clickedButton.Content as string;
            if (mode == "Standard")
                SetStandardLayout();
            else if (mode == "Programmer")
                SetProgrammerLayout();
        }

        private void Set_Programmer_Base(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            string programmerBase = clickedButton.Content as string;
            if (programmerBase == "Bin")
                Cal.SetProgrammerBase(CalculatorApp.ProgrammerBase.Bin);
            else if (programmerBase == "Oct")
                Cal.SetProgrammerBase(CalculatorApp.ProgrammerBase.Oct);
            else if (programmerBase == "Dec")
                Cal.SetProgrammerBase(CalculatorApp.ProgrammerBase.Dec);
            else if (programmerBase == "Hex")
                Cal.SetProgrammerBase(CalculatorApp.ProgrammerBase.Hex);
        }
    }
}
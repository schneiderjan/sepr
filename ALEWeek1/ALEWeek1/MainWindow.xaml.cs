using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ALEWeek1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ViewModel _viewModel;
        private readonly List<Node> _nodes;
        private readonly List<string> _lines;
        private string _infixNotation = "";

        public LogicParser _parser;

        //Removes white spaces
        private static readonly Regex SWhitespace = new Regex(@"\s+");

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new ViewModel
            {
                //By default
                FormulaTextBox = "&(|(A,~(B)),C)",//"&(A,B)",
                TruthTable = new ObservableCollection<string>(),
                SimplifiedTurthTable = new ObservableCollection<string>(),
                MainWindow = this
            };
            DataContext = _viewModel;

        }

        public void Parse(string expression)
        {
            _parser=new LogicParser(expression);
            _parser.Parser(expression);
            InfixNotationLabel.Text = _parser.Infix;
            VariablesTextBox.Text = _parser.VariablesText;


            var header = _parser.Variables[0].ToString();
            for (var i = 1; i < _parser.Variables.Count; i++)
            {
                header = header +"     "+ _parser.Variables[i];
            }

            header = header + "     " + _parser.Infix;

           

            _viewModel.TruthTable.Clear();

            _viewModel.TruthTable.Add(header);
            //_viewModel.SimplifiedTurthTable.Add(header);

            var hexadecimal="";

            for (int i = 0; i < _parser.Values.Count; i=i+_parser.Variables.Count+1)
            {
                var row="";

                row = _parser.Values[i];
                
                for (int j = 1; j < _parser.Variables.Count+1; j++)
                {
                    row = row + "     " + _parser.Values[i+j];

                    if (j == _parser.Variables.Count)
                        hexadecimal = hexadecimal + _parser.Values[i + j];

                }
                
                _viewModel.TruthTable.Add(row);
            }

            HashValueTextBox.Text = HexaDecimal(hexadecimal);

            
        }

        public string HexaDecimal(string bin)
        {
           // //string bin = "10100010";
            //bin.Reverse();
            char[] charArray = bin.ToCharArray();
            Array.Reverse(charArray);
            bin= new string(charArray);
            


            int rest = bin.Length % 4;
            if (rest != 0)
                bin = new string('0', 4 - rest) + bin; //pad the length out to by divideable by 4

            string output = "";

            for (int i = 0; i <= bin.Length - 4; i += 4)
            {
                output += string.Format("{0:X}", Convert.ToByte(bin.Substring(i, 4), 2));
            }

            return output;
        }

    }
}

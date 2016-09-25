using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ALEWeek1
{
    public class LogicParser
    {
        private readonly List<Node> _nodes;
        private readonly List<string> _lines;
        private readonly string _expression;
        public string Infix { get; set; }
        public List<char> Variables { get; set; }
        public string VariablesText { get; set; }

        public List<string> Values { get; set; }

        private static readonly Regex SWhitespace = new Regex(@"\s+");
        /// <summary>
        /// Replaces " " strings with given sring.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceWhitespace(string input, string replacement) { return SWhitespace.Replace(input, replacement); }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="exp"></param>
        public LogicParser(string exp)
        {
            _nodes=new List<Node>();
            _lines=new List<string>();
            _expression = exp;
        }

        /// <summary>
        /// Parses the stirng.
        /// </summary>
        /// <param name="expression"></param>
        public void Parser(string expression)
        {
            _nodes.Clear();
            _lines.Clear();

            var split = _expression.Split(' ', ',', '\0', ')', '(');

            //Possible "" strings.
            var elements = split.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            Array.Reverse(elements);

            var nodeNumber = elements.Length;
            var countNodes = 0;

            var stack = new Stack<Node>();
            foreach (var element in elements)
            {
                if (IsOperator(element))
                {
                    if (element != "~")
                    {
                        if (stack.Count == 0) return;
                        var leftOperand = stack.Pop();

                        var rightOperand = stack.Pop();


                        var node = new Node(element, leftOperand, rightOperand, nodeNumber, 0);
                        stack.Push(new Node(element, leftOperand, rightOperand, nodeNumber, 0));
                        _nodes.Add(node);

                        nodeNumber--;
                        countNodes++;

                    }
                    //Not must be decoupled separately, it can be applied to one element only.
                    else if (element == "~")
                    {
                        var exp = ReplaceWhitespace(_expression, "");
                        var countElem = 0;
                        for (var i = exp.Length - 1; i > 0; i--)
                        {
                            var x = exp[i].ToString();
                            //Check if is other sign than the elements
                            if (x == "," || x == ")" || x == "(")
                            {

                            }
                            else
                            {
                                countElem++;
                                var z = exp[i];
                                if (z.ToString() != "~" || countNodes != countElem - 1) continue;
                                if (exp[i + 3].ToString() == ")"|| exp[i + 2].ToString() ==",")
                                {
                                    if (stack.Count == 0) return;

                                    //If is not to a expression ~(A,B)
                                    var rightOperand = stack.Pop();

                                    var node = new Node(element, null, rightOperand, nodeNumber, 0);
                                    stack.Push(new Node(element, null, rightOperand, nodeNumber, 0));
                                    _nodes.Add(node);

                                    nodeNumber--;
                                    countNodes++;
                                }
                                else
                                {
                                    if (stack.Count == 0) return;

                                    var rightOperand = stack.Pop();


                                    var node = new Node(element, null, rightOperand, nodeNumber, 1);
                                    stack.Push(new Node(element, null, rightOperand, nodeNumber, 1));
                                    _nodes.Add(node);

                                    nodeNumber--;
                                    countNodes++;
                                }
                            }
                        }

                    }
                }
                else
                {
                    var node = new Node(element, nodeNumber, 0);
                    
                    stack.Push(node);
                    _nodes.Add(node);
                    nodeNumber--;
                    countNodes++;
                }
            }

             GenerateGraph();

            Infix = "";
            InOrder(_nodes[_nodes.Count - 1], null);

            //Add variables to main window
            Array.Reverse(elements);
            var variables = elements.Where(element => !IsOperator(element)).Aggregate("", (current, element) => current + element);



            var distinctletters = new string(variables.Distinct().ToArray());
            Variables =new List<char>();
            for (var i = 0; i < distinctletters.Length; i++)
            {
                Variables.Add(distinctletters[i]);
            }

            VariablesText = Variables[0].ToString();

            for (int i = 1; i < Variables.Count; i++)
            {
                VariablesText = VariablesText + "," + Variables[i];
            }

            //Table
            GenerateTable();
        }

        /// <summary>
        /// Check if operator.
        /// </summary>
        /// <param name="s">String representing the logical predicate.</param>
        /// <returns></returns>
        private static bool IsOperator(string s)
        {
            switch (s)
            {
                case "=":
                case ">":
                case "~":
                case "&":
                case "|":
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Create infix notation based on the .
        /// </summary>
        /// <param name="root"></param>
        /// <param name="previous"></param>
        private void InOrder(Node root, Node previous)
        {
            if (root == null)
            {
                return;
            }

            if (previous != null && root.Right != null)
            {
                Infix = Infix + "(";

                InOrder(root.Left, root);

                if (IsOperator(root.Value))
                    Infix = Infix + LogicOperator(root.Value);
                else
                    Infix = Infix + root.Value;

                InOrder(root.Right, root);

                Infix = Infix + ")";
            }
            else
            {
                InOrder(root.Left, root);

                if (IsOperator(root.Value))
                    Infix = Infix + LogicOperator(root.Value);
                else
                    Infix = Infix + root.Value;

                InOrder(root.Right, root);
            }
        }

        /// <summary>
        /// Convert logical preicate from ASCII prefix notation to logic formula notation.
        /// </summary>
        /// <param name="predicate">String representing the logical predicate.</param>
        /// <returns></returns>
        private static string LogicOperator(string predicate)
        {
            switch (predicate)
            {
                case "=":
                    return "⇔";
                case ">":
                    return "⇒";
                case "~":
                    return "¬";
                case "&":
                    return "⋀";
                case "|":
                    return "⋁";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Generate the picture.
        /// </summary>
        public void GenerateGraph()
        {
            _lines.Clear();

            _lines.Add("graph logic {");

            _lines.Add("node [ fontname = \"Arial\" ]");
            foreach (var t in _nodes)
            {
                _lines.Add(t.GraphVariable + " [ label = " + "\"" + t.Value + "\" ]");
            }
            foreach (var t in _nodes)
            {
                if (t.Left != null && t.Left.Value != "")
                {
                    _lines.Add(t.GraphVariable + " -- " + t.Left.GraphVariable);
                }
                if (t.Right != null && t.Right.Value != "")
                {
                    _lines.Add(t.GraphVariable + " -- " + t.Right.GraphVariable);
                }
            }

            _lines.Add("}");
            var currentPath = AppDomain.CurrentDomain.BaseDirectory;
            var docPath = currentPath + "dot.dot";
            var graphPath = currentPath + "dot.png";

            using (var file =
                new System.IO.StreamWriter(docPath, false))
            {
                foreach (var line in _lines)
                {
                    file.WriteLine(line);
                }
            }

            using (var dot = new Process())
            {
                dot.StartInfo.Verb = "runas";
                dot.StartInfo.FileName = @"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe";
                dot.StartInfo.Arguments = " -Tpng -odot.png " + docPath;
                dot.Start();
                dot.WaitForExit();
            }

            using (var picture = new Process())
            {
                picture.StartInfo.FileName = graphPath;
                //picture.Start();
            }

        }

        public void GenerateTable()
        {
            var inputTable = GenerateTableInput(Variables.Count);


            bool[] answer1 = new bool[inputTable.GetLength(0)];
            bool[] answer2 = new bool[inputTable.GetLength(0)]; ;
            for (int i = 0; i < inputTable.GetLength(0); i++)
            {
                for (int j = 0; j < inputTable.GetLength(1); j++)
                {
                    SetValue(Variables[j], inputTable[i, j]);
                    SetValue(Variables[j], inputTable[i, j]);
                }
                answer1[i] = Solve();
                answer2[i] = Solve();
            }

            bool equal = false;
            equal = answer1.SequenceEqual(answer2);

            //TruthTableView.Columns.Add("#1");
            //if (!equal) TruthTableView.Columns.Add("#2");
            Values = new List<string>();
            for (int i = 0; i < inputTable.GetLength(0); i++)
            {
               
                Values.Add(getBoolStr(inputTable[i, 0]));
                for (int j = 1; j < inputTable.GetLength(1); j++)
                {
                    Values.Add(getBoolStr(inputTable[i, j]));
                }
                Values.Add(getBoolStr(answer1[i]));
                if (!equal) Values.Add(getBoolStr(answer2[i])); ;
            }
        }

        // Get stirng from bool value
        private string getBoolStr(bool b)
        {
            return b ? "1" : "0";
        }

        private bool Solve()
        {
            Stack<bool> stack = new Stack<bool>();

            foreach (var t in _nodes)
            {
                if (IsOperator(t.Value)==false)
                {
                    stack.Push(t.BoolValue);
                }
                else 
                {
                    //if (t.ArgCount > stack.Count)
                    //{
                    //    throw new Exception("The user has not input sufficient values in the expression!");
                    //}

                    // evaluate the operator:
                    switch (t.Value)
                    {
                        case "|"://or
                            stack.Push(stack.Pop() | stack.Pop());
                            break;
                        case "="://xor
                            stack.Push(!(stack.Pop() ^ stack.Pop()));
                            break;
                        case ">"://implication
                            stack.Push(!stack.Pop() | stack.Pop());
                            break;
                        case "&"://and
                            stack.Push(stack.Pop() & stack.Pop());
                            break;
                        case "~"://not
                            stack.Push(!stack.Pop());
                            break;
                        default:
                            throw new Exception("Error: Invalid operation!!");
                    }
                }
            }

           // if (stack.Count > 1) throw new Exception("Error: The user input has too many values.");

            return stack.Pop();
        }

        // set variable value, returns true if successfully changed
        public bool SetValue(char c, bool val)
        {
            bool success = false;
            char ch = Char.ToUpper(c);
            foreach (var t in _nodes)
            {
                if (t.Value == ch.ToString())
                {
                    t.BoolValue = val;
                    success = true;
                }
            }

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns">The amount of varables indicate the amount of colomns</param>
        /// <returns></returns>
        private static bool[,] GenerateTableInput(int columns)
        {
            var rows = (int)Math.Pow(2, columns);

            var table = new bool[rows, columns];

            var divider = rows;

            // Iterate by column
            for (var c = 0; c < columns; c++)
            {
                divider /= 2;
                var cell = false;
                // Iterate every row by this column's index:
                for (var r = 0; r < rows; r++)
                {
                    table[r, c] = cell;
                    if ((divider == 1) || ((r + 1) % divider == 0))
                    {
                        cell = !cell;
                    }
                }
            }

            return table;
        }
    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALEWeek1
{
    public class Node
    {
        //public string Sign { get; set; }

        public Node(string value, int level, int isOneElelemntNegation)
        : this(value, null, null, level, isOneElelemntNegation)
        {
        }

        public Node(string value, Node left, Node right, int level, int isOneElelemntNegation)
        {
            Value = value;
            Left = left;
            Right = right;
            Level = level;
            GraphVariable = "Node" + level;
            IsOneElelemntNegation = isOneElelemntNegation;
        }

        public string Value;
        public Node Left;
        public Node Right;
        public string GraphVariable;
        public int Level;
        public int IsOneElelemntNegation;
        public bool BoolValue;

    }
}

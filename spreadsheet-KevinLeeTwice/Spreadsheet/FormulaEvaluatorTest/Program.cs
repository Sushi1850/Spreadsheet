using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static FormulaEvaluator.Evaluator;

namespace FormulaEvaluatorTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate("A4-A4*A4/A4", SimpleLookup));
            Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate("5/4", NoLookup));
            Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate("(5+4)*9", NoLookup));
            Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate("(5+4*9", NoLookup));
            Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate("(5+4)*9+89", NoLookup));
            Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate("5+6+8+9*9", NoLookup));
            Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate("78+58+69*0", NoLookup));
            Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate("(5*8+9)*9+65", NoLookup));
            Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate("58   +  69   *   0+A2", SimpleLookup));
            Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate("1 + A3 + 2", SimpleLookup));
            Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate("58/3+(5*3+5)", SimpleLookup));
            Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate("      5 + 3", NoLookup));
            Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate("53*(25+4)", NoLookup));
            Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate(" A1 + A2 + 3+ 3*(1+5) ", SimpleLookup));
        }
        private static int ZeroLookup(string v) { return 0; }
        private static int NoLookup(string v) { throw new ArgumentException("NoLookup method was called"); }
        private static int SimpleLookup(string v)
        {
            switch (v)
            {
                case "A1":
                    return 12;
                case "B1":
                    return 3;
                case "C1":
                    return 19;
                case "A2":
                    return 35;
                case "B2":
                    return 45;
                case "C2":
                    return 63;
                case "A3":
                    return 1;
                case "B3":
                    return 25;
                case "C3":
                    return 10;
                case "A4":
                    return 7;
                case "B4":
                    return 73;
                case "C4":
                    return 32;
                default:
                    throw new ArgumentException("The variable " + v + " does not exist in the look up");

            }
        }
    }
}
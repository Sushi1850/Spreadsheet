/*
 * Kevin Lee
 * u1175570
 * CS 3500
 */
using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// A class to evaluate expression with legal operation computation
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Delegate to lookup variable values
        /// </summary>
        /// <param name="v">The name of the variable</param>
        /// <returns>The value of the named variable, if it exists, otherwise throws a ArgumentException</returns>
        public delegate int Lookup(String v);

        /// <summary>
        /// Evaluates an infix formula expression
        /// 
        /// Throws an Argument Exception if exp cannot be evaluated due to bad format, division by zero, or nonexistant variables, ...
        /// </summary>
        /// <param name="exp">An expression that is represented as a string</param>
        /// <param name="variableEvaluator">A method to use to lookup variable values</param>
        /// <returns>The integer result of the evaluation</returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            int checkInt;

            
            String operPop = "";
            Stack value = new Stack();
            Stack oper = new Stack();
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            for (int i = 0; i < substrings.Length; i++)
            {
                // Check to see if the expression contains a letter.
                bool StrContainLetter = Regex.IsMatch(substrings[i], "[a-zA-Z]");

                //Check to see if the strings is a variable
                if (substrings[i].Length >= 2 && StrContainLetter)
                {
                    int track1 = 0;
                    int track2 = 0;
                    char[] charArray = substrings[i].ToCharArray();
                    
                    //See if the char is a number or a letter 
                    for (int x = 0; x < charArray.Length; x++)
                    {
                        if (charArray[x] == ' ') continue;
                        else if (Char.IsLetter(charArray[x]))
                        {
                            track2++;
                            if (track1 >= 1)
                                throw new ArgumentException("The string " + substrings[i] + " is not a vaild variable");
                        }
                        else if (Char.IsDigit(charArray[x]))
                            track1++;
                        else throw new ArgumentException("The strings contains an invalid symbol");
                    }
                    if (track2 > 0 && track1 > 0)
                    {
                        if (oper.Count != 0) operPop = oper.Peek().ToString();

                        if (operPop == "*" || operPop == "/")
                        {
                            if (value.Count == 0) throw new ArgumentException("Could not compute with the operator " + operPop);
                            string temp = value.Pop().ToString();
                            int num1 = int.Parse(temp);
                            string oper1 = oper.Pop().ToString();

                            value.Push(OperationStep(num1, variableEvaluator(substrings[i].Trim()), oper1));
                        }
                        else value.Push(variableEvaluator(substrings[i].Trim()));
                    }
                }

                // If the substring is a whitespace and is not located at the end of the expression, skip it
                else if (substrings[i] == " " && i != (substrings.Length - 1)) continue;

                //If the substring is either a *, /, or a open parenthesis, push the operation stack
                else if (substrings[i] == "*" || substrings[i] == "/" || substrings[i] == "(") oper.Push(substrings[i]);

                //If the substring is a closing parenthesis
                else if (substrings[i] == ")")
                {
                    //If the operation stack is not empty, assign it to the variable operPop
                    if (oper.Count != 0) operPop = oper.Peek().ToString();
                    if (value.Count >= 2 && operPop == "+" || operPop == "-")
                    {
                        Computing(value, oper);
                        if (oper.Count == 0) throw new ArgumentException("The expression is missing an opening parenthesis");
                        else
                        {
                            operPop = oper.Peek().ToString();
                            if (operPop == "(") oper.Pop();
                            else throw new ArgumentException("The expression does not have an opening parenthesis");
                        }
                        if (oper.Count != 0) operPop = oper.Peek().ToString();
                        if (operPop == "*" || operPop == "/")
                        {
                            Computing(value, oper);
                        }
                    }
                }
                //Checking to see if the string is an int
                else if (int.TryParse(substrings[i], out checkInt))
                {
                    //If the operation stack is not empty, assign it to the variable opPop
                    if (oper.Count != 0) operPop = oper.Peek().ToString();

                    if (operPop == "*" || operPop == "/")
                    {
                        if (value.Count == 0) throw new ArgumentException("Could not compute with the operator " + operPop);
                        string temp = value.Pop().ToString();
                        int num1 = int.Parse(temp);
                        string oper1 = oper.Pop().ToString();

                        value.Push(OperationStep(num1, checkInt, oper1));
                    }
                    else value.Push(substrings[i]);
                }
                else if (substrings[i] == "+" || substrings[i] == "-")
                {
                    if (oper.Count != 0) operPop = oper.Peek().ToString();
                    if (operPop == "+" || operPop == "-")
                    {
                        if (value.Count < 2) throw new ArgumentException("Could not compute with the operator " + operPop);
                        Computing(value, oper);
                        oper.Push(substrings[i]);
                    }
                    else
                    {
                        oper.Push(substrings[i]);
                    }
                }

                // Only occurs when it is the last token
                if (i == substrings.Length - 1)
                {
                    //Occurs when the stack is empty
                    if (oper.Count == 0)
                    {
                        if (value.Count == 1)
                        {
                            //Takes out the last value from the stack and return it
                            String result = value.Pop().ToString();
                            int FinalResult = int.Parse(result);
                            return FinalResult;
                        }
                        else throw new ArgumentException("The results ended up with two answer and cannot give a proper final answer");
                    }
                    else
                    {
                        if (value.Count < 1) throw new ArgumentException("The given expession has an invalid format");
                        operPop = oper.Peek().ToString();
                        if (operPop == "+" || operPop == "-")
                        {
                            if (value.Count == 1) throw new ArgumentException("The given expession has an invalid format");
                            Computing(value, oper);
                            if (value.Count == 1)
                            {
                                String result = value.Pop().ToString();
                                int FinalResult = int.Parse(result);
                                return FinalResult;
                            }
                            else throw new ArgumentException("The results ended up with two answer and cannot give a proper final answer");
                        }
                        else if (operPop == "*" || operPop == "/")
                        {
                            if (value.Count == 1) throw new ArgumentException("The given expession has an invalid format");
                            Computing(value, oper);
                            if (value.Count == 1)
                            {
                                String result = value.Pop().ToString();
                                int FinalResult = int.Parse(result);
                                return FinalResult;
                            }
                            else throw new ArgumentException("The results ended up with two answer and cannot give a proper final answer");
                        }
                        else throw new ArgumentException("The given expession has an invalid format");
                    }
                }
            }
            throw new ArgumentException("The given expresssion has an invalid format");
        }
        /// <summary>
        /// Compute the two values with the given operation
        /// </summary>
        /// <param name="v1">First value of the operation</param>
        /// <param name="v2">Second value of the operation</param>
        /// <param name="op">The operation in which both value are going to use</param>
        /// <returns>Returns the operation total</returns>
        private static int OperationStep(int val1, int val2, string oper)
        {
            switch (oper)
            {
                case "*": return val1 * val2;
                case "/": if (val2 == 0) throw new ArgumentException("The operation division cannot be computed when dividing by 0"); else return val1 / val2;
                case "+": return val1 + val2;
                case "-": return val2 - val1;
                default: throw new ArgumentException("The operation " + oper + " isn't one of the operation");
            }
        }
        /// <summary>
        /// Removes two integers from the value stack and one from operation stack and compute them
        /// </summary>
        /// <param name="v">Value Stack</param>
        /// <param name="o">Operation Stack</param>
        private static void Computing(Stack v, Stack o)
        {
            string temp1 = v.Pop().ToString();
            int num1 = int.Parse(temp1);
            string temp2 = v.Pop().ToString();
            int num2 = int.Parse(temp2);
            string oper1 = o.Pop().ToString();
            v.Push(OperationStep(num1, num2, oper1));
        }
    }
}
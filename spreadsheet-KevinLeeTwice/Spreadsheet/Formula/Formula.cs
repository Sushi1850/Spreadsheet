// Kevin Lee
// u1175570
// CS 3500
//
//Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private readonly List<string> token;
        private readonly HashSet<string> ValidToken;
        private readonly string exp;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        /// 
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            token = GetTokens(formula).ToList();
            ValidToken = new HashSet<string>();
            bool HasVariable;
            int lp_counter = 0;
            int rp_counter = 0;

            if (token.Count < 1) throw new FormulaFormatException("There are no token infix expresssion");

            for (int j = 0; j < token.Count; j++)
            {
                if (token[j] == "(") lp_counter++;
                else if (token[j] == ")") rp_counter++;

                // Check to see if the expression contains a letter.
                bool StrContainLetter = Regex.IsMatch(token[j], "[a-zA-Z]");
                HasVariable = false;
                String TempToken;

                //Check to see if the strings is a valid variable
                if (isVariable(token[j]))
                {
                    TempToken = normalize(token[j]);
                    if (isVariable(TempToken))
                    {
                        if (isValid(TempToken))
                        {
                            //Sees if there is a duplicate in the ValidToken Queue
                            if (!ValidToken.Contains(TempToken))
                            {
                                ValidToken.Add(TempToken);
                            }
                            HasVariable = true;
                            token[j] = TempToken;
                        }
                    }
                }

                Double temp = 0;
                if (Double.TryParse(token[j], out temp))
                {
                   string StringHolder = temp.ToString();
                    temp = 1;
                    token[j] = StringHolder;
                }

                if (!token[j].Equals("+") && !token[j].Equals("-") && !token[j].Equals("*") && !token[j].Equals("/") && !token[j].Equals("(") && !token[j].Equals(")") && !HasVariable && (temp.Equals(0)))
                    throw new FormulaFormatException(token[j] + " is either an invalid variable or token");

                if (j == 0)
                {
                    if ((temp.Equals(0)) && !HasVariable && !token[j].Equals("(")) throw new FormulaFormatException("At the beginning of the expression, it does not contain a valid variable, number, or opening parentheses");
                }
                if (token[j] == "0") temp = 1;

                if (j >= token.Count - 1)
                {
                    if (lp_counter < rp_counter) throw new FormulaFormatException("The expression has more closing parentheses than opening parentheses");
                    else if (lp_counter != rp_counter) throw new FormulaFormatException("The expression does not have equal amounts of opening and closing parentheses");
                    else if ((temp.Equals(0)) && !HasVariable && !token[j].Equals(")")) throw new FormulaFormatException("At the end of the expression, it does not contain a variable, number, or closing parentheses");
                }

                if (token[j].Equals("(") || token[j].Equals("+") || token[j].Equals("-") || token[j].Equals("/") || token[j].Equals("*"))
                {
                    bool HasVar = false;
                    Double temp2 = 0;

                    if (Double.TryParse(token[j + 1], out temp2))
                        temp2 = 1;

                    //See if the char is a number or a letter 
                    HasVar = isVariable(token[j + 1]);

                    if ((temp2.Equals(0)) && !HasVar && !token[j + 1].Equals("(")) throw new FormulaFormatException("You have an operator or an open parentheses that does not have a variable, number, or another open parentheses after it");
                }

                if (!temp.Equals(0) || HasVariable || token[j].Equals(")"))
                {
                    if (j + 1 >= token.Count) continue;
                    else if (!token[j + 1].Equals("+") && !token[j + 1].Equals("-") && !token[j + 1].Equals("*") && !token[j + 1].Equals("/") && !token[j + 1].Equals(")"))
                        throw new FormulaFormatException("You either have a variable, number, or opening parentheses that does not have an operation sign or opening parentheses after it or an invalid variable");
                }
            }
            exp = string.Join("", token);
        }
        private static bool isVariable(string expression)
        {
            string VariablePattern = "^[a-zA-z_][a-zA-Z0-9_]*$";
            return Regex.IsMatch(expression, VariablePattern); ;
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            double checkDouble;
            int checkInt;
            object FinalResult = 0;

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
                    char[] charArray = substrings[i].ToCharArray();

                    //See if the char is a number or a letter 

                    if (isVariable(substrings[i]))
                    {
                        if (oper.Count != 0) operPop = oper.Peek().ToString();

                        if (operPop == "*" || operPop == "/")
                        {
                            string temp = value.Pop().ToString();
                            double num1 = double.Parse(temp);
                            string oper1 = oper.Pop().ToString();

                            try { lookup(substrings[i].Trim()); } catch (ArgumentException e) { return new FormulaError(e.Message); }

                            value.Push(OperationStep(num1, lookup(substrings[i].Trim()), oper1));
                        }
                        else
                        {
                            try { lookup(substrings[i].Trim()); } catch (ArgumentException e){ return new FormulaError(e.Message); }

                            value.Push(lookup(substrings[i].Trim()));
                        }
                    }
                }

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

                        operPop = oper.Peek().ToString();
                        if (operPop == "(") oper.Pop();
                        if (oper.Count != 0) operPop = oper.Peek().ToString();

                        if (operPop == "*" || operPop == "/")
                        {
                            Computing(value, oper);
                        }
                    }
                    else if (operPop == "(")
                    {
                        oper.Pop();
                        if (oper.Count != 0) operPop = oper.Peek().ToString();
                        if (operPop == "*" || operPop == "/")
                        {
                            Computing(value, oper);
                        }
                    }
                }
                else if (int.TryParse(substrings[i], out checkInt))
                {
                    //If the operation stack is not empty, assign it to the variable opPop
                    if (oper.Count != 0) operPop = oper.Peek().ToString();

                    if (operPop == "*" || operPop == "/")
                    {
                        string temp = value.Pop().ToString();
                        double num1 = double.Parse(temp);
                        string oper1 = oper.Pop().ToString();

                        if (OperationStep(num1, checkInt, oper1).GetType().Equals(typeof(FormulaError)))
                            return OperationStep(num1, checkInt, oper1);

                        value.Push(OperationStep(num1, checkInt, oper1));
                    }
                    else value.Push(substrings[i]);
                }
                //Checking to see if the string is a doubl
                else if (double.TryParse(substrings[i], out checkDouble))
                {
                    //If the operation stack is not empty, assign it to the variable opPop
                    if (oper.Count != 0) operPop = oper.Peek().ToString();

                    if (operPop == "/" || operPop == "*")
                    {
                        string temp = value.Pop().ToString();
                        double num1 = double.Parse(temp);
                        string oper1 = oper.Pop().ToString();

                        if (OperationStep(num1, checkDouble, oper1).GetType().Equals(typeof(FormulaError)))
                            return OperationStep(num1, checkDouble, oper1);

                        value.Push(OperationStep(num1, checkDouble, oper1));
                    }
                    else value.Push(substrings[i]);
                }
                else if (substrings[i] == "+" || substrings[i] == "-")
                {
                    if (oper.Count != 0) operPop = oper.Peek().ToString();
                    if (operPop == "+" || operPop == "-")
                    {
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
                            if (value.Peek().GetType().Equals(typeof(FormulaError)))
                                return value.Pop();

                            String result = value.Pop().ToString();
                            FinalResult = double.Parse(result);
                        }
                    }
                    else
                    {
                        operPop = oper.Peek().ToString();
                        if (operPop == "+" || operPop == "-")
                        {
                            Computing(value, oper);
                            if (value.Count == 1)
                            {
                                String result = value.Pop().ToString();
                                FinalResult = double.Parse(result);
                            }
                        }
                    }
                }
            }
            return FinalResult;
        }


        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            foreach (string vToken in ValidToken)
            {
                yield return vToken;
            }
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return exp;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            else if ((obj.GetType() != typeof(Formula))) return false;

            string formula1 = obj.ToString();
            string formula2 = this.ToString();

            string[] f1 = GetTokens(formula1).ToArray();
            string[] f2 = GetTokens(formula2).ToArray();

            if (f1.Length != f2.Length) return false;
            for (int i = 0; i < f2.Length; i++)
            {
                double f1Result = 0.0;
                double f2Result = 0.0;

                if (isVariable(f1[i]) && isVariable(f2[i]))
                {
                    if (f1[i] != f2[i]) return false;
                }
                else if (double.TryParse(f1[i], out f1Result) && double.TryParse(f2[i], out f2Result))
                {
                    if (f1Result.ToString() != f2Result.ToString()) return false;
                }
                else
                    if (f1[i] != f2[i]) return false;
            }
            return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (ReferenceEquals(f1, null) && ReferenceEquals(f2, null)) return true;
            else if (ReferenceEquals(f1, null)) return false;
            else if (ReferenceEquals(f2, null)) return false;
            else return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {

            return exp.GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
        private static void Computing(Stack v, Stack o)
        {
            string temp1 = v.Pop().ToString();
            double num1 = double.Parse(temp1);
            string temp2 = v.Pop().ToString();
            double num2 = double.Parse(temp2);
            string oper1 = o.Pop().ToString();
            v.Push(OperationStep(num2, num1, oper1));
        }
        private static object OperationStep(double val1, double val2, string oper)
        {
            switch (oper)
            {
                case "*": return val1 * val2;
                case "/": if (val2 == 0) return new FormulaError("The operation division cannot be computed when dividing by 0"); else return val1 / val2;
                case "-": return val1 - val2;
                default: return val1 + val2;

            }
        }

    }
        /// <summary>
        /// Used to report syntactic errors in the argument to the Formula constructor.
        /// </summary>
        public class FormulaFormatException : Exception
        {
            /// <summary>
            /// Constructs a FormulaFormatException containing the explanatory message.
            /// </summary>
            public FormulaFormatException(String message)
                : base(message)
            {
            }
        }

        /// <summary>
        /// Used as a possible return value of the Formula.Evaluate method.
        /// </summary>
        public struct FormulaError
        {
            /// <summary>
            /// Constructs a FormulaError containing the explanatory reason.
            /// </summary>
            /// <param name="reason"></param>
            public FormulaError(String reason)
                : this()
            {
                Reason = reason;
            }

            /// <summary>
            ///  The reason why this FormulaError was created.
            /// </summary>
            public string Reason { get; private set; }
        }
}
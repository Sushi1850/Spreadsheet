using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using SpreadsheetUtilities;

namespace GradingTests
{
    [TestClass]
    public class GradingTests
    {

        // Normalizer tests
        [TestMethod(), Timeout(2000)]
        [TestCategory("1")]
        public void TestNormalizerGetVars()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            HashSet<string> vars = new HashSet<string>(f.GetVariables());

            Assert.IsTrue(vars.SetEquals(new HashSet<string> { "X1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("2")]
        public void TestNormalizerEquals()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("2+X1", s => s.ToUpper(), s => true);

            Assert.IsTrue(f.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("3")]
        public void TestNormalizerToString()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            Formula f2 = new Formula(f.ToString());

            Assert.IsTrue(f.Equals(f2));
        }

        // Validator tests
        [TestMethod(), Timeout(2000)]
        [TestCategory("4")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorFalse()
        {
            Formula f = new Formula("2+x1", s => s, s => false);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("5")]
        public void TestValidatorX1()
        {
            Formula f = new Formula("2+x", s => s, s => (s == "x"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("6")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorX2()
        {
            Formula f = new Formula("2+y1", s => s, s => (s == "x"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("7")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorX3()
        {
            Formula f = new Formula("2+x1", s => s, s => (s == "x"));
        }


        // Simple tests that return FormulaErrors
        [TestMethod(), Timeout(2000)]
        [TestCategory("8")]
        public void TestUnknownVariable()
        {
            Formula f = new Formula("2+X1");
            Assert.IsInstanceOfType(f.Evaluate(s => { throw new ArgumentException("Unknown variable"); }), typeof(FormulaError));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("9")]
        public void TestDivideByZero()
        {
            Formula f = new Formula("5/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("10")]
        public void TestDivideByZeroVars()
        {
            Formula f = new Formula("(5 + X1) / (X1 - 3)");
            Assert.IsInstanceOfType(f.Evaluate(s => 3), typeof(FormulaError));
        }


        // Tests of syntax errors detected by the constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("11")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleOperator()
        {
            Formula f = new Formula("+");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("12")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOperator()
        {
            Formula f = new Formula("2+5+");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("13")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraCloseParen()
        {
            Formula f = new Formula("2+5*7)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("14")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOpenParen()
        {
            Formula f = new Formula("((3+5*7)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("15")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator()
        {
            Formula f = new Formula("5x");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("16")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator2()
        {
            Formula f = new Formula("5+5x");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("17")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator3()
        {
            Formula f = new Formula("5+7+(5)8");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("18")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator4()
        {
            Formula f = new Formula("5 5");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("19")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestDoubleOperator()
        {
            Formula f = new Formula("5 + + 3");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("20")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmpty()
        {
            Formula f = new Formula("");
        }

        // Some more complicated formula evaluations
        [TestMethod(), Timeout(2000)]
        [TestCategory("21")]
        public void TestComplex1()
        {
            Formula f = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
            Assert.AreEqual(5.14285714285714, (double)f.Evaluate(s => (s == "x7") ? 1 : 4), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("22")]
        public void TestRightParens()
        {
            Formula f = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6, (double)f.Evaluate(s => 1), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("23")]
        public void TestLeftParens()
        {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12, (double)f.Evaluate(s => 2), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("53")]
        public void TestRepeatedVar()
        {
            Formula f = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0, (double)f.Evaluate(s => 3), 1e-9);
        }

        // Test of the Equals method
        [TestMethod(), Timeout(2000)]
        [TestCategory("24")]
        public void TestEqualsBasic()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula("X1+X2");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("25")]
        public void TestEqualsWhitespace()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula(" X1  +  X2   ");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("26")]
        public void TestEqualsDouble()
        {
            Formula f1 = new Formula("2+X1*3.00");
            Formula f2 = new Formula("2.00+X1*3.0");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("27")]
        public void TestEqualsComplex()
        {
            Formula f1 = new Formula("1e-2 + X5 + 17.00 * 19 ");
            Formula f2 = new Formula("   0.0100  +     X5+ 17 * 19.00000 ");
            Assert.IsTrue(f1.Equals(f2));
        }


        [TestMethod(), Timeout(2000)]
        [TestCategory("28")]
        public void TestEqualsNullAndString()
        {
            Formula f = new Formula("2");
            Assert.IsFalse(f.Equals(null));
            Assert.IsFalse(f.Equals(""));
        }


        // Tests of == operator
        [TestMethod(), Timeout(2000)]
        [TestCategory("29")]
        public void TestEq()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("30")]
        public void TestEqFalse()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsFalse(f1 == f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("31")]
        public void TestEqNull()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(null == f1);
            Assert.IsFalse(f1 == null);
            Assert.IsTrue(f1 == f2);
        }


        // Tests of != operator
        [TestMethod(), Timeout(2000)]
        [TestCategory("32")]
        public void TestNotEq()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(f1 != f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("33")]
        public void TestNotEqTrue()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsTrue(f1 != f2);
        }


        // Test of ToString method
        [TestMethod(), Timeout(2000)]
        [TestCategory("34")]
        public void TestString()
        {
            Formula f = new Formula("2*5");
            Assert.IsTrue(f.Equals(new Formula(f.ToString())));
        }


        // Tests of GetHashCode method
        [TestMethod(), Timeout(2000)]
        [TestCategory("35")]
        public void TestHashCode()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("2*5");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }

        // Technically the hashcodes could not be equal and still be valid,
        // extremely unlikely though. Check their implementation if this fails.
        [TestMethod(), Timeout(2000)]
        [TestCategory("36")]
        public void TestHashCodeFalse()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("3/8*2+(7)");
            Assert.IsTrue(f1.GetHashCode() != f2.GetHashCode());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("37")]
        public void TestHashCodeComplex()
        {
            Formula f1 = new Formula("2 * 5 + 4.00 - _x");
            Formula f2 = new Formula("2*5+4-_x");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }


        // Tests of GetVariables method
        [TestMethod(), Timeout(2000)]
        [TestCategory("38")]
        public void TestVarsNone()
        {
            Formula f = new Formula("2*5");
            Assert.IsFalse(f.GetVariables().GetEnumerator().MoveNext());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("39")]
        public void TestVarsSimple()
        {
            Formula f = new Formula("2*X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("40")]
        public void TestVarsTwo()
        {
            Formula f = new Formula("2*X2+Y3");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "Y3", "X2" };
            Assert.AreEqual(actual.Count, 2);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("41")]
        public void TestVarsDuplicate()
        {
            Formula f = new Formula("2*X2+X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("42")]
        public void TestVarsComplex()
        {
            Formula f = new Formula("X1+Y2*X3*Y2+Z7+X1/Z8");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X1", "Y2", "X3", "Z7", "Z8" };
            Assert.AreEqual(actual.Count, 5);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        // Tests to make sure there can be more than one formula at a time
        [TestMethod(), Timeout(2000)]
        [TestCategory("43")]
        public void TestMultipleFormulae()
        {
            Formula f1 = new Formula("2 + a1");
            Formula f2 = new Formula("3");
            Assert.AreEqual(2.0, f1.Evaluate(x => 0));
            Assert.AreEqual(3.0, f2.Evaluate(x => 0));
            Assert.IsFalse(new Formula(f1.ToString()) == new Formula(f2.ToString()));
            IEnumerator<string> f1Vars = f1.GetVariables().GetEnumerator();
            IEnumerator<string> f2Vars = f2.GetVariables().GetEnumerator();
            Assert.IsFalse(f2Vars.MoveNext());
            Assert.IsTrue(f1Vars.MoveNext());
        }

        // Repeat this test to increase its weight
        [TestMethod(), Timeout(2000)]
        [TestCategory("44")]
        public void TestMultipleFormulaeB()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("45")]
        public void TestMultipleFormulaeC()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("46")]
        public void TestMultipleFormulaeD()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("47")]
        public void TestMultipleFormulaeE()
        {
            TestMultipleFormulae();
        }

        // Stress test for constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("48")]
        public void TestConstructor()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // This test is repeated to increase its weight
        [TestMethod(), Timeout(2000)]
        [TestCategory("49")]
        public void TestConstructorB()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("50")]
        public void TestConstructorC()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("51")]
        public void TestConstructorD()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("52")]
        public void TestConstructorE()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }


    }
}
/*using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        public void FormulaSyntaxTest1()
        {
            Formula formula = new Formula("a1/2.14", normalize, isValid);
            Assert.AreEqual(12.0/2.14, formula.Evaluate(SimpleLookup));
        }

        [TestMethod]
        public void FormulaSyntaxTest2()
        {
            Formula formula = new Formula("1*(5+8+9*6)", s => s, s => true);
            Assert.AreEqual(67, (double)formula.Evaluate(SimpleLookup), 1e-9);
        }

        [TestMethod]
        public void FormulaSyntaxTest3()
        {
            Formula formula = new Formula("5+6+(7*9)", s => s, s => true);
            Assert.AreEqual(74, (double)formula.Evaluate(s => 0), 1e-9);
        }

        [TestMethod]
        public void FormulaSyntaxTest4()
        {
            Formula formula = new Formula("5*8", s => s, s => true);
            Assert.AreEqual(40.0, formula.Evaluate(SimpleLookup));
        }

        [TestMethod]
        public void FormulaSyntaxTest5()
        {
            Formula formula = new Formula("12-(5+5)", s => s, s => true);
            Assert.AreEqual(2, (double)formula.Evaluate(SimpleLookup), 1e-9);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaSyntaxException1()
        {
            Formula formula = new Formula("+ 5", s => s, s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaSyntaxException2()
        {
            Formula formula = new Formula("", s => s, s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaSyntaxException3()
        {
            Formula formula = new Formula("(4+6+98)/9*(4-5)*a1+b2+(5+6))", normalize, isValid);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaSyntaxException4()
        {
            Formula formula = new Formula("((5+8-8)/6", s => s, s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaSyntaxException5()
        {
            Formula formula = new Formula("5+8/9 + a6#a", normalize, isValid);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaSyntaxException6()
        {
            Formula formula = new Formula("(1+3+8/4)4", s => s, s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaSyntaxException7()
        {
            Formula formula = new Formula("(1+3+8/4)+", s => s, s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaSyntaxException8()
        {
            Formula formula = new Formula("#56", normalize, isValid);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaSyntaxException9()
        {
            Formula formula = new Formula(" 6 +  + 5", normalize, isValid);
        }

        [TestMethod]
        public void FormulaToStringTest1()
        {
            Formula formula = new Formula("(5+7 ) * 9/(18-9)", s => s, s => true);
            Assert.AreEqual("(5+7)*9/(18-9)", formula.ToString());
        }

        [TestMethod]
        public void FormulaToStringTest2()
        {
            Formula formula = new Formula("X + Y", s => s, s => true);
            Assert.AreEqual("X+Y", formula.ToString());
        }

        [TestMethod]
        public void OperationTest()
        {
            Formula formula = new Formula("5*_A4", normalize, isValid);
            Assert.AreEqual(35.0, formula.Evaluate(SimpleLookup));
        }

        [TestMethod]
        public void DivideByZeroExceptionTest()
        {
            Formula formula = new Formula("5/0", s => s, s => true);
            Assert.IsInstanceOfType(formula.Evaluate(SimpleLookup), typeof(FormulaError));
            if (formula.Evaluate(SimpleLookup) is FormulaError f)
                Console.WriteLine(f.Reason);
        }

        [TestMethod]
        public void DivideByZeroExceptionTest2()
        {
            Formula formula = new Formula("5/0.0", s => s, s => true);
            Assert.IsInstanceOfType(formula.Evaluate(SimpleLookup), typeof(FormulaError));
            if (formula.Evaluate(SimpleLookup) is FormulaError f)
                Console.WriteLine(f.Reason);
        }

        [TestMethod]
        public void HashCodeTest()
        {
            Formula formula1 = new Formula("(5+7 ) * 9/ (5 + 6 )", s => s, s => true);
            Formula formula2 = new Formula("(5+7)*9/(5+6)", s => s, s => true);
            Assert.AreEqual(formula2.GetHashCode(), formula1.GetHashCode());
        }


        [TestMethod]
        public void InequalityTest()
        {
            Formula formula = new Formula("(5+7 ) * 9/ 1+(5 + 6 )", s => s, s => true);
            Formula formula2 = new Formula("(5+7) * 9/ 1.2+( 5 +6 )", s => s, s => true);
            Assert.IsTrue(formula != formula2);
        }

        [TestMethod]
        public void InequalityTestWithBothNull()
        {
            Formula formula = null;
            Formula formula2 = null;
            Assert.IsFalse(formula != formula2);
        }

        [TestMethod]
        public void InequalityTestWithOneNull()
        {
            Formula formula = new Formula("(5+7 ) * 9/ 1+(5 + 6 )", s => s, s => true);
            Formula formula2 = null;
            Assert.IsTrue(formula != formula2);
        }

        [TestMethod]
        public void InequalityTestThatAreEqual()
        {
            Formula formula = new Formula("(5+7 ) * 9/ 1+(5 + 6 )", s => s, s => true);
            Formula formula2 = new Formula("(5+7) * 9/ 1.00+( 5 +6 )", s => s, s => true);
            Assert.IsFalse(formula != formula2);
        }

        [TestMethod]
        public void EqualityTest()
        {
            Formula formula = new Formula("(5+7 ) * 9/ 1+(5 + 6 )", s => s, s => true);
            Formula formula2 = new Formula("(5+7) * 9/ 1.000000000000000000000+( 5 + 6 )", s => s, s => true);
            Assert.IsTrue(formula == formula2);
        }

        [TestMethod]
        public void EqualityTestNotEqual()
        {
            Formula formula = new Formula("(5+7 ) * 9/ 1+(5 + 6 )", s => s, s => true);
            Formula formula2 = new Formula("(5+7) * 9/ 1.2+( 5 + 6 )", s => s, s => true);
            Assert.IsFalse(formula == formula2);
        }

        [TestMethod]
        public void EqualityTestWithOneNull1()
        {
            Formula formula = new Formula("(5+7 ) * 9/ 1+(5 + 6 )", s => s, s => true);
            Formula formula2 = null;
            Assert.IsFalse(formula == formula2);
        }

        [TestMethod]
        public void EqualityTestWithOneNull2()
        {
            Formula formula = null;
            Formula formula2 = new Formula("(5+7 ) * 9/ 1+(5 + 6 )", s => s, s => true);
            Assert.IsFalse(formula == formula2);
        }

        [TestMethod]
        public void EqualityTestWithBothNull()
        {
            Formula formula = null;
            Formula formula2 = null;
            Assert.IsTrue(formula == formula2);
        }

        [TestMethod]
        public void FormulaGetVariableTest1()
        {
            Formula formula = new Formula("a5 + Y5 - d7", normalize, isValid);
            IEnumerator<string> e = formula.GetVariables().GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("A5", e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("Y5", e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("D7", e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        [TestMethod]
        public void FormulaGetVariableTest2()
        {
            Formula formula = new Formula("a5 + a5 - d7", normalize, isValid);
            IEnumerator<string> e = formula.GetVariables().GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("A5", e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("D7", e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        [TestMethod]
        public void FormulaEvaluateTest1()
        {
            Formula formula = new Formula("(9.56 - A1)+ 89+B3", s => s, s => true);
            Assert.AreEqual(111.56, formula.Evaluate(SimpleLookup));
        }

        [TestMethod]
        public void FormulaEvaluateTest2()
        {
            Formula formula = new Formula("A1-A1*(A1/A1)+0.1", s => s, s => true);
            Assert.AreEqual(0.1, formula.Evaluate(SimpleLookup));
        }

        [TestMethod]
        public void FormulaEvaluateTest3()
        {
            Formula formula = new Formula("9.56 - A1+ 89+B3", s => s, s => true);
            Assert.AreEqual(111.56, formula.Evaluate(SimpleLookup));
        }

        [TestMethod]
        public void FormulaEvaluateTest4()
        {
            Formula formula = new Formula("A1+A1/(A1+A1)", s => s, s => true);
            Assert.AreEqual(14.0, formula.Evaluate(SimpleLookup));
        }

        [TestMethod]
        public void FormulaEvaluateException1()
        {
            Formula formula = new Formula("5/B6", s => s, s => true);
            Assert.IsInstanceOfType(formula.Evaluate(SimpleLookup), typeof(FormulaError));
            if (formula.Evaluate(SimpleLookup) is FormulaError f)
                Console.WriteLine(f.Reason);
        }

        [TestMethod]
        public void FormulaEvaluateException2()
        {
            Formula formula = new Formula("5+A3A", s => s, s => true);
            Assert.IsInstanceOfType(formula.Evaluate(SimpleLookup), typeof(FormulaError));
            if (formula.Evaluate(SimpleLookup) is FormulaError f)
                Console.WriteLine(f.Reason);

        }

        [TestMethod]
        public void FormulaEqualsTest()
        {
            Formula formula = new Formula("1.2356896258 + a5+ 89+b6", normalize, isValid);
            Assert.IsTrue(new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")));
        }

        [TestMethod]
        public void FormulaEqualsWithNull1()
        {
            Formula formula = new Formula("1.2356896258 + a5+ 89+b6", normalize, isValid);
            Assert.IsFalse((formula).Equals(null));
        }

        [TestMethod]
        public void FormulaEqualsNotFormula()
        {
            Formula formula = new Formula("1.2356896258 + a5+ 89+b6", normalize, isValid);
            Assert.IsFalse(new Formula("2.0 + x7").Equals(new Object()));
        }

        [TestMethod]
        public void FormulaEqualsTestFail1()
        {
            Formula formula1 = new Formula("1.2356896258 + a5+ 89+b6", normalize, isValid);
            Formula formula2 = new Formula("1.2356896258 + a5+ 89", normalize, isValid);
            Assert.IsFalse((formula1).Equals(formula2));
        }

        [TestMethod]
        public void FormulaEqualsTestFail2()
        {
            Formula formula1 = new Formula("1.2356896258 + a5+ 89+b6", normalize, isValid);
            Formula formula2 = new Formula("1.2356896258 + a5+ 89+B6", s => s, s => true);
            Assert.IsFalse((formula1).Equals(formula2));
        }

        [TestMethod]
        public void FormulaEqualsTestFail3()
        {
            Formula formula1 = new Formula("1.2356896258 + a5+ 89+b6", normalize, isValid);
            Formula formula2 = new Formula("1.2356896258 - a5+ 89+b6", s => s, s => true);
            Assert.IsFalse((formula1).Equals(formula2));
        }

        private static object SimpleLookup(string v)
        {
            switch (v)
            {
                case "A1":
                    return 12.0;
                case "B1":
                    return 3.0;
                case "C1":
                    return 19.0;
                case "A2":
                    return 35.0;
                case "B2":
                    return 45.0;
                case "C2":
                    return 63.0;
                case "A3":
                    return 1.0;
                case "B3":
                    return 25.0;
                case "C3":
                    return 10.0;
                case "_A4":
                    return 7.0;
                case "B4":
                    return 73.0;
                case "C4":
                    return 32.0;
                case "D6":
                    return 0;
                default:
                    return new FormulaError("The variable " + v + " does not exist in the look up");

            }
        }
        private static string normalize(string formula)
        {
            return formula.ToUpper();
        }

        private static bool isValid(string formula)
        {
            string VariablePattern = "^[a-zA-z_][a-zA-Z0-9_]*$";
            return Regex.IsMatch(formula, VariablePattern); ;
        }
    }
}*/

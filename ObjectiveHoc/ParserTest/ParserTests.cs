using Parser.Interfaces;
using IntermediateCode;
using System.Diagnostics.Metrics;

namespace ParserTest
{
    [TestClass]
    public class ParserTests
    {
        private void TestParse(string input)
        {
            TextReader reader = new StringReader(input);

            Program program = new Program();

            IParser parser = new Parser.Parser(reader, program);

            parser.Parse();
        }

        private void TestParse(string input, List<string> expectedCode)
        {
            TextReader reader = new StringReader(input);

            Program program = new Program();

            IParser parser = new Parser.Parser(reader, program);

            while (reader.Peek() != -1)
            {
                parser.Parse();
            }

            Assert.AreEqual(expectedCode.Count, program.Code.Count);

            for (int i = 0; i < expectedCode.Count; i++)
            {
                Assert.AreEqual(expectedCode[i], program.Code[i]);
            }
        }

        [TestMethod]
        public void TestPrint_With_One_Expression()
        {
            string input = "print 3\n\r";

            List<string> expectedCode = new List<string> { "constpush", "3", "arguments", "1", "builtincall", "print" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestPrint_With_Multiple_Expression()
        {
            string input = "print 3, 5\n\r";

            List<string> expectedCode = new List<string> { "constpush", "3", "constpush", "5", "arguments", "2", "builtincall", "print" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestPrint_With_No_Expression()
        {
            string input = "print\n\r";

            try
            {
                TestParse(input);

                Assert.Fail("test failed: exception expected");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("syntax error: arguments expected", ex.Message);
            }
        }

        [TestMethod]
        public void Test_Arithmetic_Add_Subtract()
        {
            string input = "9-5+2\n\r";

            List<string> expectedCode = new List<string> { "constpush", "9", "constpush", "5", "sub", "constpush", "2", "add", "arguments", "1", "builtincall", "print" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_Arithmetic_Multiply_Divide()
        {
            string input = "3*6/2\n\r";

            List<string> expectedCode = new List<string> { "constpush", "3", "constpush", "6", "mul", "constpush", "2", "div", "arguments", "1", "builtincall", "print" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_Arithmetic_Exponential()
        {
            string input = "4^2\n\r";

            List<string> expectedCode = new List<string> { "constpush", "4", "constpush", "2", "power", "arguments", "1", "builtincall", "print" };


            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_Arithmetic_UnaryMinus()
        {
            string input = "-7\n\r";

            List<string> expectedCode = new List<string> { "constpush", "7", "negate", "arguments", "1", "builtincall", "print" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestArithmetic_Brackets()
        {
            string input = "(9+2)*3\n\r";

            List<string> expectedCode = new List<string> { "constpush", "9", "constpush", "2", "add", "constpush", "3", "mul" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestBuiltinVariable_PI()
        {
            string input = "PI";

            List<string> expectedCode = new List<string> { "eval", "PI" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestBuiltinVariable_E()
        {
            string input = "E";

            List<string> expectedCode = new List<string> { "eval", "E" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestBuiltinVariable_GAMMA()
        {
            string input = "GAMMA";

            List<string> expectedCode = new List<string> { "eval", "GAMMA" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestBuiltinVariable_DEG()
        {
            string input = "DEG";

            List<string> expectedCode = new List<string> { "eval", "DEG" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestBuiltinVariable_PHI()
        {
            string input = "PHI";

            List<string> expectedCode = new List<string> { "eval", "PHI" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestBuiltinFunction_sin()
        {
            string input = "sin(1)";

            List<string> expectedCode = new List<string> { "constpush", "1", "arguments", "1", "builtincall", "sin" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestBuiltinFunction_cos()
        {
            string input = "cos(1)";

            List<string> expectedCode = new List<string> { "constpush", "1", "arguments", "1", "builtincall", "cos" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestBuiltinFunction_atan()
        {
            string input = "atan(1)";

            List<string> expectedCode = new List<string> { "constpush", "1", "arguments", "1", "builtincall", "atan" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestBuiltinFunction_exp()
        {
            string input = "exp(1)";

            List<string> expectedCode = new List<string> { "constpush", "1", "arguments", "1", "builtincall", "exp" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestBuiltinFunction_log()
        {
            string input = "log(1)";

            List<string> expectedCode = new List<string> { "constpush", "1", "arguments", "1", "builtincall", "log" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestBuiltinFunction_log10()
        {
            string input = "log10(1)";

            List<string> expectedCode = new List<string> { "constpush", "1", "arguments", "1", "builtincall", "log10" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestBuiltinFunction_sqrt()
        {
            string input = "sqrt(1)";

            List<string> expectedCode = new List<string> { "constpush", "1", "arguments", "1", "builtincall", "sqrt" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestBuiltinFunction_int()
        {
            string input = "int(1)";

            List<string> expectedCode = new List<string> { "constpush", "1", "arguments", "1", "builtincall", "int" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestBuiltinFunction_abs()
        {
            string input = "abs(1)";

            List<string> expectedCode = new List<string> { "constpush", "1", "arguments", "1", "builtincall", "abs" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestParse_InvalidNumberException()
        {
            string input = "1e_test\n\r";

            try
            {
                TestParse(input);

                Assert.Fail("test failed: exception expected");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("syntax error: invalid input for decimal number", ex.Message);
            }
        }

        [TestMethod]
        public void TestParse_RightBracketMissingException()
        {
            string input = "(9+2-3\n\r";

            try
            {
                TestParse(input);

                Assert.Fail("test failed: exception expected");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("syntax error: missing right bracket", ex.Message);
            }
        }

        [TestMethod]
        public void TestAssign_LeftVariable()
        {
            string input = "x=3\n\r";

            List<string> expectedCode = new List<string> { "constpush", "3", "assign", "x" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void TestAssign_RightVariable()
        {
            string input = "x=3\n\ry=x\n\r";

            List<string> expectedCode = new List<string> {"constpush", "3", "assign", "x", "eval", "x", "assign", "y"};

            TestParse(input, expectedCode);
        }

        [TestMethod]

        public void TestAssign_UndefinedVariableException()
        {
            string input = "x=y";

            try
            {
                TestParse(input);

                Assert.Fail("test failed: exception expected");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("syntax error: undefined variable", ex.Message);
            }
        }

        [TestMethod]

        public void TestVariableDefinition()
        {
            string input = "y=3\n\rx=2*y\n\r";

            List<string> expectedCode = new List<string> { 
                "constpush", "3", "assign", "y", 
                "constpush", "2", "eval", "y","mul", "assign", "x"
            };

            TestParse(input, expectedCode);
        }

        //test comparison
        [TestMethod]
        public void Test_if_less_equal()
        {
            string input = "if (3 <= 5) 120";

            List<string> expectedCode = new List<string>
            { "constpush", "3", "constpush", "5", "LE", "gofalse", "#Label1",
                "constpush", "120", "arguments", "1", "builtincall", "print", "goto",
                "#Label2", "label", "#Label1", "label", "#Label2" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_if_greater_equal()
        {
            string input = "if (3 >= 5) 120";

            List<string> expectedCode = new List<string>
            { "constpush", "3", "constpush", "5", "GE", "gofalse", "#Label1",
                "constpush", "120", "arguments", "1", "builtincall", "print", "goto",
                "#Label2", "label", "#Label1", "label", "#Label2" };

            TestParse(input, expectedCode);
        }


        [TestMethod]
        public void Test_if_equal()
        {
            string input = "if (3 == 5) 120";

            List<string> expectedCode = new List<string>
            { "constpush", "3", "constpush", "5", "EQ", "gofalse", "#Label1",
                "constpush", "120", "arguments", "1", "builtincall", "print", "goto",
                "#Label2", "label", "#Label1", "label", "#Label2" };

            TestParse(input, expectedCode);
        }

        //test conditional
        [TestMethod]
        public void Test_AND()
        {
            string input = "if (3 < 5 && 4 < 6) 120";

            List<string> expectedCode = new List<string>
            { "constpush", "3", "constpush", "5", "LT", 
                "constpush", "4", "constpush", "6", "LT", 
                "and", "gofalse", "#Label1", "constpush", 
                "120", "arguments", "1", "builtincall", "print", 
                "goto", "#Label2", "label", "#Label1", 
                "label", "#Label2"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_OR()
        {
            string input = "if (3 < 5 || 4 < 6) 120";

            List<string> expectedCode = new List<string>
            { "constpush", "3", "constpush", "5", "LT",
                "constpush", "4", "constpush", "6", "LT",
                "or", "gofalse", "#Label1", "constpush",
                "120", "arguments", "1", "builtincall", "print",
                "goto", "#Label2", "label", "#Label1",
                "label", "#Label2"};

            TestParse(input, expectedCode);
        }

        //test control flow
        [TestMethod]

        public void Test_if_true_no_else_no_curly_brackets()
        {
            string input = "if (3 < 5) 120";

            List<string> expectedCode = new List<string>
            { "constpush", "3", "constpush", "5", "LT", "gofalse", "#Label1",
                "constpush", "120", "arguments", "1", "builtincall", "print", "goto",
                "#Label2", "label", "#Label1", "label", "#Label2" };

            TestParse(input, expectedCode);
        }


        [TestMethod]

        public void Test_if_true_else_no_curly_brackets()
        {
            string input = "if (3 < 5) 120 else 150";

            List<string> expectedCode = new List<string>
            {"constpush", "3", "constpush", "5", "LT",
                "gofalse", "#Label1", "constpush", "120", 
                "arguments", "1", "builtincall", "print",
                "goto", "#Label2", "label", "#Label1", 
                "constpush", "150", "arguments", "1",
                "builtincall", "print", "label", "#Label2"};

            TestParse(input, expectedCode);
        }

        [TestMethod]

        public void Test_if_true_else()
        {
            string input = "if (3 < 5) {120} else {150}";

            List<string> expectedCode = new List<string>
            {"constpush", "3", "constpush", "5", "LT",
                "gofalse", "#Label1", "constpush", "120",
                "arguments", "1", "builtincall", "print",
                "goto", "#Label2", "label", "#Label1",
                "constpush", "150", "arguments", "1",
                "builtincall", "print", "label", "#Label2"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_if_false_else()
        {
            string input = "if (3 > 5) {120} else {150}";

            List<string> expectedCode = new List<string>
            {"constpush", "3", "constpush", "5", "GT",
                "gofalse", "#Label1", "constpush", "120",
                "arguments", "1", "builtincall", "print",
                "goto", "#Label2", "label", "#Label1",
                "constpush", "150", "arguments", "1",
                "builtincall", "print", "label", "#Label2"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_if_nested_if()
        {
            string input = "if (3 < 5) {if (4 < 6) {120}}";

            List<string> expectedCode = new List<string>
            {"constpush", "3", "constpush", "5", "LT",
                "gofalse", "#Label1", "constpush", "4",
                "constpush", "6", "LT", "gofalse", "#Label2",
                "constpush", "120", "arguments", "1", "builtincall",
                "print", "goto", "#Label3","label", "#Label2",
                "label", "#Label3", "goto", "#Label4",
                "label", "#Label1", "label", "#Label4"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_if_nested_else()
        {
            string input = "if (3 < 5) {120} else {if (4 < 6) {150}}";

            List<string> expectedCode = new List<string>
            {"constpush", "3", "constpush", "5", "LT", "gofalse",
                "#Label1", "constpush", "120", "arguments", "1",
                "builtincall", "print", "goto", "#Label2", "label", "#Label1",
                "constpush", "4", "constpush", "6", "LT", "gofalse",
                "#Label3", "constpush", "150", "arguments", "1", "builtincall",
                "print", "goto", "#Label4", "label", "#Label3", "label",
                "#Label4", "label", "#Label2"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_if_else_expression()
        {
            string input = "x = 2\n\rif (x < 4) {x = x + 1} else {150}\n\rprint x\n\r";

            List<string> expectedCode = new List<string>
            { "constpush", "2", "assign", "x", "eval",
                "x", "constpush", "4", "LT", "gofalse",
                "#Label1", "eval", "x", "constpush", "1",
                "add", "assign", "x", "goto", "#Label2",
                "label", "#Label1", "constpush", "150",
                "arguments", "1", "builtincall", "print", "label",
                "#Label2", "eval", "x", "arguments", "1",
                "builtincall", "print" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_while()
        {
            string input = "x = 2\n\rwhile (x < 4) {x = x + 1\n\rprint x}";

            List<string> expectedCode = new List<string>
            { "constpush", "2", "assign", "x", "goto", "#Label1", 
                "label", "#Label1", "eval", "x", "constpush", "4", 
                "LT", "gofalse", "#Label2", "eval", "x", "constpush", 
                "1", "add", "assign", "x", "eval", "x", "arguments", 
                "1", "builtincall", "print", "goto", "#Label1", "label", 
                "#Label2"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_while_nested_if()
        {
            string input = "x = 2\n\rwhile (x < 4) { if(4 < 6) {x = x + 1\n\rprint x} else {150} }";

            List<string> expectedCode = new List<string>
            { "constpush", "2", "assign", "x", "goto", "#Label1", 
                "label", "#Label1", "eval", "x", "constpush", "4", 
                "LT", "gofalse", "#Label2", "constpush", "4", "constpush", 
                "6", "LT", "gofalse", "#Label3", "eval", "x", "constpush", 
                "1", "add", "assign", "x", "eval", "x", "arguments", "1", 
                "builtincall", "print", "goto", "#Label4", "label", "#Label3", 
                "constpush", "150", "arguments", "1", "builtincall", "print", 
                "label", "#Label4", "goto", "#Label1", "label", "#Label2" };

            TestParse(input, expectedCode);
        }

        //test procedure and recursive

        [TestMethod]
        public void Test_procdef()
        {
            string input = "proc test() {print 12}";

            List<string> expectedCode = new List<string>
            {"define", "test", "constpush", "12", "arguments", 
                "1", "builtincall", "print", "return", "mainfuncdef"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_procdef_call()
        {
            string input = "proc test() {print 12}\n\rtest()";

            List<string> expectedCode = new List<string>
            {"define", "test", "constpush", "12", "arguments",
                "1", "builtincall", "print", "return", "mainfuncdef", 
                "arguments", "0", "call", "test"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_multi_procdef_call()
        {
            string input = "proc test() {print 12}\n\rproc testTwo() {print 25}\n\rtestTwo()\n\rtest()";

            List<string> expectedCode = new List<string>
            { "define", "test", "constpush", "12", "arguments",
                "1", "builtincall", "print", "return", "mainfuncdef",
                "define", "testTwo", "constpush", "25", "arguments",
                "1", "builtincall", "print", "return", "mainfuncdef",
                "arguments", "0", "call", "testTwo", "arguments", "0",
                "call", "test"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_one_param()
        {
            string input = "proc test() {print $1}\n\rtest(120)";

            List<string> expectedCode = new List<string>
            {"define", "test", "arg", "$1", "arguments", "1", 
                "builtincall", "print", "return", "mainfuncdef", 
                "constpush", "120", "arguments", "1", "call", "test"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_two_param()
        {
            string input = "proc test() {print $1, $2}\n\rtest(120,250)";

            List<string> expectedCode = new List<string>
            {"define", "test", "arg", "$1", "arg", "$2", 
                "arguments", "2", "builtincall", "print", 
                "return", "mainfuncdef", "constpush", "120", 
                "constpush", "250", "arguments", "2", "call", "test"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_param_if_else_expr()
        {
            string input = "proc test() {a=2\n\rb=3\n\rif (a < 3) {c = a + b + $1} else {print 120, $2}\n\rprint c}\n\rtest(25)";

            List<string> expectedCode = new List<string>
            {"define", "test", "constpush", "2", "assign", 
                "a", "constpush", "3", "assign", "b", "eval", 
                "a", "constpush", "3", "LT", "gofalse", "#Label1", 
                "eval", "a", "eval", "b", "add", "arg", "$1", "add", 
                "assign", "c", "goto", "#Label2", "label", "#Label1", 
                "constpush", "120", "arg", "$2", "arguments", "2", 
                "builtincall", "print", "label", "#Label2", "eval", "c", 
                "arguments", "1", "builtincall", "print", "return", 
                "mainfuncdef", "constpush", "25", "arguments", "1", 
                "call", "test"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_procedure_recursive()
        {
            string input = "proc PrintNumbers()\n\r{\n\rif ($1 == 0) return\n\rPrintNumbers($1 - 1)\n\rprint $1\n\r}\n\rPrintNumbers(3)";

            List<string> expectedCode = new List<string>
            { "define", "PrintNumbers", "arg", "$1", "constpush", 
                "0", "EQ", "gofalse", "#Label1", "return", "goto", 
                "#Label2", "label", "#Label1", "label", "#Label2", 
                "arg", "$1", "constpush", "1", "sub", "arguments", 
                "1", "call", "PrintNumbers", "arg", "$1", "arguments", 
                "1", "builtincall", "print", "return", "mainfuncdef", 
                "constpush", "3", "arguments", "1", "call", "PrintNumbers"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_procedure_fib()
        {
            string input = "proc fib()\r\n{\r\na = 0\r\nb = 1\r\nwhile (b < $1)\r\n{\r\nprint b\r\nc=b\r\nb=a+b\r\na=c\r\nprint \" \"\r\n}\r\n}\r\nfib(1000)";

            List<string> expectedCode = new List<string>
            {"define", "fib", "constpush", "0", "assign", 
                "a", "constpush", "1", "assign", "b", "goto", 
                "#Label1", "label", "#Label1", "eval", "b", "arg", 
                "$1", "LT", "gofalse", "#Label2", "eval", "b", 
                "arguments", "1", "builtincall", "print", "eval", 
                "b", "assign", "c", "eval", "a", "eval", "b", "add", 
                "assign", "b", "eval", "c", "assign", "a", "stringpush", 
                " ", "arguments", "1", "builtincall", "print", "goto", 
                "#Label1", "label", "#Label2", "return", "mainfuncdef", 
                "constpush", "1000", "arguments", "1", "call", "fib"};

            TestParse(input, expectedCode);
        }

        // test function and recrusive
        [TestMethod]
        public void Test_if_return()
        {
            string input = "func test()\n\r{\n\rif ($1 <= 0) return 1\n\r}\n\rprint test(0)";

            List<string> expectedCode = new List<string>
            { "define", "test", "arg", "$1", "constpush", 
                "0", "LE", "gofalse", "#Label1", "constpush", 
                "1", "return", "goto", "#Label2", "label", 
                "#Label1", "label", "#Label2", "mainfuncdef", 
                "constpush", "0", "arguments", "1", "call",
                "test", "arguments", "1", "builtincall", "print"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_function_recursive_factorial()
        {
            string input = "func fac()\n\r{\n\rif ($1 <= 0) return 1 else return $1 * fac($1-1)\n\r}\n\rprint fac(3)";

            List<string> expectedCode = new List<string>
            {"define", "fac", "arg", "$1", "constpush", "0",
                "LE", "gofalse", "#Label1", "constpush", "1",
                "return", "goto", "#Label2", "label", "#Label1",
                "arg", "$1", "arg", "$1", "constpush", "1", "sub",
                "arguments", "1", "call", "fac", "mul",
                "return", "label", "#Label2", "mainfuncdef",
                "constpush", "3", "arguments", "1", "call",
                "fac", "arguments", "1", "builtincall", "print" };

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_func_return_1()
        {
            string input = "func fac()\n\r{\n\rreturn 1\n\r}\n\rprint fac()";

            List<string> expectedCode = new List<string>
            {"define", "fac", "constpush", "1", "return",
                "mainfuncdef", "arguments", "0", "call",
                "fac", "arguments", "1", "builtincall", "print"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_Ack_Function()
        {
            string input = "func ack()\n\r{\n\rif ($1 == 0) return $2 + 1\n\rif ($2 == 0) return ack($1-1, 1)\n\rreturn ack($1-1, ack($1, $2-1))\n\r}\n\rprint ack(3, 3)";

            List<string> expectedCode = new List<string>
            {"define", "ack", "arg", "$1", "constpush", 
                "0", "EQ", "gofalse", "#Label1", "arg", 
                "$2", "constpush", "1", "add", "return", 
                "goto", "#Label2", "label", "#Label1", 
                "label", "#Label2", "arg", "$2", "constpush", 
                "0", "EQ", "gofalse", "#Label3", "arg", "$1", 
                "constpush", "1", "sub", "constpush", "1", 
                "arguments", "2", "call", "ack", "return", "goto", 
                "#Label4", "label", "#Label3", "label", "#Label4", 
                "arg", "$1", "constpush", "1", "sub", "arg", "$1", 
                "arg", "$2", "constpush", "1", "sub", "arguments", 
                "2", "call", "ack", "arguments", "2", "call", "ack", 
                "return", "mainfuncdef", "constpush", "3", "constpush", "3", 
                "arguments", "2", "call", "ack", "arguments", "1", "builtincall", 
                "print"};

            TestParse(input, expectedCode);
        }

        [TestMethod]
        public void Test_Stirling_Function()
        {
            string input = "func stir1()\n\r{\n\rreturn sqrt(2*$1*PI) * ($1/E)^$1*(1+1/(12*$1))\n\r}\n\rprint stir1(10)";

            List<string> expectedCode = new List<string>
            {"define", "stir1", "constpush", "2", "arg", 
                "$1", "mul", "eval", "PI", "mul", "arguments",
                "1", "builtincall", "sqrt", "arg", "$1", "eval", 
                "E", "div", "arg", "$1", "power", "mul", "constpush", 
                "1", "constpush", "1", "constpush", "12", "arg", "$1", 
                "mul", "div", "add", "mul", "return", "mainfuncdef", 
                "constpush", "10", "arguments", "1", "call", "stir1", 
                "arguments", "1", "builtincall", "print"};

            TestParse(input, expectedCode);
        }
    }
}
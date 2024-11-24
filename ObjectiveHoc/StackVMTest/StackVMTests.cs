using StackVM.Interfaces;
using IntermediateCode;
using Newtonsoft.Json.Linq;
using System;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using StackVM;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StackVMTest
{
    [TestClass]
    public class StackVMTests
    {
        private void TestStackVM(List<string> intermediateCode, string expectedResults)
        {
            StringWriter output = new StringWriter();

            Program program = new Program();

            foreach (string code in intermediateCode)
            {
                program.Code.Add(code);
            }

            IStackVM stackVM = new StackVM.StackVM(output);

            stackVM.Load(program);

            stackVM.Execute();

            Assert.AreEqual(expectedResults, output.ToString());
        }

        private void TestStackVM(List<string> intermediateCode)
        {
            StackVMLoadExecute(intermediateCode);
        }


        private StackVM.StackVM StackVMLoadExecute(List<string> intermediateCode)
        {
            StringWriter output = new StringWriter();

            Program program = new Program();

            foreach (string code in intermediateCode)
            {
                program.Code.Add(code);
            }

            StackVM.StackVM stackVM = new StackVM.StackVM(output);

            stackVM.Load(program);

            stackVM.Execute();

            return stackVM;
        }

        [TestMethod]
        public void TestInstruction_print()
        {
            List<string> intermediateCode = new List<string> { "constpush", "3", "arguments", "1", "builtincall", "print" };

            string expectedResults = "3";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void TestInstruction_print_Multiple_Arguments()
        {
            List<string> intermediateCode = new List<string> { "constpush", "3", "constpush", "5", "arguments", "2", "builtincall", "print" };

            string expectedResults = "35";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void TestInstruction_sin()
        {
            List<string> intermediateCode = new List<string> { "constpush", "1", "arguments", "1", "builtincall", "sin" };

            string expectedResults = "";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void TestInstruction_print_sin()
        {
            List<string> intermediateCode = new List<string> { "constpush", "1", "arguments", "1", "builtincall", "sin", "arguments", "1", "builtincall", "print" };

            string expectedResults = "0.841470984807896";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void TestInstruction_print_cos()
        {
            List<string> intermediateCode = new List<string> { "constpush", "1", "arguments", "1", "builtincall", "cos", "arguments", "1", "builtincall", "print" };

            string expectedResults = "0.54030230586814";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void TestInstruction_print_atan()
        {
            List<string> intermediateCode = new List<string> { "constpush", "1", "arguments", "1", "builtincall", "atan", "arguments", "1", "builtincall", "print" };

            string expectedResults = "0.785398163397448";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void TestInstruction_print_exp()
        {
            List<string> intermediateCode = new List<string> { "constpush", "1", "arguments", "1", "builtincall", "exp", "arguments", "1", "builtincall", "print" };

            string expectedResults = "2.71828182845904";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void TestInstruction_print_log()
        {
            List<string> intermediateCode = new List<string> { "constpush", "10", "arguments", "1", "builtincall", "log", "arguments", "1", "builtincall", "print" };

            string expectedResults = "2.30258509299405";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void TestInstruction_print_log10()
        {
            List<string> intermediateCode = new List<string> { "constpush", "10", "arguments", "1", "builtincall", "log10", "arguments", "1", "builtincall", "print" };

            string expectedResults = "1";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void TestInstruction_print_sqrt()
        {
            List<string> intermediateCode = new List<string> { "constpush", "4", "arguments", "1", "builtincall", "sqrt", "arguments", "1", "builtincall", "print" };

            string expectedResults = "2";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void TestInstruction_print_int()
        {
            List<string> intermediateCode = new List<string> { "constpush", "12.123", "arguments", "1", "builtincall", "int", "arguments", "1", "builtincall", "print" };

            string expectedResults = "12";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void TestInstruction_print_abs()
        {
            List<string> intermediateCode = new List<string> { "constpush", "10", "negate", "arguments", "1", "builtincall", "abs", "arguments", "1", "builtincall", "print" };

            string expectedResults = "10";

            TestStackVM(intermediateCode, expectedResults);
        }

        //test assign
        [TestMethod]
        public void Test_assign()
        {
            List<string> intermediateCode = new List<string> { "constpush", "12", "assign", "x", "eval", "x", "arguments", "1", "builtincall", "print" };

            string expectedResults = "12";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_assign2()
        {
            List<string> intermediateCode = new List<string> { "constpush", "12", "assign", "x" };

            StackVM.StackVM stackVM = StackVMLoadExecute(intermediateCode);

            var expectedData = 12;

            FieldInfo fieldInfosymbolStaticDataMap = typeof(StackVM.StackVM).GetField("symbolStaticDataMap", BindingFlags.NonPublic | BindingFlags.Instance);
           
            Dictionary<string, int> symbolStaticDataMap = (Dictionary<string, int>)fieldInfosymbolStaticDataMap.GetValue(stackVM);

            int location = symbolStaticDataMap["x"];

            FieldInfo fieldInfostaticData = typeof(StackVM.StackVM).GetField("staticData", BindingFlags.NonPublic | BindingFlags.Instance);
            
            List<dynamic> staticData = (List<dynamic>)fieldInfostaticData.GetValue(stackVM);

            var data = staticData[location];

            Assert.AreEqual(data, expectedData);
        }

        [TestMethod]
        public void Test_Eval()
        {
            List<string> intermediateCode = new List<string> { "constpush", "12", "assign", "x", "eval", "x", "assign", "y" };

            var expectedData = 12;

            StackVM.StackVM stackVM = StackVMLoadExecute(intermediateCode);

            FieldInfo fieldInfosymbolStaticDataMap = typeof(StackVM.StackVM).GetField("symbolStaticDataMap", BindingFlags.NonPublic | BindingFlags.Instance);

            Dictionary<string, int> symbolStaticDataMap = (Dictionary<string, int>)fieldInfosymbolStaticDataMap.GetValue(stackVM);

            int location = symbolStaticDataMap["y"];

            FieldInfo fieldInfostaticData = typeof(StackVM.StackVM).GetField("staticData", BindingFlags.NonPublic | BindingFlags.Instance);

            List<dynamic> staticData = (List<dynamic>)fieldInfostaticData.GetValue(stackVM);

            var data = staticData[location];

            Assert.AreEqual(data, expectedData);
        }

        //test arithmetic

        [TestMethod]
        public void Test_Arithmetic_Add_Subtract()
        {
            //9-5+2

            List<string> intermediateCode = new List<string>
            { "constpush", "9", "constpush", "5", "sub", "constpush", "2", "add", "arguments", "1", "builtincall", "print" };

            string expectedResults = "6";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_Arithmetic_UnaryMinus()
        {
            //-7

            List<string> intermediateCode = new List<string>
            { "constpush", "7", "negate", "arguments", "1", "builtincall", "print"  };

            string expectedResults = "-7";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_Arithmetic_Multiply_Divide()
        {
            //3*6/2

            List<string> intermediateCode = new List<string>
            { "constpush", "3", "constpush", "6", "mul", "constpush", "2", "div", "arguments", "1", "builtincall", "print" };

            string expectedResults = "9";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_Arithmetic_Exponential()
        {
            //3^2

            List<string> intermediateCode = new List<string>
            { "constpush", "4", "constpush", "2", "power", "arguments", "1", "builtincall", "print" };

            string expectedResults = "16";

            TestStackVM(intermediateCode, expectedResults);
        }

        // test control flow
        [TestMethod]
        public void Test_if_no_else()
        {
            List<string> intermediateCode = new List<string> 
            { "constpush", "3", "constpush", "5", "LT", "gofalse", "#Label1", 
                "constpush", "120", "arguments", "1", "builtincall", "print", "goto", 
                "#Label2", "label", "#Label1", "label", "#Label2" };

            string expectedResults = "120";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_if_else()
        {
            List<string> intermediateCode = new List<string>
            {"constpush", "3", "constpush", "5", "GT", 
                "gofalse", "#Label1", "constpush", "120", 
                "arguments", "1", "builtincall", "print", 
                "goto", "#Label2", "label", "#Label1", 
                "constpush", "150", "arguments", "1", 
                "builtincall", "print", "label", "#Label2"};

            string expectedResults = "150";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_if_nested_if()
        {
            List<string> intermediateCode = new List<string>
            {"constpush", "3", "constpush", "5", "LT",
                "gofalse", "#Label1", "constpush", "4", 
                "constpush", "6", "LT", "gofalse", "#Label2", 
                "constpush", "120", "arguments", "1", "builtincall", 
                "print", "goto", "#Label3","label", "#Label2", 
                "label", "#Label3", "goto", "#Label4", 
                "label", "#Label1", "label", "#Label4"};

            string expectedResults = "120";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_if_nested_else()
        {
            List<string> intermediateCode = new List<string>
                {"constpush", "3", "constpush", "5", "LT", "gofalse",
                "#Label1", "constpush", "120", "arguments", "1",
                "builtincall", "print", "goto", "#Label2", "label", "#Label1",
                "constpush", "4", "constpush", "6", "LT", "gofalse",
                "#Label3", "constpush", "150", "arguments", "1", "builtincall",
                "print", "goto", "#Label4", "label", "#Label3", "label",
                "#Label4", "label", "#Label2"};

            string expectedResults = "120";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_if_else_expression() // x = 2 if(x < 4) {x = x + 1} else {150} print x
        {
            List<string> intermediateCode = new List<string>
            { "constpush", "2", "assign", "x", "eval", 
                "x", "constpush", "4", "LT", "gofalse", 
                "#Label1", "eval", "x", "constpush", "1",
                "add", "assign", "x", "goto", "#Label2", 
                "label", "#Label1", "constpush", "150", 
                "arguments", "1", "builtincall", "print", "label", 
                "#Label2", "eval", "x", "arguments", "1", 
                "builtincall", "print"};

            string expectedResults = "3";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_while()
        {
            //x = 2\n\rwhile (x < 4) {x = x + 1\n\rprint x};

            List<string> intermediateCode = new List<string>
            { "constpush", "2", "assign", "x", "goto", "#Label1",
                "label", "#Label1", "eval", "x", "constpush", "4",
                "LT", "gofalse", "#Label2", "eval", "x", "constpush",
                "1", "add", "assign", "x", "eval", "x", "arguments",
                "1", "builtincall", "print", "goto", "#Label1", "label",
                "#Label2"};

            string expectedResults = "34";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_while_nested_if()
        {
            //x = 2\n\rwhile (x < 4) { if(4 < 6) {x = x + 1\n\rprint x} else {150} };

            List<string> intermediateCode = new List<string>
            { "constpush", "2", "assign", "x", "goto", "#Label1",
                "label", "#Label1", "eval", "x", "constpush", "4",
                "LT", "gofalse", "#Label2", "constpush", "4", "constpush",
                "6", "LT", "gofalse", "#Label3", "eval", "x", "constpush",
                "1", "add", "assign", "x", "eval", "x", "arguments", "1",
                "builtincall", "print", "goto", "#Label4", "label", "#Label3",
                "constpush", "150", "arguments", "1", "builtincall", "print",
                "label", "#Label4", "goto", "#Label1", "label", "#Label2"};

            string expectedResults = "34";

            TestStackVM(intermediateCode, expectedResults);
        }

        //test conditional
        [TestMethod]
        public void Test_AND()
        {
            List<string> intermediateCode = new List<string>
            { "constpush", "3", "constpush", "5", "LT",
                "constpush", "4", "constpush", "6", "LT",
                "and", "gofalse", "#Label1", "constpush",
                "120", "arguments", "1", "builtincall", "print",
                "goto", "#Label2", "label", "#Label1",
                "label", "#Label2"};

            string expectedResults = "120";

            TestStackVM(intermediateCode, expectedResults);
        }
        
        [TestMethod]
        public void Test_OR()
        {
            List<string> intermediateCode = new List<string>
            { "constpush", "3", "constpush", "5", "LT",
                "constpush", "4", "constpush", "6", "LT",
                "or", "gofalse", "#Label1", "constpush",
                "120", "arguments", "1", "builtincall", "print",
                "goto", "#Label2", "label", "#Label1",
                "label", "#Label2"};

            string expectedResults = "120";

            TestStackVM(intermediateCode, expectedResults);
        }

        //comparison
        [TestMethod]
        public void Test_if_less_equal()
        {
            //if (3 <= 5) 120;

            List<string> intermediateCode = new List<string>
            { "constpush", "3", "constpush", "5", "LE", "gofalse", "#Label1",
                "constpush", "120", "arguments", "1", "builtincall", "print", "goto",
                "#Label2", "label", "#Label1", "label", "#Label2" };

            string expectedResults = "120";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_if_greater_equal()
        {
            //if (5 >= 5) 120;

            List<string> intermediateCode = new List<string>
            { "constpush", "5", "constpush", "5", "GE", "gofalse", "#Label1",
                "constpush", "120", "arguments", "1", "builtincall", "print", "goto",
                "#Label2", "label", "#Label1", "label", "#Label2" };

            string expectedResults = "120";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void TestInstruction_KeyNotFoundException()
        {
            List<string> intermediateCode = new List<string> { "constpush", "3", "constpush", "5", "arguments", "2", "abcd", "print" };

            try
            {
                TestStackVM(intermediateCode);

                Assert.Fail("test failed: exception expected");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("runtime error: instruction not found", ex.Message);
            }
        }

        //procedure
        [TestMethod]
        public void Test_procdef_call()
        {
            //proc test() {print 12}\n\rtest()

            List<string> intermediateCode = new List<string>
            {"define", "test", "constpush", "12", "arguments", 
                "1", "builtincall", "print", "return", "mainfuncdef", 
                "arguments", "0", "call", "test"};

            string expectedResults = "12";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_procdef_multi_proc_call()
        {
            //proc test() {print 12}\n\rproc testTwo() {print 25}\n\rtestTwo()\n\rtest()

            List<string> intermediateCode = new List<string>
            {"define", "test", "constpush", "12", "arguments", 
                "1", "builtincall", "print", "return", "mainfuncdef", 
                "define", "testTwo", "constpush", "25", "arguments", 
                "1", "builtincall", "print", "return", "mainfuncdef", 
                "arguments", "0", "call", "testTwo", "arguments", "0", 
                "call", "test"};


            string expectedResults = "2512";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_one_param()
        {
            //proc test() {print $1}\n\rtest(120)

            List<string> intermediateCode = new List<string>
            {"define", "test", "arg", "$1", "arguments", "1",
                "builtincall", "print", "return", "mainfuncdef",
                "constpush", "120", "arguments", "1", "call", "test"};


            string expectedResults = "120";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_two_param()
        {
            //proc test() {print $1, $2}\n\rtest(120,250)

            List<string> intermediateCode = new List<string>
            {"define", "test", "arg", "$1", "arg", "$2",
                "arguments", "2", "builtincall", "print",
                "return", "mainfuncdef", "constpush", "120",
                "constpush", "250", "arguments", "2", "call", "test"};

            string expectedResults = "120250";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_param_if_else_expr()
        {
            //proc test() {a=2\n\rb=3\n\rif (a < 3) {c = a + b + $1} else {print 120, $2}\n\rprint c}\n\rtest(25)

            List<string> intermediateCode = new List<string>
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

            string expectedResults = "30";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_if_return()
        {
            //func fac()\n\r{\n\rif ($1 <= 0) return 1\n\r}\n\rprint fac(0);

            List<string> intermediateCode = new List<string>
            {"define", "fac", "arg", "$1", "constpush",
                "0", "LE", "gofalse", "#Label1", "constpush",
                "1", "return", "goto", "#Label2", "label",
                "#Label1", "label", "#Label2", "mainfuncdef",
                "constpush", "0", "arguments", "1", "call",
                "fac", "arguments", "1", "builtincall", "print" };

            string expectedResults = "1";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_proc_fib()
        {
            //proc fib()\r\n{\r\na = 0\r\nb = 1\r\nwhile (b < $1)\r\n{\r\nprint b\r\nc=b\r\nb=a+b\r\na=c\r\nprint \" \"\r\n}\r\n}\r\nfib(1000);

            List<string> intermediateCode = new List<string>
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

            string expectedResults = "1 1 2 3 5 8 13 21 34 55 89 144 233 377 610 987 ";

            TestStackVM(intermediateCode, expectedResults);
        }

        //function and recursive test
        [TestMethod]
        public void Test_procedure_recursive()
        {
            //proc PrintNumbers()\n\r{\n\rif ($1 == 0) return\n\rPrintNumbers($1 - 1)\n\rprint $1\n\r}\n\rPrintNumbers(3)

            List<string> intermediateCode = new List<string>
            { "define", "PrintNumbers", "arg", "$1", "constpush",
                "0", "EQ", "gofalse", "#Label1", "return", "goto",
                "#Label2", "label", "#Label1", "label", "#Label2",
                "arg", "$1", "constpush", "1", "sub", "arguments",
                "1", "call", "PrintNumbers", "arg", "$1", "arguments",
                "1", "builtincall", "print", "return", "mainfuncdef",
                "constpush", "3", "arguments", "1", "call", "PrintNumbers"};

            string expectedResults = "123";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_function_recursive_factorial()
        {
            //func fac()\n\r{\n\rif ($1 <= 0) return 1 else return $1 * fac($1-1)\n\r}\n\rprint fac(3);

            List<string> intermediateCode = new List<string>
            {"define", "fac", "arg", "$1", "constpush", "0", 
                "LE", "gofalse", "#Label1", "constpush", "1", 
                "return", "goto", "#Label2", "label", "#Label1", 
                "arg", "$1", "arg", "$1", "constpush", "1", "sub", 
                "arguments", "1", "call", "fac", "mul", 
                "return", "label", "#Label2", "mainfuncdef", 
                "constpush", "3", "arguments", "1", "call", 
                "fac", "arguments", "1", "builtincall", "print" };

            string expectedResults = "6";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_func_return_1()
        {
            //func fac()\n\r{\n\rreturn 1\n\r}\n\rprint fac();

            List<string> intermediateCode = new List<string>
            {"define", "fac", "constpush", "1", "return", 
                "mainfuncdef", "arguments", "0", "call", 
                "fac", "arguments", "1", "builtincall", "print"};

            string expectedResults = "1";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_Ack()
        {
            //func ack()\n\r{\n\rif ($1 == 0) return $2 + 1\n\rif ($2 == 0) return ack($1-1, 1)\n\rreturn ack($1-1, ack($1, $2-1))\n\r}\n\rprint ack(3, 3);

            List<string> intermediateCode = new List<string>
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

            string expectedResults = "61";

            TestStackVM(intermediateCode, expectedResults);
        }

        [TestMethod]
        public void Test_Stirling_Function()
        {
            //func stir1()\n\r{\n\rreturn sqrt(2*$1*PI) * ($1/E)^$1*(1+1/(12*$1))\n\r}\n\rprint stir1(10);

            List<string> intermediateCode = new List<string>
            {"define", "stir1", "constpush", "2", "arg",
                "$1", "mul", "eval", "PI", "mul", "arguments",
                "1", "builtincall", "sqrt", "arg", "$1", "eval",
                "E", "div", "arg", "$1", "power", "mul", "constpush",
                "1", "constpush", "1", "constpush", "12", "arg", "$1",
                "mul", "div", "add", "mul", "return", "mainfuncdef",
                "constpush", "10", "arguments", "1", "call", "stir1",
                "arguments", "1", "builtincall", "print"};

            string expectedResults = "3628684.7488972141668061886139";

            TestStackVM(intermediateCode, expectedResults);
        }
    }
}
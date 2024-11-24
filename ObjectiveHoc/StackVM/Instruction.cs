using IntermediateCode;
using StackVM.Interfaces;
using System.Reflection.Emit;
using System.Text;
using static StackVM.StackVM;

namespace StackVM
{
    internal abstract class Instruction
    {
        protected StackVM stackVM;

        internal abstract Instruction DeepCopy();

        internal abstract void Load(ref int index, Program program);

        internal abstract void Execute();

        internal void SetStackVM(StackVM stackVM)
        {
            this.stackVM = stackVM;
        }
    }

    internal abstract class StackVMBuiltinCallExecute
    {
        internal abstract void BuiltinCallInstructionExecute(StackVM stackVM);

        internal void BuiltinMathFunction(Func<double, double> operation, StackVM stackVM)
        {
            int nargs = (int)stackVM.stack.Pop();

            if (nargs == 1)
            {
                double a = (double)stackVM.stack.Pop();

                decimal result = (decimal)operation(a);

                stackVM.stack.Push(result);
            }
            else
            {
                throw new Exception("runtime error: mismatched arguments");
            }
        }
    }

    internal class ConstantPushInstruction : Instruction
    {
        private decimal argument;

        internal override Instruction DeepCopy()
        {
            ConstantPushInstruction copy = new ConstantPushInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            argument = Convert.ToDecimal(program.Code[++index]);
        }

        internal override void Execute()
        {
            stackVM.stack.Push(argument);
        }
    }

    internal class EvalInstruction : Instruction
    {
        private int argument;

        internal override Instruction DeepCopy()
        {
            EvalInstruction copy = new EvalInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            string symbol = program.Code[++index];

            if (stackVM.symbolStaticDataMap.ContainsKey(symbol))
            {
                argument = stackVM.symbolStaticDataMap[symbol];
            }
            else
            {
                throw new Exception($"runtime error: {symbol} not found in instruction");
            }
        }

        internal override void Execute()
        {
            var value = stackVM.staticData[argument];

            stackVM.stack.Push(value);
        }
    }

    internal class AssignInstruction : Instruction
    {
        private int argument;

        internal override Instruction DeepCopy()
        {
            AssignInstruction copy = new AssignInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            string symbol = program.Code[++index];

            if (stackVM.symbolStaticDataMap.ContainsKey(symbol))
            {
                argument = stackVM.symbolStaticDataMap[symbol];
            }
            else
            {
                stackVM.staticData.Add(0);

                argument = stackVM.staticData.Count - 1;

                stackVM.symbolStaticDataMap.Add(symbol, argument);
            }
        }

        internal override void Execute()
        {
            var value = stackVM.stack.Pop();

            stackVM.staticData[argument] = value;
        }
    }

    internal class ReadInstruction : Instruction
    {
        private int argument;

        internal override Instruction DeepCopy()
        {
            ReadInstruction copy = new ReadInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            string symbol = program.Code[++index];

            if (stackVM.symbolStaticDataMap.ContainsKey(symbol))
            {
                argument = stackVM.symbolStaticDataMap[symbol];
            }
            else
            {
                stackVM.staticData.Add(0);

                argument = stackVM.staticData.Count - 1;

                stackVM.symbolStaticDataMap.Add(symbol, argument);
            }
        }

        internal override void Execute()
        {
            //work around for stripping whitespace before entering a value for read
            int c = Console.In.Read();

            while (true)
            {
                c = Console.In.Peek();

                if (c == '\n' || c == '\r')
                {
                    c = Console.In.Read();
                }
                else
                {
                    break;
                }
            }

            string value = Console.In.ReadLine();

            decimal val;

            if (value[0] == '"' && value[value.Length - 1] == '"')
            {
                stackVM.staticData[argument] = value;
            }
            else if (decimal.TryParse(value, out val))
            {
                stackVM.staticData[argument] = val;
            }
            else
            {
                throw new Exception("runtime error: unexpected type");
            }
        }
    }

    internal class StringPushInstruction : Instruction
    {
        private string argument;

        internal override Instruction DeepCopy()
        {
            StringPushInstruction copy = new StringPushInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            argument = program.Code[++index];
        }

        internal override void Execute()
        {
            stackVM.stack.Push(argument);
        }
    }

    internal class AddInstruction : Instruction
    {
        internal override Instruction DeepCopy()
        {
            AddInstruction copy = new AddInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            //do nothing
        }

        internal override void Execute()
        {
            decimal b = stackVM.stack.Pop();
            decimal a = stackVM.stack.Pop();
            stackVM.stack.Push(a + b);
        }
    }

    internal class SubInstruction : Instruction
    {
        internal override Instruction DeepCopy()
        {
            SubInstruction copy = new SubInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            //do nothing
        }

        internal override void Execute()
        {
            decimal b = stackVM.stack.Pop();
            decimal a = stackVM.stack.Pop();
            stackVM.stack.Push(a - b);
        }
    }

    internal class MulInstruction : Instruction
    {
        internal override Instruction DeepCopy()
        {
            MulInstruction copy = new MulInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            //do nothing
        }

        internal override void Execute()
        {
            decimal b = stackVM.stack.Pop();
            decimal a = stackVM.stack.Pop();
            stackVM.stack.Push(a * b);
        }
    }

    internal class DivInstruction : Instruction
    {
        internal override Instruction DeepCopy()
        {
            DivInstruction copy = new DivInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            //do nothing
        }

        internal override void Execute()
        {
            decimal b = stackVM.stack.Pop();
            decimal a = stackVM.stack.Pop();
            stackVM.stack.Push(a / b);
        }
    }

    internal class ExponentialInstruction : Instruction
    {
        internal override Instruction DeepCopy()
        {
            ExponentialInstruction copy = new ExponentialInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            //do nothing
        }

        internal override void Execute()
        {
            double b = (double)stackVM.stack.Pop();
            double a = (double)stackVM.stack.Pop();
            decimal result = (decimal)Math.Pow(a, b);
            stackVM.stack.Push(result);
        }
    }

    internal class NegateInstruction : Instruction
    {
        internal override Instruction DeepCopy()
        {
            NegateInstruction copy = new NegateInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            //do nothing
        }

        internal override void Execute()
        {
            decimal a = stackVM.stack.Pop();

            stackVM.stack.Push(a * (-1));
        }
    }

    internal class LessThanInstruction : Instruction
    {
        internal override Instruction DeepCopy()
        {
            LessThanInstruction copy = new LessThanInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            //do nothing
        }

        internal override void Execute()
        {
            if (stackVM.stack.Count < 2)
            {
                throw new Exception("runtime error: argument mismatch for <");
            }
            decimal b = stackVM.stack.Pop();
            decimal a = stackVM.stack.Pop();
            stackVM.stack.Push(a < b);
        }
    }

    internal class LessEqualInstruction : Instruction
    {
        internal override Instruction DeepCopy()
        {
            LessEqualInstruction copy = new LessEqualInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            //do nothing
        }

        internal override void Execute()
        {
            if (stackVM.stack.Count < 2)
            {
                throw new Exception("runtime error: argument mismatch for <=");
            }
            decimal b = stackVM.stack.Pop();
            decimal a = stackVM.stack.Pop();
            stackVM.stack.Push(a <= b);
        }
    }

    internal class GreaterThanInstruction : Instruction
    {
        internal override Instruction DeepCopy()
        {
            GreaterThanInstruction copy = new GreaterThanInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            //do nothing
        }

        internal override void Execute()
        {
            if (stackVM.stack.Count < 2)
            {
                throw new Exception("runtime error: argument mismatch for >");
            }
            decimal b = stackVM.stack.Pop();
            decimal a = stackVM.stack.Pop();
            stackVM.stack.Push(a > b);
        }
    }

    internal class GreaterEqualInstruction : Instruction
    {
        internal override Instruction DeepCopy()
        {
            GreaterEqualInstruction copy = new GreaterEqualInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            //do nothing
        }

        internal override void Execute()
        {
            if (stackVM.stack.Count < 2)
            {
                throw new Exception("runtime error: argument mismatch for >=");
            }
            decimal b = stackVM.stack.Pop();
            decimal a = stackVM.stack.Pop();
            stackVM.stack.Push(a >= b);
        }
    }

    internal class EqualInstruction : Instruction
    {
        internal override Instruction DeepCopy()
        {
            EqualInstruction copy = new EqualInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            //do nothing
        }

        internal override void Execute()
        {
            if (stackVM.stack.Count < 2)
            {
                throw new Exception("runtime error: argument mismatch for <");
            }
            decimal b = stackVM.stack.Pop();
            decimal a = stackVM.stack.Pop();
            stackVM.stack.Push(a == b);
        }
    }

    internal class AndInstruction : Instruction
    {
        internal override Instruction DeepCopy()
        {
            AndInstruction copy = new AndInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            //do nothing
        }

        internal override void Execute()
        {
            bool b = stackVM.stack.Pop();
            bool a = stackVM.stack.Pop();

            if (a == b)
            {
                stackVM.stack.Push(true);
            }
            else
            {
                stackVM.stack.Push(false);
            }
        }
    }

    internal class OrInstruction : Instruction
    {
        internal override Instruction DeepCopy()
        {
            OrInstruction copy = new OrInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            //do nothing
        }

        internal override void Execute()
        {
            bool b = stackVM.stack.Pop();
            bool a = stackVM.stack.Pop();

            if (a == true || b == true)
            {
                stackVM.stack.Push(true);
            }
            else
            {
                stackVM.stack.Push(false);
            }
        }
    }

    internal class ParameterArgumentInstruction : Instruction
    {
        private int argument;
        internal override Instruction DeepCopy()
        {
            ParameterArgumentInstruction copy = new ParameterArgumentInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            string label = program.Code[++index];

            argument = Convert.ToInt32(label.Replace("$", ""));
        }

        internal override void Execute()
        {
            StackVM.Frame frame = stackVM.frameStack.Pop();

            List<dynamic> args = frame.args;

            var param = args[argument - 1];

            stackVM.stack.Push(param);

            stackVM.frameStack.Push(frame);
        }
    }

    internal class GoToInstruction : Instruction
    {
        private int argument;

        internal override Instruction DeepCopy()
        {
            GoToInstruction copy = new GoToInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            string label = program.Code[++index];

            if (stackVM.labelMap.ContainsKey(label))
            {
                argument = stackVM.labelMap[label];
            }
            else
            {
                stackVM.staticData.Add(0);

                argument = stackVM.staticData.Count - 1;

                stackVM.labelMap.Add(label, argument);
            }
        }

        internal override void Execute()
        {
            int destinationPc = stackVM.staticData[argument];

            stackVM.pc = destinationPc;
        }
    }

    internal class GoFalseInstruction : Instruction
    {
        private int argument;

        internal override Instruction DeepCopy()
        {
            GoFalseInstruction copy = new GoFalseInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            string label = program.Code[++index];

            if (stackVM.labelMap.ContainsKey(label))
            {
                argument = stackVM.labelMap[label];
            }
            else
            {
                stackVM.staticData.Add(0);

                argument = stackVM.staticData.Count - 1;

                stackVM.labelMap.Add(label, argument);
            }
        }

        internal override void Execute()
        {
            bool goFalse = stackVM.stack.Pop();

            int destinationPc = stackVM.staticData[argument];

            if (!goFalse)
            {
               stackVM.pc = destinationPc;
            }
        }
    }

    internal class LabelInstruction : Instruction
    {
        private int argument;

        internal override Instruction DeepCopy()
        {
            LabelInstruction copy = new LabelInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            string label = program.Code[++index];

            if (stackVM.labelMap.ContainsKey(label))
            {
                int executableCodeCount = stackVM.executableCode.Count;

                argument = stackVM.labelMap[label];

                stackVM.staticData[argument] = executableCodeCount;
            }
            else
            {
                throw new Exception("runtime error: label missing");
            }
        }

        internal override void Execute()
        {
            //do nothing
        }
    }

    internal class ArgumentsInstruction : Instruction
    {
        private int arguments;

        internal override Instruction DeepCopy()
        {
            ArgumentsInstruction copy = new ArgumentsInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            arguments = Convert.ToInt32(program.Code[++index]);
        }

        internal override void Execute()
        {
            stackVM.stack.Push(arguments);
        }
    }

    internal class DefineInstruction : Instruction
    {
        private int argument;

        internal override Instruction DeepCopy()
        {
            DefineInstruction copy = new DefineInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            string funcName = program.Code[++index];

            int entpc = stackVM.executableCode.Count;

            StackVM.Frame frame = new StackVM.Frame();

            frame.entrypc = entpc;

            stackVM.staticData.Add(frame);

            int frameLocation = stackVM.staticData.Count - 1;

            stackVM.labelMap.Add(funcName, frameLocation);
        }

        internal override void Execute()
        {
            //do nothing
        }
    }

    internal class CallInstruction : Instruction
    {
        private int argument;

        private int retpc;

        internal override Instruction DeepCopy()
        {
            CallInstruction copy = new CallInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            string label = program.Code[++index];

            int location = stackVM.labelMap[label];

            argument = location;

            retpc = stackVM.executableCode.Count;
        }

        internal override void Execute()
        {
            StackVM.Frame frame = stackVM.staticData[argument];

            int nargs = stackVM.stack.Pop();

            List<dynamic> args = new List<dynamic>();

            for (int i = 0; i < nargs; i++)
            {
                var arg = stackVM.stack.Pop();

                args.Add(arg);
            }

            args.Reverse();

            frame.args = args;

            stackVM.pc = frame.entrypc;

            frame.returnpc = retpc;

            stackVM.frameStack.Push(frame);
        }
    }

    internal class ReturnInstruction : Instruction
    {
        private int argument;

        internal override Instruction DeepCopy()
        {
            ReturnInstruction copy = new ReturnInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            //do nothing
        }

        internal override void Execute()
        {
            StackVM.Frame frame = stackVM.frameStack.Pop();

            stackVM.pc = frame.returnpc;
        }
    }

    internal class MainFunctionDefineInstruction : Instruction
    {
        private int argument;

        internal override Instruction DeepCopy()
        {
            MainFunctionDefineInstruction copy = new MainFunctionDefineInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            int mainpc = stackVM.executableCode.Count;

            stackVM.staticData[0] = mainpc;
        }

        internal override void Execute()
        {
            //do nothing
        }
    }

    internal class BuiltinCallInstruction : Instruction
    {
        private string argument = string.Empty;

        private Dictionary<string, StackVMBuiltinCallExecute> callMap;

        internal BuiltinCallInstruction()
        {
            callMap = new Dictionary<string, StackVMBuiltinCallExecute>
            {
                {"sin", new SinInstructionExecute()},
                {"cos", new CosInstructionExecute()},
                {"atan", new AtanInstructionExecute()},
                {"exp", new ExpInstructionExecute()},
                {"log", new LogInstructionExecute()},
                {"log10", new Log10InstructionExecute()},
                {"sqrt", new SqrtInstructionExecute()},
                {"int", new IntInstructionExecute()},
                {"abs", new AbsInstructionExecute()},
                {"print", new PrintInstructionExecute()}
            };
        }

        internal override Instruction DeepCopy()
        {
            BuiltinCallInstruction copy = new BuiltinCallInstruction();
            copy.SetStackVM(this.stackVM);
            return copy;
        }

        internal override void Load(ref int index, Program program)
        {
            argument = program.Code[++index];
        }

        internal override void Execute()
        {
            if (callMap.ContainsKey(argument))
            {
                StackVMBuiltinCallExecute callExecute = callMap[argument];

                callExecute.BuiltinCallInstructionExecute(stackVM);
            }
        }
    }

    internal class PrintInstructionExecute : StackVMBuiltinCallExecute
    {
        internal override void BuiltinCallInstructionExecute(StackVM stackVM)
        {
            int nargs = (int)stackVM.stack.Pop();

            List<dynamic> args = new List<dynamic>();

            for (int i = 0; i < nargs; i++)
            {
                args.Add(stackVM.stack.Pop());
            }

            args.Reverse();

            StringBuilder sb = new StringBuilder();

            foreach (var arg in args)
            {
                sb.Append(arg.ToString());
            }

            stackVM.output.Write(sb.ToString());
        }
    }

    internal class SinInstructionExecute : StackVMBuiltinCallExecute
    {
        internal override void BuiltinCallInstructionExecute(StackVM stackVM)
        {
            BuiltinMathFunction(Math.Sin, stackVM);
        }
    }

    internal class CosInstructionExecute : StackVMBuiltinCallExecute
    {
        internal override void BuiltinCallInstructionExecute(StackVM stackVM)
        {
            BuiltinMathFunction(Math.Cos, stackVM);
        }
    }

    internal class AtanInstructionExecute : StackVMBuiltinCallExecute
    {
        internal override void BuiltinCallInstructionExecute(StackVM stackVM)
        {
            BuiltinMathFunction(Math.Atan, stackVM);
        }
    }

    internal class ExpInstructionExecute : StackVMBuiltinCallExecute
    {
        internal override void BuiltinCallInstructionExecute(StackVM stackVM)
        {
            BuiltinMathFunction(Math.Exp, stackVM);
        }
    }

    internal class LogInstructionExecute : StackVMBuiltinCallExecute
    {
        internal override void BuiltinCallInstructionExecute(StackVM stackVM)
        {
            BuiltinMathFunction(Math.Log, stackVM);
        }
    }

    internal class Log10InstructionExecute : StackVMBuiltinCallExecute
    {
        internal override void BuiltinCallInstructionExecute(StackVM stackVM)
        {
            BuiltinMathFunction(Math.Log10, stackVM);
        }
    }

    internal class SqrtInstructionExecute : StackVMBuiltinCallExecute
    {
        internal override void BuiltinCallInstructionExecute(StackVM stackVM)
        {
            BuiltinMathFunction(Math.Sqrt, stackVM);
        }
    }

    internal class IntInstructionExecute : StackVMBuiltinCallExecute
    {
        internal override void BuiltinCallInstructionExecute(StackVM stackVM)
        {
            BuiltinMathFunction(Math.Truncate, stackVM);
        }
    }

    internal class AbsInstructionExecute : StackVMBuiltinCallExecute
    {
        internal override void BuiltinCallInstructionExecute(StackVM stackVM)
        {
            BuiltinMathFunction(Math.Abs, stackVM);
        }
    }
}

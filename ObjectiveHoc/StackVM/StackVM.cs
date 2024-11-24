using IntermediateCode;
using StackVM.Interfaces;

namespace StackVM
{
    public class StackVM : IStackVM
    {
        internal TextWriter output;

        internal List<Instruction> executableCode;

        internal Stack<dynamic> stack;

        private Dictionary<string, Instruction> instructionMap;

        internal Dictionary<string, int> symbolStaticDataMap;

        internal List<dynamic> staticData;

        internal Dictionary<string, int> labelMap;

        internal int pc;

        internal struct Frame
        {
            public List<dynamic> args; //arguments for calls
            public int entrypc; // program counter where to enter the procedure;
            public int returnpc; //program counter where to resume after return
        }

        internal Stack<Frame> frameStack;

        public StackVM(TextWriter output)
        {
            this.output = output;

            stack = new Stack<dynamic>();

            frameStack = new Stack<Frame>();

            executableCode = new List<Instruction>();

            instructionMap = new Dictionary<string, Instruction>
            {
                {"constpush", new ConstantPushInstruction()},
                {"add", new AddInstruction()},
                {"sub", new SubInstruction()},
                {"mul", new MulInstruction()},
                {"div", new DivInstruction()},
                {"power", new ExponentialInstruction()},
                {"arguments", new ArgumentsInstruction() },
                {"builtincall", new BuiltinCallInstruction() },
                {"stringpush", new StringPushInstruction() },
                {"negate", new NegateInstruction() },
                {"eval", new EvalInstruction() },
                {"assign", new AssignInstruction() },

                //comparison
                {"LT", new LessThanInstruction() },
                {"LE", new LessEqualInstruction() },
                {"GT", new GreaterThanInstruction() },
                {"GE", new GreaterEqualInstruction() },
                {"EQ", new EqualInstruction() },

                //Conditions
                {"and", new AndInstruction() },
                {"or", new OrInstruction() },

                //Control flow
                {"gofalse", new GoFalseInstruction() },
                {"goto", new GoToInstruction() },
                {"label", new LabelInstruction() },

                //input
                {"read", new ReadInstruction() },

                //function and procedure call
                {"arg", new ParameterArgumentInstruction() },
                {"define", new DefineInstruction() },
                {"call", new CallInstruction() },
                {"return", new ReturnInstruction() },
                {"mainfuncdef", new MainFunctionDefineInstruction() },
            };

            foreach (var instruction in instructionMap)
            {
                instruction.Value.SetStackVM(this);
            }

            InitialiseData();
        }

        internal void InitialiseData()
        {
            symbolStaticDataMap = new Dictionary<string, int>
            {
                //0 reserved for main entry
                {"PI", 1},
                {"DEG", 2},
                {"E", 3},
                {"GAMMA", 4},
                {"PHI", 5}
            };

            staticData = new List<dynamic>
            {
                0, //placeholder for main entry
                3.14159265358979323856m,
                57.29577951308232087680m,
                2.71828182845904523536m,
                0.57721566490153286060m,
                1.61803398874989484820m
            };

            labelMap = new Dictionary<string, int>();
        }

        public void Load(Program program)
        {
            for (int i = 0; i < program.Code.Count; i++)
            {
                string code = program.Code[i];

                if (instructionMap.ContainsKey(program.Code[i]))
                {
                    Instruction instruction = instructionMap[program.Code[i]].DeepCopy();

                    instruction.Load(ref i, program);

                    executableCode.Add(instruction);
                }
                else
                {
                    throw new Exception("runtime error: instruction not found");
                }
            }
        }

        public void Execute()
        {
            for (pc = staticData[0]; pc < executableCode.Count; pc++)
            {
                Instruction instruction = executableCode[pc];
                instruction.Execute();
            }
        }
    }
}

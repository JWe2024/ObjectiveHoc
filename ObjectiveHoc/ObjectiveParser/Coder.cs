using IntermediateCode;
using Parser.Interfaces;

namespace Parser
{
    internal class Code
    {
        private Program program;

        private string action;

        public Code(Program program, string action)
        {
            this.program = program;
            this.action = action;
        }

        public override string ToString()
        {
            return action;
        }

        public void GenerateCode(string value)
        {
            program.Code.Add(action);

            if (string.IsNullOrEmpty(value) == false)
            {
                program.Code.Add(value);
            }
        }
    }

    internal class Coder : ICoder
    {
        private TextWriter? verboseOutput;

        private Program program;

        private Dictionary<ISymbolTable.SemanticAction, Code> codeMap;

        public Coder(Program program)
        {
            this.verboseOutput = null;

            this.program = program;

            Initialise();
        }

        private void Initialise()
        {
            codeMap = new Dictionary<ISymbolTable.SemanticAction, Code>
            {
                {ISymbolTable.SemanticAction.Assign, new Code(program, "assign")},
                {ISymbolTable.SemanticAction.Constant, new Code(program, "constpush")},
                {ISymbolTable.SemanticAction.LeftVariable, new Code(program, "lvarpush")},
                {ISymbolTable.SemanticAction.RightVariable, new Code(program, "rvarpush")},
                {ISymbolTable.SemanticAction.Evaluate, new Code(program, "eval")},
                {ISymbolTable.SemanticAction.Add, new Code(program, "add")},
                {ISymbolTable.SemanticAction.Subtract, new Code(program, "sub")},
                {ISymbolTable.SemanticAction.Multiply, new Code(program, "mul")},
                {ISymbolTable.SemanticAction.Divide, new Code(program, "div")},
                {ISymbolTable.SemanticAction.Exponential, new Code(program, "power")},
                {ISymbolTable.SemanticAction.UnaryMinus, new Code(program, "negate")},
                {ISymbolTable.SemanticAction.BuiltinCall, new Code(program, "builtincall")},
                {ISymbolTable.SemanticAction.Arguments, new Code(program, "arguments")},
                {ISymbolTable.SemanticAction.String, new Code(program, "stringpush")},

                //control flow
                {ISymbolTable.SemanticAction.LT, new Code(program, "LT")},
                {ISymbolTable.SemanticAction.LE, new Code(program, "LE")},
                {ISymbolTable.SemanticAction.GT, new Code(program, "GT")},
                {ISymbolTable.SemanticAction.GE, new Code(program, "GE")},
                {ISymbolTable.SemanticAction.EQ, new Code(program, "EQ")},
                {ISymbolTable.SemanticAction.And, new Code(program, "and")},
                {ISymbolTable.SemanticAction.Or, new Code(program, "or")},
                {ISymbolTable.SemanticAction.Label, new Code(program, "label")},
                {ISymbolTable.SemanticAction.GoFalse, new Code(program, "gofalse")},
                {ISymbolTable.SemanticAction.GoTo, new Code(program, "goto")},

                //function and procedure call
                {ISymbolTable.SemanticAction.Arg, new Code(program, "arg")},
                {ISymbolTable.SemanticAction.Define, new Code(program, "define")},
                {ISymbolTable.SemanticAction.Call, new Code(program, "call")},
                {ISymbolTable.SemanticAction.Return, new Code(program, "return")},
                {ISymbolTable.SemanticAction.MainFuncDef, new Code(program, "mainfuncdef")},

                //read
                {ISymbolTable.SemanticAction.Read, new Code(program, "read")},
            };
        }

        public void SetVerboseOutput(TextWriter? verboseOutput)
        {
            this.verboseOutput = verboseOutput;
        }

        public void Emit(ISymbolTable.ISemanticAction action)
        {
            Code code = codeMap[action.Action];

            Verbose(code.ToString(), action.Value);

            code.GenerateCode(action.Value);
        }

        private void Verbose(string action, string value) 
        {
            this.verboseOutput?.WriteLine(action);

            if (string.IsNullOrEmpty(value) == false)
            {
                verboseOutput?.WriteLine(value);
            }
        }
    }
}

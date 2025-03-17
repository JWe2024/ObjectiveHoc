namespace Parser.Interfaces
{
    internal interface ISymbolTable
    {
        enum TokenType
        {
            Undefined,
            Identifier,
            Assign,
            Number,
            Add,
            Minus,
            Multiply,
            Divide,
            Exponential,
            LeftBracket,
            RightBracket,
            BuiltinVariable,
            BuiltinFunction,
            Comma,
            PrintStatement,
            String,
            If,
            Else,
            While,
            LT,
            LE,
            GE,
            GT,
            EQ,
            RightCurlyBracket,
            LeftCurlyBracket,
            Label,
            BitAnd,
            And,
            Or,
            BitOr,
            Arg,
            ProcDef,
            Procedure,
            FuncDef,
            Function,
            Return,
            Read,
            CtrlE,
            EOF
        }

        interface IToken
        {
            TokenType Type { get; }

            string Value { get; }
        }

        IToken GetToken(decimal number);

        IToken GetToken(string symbol);

        public struct Identifier
        {
            private readonly string value;

            public Identifier(string value)
            {
                this.value = value ?? throw new ArgumentNullException(nameof(value));
            }

            public string Value => value;
        }

        public struct StringIdentifier
        {
            private readonly string value;

            public StringIdentifier(string value)
            {
                this.value = value ?? throw new ArgumentNullException(nameof(value));
            }

            public string Value => value;
        }

        public struct ArgIdentifier
        {
            private readonly string value;

            public ArgIdentifier(string value)
            {
                this.value = value ?? throw new ArgumentNullException(nameof(value));
            }

            public string Value => value;
        }

        IToken GetToken(Identifier id);

        IToken GetToken(StringIdentifier id);

        IToken GetToken(ArgIdentifier id);


        enum SemanticAction
        {
            LeftVariable,
            RightVariable,
            Evaluate,
            Assign,
            Constant,
            Add,
            Subtract,
            Multiply,
            Divide,
            Exponential,
            UnaryMinus,
            BuiltinVariable,
            BuiltinFunction,
            Arguments,
            PrintStatement,
            BuiltinCall,
            String,
            If,
            Else,
            While,
            LT,
            LE,
            GT,
            EQ,
            GE,
            GoFalse,
            GoTo,
            Label,
            And,
            Or,
            Arg,
            Define,
            Call,
            Return,
            Read,
            MainFuncDef
        }

        interface ISemanticAction
        {
            SemanticAction Action { get; }

            string Value { get; }
        }

        void Define(string identifier, ISymbolTable.SemanticAction action);

        bool IsDefined(string identifier);

        void SymbolTableDefine(string identifier, ISymbolTable.IToken token);

        bool SymbolTableIsDefined(string identifier);
    }
}

using Parser.Interfaces;

namespace Parser
{
    internal class SymbolTable : ISymbolTable
    {
        internal class Token : ISymbolTable.IToken
        {
            private ISymbolTable.TokenType type;
            private string value;

            public Token()
            {
                type = ISymbolTable.TokenType.Undefined;
                value = string.Empty;
            }

            public Token(ISymbolTable.TokenType type, string value)
            {
                this.type = type;
                this.value = value;
            }

            public ISymbolTable.TokenType Type
            {
                get { return type; }
            }

            public string Value
            {
                get { return this.value; }
            }
        }

        internal class SemanticAction : ISymbolTable.ISemanticAction
        {
            private ISymbolTable.SemanticAction action;
            private string value;

            public SemanticAction(ISymbolTable.SemanticAction action, string value)
            {
                this.action = action;
                this.value = value;
            }

            public ISymbolTable.SemanticAction Action
            {
                get { return action; }
            }

            public string Value
            {
                get { return value; }
            }
        }

        private Dictionary<string, ISymbolTable.IToken> tokenMap;

        private Dictionary<string, ISymbolTable.SemanticAction> identifierMap;

        public SymbolTable()
        {
            Initialise();
        }

        private void Initialise()
        {
            tokenMap = new Dictionary<string, ISymbolTable.IToken>
            {
                {"-1", new Token(ISymbolTable.TokenType.EOF, "-1")},
                {"=", new Token(ISymbolTable.TokenType.Assign, "=")},
                {"+", new Token(ISymbolTable.TokenType.Add, "+")},
                {"-", new Token(ISymbolTable.TokenType.Minus, "-")},
                {"*", new Token(ISymbolTable.TokenType.Multiply, "*") },
                {"/", new Token(ISymbolTable.TokenType.Divide, "/") },
                {"^", new Token(ISymbolTable.TokenType.Exponential, "^") },
                {"(", new Token(ISymbolTable.TokenType.LeftBracket, "(") },
                {")", new Token(ISymbolTable.TokenType.RightBracket, ")") },
                {"}", new Token(ISymbolTable.TokenType.RightCurlyBracket, "}") },
                {"{", new Token(ISymbolTable.TokenType.LeftCurlyBracket, "{") },
                {",", new Token(ISymbolTable.TokenType.Comma, ",") },
                { "\n", new Token(ISymbolTable.TokenType.Undefined, "\n")},
                { "\r", new Token(ISymbolTable.TokenType.Undefined, "\r")},

                //builtin variables
                { "PI", new Token(ISymbolTable.TokenType.BuiltinVariable, "PI")},
                { "E", new Token(ISymbolTable.TokenType.BuiltinVariable, "E")},
                { "GAMMA", new Token(ISymbolTable.TokenType.BuiltinVariable, "GAMMA")},
                { "DEG", new Token(ISymbolTable.TokenType.BuiltinVariable, "DEG")},
                { "PHI", new Token(ISymbolTable.TokenType.BuiltinVariable, "PHI")},

                //builtin functions
                { "sin", new Token(ISymbolTable.TokenType.BuiltinFunction, "sin")},
                { "cos", new Token(ISymbolTable.TokenType.BuiltinFunction, "cos")},
                { "atan", new Token(ISymbolTable.TokenType.BuiltinFunction, "atan")},
                { "exp", new Token(ISymbolTable.TokenType.BuiltinFunction, "exp")},
                { "log", new Token(ISymbolTable.TokenType.BuiltinFunction, "log")},
                { "log10", new Token(ISymbolTable.TokenType.BuiltinFunction, "log10")},
                { "sqrt", new Token(ISymbolTable.TokenType.BuiltinFunction, "sqrt")},
                { "int", new Token(ISymbolTable.TokenType.BuiltinFunction, "int")},
                { "abs", new Token(ISymbolTable.TokenType.BuiltinFunction, "abs")},
                { "print", new Token(ISymbolTable.TokenType.PrintStatement, "print")},

                //read
                { "read", new Token(ISymbolTable.TokenType.Read, "read") },

                //control flow
                { "if", new Token(ISymbolTable.TokenType.If, "if")},
                { "else", new Token(ISymbolTable.TokenType.Else, "else")},
                { "while", new Token(ISymbolTable.TokenType.While, "while")},

                //comparison
                { "<", new Token(ISymbolTable.TokenType.LT, "LT") },
                { "<=", new Token(ISymbolTable.TokenType.LE, "LE") },
                { ">", new Token(ISymbolTable.TokenType.GT, "GT") },
                { ">=", new Token(ISymbolTable.TokenType.GE, "GE") },
                { "==", new Token(ISymbolTable.TokenType.EQ, "EQ") },

                //condition
                { "&&", new Token(ISymbolTable.TokenType.And, "and") },
                { "||", new Token(ISymbolTable.TokenType.Or, "or") },

                //conditions not supported
                { "&", new Token(ISymbolTable.TokenType.BitAnd, "bitand") },
                { "|", new Token(ISymbolTable.TokenType.BitOr, "bitor") },

                //functions and procedures
                { "arg", new Token(ISymbolTable.TokenType.Arg, "arg") },
                { "func", new Token(ISymbolTable.TokenType.FuncDef, "funcdef") },
                { "proc", new Token(ISymbolTable.TokenType.ProcDef, "procdef") },
                { "return", new Token(ISymbolTable.TokenType.Return, "return") },
            };

            identifierMap = new Dictionary<string, ISymbolTable.SemanticAction>
            {
                //builtin variables
                { "PI", ISymbolTable.SemanticAction.BuiltinVariable},
                { "E", ISymbolTable.SemanticAction.BuiltinVariable},
                { "GAMMA", ISymbolTable.SemanticAction.BuiltinVariable},
                { "DEG", ISymbolTable.SemanticAction.BuiltinVariable},
                { "PHI", ISymbolTable.SemanticAction.BuiltinVariable},

                //builtin functions
                {"sin", ISymbolTable.SemanticAction.BuiltinFunction},
                { "cos", ISymbolTable.SemanticAction.BuiltinFunction},
                { "atan", ISymbolTable.SemanticAction.BuiltinFunction},
                { "exp", ISymbolTable.SemanticAction.BuiltinFunction},
                { "log", ISymbolTable.SemanticAction.BuiltinFunction},
                { "log10", ISymbolTable.SemanticAction.BuiltinFunction},
                { "sqrt", ISymbolTable.SemanticAction.BuiltinFunction},
                { "int", ISymbolTable.SemanticAction.BuiltinFunction},
                { "abs", ISymbolTable.SemanticAction.BuiltinFunction},
                { "print", ISymbolTable.SemanticAction.PrintStatement},
            };
        }

        public ISymbolTable.IToken GetToken(ISymbolTable.ArgIdentifier Arg)
        {
            return new Token(ISymbolTable.TokenType.Arg, Arg.Value);
        }

        public ISymbolTable.IToken GetToken(ISymbolTable.Identifier identifier)
        {
            if (tokenMap.ContainsKey(identifier.Value))
            {
                return tokenMap[identifier.Value];
            }
            else
            {
                return new Token(ISymbolTable.TokenType.Identifier, identifier.Value);
            }
        }

        public ISymbolTable.IToken GetToken(ISymbolTable.StringIdentifier String)
        {
            return new Token(ISymbolTable.TokenType.String, String.Value);
        }

        public ISymbolTable.IToken GetToken(decimal number)
        {
            return new Token(ISymbolTable.TokenType.Number, number.ToString());
        }

        public ISymbolTable.IToken GetToken(string symbol)
        {
            return tokenMap[symbol];
        }

        public void Define(string identifier, ISymbolTable.SemanticAction action)
        {
            if (!IsDefined(identifier))
            {
                identifierMap.Add(identifier, action);
            }
        }

        public bool IsDefined(string identifier)
        {
            return identifierMap.ContainsKey(identifier);
        }

        public void SymbolTableDefine(string identifier, ISymbolTable.IToken token)
        {
            if (!SymbolTableIsDefined(identifier))
            {
                tokenMap.Add(identifier, token);
            }
        }

        public bool SymbolTableIsDefined(string identifier)
        {
            return tokenMap.ContainsKey(identifier);
        }
    }
}

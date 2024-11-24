using IntermediateCode;
using Parser.Interfaces;
using System.Reflection.Emit;
using static Parser.Interfaces.ISymbolTable;
using static Parser.SymbolTable;

namespace Parser
{
    internal abstract class StatementParser
    {
        protected Parser parser;

        internal abstract void Parse();

        internal void SetParser(Parser paser)
        {
            this.parser = paser;
        }

        protected void Match(ISymbolTable.IToken token)
        {
            if (parser.lookAhead.Type == token.Type)
            {
                parser.lookAhead = parser.lexicalAnalyser.GetToken();
            }
            else
            {
                throw new Exception("syntax error: Match() failed with token type mismatch");
            }
        }

        protected void Define(string identifier)
        {
            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Define, identifier));

            Match(parser.lookAhead);
            Match(new Token(ISymbolTable.TokenType.LeftBracket, "("));
            Match(new Token(ISymbolTable.TokenType.RightBracket, ")"));

            StripWhiteSpace();

            if (parser.lookAhead.Type == ISymbolTable.TokenType.LeftCurlyBracket)
            {
                Match(parser.lookAhead);

                StripWhiteSpace();

                while (parser.lookAhead.Type != ISymbolTable.TokenType.RightCurlyBracket)
                {
                    parser.StatementParse();

                    StripWhiteSpace();
                }

                Match(parser.lookAhead);
            }
            else
            {
                parser.StatementParse();
            }
        }

        protected void ParseCallParameters(string identifier)
        {
            int nargs = 0;

            Match(parser.lookAhead);
            Match(new Token(ISymbolTable.TokenType.LeftBracket, "("));
            while (parser.lookAhead.Type != ISymbolTable.TokenType.RightBracket)
            {
                Expr();
                nargs++;

                if (parser.lookAhead.Type == ISymbolTable.TokenType.Comma)
                {
                    Match(new Token(ISymbolTable.TokenType.Comma, ","));
                }
            }
            Match(new Token(ISymbolTable.TokenType.RightBracket, ")"));

            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Arguments, nargs.ToString()));
        }

        protected void CompExpr()
        {
            ISymbolTable.IToken t;

            Expr();

            switch (parser.lookAhead.Type)
            {
                case ISymbolTable.TokenType.LE:
                    t = parser.lookAhead;
                    Match(parser.lookAhead);
                    Expr();
                    parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.LE, string.Empty));
                    break;
                case ISymbolTable.TokenType.LT:
                    t = parser.lookAhead;
                    Match(parser.lookAhead);
                    Expr();
                    parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.LT, string.Empty));
                    break;
                case ISymbolTable.TokenType.GE:
                    t = parser.lookAhead;
                    Match(parser.lookAhead);
                    Expr();
                    parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.GE, string.Empty));
                    break;
                case ISymbolTable.TokenType.GT:
                    t = parser.lookAhead;
                    Match(parser.lookAhead);
                    Expr();
                    parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.GT, string.Empty));
                    break;
                case ISymbolTable.TokenType.EQ:
                    t = parser.lookAhead;
                    Match(parser.lookAhead);
                    Expr();
                    parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.EQ, string.Empty));
                    break;
                default:
                    return;
            }
        }

        protected void StripWhiteSpace()
        {
            while (parser.lookAhead.Type == ISymbolTable.TokenType.Undefined)
            {
                Match(parser.lookAhead);
            }
        }

        protected void CondExpr()
        {
            CompExpr();

            ISymbolTable.IToken t;

            while (true)
            {
                switch (parser.lookAhead.Type)
                {
                    case ISymbolTable.TokenType.And:
                        t = parser.lookAhead;
                        Match(parser.lookAhead);
                        CondExpr();
                        parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.And, string.Empty));
                        continue;
                    case ISymbolTable.TokenType.Or:
                        t = parser.lookAhead;
                        Match(parser.lookAhead);
                        CondExpr();
                        parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Or, string.Empty));
                        continue;
                    case ISymbolTable.TokenType.BitAnd:
                        throw new Exception("syntax error: bitwise AND not supported");
                        break;
                    case ISymbolTable.TokenType.BitOr:
                        throw new Exception("syntax error: bitwise OR not supported");
                        break;
                    default:
                        return;
                }
            }
        }

        protected void Expr()
        {
            ISymbolTable.IToken t;

            Term();

            while (true)
            {
                switch (parser.lookAhead.Type)
                {
                    case ISymbolTable.TokenType.Add:
                        t = parser.lookAhead;
                        Match(parser.lookAhead);
                        Term();
                        parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Add, string.Empty));
                        continue;
                    case ISymbolTable.TokenType.Minus:
                        t = parser.lookAhead;
                        Match(parser.lookAhead);
                        Term();
                        parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Subtract, string.Empty));
                        continue;
                    default:
                        return;
                }
            }
        }

        private void Term()
        {
            ISymbolTable.IToken t;

            ExpoTerm();

            while (true)
            {
                switch (parser.lookAhead.Type)
                {
                    case ISymbolTable.TokenType.Multiply:
                        t = parser.lookAhead;
                        Match(parser.lookAhead);
                        ExpoTerm();
                        parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Multiply, string.Empty));
                        continue;
                    case ISymbolTable.TokenType.Divide:
                        t = parser.lookAhead;
                        Match(parser.lookAhead);
                        ExpoTerm();
                        parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Divide, string.Empty));
                        continue;
                    default:
                        return;
                }
            }
        }

        private void ExpoTerm()
        {
            ISymbolTable.IToken t;

            Factor();

            switch (parser.lookAhead.Type)
            {
                case ISymbolTable.TokenType.Exponential:
                    t = parser.lookAhead;
                    Match(parser.lookAhead);
                    ExpoTerm();
                    parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Exponential, string.Empty));
                    break;
                default:
                    break;
            }
        }

        private void Factor()
        {
            switch (parser.lookAhead.Type)
            {
                case ISymbolTable.TokenType.LeftBracket:
                    Match(parser.lookAhead);
                    Expr();
                    if (parser.lookAhead.Type != ISymbolTable.TokenType.RightBracket)
                    {
                        throw new Exception("syntax error: missing right bracket");
                    }
                    else
                    {
                        Match(parser.lookAhead);
                    }
                    break;
                case ISymbolTable.TokenType.Minus:
                    Match(parser.lookAhead);
                    Factor();
                    parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.UnaryMinus, string.Empty));
                    break;
                case ISymbolTable.TokenType.Number:
                    parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Constant, parser.lookAhead.Value));
                    Match(parser.lookAhead);
                    break;
                case ISymbolTable.TokenType.String:
                    parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.String, parser.lookAhead.Value));
                    Match(parser.lookAhead);
                    break;
                case ISymbolTable.TokenType.Identifier:
                case ISymbolTable.TokenType.BuiltinVariable:

                    if (parser.symbolTable.IsDefined(parser.lookAhead.Value) == false)
                    {
                        throw new Exception("syntax error: undefined variable");
                    }

                    parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Evaluate, parser.lookAhead.Value));
                    Match(parser.lookAhead);
                    break;
                case ISymbolTable.TokenType.BuiltinFunction:

                    ISymbolTable.IToken t = parser.lookAhead;

                    int nargs = 0;

                    Match(parser.lookAhead);
                    Match(new Token(ISymbolTable.TokenType.LeftBracket, "("));
                    while (parser.lookAhead.Type != ISymbolTable.TokenType.RightBracket)
                    {
                        Expr();
                        nargs++;

                        if (parser.lookAhead.Type == ISymbolTable.TokenType.Comma)
                        {
                            Match(new Token(ISymbolTable.TokenType.Comma, ","));
                        }
                    }
                    Match(new Token(ISymbolTable.TokenType.RightBracket, ")"));

                    parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Arguments, nargs.ToString()));
                    parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.BuiltinCall, t.Value));
                    break;
                case ISymbolTable.TokenType.Arg:
                    parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Arg, parser.lookAhead.Value));
                    Match(parser.lookAhead);
                    break;
                case ISymbolTable.TokenType.Function:
                    ISymbolTable.IToken identifier = parser.lookAhead;

                    ParseCallParameters(identifier.Value);

                    parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Call, identifier.Value));
                    break;
                default:
                    throw new Exception("syntax error: unexpected symbol");
                    break;
            }
        }
    }

    internal class ArithmeticStatementParser : StatementParser
    {
        internal override void Parse()
        {
            Expr();

            int nargs = 1;

            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Arguments, nargs.ToString()));
            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.BuiltinCall, "print"));
        }
    }

    internal class ExpressionStatementParser : StatementParser
    {
        internal override void Parse()
        {
            Expr();
        }
    }

    internal class FuncDefStatementParser : StatementParser
    {
        internal override void Parse()
        {
            Match(parser.lookAhead);

            ISymbolTable.IToken identifier = parser.lookAhead;

            if (parser.symbolTable.SymbolTableIsDefined(identifier.Value))
            {
                throw new Exception("syntax error: function with same name already defined");
            }

            parser.symbolTable.SymbolTableDefine(identifier.Value, new Token(ISymbolTable.TokenType.Function, identifier.Value));

            parser.Mode = Parser.ParseMode.Function;
            Define(identifier.Value);
            parser.Mode = Parser.ParseMode.Default;

            //define main entry
            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.MainFuncDef, string.Empty));
        }
    }

    internal class ProcDefStatementParser : StatementParser
    {
        internal override void Parse()
        {
            Match(parser.lookAhead);

            ISymbolTable.IToken identifier = parser.lookAhead;

            if (parser.symbolTable.SymbolTableIsDefined(identifier.Value))
            {
                throw new Exception("syntax error: procedure with same name already defined");
            }

            parser.symbolTable.SymbolTableDefine(identifier.Value, new Token(ISymbolTable.TokenType.Procedure, identifier.Value));

            parser.Mode = Parser.ParseMode.Procedure;
            Define(identifier.Value);
            parser.Mode = Parser.ParseMode.Default;

            //default producedure return
            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Return, string.Empty));

            //define main entry
            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.MainFuncDef, string.Empty));
        }
    }

    internal class ProcCallStatementParser : StatementParser
    {
        internal override void Parse()
        {
            ISymbolTable.IToken identifier = parser.lookAhead;

            ParseCallParameters(identifier.Value);

            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Call, identifier.Value));
        }
    }

    internal class FuncCallStatementParser : StatementParser
    {
        internal override void Parse()
        {
            Expr();
        }
    }

    internal class ArgStatementParser : StatementParser
    {
        internal override void Parse()
        {
            Expr();
        }
    }

    internal class ReturnStatementParser : StatementParser
    {
        internal override void Parse()
        {
            Match(parser.lookAhead);

            if (parser.Mode == Parser.ParseMode.Default || parser.Mode == Parser.ParseMode.Function)
            {
                Expr();
            }
            else if (parser.Mode == Parser.ParseMode.Procedure)
            {
                 //do nothing
            }
            else
            {
                throw new Exception("error: invalid parse mode");
            }

            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Return, string.Empty));
        }
    }

    internal class IfStatementParser : StatementParser
    {
        internal override void Parse()
        {
            Match(parser.lookAhead);
            Match(new Token(ISymbolTable.TokenType.LeftBracket, "("));
            CondExpr();
            Match(new Token(ISymbolTable.TokenType.RightBracket, ")"));

            StripWhiteSpace();

            string label1 = parser.NewLabel();

            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.GoFalse, label1));

            if (parser.lookAhead.Type == ISymbolTable.TokenType.LeftCurlyBracket)
            {
                Match(parser.lookAhead);

                StripWhiteSpace();

                while (parser.lookAhead.Type != ISymbolTable.TokenType.RightCurlyBracket)
                {
                    parser.StatementParse();

                    StripWhiteSpace();
                }

                Match(parser.lookAhead);
            }
            else
            {
                parser.StatementParse();
            }

            string label2 = parser.NewLabel();

            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.GoTo, label2));

            StripWhiteSpace();

            if (parser.lookAhead.Type == ISymbolTable.TokenType.Else)
            {
                Match(parser.lookAhead);

                StripWhiteSpace();

                parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Label, label1));

                if (parser.lookAhead.Type == ISymbolTable.TokenType.LeftCurlyBracket)
                {
                    Match(parser.lookAhead);

                    StripWhiteSpace();

                    while (parser.lookAhead.Type != ISymbolTable.TokenType.RightCurlyBracket)
                    {
                        parser.StatementParse();

                        StripWhiteSpace();
                    }

                    Match(parser.lookAhead);
                }
                else
                {
                    parser.StatementParse();
                }
            }
            else
            {
                parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Label, label1));
            }

            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Label, label2));
        }
    }

    internal class WhileStatementParser : StatementParser
    {
        internal override void Parse()
        {
            Match(parser.lookAhead);

            string label2 = parser.NewLabel();

            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.GoTo, label2));

            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Label, label2));

            Match(new Token(ISymbolTable.TokenType.LeftBracket, "("));
            CondExpr();
            Match(new Token(ISymbolTable.TokenType.RightBracket, ")"));

            StripWhiteSpace();

            string label1 = parser.NewLabel();

            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.GoFalse, label1));

            if (parser.lookAhead.Type == ISymbolTable.TokenType.LeftCurlyBracket)
            {
                Match(parser.lookAhead);

                StripWhiteSpace();

                while (parser.lookAhead.Type != ISymbolTable.TokenType.RightCurlyBracket)
                {
                    parser.StatementParse();

                    StripWhiteSpace();
                }

                Match(parser.lookAhead);
            }
            else
            {
                parser.StatementParse();
            }

            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.GoTo, label2));

            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Label, label1));
        }
    }

    internal class AssignStatementParser : StatementParser
    {
        internal override void Parse()
        {
            ISymbolTable.IToken identifier = parser.lookAhead;

            Match(parser.lookAhead);

            if (parser.lookAhead.Type == ISymbolTable.TokenType.Assign)
            {
                Match(parser.lookAhead);

                parser.symbolTable.Define(identifier.Value, ISymbolTable.SemanticAction.Assign);

                Expr();

                parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Assign, identifier.Value));
            }
            else
            {
                throw new Exception("syntax error: '=' expected");
            }

        }
    }

    internal class ReadStatementParser : StatementParser
    {
        internal override void Parse()
        {
            if (parser.lookAhead.Type == ISymbolTable.TokenType.Read)
            {
                Match(parser.lookAhead);

                Match(new Token(ISymbolTable.TokenType.LeftBracket, "("));

                ISymbolTable.IToken identifier = parser.lookAhead;

                Expr();

                Match(new Token(ISymbolTable.TokenType.RightBracket, ")"));

                parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Read, identifier.Value));
            }
            else
            {
                throw new Exception("syntax error: '=' expected");
            }

        }
    }

    internal class PrintStatementParser : StatementParser
    {
        internal override void Parse()
        {
            ISymbolTable.IToken t = parser.lookAhead;

            int nargs = 0;

            Match(parser.lookAhead);

            if (parser.lookAhead.Type == ISymbolTable.TokenType.Undefined)
            {
                throw new Exception("syntax error: arguments expected");
            }

            while (true)
            {
                Expr();
                nargs++;

                if (parser.lookAhead.Type == ISymbolTable.TokenType.Comma)
                {
                    Match(new Token(ISymbolTable.TokenType.Comma, ","));
                }
                else
                {
                    break;
                }
            }

            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.Arguments, nargs.ToString()));
            parser.coder.Emit(new SymbolTable.SemanticAction(ISymbolTable.SemanticAction.BuiltinCall, t.Value));
        }
    }
}

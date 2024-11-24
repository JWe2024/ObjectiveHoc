using IntermediateCode;
using Parser.Interfaces;
using System.Xml.Linq;
using static Parser.SymbolTable;

namespace Parser
{
    public class Parser : IParser
    {
        public enum ParseMode
        {
            Default,
            Procedure,
            Function
        };

        internal ISymbolTable symbolTable;
        internal ICoder coder;
        internal ILexicalAnalyser lexicalAnalyser;
        internal ISymbolTable.IToken lookAhead;
        private Dictionary<ISymbolTable.TokenType, StatementParser> statementParserMap;
        private int labelNumber;
        private ParseMode parseMode;

        public ParseMode Mode
        {
            get { return parseMode; }
            set { parseMode = value; }
        }

        public Parser(TextReader input, Program program)
        {
            symbolTable = new SymbolTable();
            coder = new Coder(program);
            lexicalAnalyser = new LexicalAnalyser(input, symbolTable);
            lookAhead = new SymbolTable.Token();
            labelNumber = 0;
            parseMode = ParseMode.Default;

            statementParserMap = new Dictionary<ISymbolTable.TokenType, StatementParser>
            {
                {ISymbolTable.TokenType.Number, new ArithmeticStatementParser()},
                {ISymbolTable.TokenType.Minus, new ArithmeticStatementParser()},
                {ISymbolTable.TokenType.LeftBracket, new ExpressionStatementParser()},
                {ISymbolTable.TokenType.BuiltinVariable, new ExpressionStatementParser()},
                {ISymbolTable.TokenType.BuiltinFunction, new ExpressionStatementParser()},
                {ISymbolTable.TokenType.String, new ExpressionStatementParser()},
                {ISymbolTable.TokenType.Identifier, new AssignStatementParser()},
                {ISymbolTable.TokenType.PrintStatement, new PrintStatementParser()},
                {ISymbolTable.TokenType.If, new IfStatementParser()},
                {ISymbolTable.TokenType.While, new WhileStatementParser()},

                // function and procedure call
                {ISymbolTable.TokenType.Arg, new ArgStatementParser()},
                {ISymbolTable.TokenType.ProcDef, new ProcDefStatementParser()},
                {ISymbolTable.TokenType.Procedure, new ProcCallStatementParser()},
                {ISymbolTable.TokenType.FuncDef, new FuncDefStatementParser()},
                {ISymbolTable.TokenType.Function, new FuncCallStatementParser()},
                {ISymbolTable.TokenType.Return, new ReturnStatementParser()},
                {ISymbolTable.TokenType.Read, new ReadStatementParser()},
            };

            foreach (var statementParser in statementParserMap)
            {
                statementParser.Value.SetParser(this);
            }
        }

        public string NewLabel()
        {
            labelNumber++;
            string label = "#Label" + labelNumber;
            return label;
        }

        public void SetVerboseOutput(TextWriter? verboseOutput)
        {
            coder.SetVerboseOutput(verboseOutput);
        }

        public void Parse()
        {
            lookAhead = lexicalAnalyser.GetToken();

            statementParserMap[lookAhead.Type].Parse();
        }
        public void StatementParse()
        {
            if (this.Mode == ParseMode.Procedure || this.Mode == ParseMode.Function)
            {
                if (this.lookAhead.Type == ISymbolTable.TokenType.Number
                    || this.lookAhead.Type == ISymbolTable.TokenType.Minus
                    || this.lookAhead.Type == ISymbolTable.TokenType.LeftBracket
                    || this.lookAhead.Type == ISymbolTable.TokenType.BuiltinVariable
                    || this.lookAhead.Type == ISymbolTable.TokenType.BuiltinFunction
                    || this.lookAhead.Type == ISymbolTable.TokenType.String)
                {
                    throw new Exception("syntax error: unexpected Arithmetic statement");
                }
                else
                {
                    statementParserMap[lookAhead.Type].Parse();
                }
            }
            else if (this.Mode == ParseMode.Default)
            {
                statementParserMap[lookAhead.Type].Parse();
            }
            else
            {
                throw new Exception("error: invalid parse mode");
            }
        }
    }
}

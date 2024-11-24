namespace Parser.Interfaces
{
    internal interface ILexicalAnalyser
    {
        ISymbolTable.IToken GetToken();
    }
}

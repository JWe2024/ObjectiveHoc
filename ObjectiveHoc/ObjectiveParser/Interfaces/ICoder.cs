using IntermediateCode;

namespace Parser.Interfaces
{
    internal interface ICoder
    {
        void SetVerboseOutput(TextWriter? verboseOutput);

        void Emit(ISymbolTable.ISemanticAction action);
    }
}

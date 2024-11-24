namespace Parser.Interfaces
{
    public interface IParser
    {
        void SetVerboseOutput(TextWriter? verboseOutput);

        void Parse();
    }
}

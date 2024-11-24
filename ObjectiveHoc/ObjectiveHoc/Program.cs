using IntermediateCode;
using Parser.Interfaces;
using StackVM.Interfaces;

namespace ObjectiveHoc
{
    internal class ObjectiveHocApp
    {
        private TextReader input;
        private Program program;
        private IParser parser;
        private IStackVM stackVM;

        private bool verbose;

        public ObjectiveHocApp(TextReader reader, Program program, IParser parser, IStackVM stackVM)
        {
            this.input = reader;
            this.program = program;
            this.parser = parser;
            this.stackVM = stackVM;
            this.verbose = false;
        }

        public void run()
        {
            while (true)
            {
                int c = input.Peek();

                if (c == -1) //EOF
                {
                    break;
                }
                else if (c == '\n' || c == '\r')
                {
                    c = input.Read();
                }
                else
                {
                    if (c == 24) //ctrl+X
                    {
                        DiscardRemainingInput();

                        //command: verbose
                        verbose = !verbose;
                        if (verbose)
                        {
                            parser.SetVerboseOutput(Console.Out);
                        }
                        else
                        {
                            parser.SetVerboseOutput(null);
                        }
                    }
                    else if (c == 5) //ctrl+E
                    {
                        DiscardRemainingInput();

                        try
                        {
                            this.stackVM.Load(this.program);

                            this.stackVM.Execute();
                        }
                        catch (Exception ex)
                        {
                            Console.Out.WriteLine(ex.Message);

                            DiscardRemainingInput();
                        }
                    }
                    else
                    {
                        try
                        {
                            this.parser.Parse();
                        }
                        catch (Exception ex)
                        {
                            Console.Out.WriteLine(ex.Message);

                            DiscardRemainingInput();
                        }
                    }
                }
            }
        }

        private void DiscardRemainingInput()
        {
            while (true)
            {
                int c = input.Read();

                if (c == '\n' || c == '\r')
                {
                    break;
                }
            }
        }

        static void Main(string[] args)
        {
            TextReader reader = Console.In;
            TextWriter writer = Console.Out;

            Program program = new Program();

            IParser parser = new Parser.Parser(reader, program);

            IStackVM stackVM = new StackVM.StackVM(writer);

            ObjectiveHocApp app = new ObjectiveHocApp(reader, program, parser, stackVM);

            app.run();
        }
    }
}

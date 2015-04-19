

using System;
using System.Diagnostics;

namespace BizUnit.TestDocumentor.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 7)
            {
                if(null != args[6] && 0 < args[6].Length && 0 == string.Compare("debug", args[6]))
                {
                    Debugger.Break();
                }
            }
            else if(args.Length != 6)
            {
                Usage(args);
                return;
            }

            var documentor = new DocumentBuilder(new Logger());

            documentor.GenerateDocumentation(args[0], args[1], args[2], args[3], args[4], null, Convert.ToBoolean(args[5]));
        }

        private static void Usage(string[] args)
        {
            System.Console.WriteLine("Usage:");
            System.Console.WriteLine("{0} arguments were supplied.", args.Length);
            System.Console.WriteLine("BizUnit.TestDocumentor.exe [Test Report Template] [Category Template] [Test Case Template] [BizUnit Test Directory] [Output File Name (.XML)] [Recursive]");
        }
    }
}

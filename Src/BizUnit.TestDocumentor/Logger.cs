
namespace BizUnit.TestDocumentor
{
    using System;

    public class Logger : ILogger
    {
        public void Info(string message, params object[] args)
        {
            Console.WriteLine("Info: {0}", string.Format(message, args));
        }

        public void Warning(string message, params object[] args)
        {
            Console.WriteLine("Warning: {0}", string.Format(message, args));
        }

        public void Error(Exception ex)
        {
            Console.WriteLine("Error: {0}", ex);
        }
    }
}

using System;

namespace BizUnit.TestDocumentor
{
    /// <summary>
    /// </summary>
    public class Logger : ILogger
    {
        /// <summary>
        /// </summary>
        public void Info(string message, params object[] args)
        {
            Console.WriteLine("Info: {0}", string.Format(message, args));
        }

        /// <summary>
        /// </summary>
        public void Warning(string message, params object[] args)
        {
            Console.WriteLine("Warning: {0}", string.Format(message, args));
        }

        /// <summary>
        /// </summary>
        public void Error(Exception ex)
        {
            Console.WriteLine("Error: {0}", ex);
        }
    }
}

using System;

namespace BizUnit.TestDocumentor
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Info(string message, params object[] args);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Warning(string message, params object[] args);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        void Error(Exception ex);
    }
}

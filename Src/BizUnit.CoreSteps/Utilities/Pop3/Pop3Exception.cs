using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizUnit.CoreSteps.Utilities.Pop3
{
    public class Pop3Exception : Exception
    {
        public Pop3Exception()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public Pop3Exception(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

//---------------------------------------------------------------------
// File: ComplexTypeParameterFormatter.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2011, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace BizUnit.BizUnitOM
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("ComplexTypeParameterFormatter has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public class ComplexTypeParameterFormatter : ITestStepParameterFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public object[] FormatParameters(Type type, object[] args, Context ctx)
        {
            object[] retVal;

            if (typeof(IList<string>) == type)
            {
                IList<string> argsAsstringList = args.Select(arg => ctx.ReadArgument(arg).ToString()).ToList();
                retVal = new object[] { argsAsstringList };
            }
            else if (typeof(IList<Pair>) == type)
            {
                IList<Pair> argsAsPairList = new List<Pair>();
                for (int c = 0; c < args.Length; c += 2)
                {
                    argsAsPairList.Add(new Pair(ctx.ReadArgument(args[c]), ctx.ReadArgument(args[c + 1])));
                }
                retVal = new object[] { argsAsPairList };
            }
            else
            {
                throw new ArgumentException(string.Format("The type {0} is not supported in the BizBizUnit object model", type.FullName), "type");
            }

            return retVal;
        }
    }
}
